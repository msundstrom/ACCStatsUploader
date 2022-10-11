
using System.Threading.Tasks;

namespace ACCStatsUploader {
    class TelemetryController {
        private Physics? latestPhysicsUpdate = null;
        private Graphics? latestGraphicsUpdate = null;
        private StaticInfo? latestStaticInfo = null;

        private LapInfo? lapInfo = null;

        private SheetController sheetController;

        private PitInEvent? pitInEvent = null;
        private PitOutEvent? pitOutEvent = null;

        private ClockManager clockManager = new ClockManager();

        private float lastSessionTime = -1;
        private int lastPacketId = -1;

        enum TRACK_STATE {
            ON_TRACK,
            PIT_LANE,
            PIT_BOX
        }

        private TRACK_STATE currentState = TRACK_STATE.ON_TRACK;

        public TelemetryController(SheetController sheetController) {
            this.sheetController = sheetController;
        }

        public void newPhysics(Physics physicsUpdate) {
            latestPhysicsUpdate = physicsUpdate;
        }

        public void newGraphics(Graphics graphicsUpdate) {
            latestGraphicsUpdate = graphicsUpdate;
        }

        public void newStaticInfo(StaticInfo staticInfoUpdate) {
            latestStaticInfo = staticInfoUpdate;
        }

        public async Task update() {
            if (
                latestGraphicsUpdate == null ||
                latestPhysicsUpdate == null ||
                latestStaticInfo == null
                ) {
                return;
            }

            Graphics unwrappedGraphics = (Graphics)latestGraphicsUpdate;
            Physics unwrappedPhysics = (Physics)latestPhysicsUpdate;
            StaticInfo unwrappedStaticInfo = (StaticInfo)latestStaticInfo;

            if (unwrappedPhysics.packetId == lastPacketId) {
                return;
            }

            lastPacketId = unwrappedPhysics.packetId;

            switch (unwrappedGraphics.session) {
                case ACC_SESSION_TYPE.ACC_PRACTICE:
                    await parsePractice(unwrappedGraphics, unwrappedPhysics, unwrappedStaticInfo);
                    break;
                case ACC_SESSION_TYPE.ACC_QUALIFY:
                    await parseQualifying(unwrappedGraphics, unwrappedPhysics, unwrappedStaticInfo);
                    break;
                case ACC_SESSION_TYPE.ACC_RACE:
                    await parseRace(unwrappedGraphics, unwrappedPhysics, unwrappedStaticInfo);
                    return;
                case ACC_SESSION_TYPE.ACC_UNKNOWN:
                case ACC_SESSION_TYPE.ACC_HOTLAP:
                case ACC_SESSION_TYPE.ACC_TIME_ATTACK:
                case ACC_SESSION_TYPE.ACC_DRIFT:
                case ACC_SESSION_TYPE.ACC_DRAG:
                    return;
            }
        }

        public async Task parsePractice(Graphics unwrappedGraphics, Physics unwrappedPhysics, StaticInfo unwrappedStaticInfo) {

            if (lapInfo is null) {
                lapInfo = new LapInfo(
                   unwrappedGraphics,
                   unwrappedPhysics,
                   unwrappedStaticInfo
               );
            }   
            
            TRACK_STATE? newStateMaybe = checkStateUpdate(unwrappedGraphics);
            if (newStateMaybe != null) {
                TRACK_STATE newState = (TRACK_STATE)newStateMaybe;
                switch (newState) {
                    case TRACK_STATE.ON_TRACK:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            // put out event
                            lapInfo.isOutLap = true;
                            if (pitOutEvent == null) {
                                pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                            }
                            pitOutEvent.setPitOut(unwrappedGraphics, unwrappedStaticInfo);
                            await sheetController.insertPitOutEvent(pitOutEvent);
                            pitOutEvent = null;
                        }
                        break;
                    case TRACK_STATE.PIT_LANE:
                        if (currentState == TRACK_STATE.ON_TRACK) {
                            pitInEvent = new PitInEvent(unwrappedGraphics, unwrappedStaticInfo);
                            lapInfo.isInLap = true;
                        } else {
                            // pit box out event?
                            pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                        }

                        break;
                    case TRACK_STATE.PIT_BOX:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            pitInEvent!.setInBox(unwrappedGraphics);
                            await sheetController.insertPitInEvent(pitInEvent);
                            pitInEvent = null;
                        }
                        break;
                }

