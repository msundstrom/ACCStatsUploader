using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Google.Apis.Sheets.v4.Data.Color;

namespace ACCStatsUploader.Converters {
    public class ColorConverter {
        public static Color fromHex(string hexColor) {
            System.Drawing.Color color = ColorTranslator.FromHtml(hexColor);
            int r = Convert.ToInt16(color.R);
            int g = Convert.ToInt16(color.G);
            int b = Convert.ToInt16(color.B);

            return new Color {
                Red = (float)(r / 255.0),
                Green = (float)(g / 255.0),
                Blue = (float)(b / 255.0),
                Alpha = 1
            };
        }
    }
}
