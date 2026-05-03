using System;
using System.Drawing;
using System.Windows.Forms;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public class RestartConfirmForm : Form
    {
        private Panel mainPanel;
        private Label titleLabel;
        private Label textLabel;
        private Button yesButton;
        private Button noButton;

        public RestartConfirmForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(460, 240);
            BackColor = Color.FromArgb(117, 104, 73);
            DoubleBuffered = true;

            CreateControls();
            UpdateLayout();
        }

        private void CreateControls()
        {
            mainPanel = new Panel();
            mainPanel.Size = new Size(420, 200);
            mainPanel.BackColor = Theme.PanelColor;
            mainPanel.BorderStyle = BorderStyle.FixedSingle;

            titleLabel = new Label();
            titleLabel.Text = "Подтверждение";
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Georgia", 20, FontStyle.Bold);
            titleLabel.ForeColor = Theme.TitleColor;
            titleLabel.BackColor = Color.Transparent;

            textLabel = new Label();
            textLabel.Text = "Начать игру заново?";
            textLabel.AutoSize = false;
            textLabel.Size = new Size(340, 50);
            textLabel.TextAlign = ContentAlignment.MiddleCenter;
            textLabel.Font = new Font("Georgia", 15, FontStyle.Bold);
            textLabel.ForeColor = Theme.TextColor;
            textLabel.BackColor = Color.Transparent;

            yesButton = new Button();
            yesButton.Text = "Да";
            yesButton.Size = new Size(130, 50);
            StyleHelper.ApplyMenuButtonStyle(yesButton);
            yesButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.Yes;
                Close();
            };

            noButton = new Button();
            noButton.Text = "Нет";
            noButton.Size = new Size(130, 50);
            StyleHelper.ApplyMenuButtonStyle(noButton);
            noButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.No;
                Close();
            };

            mainPanel.Controls.Add(titleLabel);
            mainPanel.Controls.Add(textLabel);
            mainPanel.Controls.Add(yesButton);
            mainPanel.Controls.Add(noButton);

            Controls.Add(mainPanel);
        }

        private void UpdateLayout()
        {
            mainPanel.Location = new Point(
                ClientSize.Width / 2 - mainPanel.Width / 2,
                ClientSize.Height / 2 - mainPanel.Height / 2
            );

            titleLabel.Location = new Point(
                mainPanel.Width / 2 - titleLabel.Width / 2,
                20
            );

            textLabel.Location = new Point(
                mainPanel.Width / 2 - textLabel.Width / 2,
                70
            );

            yesButton.Location = new Point(65, 130);
            noButton.Location = new Point(mainPanel.Width - noButton.Width - 65, 130);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var pen = new Pen(Theme.BorderColor, 2))
            {
                e.Graphics.DrawRectangle(
                    pen,
                    1,
                    1,
                    ClientSize.Width - 3,
                    ClientSize.Height - 3
                );
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                DialogResult = DialogResult.No;
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}