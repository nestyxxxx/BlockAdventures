using System.Drawing;
using System.Windows.Forms;

namespace BlockAdventures.Styles
{
    public static class StyleHelper
    {
        public static void ApplyMenuButtonStyle(Button button)
        {
            button.BackColor = Theme.ButtonColor;
            button.ForeColor = Theme.TextColor;
            button.Font = Theme.ButtonFont;

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Theme.BorderColor;
            button.Cursor = Cursors.Hand;

            button.MouseEnter += (s, e) =>
            {
                button.BackColor = Theme.ButtonHoverColor;
            };

            button.MouseLeave += (s, e) =>
            {
                button.BackColor = Theme.ButtonColor;
            };
        }

        public static void ApplyPanelStyle(Panel panel)
        {
            panel.BackColor = Theme.PanelColor;
            panel.BorderStyle = BorderStyle.FixedSingle;
        }

        public static void ApplyTitleStyle(Label label)
        {
            label.ForeColor = Theme.TitleColor;
            label.BackColor = Color.Transparent;
        }

        public static void ApplyTextStyle(Label label)
        {
            label.ForeColor = Theme.TextColor;
            label.BackColor = Color.Transparent;
        }
    }
}