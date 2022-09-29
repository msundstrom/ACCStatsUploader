﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ACCStatsUploader {
    public class LapSheet : Sheet {
        public LapSheet() {
            sheetTitle = Sheet.SHEET_NAMES.LAP;
            columnTitles = new List<object>{
                "Session Type",
                "Lap",
                "Driver",
                "Sector 1",
                "Sector 2",
                "Sector 3",
                "Lap time",
                "Valid lap?",
                "Out lap?",
                "In lap?",
                "Fuel level (end of lap)",
                "Fuel consumption",
                "Session time left",
                "Position overall",
                "Car count",
                "In-game Clock",
                "Air temp",
                "Track temp",
                "Damage front",
                "Damage right",
                "Damage rear",
                "Damage left",
                "Brake pad level FL",
                "Brake pad level FR",
                "Brake pad level RL",
                "Brake pad level RR",
                "Brake avg temp FL",
                "Brake avg temp FR",
                "Brake avg temp RL",
                "Brake avg temp RR",
                "Brake max temp FL",
                "Brake max temp FR",
                "Brake max temp RL",
                "Brake max temp RR",
                "Avg tyre temp FL",
                "Avg tyre temp FR",
                "Avg tyre temp RL",
                "Avg tyre temp RR",
                "Max tyre temp FL",
                "Max tyre temp FR",
                "Max tyre temp RL",
                "Max tyre temp RR",
                "Avg tyre pressure FL",
                "Avg tyre pressure FR",
                "Avg tyre pressure RL",
                "Avg tyre pressure RR",
                "Max tyre pressure FL",
                "Max tyre pressure FR",
                "Max tyre pressure RL",
                "Max tyre pressure RR",
            };
        }
    }
}
