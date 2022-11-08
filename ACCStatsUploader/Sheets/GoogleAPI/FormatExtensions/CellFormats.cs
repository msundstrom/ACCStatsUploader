using ACCStatsUploader.Converters;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.GoogleAPI {
    public static class CellFormats {
        public static CellFormat centered {
            get {
                return new CellFormat {
                    HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER),
                    VerticalAlignment = AlignmentConverter.vertical(CellVerticalAlignment.MIDDLE),
                };
            }
        }

        public static CellFormat centeredWithNumberFormat(NumberFormat numberFormat) {
            return new CellFormat {
                HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER),
                VerticalAlignment = AlignmentConverter.vertical(CellVerticalAlignment.MIDDLE),
                NumberFormat = numberFormat
            };
        }

        public static CellFormat centeredWithTextFormat(TextFormat textFormat) {
            return new CellFormat {
                HorizontalAlignment = AlignmentConverter.horizontal(CellHorizontalAlignment.CENTER),
                VerticalAlignment = AlignmentConverter.vertical(CellVerticalAlignment.MIDDLE),
                TextFormat = textFormat
            };
        }
    }
}
