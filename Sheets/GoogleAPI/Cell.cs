using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public class Cell {
        public object value = "";
        public CellFormat? format = null;
    }
}
