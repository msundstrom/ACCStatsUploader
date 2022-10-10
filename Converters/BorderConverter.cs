using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACCStatsUploader.Converters {
    public enum BorderStyle { DOTTED, DASHED, SOLID, SOLID_MEDIUM, SOLID_THICK, NONE, DOUBLE };

    public static class BorderStyleExtension {
        public static string asString(this BorderStyle style) {
            switch (style) {
                case BorderStyle.DOTTED: return "DOTTED";
                case BorderStyle.DASHED: return "DASHED";
                case BorderStyle.SOLID: return "SOLID";
                case BorderStyle.SOLID_MEDIUM: return "SOLID_MEDIUM";
                case BorderStyle.SOLID_THICK: return "SOLID_THICK";
                case BorderStyle.NONE: return "NONE";
                case BorderStyle.DOUBLE: return "DOUBLE";
                default:
                    return "NONE";
            }
        }
    }

    public enum BorderEdge { TOP, RIGHT, LEFT, BOTTOM };

    public static class BorderEdgeExtension {
        public static string asString(this BorderEdge edge) {
            switch (edge) {
                case BorderEdge.TOP: return "TOP";
                case BorderEdge.RIGHT: return "RIGHT";
                case BorderEdge.BOTTOM: return "BOTTOM";
                case BorderEdge.LEFT: return "LEFT";
                default:
                    return "";
            }
        }
    }
}
