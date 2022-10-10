using ACCStatsUploader.GoogleAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACCStatsUploader.Converters {
    public class AlignmentConverter {
        public static string vertical(CellVerticalAlignment alignment) {
            switch (alignment) {
                case CellVerticalAlignment.TOP:
                    return "TOP";
                case CellVerticalAlignment.MIDDLE:
                    return "MIDDLE";
                case CellVerticalAlignment.BOTTOM:
                    return "BOTTOM";
                default:
                    return "TOP";
            }
        }

        public static string horizontal(CellHorizontalAlignment alignment) {
            switch (alignment) {
                case CellHorizontalAlignment.LEFT:
                    return "LEFT";
                case CellHorizontalAlignment.CENTER:
                    return "CENTER";
                case CellHorizontalAlignment.RIGHT:
                    return "RIGHT";
                default:
                    return "LEFT";
            }
        }
    }
}