                currentState = newState;
            }

            // check if new lap
            // comparing the currently stored lap number vs the one given in last update
            if (lapInfo.lapNumber < unwrappedGraphics.completedLaps + 1) {
                lapInfo.endLap(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
                System.Diagnostics.Debug.WriteLine("Sending update (" + unwrappedGraphics.packetId + ")...");
                await sheetController.insertLapInfo(lapInfo);
                System.Diagnostics.Debug.WriteLine("Sent update (" + unwrappedGraphics.packetId + ")!");
                lapInfo = new LapInfo(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
            } else {
                lapInfo.update(unwrappedPhysics);
                lapInfo.update(unwrappedGraphics);
            }

            // WEATHER UPDATE
            var oldTime = clockManager.currentTime;
            var newTime = new Time(unwrappedGraphics.Clock);

            if (oldTime < newTime) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            } else if ( // going past midnight... should work?
                (oldTime.hours == 23 && oldTime.minutes == 59) &&
                (newTime.hours == 00 && newTime.minutes == 00)
            ) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            }
        }

        public async Task parseQualifying(Graphics unwrappedGraphics, Physics unwrappedPhysics, StaticInfo unwrappedStaticInfo) {

            if (lapInfo is null) {
                lapInfo = new LapInfo(
                   unwrappedGraphics,
                   unwrappedPhysics,
                   unwrappedStaticInfo
               );
            }

            TRACK_STATE? newStateMaybe = checkStateUpdate(unwrappedGraphics);
            if (newStateMaybe != null) {
                TRACK_STATE newState = (TRACK_STATE)newStateMaybe;
                switch (newState) {
                    case TRACK_STATE.ON_TRACK:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            // put out event
                            lapInfo.isOutLap = true;
                            if (pitOutEvent == null) {
                                pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                            }
                            pitOutEvent.setPitOut(unwrappedGraphics, unwrappedStaticInfo);
                            await sheetController.insertPitOutEvent(pitOutEvent);
                            pitOutEvent = null;
                        }
                        break;
                    case TRACK_STATE.PIT_LANE:
                        if (currentState == TRACK_STATE.ON_TRACK) {
                            pitInEvent = new PitInEvent(unwrappedGraphics, unwrappedStaticInfo);
                            lapInfo.isInLap = true;
                        } else {
                            // pit box out event?
                            pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                        }

                        break;
                    case TRACK_STATE.PIT_BOX:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            pitInEvent!.setInBox(unwrappedGraphics);
                            await sheetController.insertPitInEvent(pitInEvent);
                            pitInEvent = null;
                        }
                        break;
                }

                currentState = newState;
            }

            // check if new lap
            // comparing the currently stored lap number vs the one given in last update
            if (lapInfo.lapNumber < unwrappedGraphics.completedLaps + 1) {
                lapInfo.endLap(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
                System.Diagnostics.Debug.WriteLine("Sending update (" + unwrappedGraphics.packetId + ")...");
                await sheetController.insertLapInfo(lapInfo);
                System.Diagnostics.Debug.WriteLine("Sent update (" + unwrappedGraphics.packetId + ")!");
                lapInfo = new LapInfo(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
            } else {
                lapInfo.update(unwrappedPhysics);
                lapInfo.update(unwrappedGraphics);
            }

            // WEATHER UPDATE
            var oldTime = clockManager.currentTime;
            var newTime = new Time(unwrappedGraphics.Clock);

            if (oldTime < newTime) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            } else if ( // going past midnight... should work?
                (oldTime.hours == 23 && oldTime.minutes == 59) &&
                (newTime.hours == 00 && newTime.minutes == 00)
            ) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            }
        }

        public async Task parseRace(Graphics unwrappedGraphics, Physics unwrappedPhysics, StaticInfo unwrappedStaticInfo) {
            if (lastSessionTime == -1) {
                lastSessionTime = unwrappedGraphics.sessionTimeLeft;
            }

            if (lapInfo is null && unwrappedGraphics.sessionTimeLeft == lastSessionTime) {
                lastSessionTime = unwrappedGraphics.sessionTimeLeft;
                return;
            }

            if (lapInfo is null) {
                System.Diagnostics.Debug.WriteLine("Race started!");
                lapInfo = new LapInfo(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
                lapInfo.timingInfo.isFirstLap = true;
            }

            TRACK_STATE? newStateMaybe = checkStateUpdate(unwrappedGraphics);
            if (newStateMaybe != null) {
                TRACK_STATE newState = (TRACK_STATE)newStateMaybe;
                switch (newState) {
                    case TRACK_STATE.ON_TRACK:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            // put out event
                            lapInfo.isOutLap = true;
                            if (pitOutEvent == null) {
                                pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                            }
                            pitOutEvent.setPitOut(unwrappedGraphics, unwrappedStaticInfo);
                            await sheetController.insertPitOutEvent(pitOutEvent);
                            pitOutEvent = null;
                        }
                        break;
                    case TRACK_STATE.PIT_LANE:
                        if (currentState == TRACK_STATE.ON_TRACK) {
                            pitInEvent = new PitInEvent(unwrappedGraphics, unwrappedStaticInfo);
                            lapInfo.isInLap = true;
                        } else {
                            // pit box out event?
                            pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                        }

                        break;
                    case TRACK_STATE.PIT_BOX:
                        if (currentState == TRACK_STATE.PIT_LANE) {
                            pitInEvent!.setInBox(unwrappedGraphics);
                            await sheetController.insertPitInEvent(pitInEvent);
                            pitInEvent = null;
                        }
                        break;
                }

                currentState = newState;
            }

            // check if new lap
            // comparing the currently stored lap number vs the one given in last update
            if (lapInfo.lapNumber < unwrappedGraphics.completedLaps + 1) {
                lapInfo.endLap(
                    unwrappedGraphics, 
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
                System.Diagnostics.Debug.WriteLine("Sending update (" + unwrappedGraphics.packetId + ")...");
                await sheetController.insertLapInfo(lapInfo);
                System.Diagnostics.Debug.WriteLine("Sent update (" + unwrappedGraphics.packetId + ")!");
                lapInfo = new LapInfo(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
            } else {
                lapInfo.update(unwrappedPhysics);
                lapInfo.update(unwrappedGraphics);
            }

            // WEATHER UPDATE
            var oldTime = clockManager.currentTime;
            var newTime = new Time(unwrappedGraphics.Clock);

            if (oldTime < newTime) {
               await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            } else if ( // going past midnight... should work?
                (oldTime.hours == 23 && oldTime.minutes == 59) &&
                (newTime.hours == 00 && newTime.minutes == 00)
            ) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = new Time(unwrappedGraphics.Clock),
                    currentWeatherValue = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecastValue = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackStateValue = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            }
        }

        private TRACK_STATE? checkStateUpdate(Graphics graphicsUpdate) {
            TRACK_STATE newState;
            if (graphicsUpdate.isInPit == 1) {
                newState = TRACK_STATE.PIT_BOX;
            } else if (graphicsUpdate.isInPitLane == 1) {
                newState = TRACK_STATE.PIT_LANE;
            } else {
                newState = TRACK_STATE.ON_TRACK;
            }

            if (currentState == newState) {
                return null;
            }

            return newState;
        }
    }
}