
using System.Threading.Tasks;

namespace ACCStatsUploader {
    class TelemetryController {
        private Physics? latestPhysicsUpdate = null;
        private Graphics? latestGraphicsUpdate = null;
        private StaticInfo? latestStaticInfo = null;

        private LapInfo? lapInfo;

        private SheetController sheetController;

        private PitInEvent? pitInEvent = null;
        private PitOutEvent? pitOutEvent = null;

        private ClockManager clockManager = new ClockManager();

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

            if (lapInfo == null) {
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
                                pitOutEvent = new PitOutEvent(unwrappedGraphics);
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
                            pitOutEvent = new PitOutEvent(unwrappedGraphics);
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
                    inGameClock = unwrappedGraphics.Clock,
                    currentWeather = (int)unwrappedGraphics.rainIntensity,
                    airTemp = unwrappedPhysics.airTemp,
                    trackTemp = unwrappedPhysics.roadTemp,
                    windSpeed = unwrappedGraphics.windSpeed,
                    tenMinuteForecast = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecast = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackState = (int)unwrappedGraphics.trackGripStatus
                });

                clockManager.update(unwrappedGraphics);
            } else if ( // going past midnight... should work?
                (oldTime.hours == 23 && oldTime.minutes == 59) &&
                (newTime.hours == 00 && newTime.minutes == 00)
            ) {
                await sheetController.insertWeatherEvent(new WeatherUpdateEvent {
                    inGameClock = unwrappedGraphics.Clock,
                    tenMinuteForecast = (int)unwrappedGraphics.rainIntensityIn10min,
                    thirtyMinuteForecast = (int)unwrappedGraphics.rainIntensityIn30min,
                    trackState = (int)unwrappedGraphics.trackGripStatus
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