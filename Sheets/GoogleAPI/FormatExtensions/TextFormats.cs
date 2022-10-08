using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public static class TextFormats {
        public static TextFormat titleFormat {
            get {
                return new TextFormat {
                    Bold = true,
                    FontSize = 11
                };
            }
        }
    }
}
