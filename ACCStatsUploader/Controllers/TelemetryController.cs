
using System;
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

        private TyreSetController tyreSetController = new TyreSetController();

        enum TRACK_STATE {
            ON_TRACK,
            PIT_LANE,
            PIT_BOX,
            UNDETERMINED
        }

        private TRACK_STATE currentState = TRACK_STATE.UNDETERMINED;

        public TelemetryController(SheetController sheetController) {
            this.sheetController = sheetController;

            // Should happen at race start, probably
            tyreSetController.start(this.sheetController.tyreSetsSheet);
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


            // some hacks to try to achieve a "not owning car" filter
            if (
                unwrappedPhysics.packetId == lastPacketId || 
                unwrappedPhysics.finalFF == 0.0
            ) {
                return;
            }

            lastPacketId = unwrappedPhysics.packetId;

            if (
                unwrappedGraphics.session == ACC_SESSION_TYPE.ACC_PRACTICE || 
                unwrappedGraphics.session == ACC_SESSION_TYPE.ACC_QUALIFY || 
                unwrappedGraphics.session == ACC_SESSION_TYPE.ACC_RACE
            ) {
                await parse(
                    unwrappedGraphics.session,
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
            }
        }

        private async Task parse(
            ACC_SESSION_TYPE sessionType, 
            Graphics unwrappedGraphics, 
            Physics unwrappedPhysics, 
            StaticInfo unwrappedStaticInfo
        ) {
            if (sessionType == ACC_SESSION_TYPE.ACC_RACE) {
                if (lastSessionTime == -1) {
                    lastSessionTime = unwrappedGraphics.sessionTimeLeft;
                }

                if (lapInfo is null && unwrappedGraphics.sessionTimeLeft == lastSessionTime) {
                    lastSessionTime = unwrappedGraphics.sessionTimeLeft;
                    return;
                }

                if (lapInfo is null && currentState == TRACK_STATE.ON_TRACK) {
                    System.Diagnostics.Debug.WriteLine("Race started!");
                    lapInfo = new LapInfo(
                        unwrappedGraphics,
                        unwrappedPhysics,
                        unwrappedStaticInfo
                    );
                    lapInfo.timingInfo.isFirstLap = true;
                }
            }
            

            TRACK_STATE? newStateMaybe = checkStateUpdate(unwrappedGraphics);
            if (newStateMaybe != null) {
                TRACK_STATE newState = (TRACK_STATE)newStateMaybe;

                if (currentState == TRACK_STATE.UNDETERMINED) {
                    currentState = newState;
                } else {
                    switch (newState) {
                        case TRACK_STATE.ON_TRACK:
                            if (currentState == TRACK_STATE.PIT_LANE) {
                                // start a new lap
                                lapInfo = new LapInfo(
                                    unwrappedGraphics,
                                    unwrappedPhysics,
                                    unwrappedStaticInfo
                                );

                                // pit out event
                                lapInfo.isOutLap = true;

                                // if we just drove through the pit lane
                                if (pitInEvent != null) {
                                    await sheetController.insertPitInEvent(pitInEvent);
                                    pitInEvent = null;
                                }

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

                                lapInfo.endLap(
                                    unwrappedGraphics,
                                    unwrappedPhysics,
                                    unwrappedStaticInfo
                                );

                                await sheetController.insertLapInfo(lapInfo);

                            } else if (currentState == TRACK_STATE.PIT_BOX) {
                                // pit box out event?
                                pitOutEvent = new PitOutEvent(unwrappedGraphics, unwrappedPhysics);
                                pitOutEvent.setPitBoxOut(unwrappedGraphics);
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
                }

                currentState = newState;
            }

            // check if new lap
            // comparing the currently stored lap number vs the one given in last update
            // when on an inlap this needs to happen at the _pit entry_ instead!
            if (
                lapInfo != null &&
                lapInfo.lapNumber < unwrappedGraphics.completedLaps + 1 && 
                currentState == TRACK_STATE.ON_TRACK
            ) {
                lapInfo.endLap(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
                System.Diagnostics.Debug.WriteLine("Sending update (" + unwrappedGraphics.packetId + ")...");

                // Ugly hack to fix the "phantom laps" issue when driver swapping
                if (lapInfo.timingInfo.sectorTimes.FindAll(sector => sector == -1).Count == 0) {
                    await sheetController.insertLapInfo(lapInfo);
                }

                System.Diagnostics.Debug.WriteLine("Sent update (" + unwrappedGraphics.packetId + ")!");
                lapInfo = new LapInfo(
                    unwrappedGraphics,
                    unwrappedPhysics,
                    unwrappedStaticInfo
                );
            } else if (
                lapInfo != null && 
                currentState == TRACK_STATE.ON_TRACK) {
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

            tyreSetController.update(unwrappedGraphics, unwrappedPhysics, unwrappedStaticInfo);
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