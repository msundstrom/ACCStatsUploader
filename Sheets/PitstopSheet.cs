using System;
using System.Collections.Generic;
using System.Text;

//TODO: 
/*
 
 race summary log
 push position in class and overall each lap
 push session time left
 
 
 */

namespace ACCStatsUploader {
    public class PitstopSheet: Sheet {
        public PitstopSheet() {
            sheetTitle = Sheet.SHEET_NAMES.PITSTOP;
            columnTitles = new List<object>{
                "Session",
                "Type",
                "Inlap",
                "Outlap",
                "Pit in session time left",
                "Pit out session time left",
                "Pitbox in session time left",
                "Pitbox out session time left",
                "Driver",
                "Tyre set",
                "Initial PSI FL",
                "Initial PSI FR",
                "Initial PSI RL",
                "Initial PSI RR"
            };
        }
    }
}
