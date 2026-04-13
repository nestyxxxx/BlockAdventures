using System.Drawing;

namespace BlockAdventures.Styles
{
    public static class Theme
    {
        public static Color PanelColor = Color.FromArgb(124, 111, 78);
        public static Color ButtonColor = Color.FromArgb(97, 84, 55);
        public static Color ButtonHoverColor = Color.FromArgb(120, 100, 65);

        public static Color TitleColor = Color.FromArgb(232, 206, 110);
        public static Color TextColor = Color.FromArgb(245, 240, 220);
        public static Color BorderColor = Color.FromArgb(170, 150, 95);

        public static Color FieldCellColor = Color.FromArgb(135, 121, 84);
        public static Color FieldBorderColor = Color.FromArgb(80, 68, 45);
        public static Color FieldOuterBorderColor = Color.FromArgb(165, 145, 95);

        public static Color ScoreBarColor = Color.FromArgb(120, 98, 52);
        public static Color ScoreFillColor = Color.FromArgb(214, 184, 63);

        public static Font MainTitleFont = new Font("Georgia", 42, FontStyle.Bold);
        public static Font PanelTitleFont = new Font("Georgia", 24, FontStyle.Bold);
        public static Font TextFont = new Font("Georgia", 16, FontStyle.Bold);
        public static Font ButtonFont = new Font("Georgia", 16, FontStyle.Bold);
        public static Font ScoreFont = new Font("Georgia", 18, FontStyle.Bold);
    }
}