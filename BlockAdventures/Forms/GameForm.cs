using System;
using System.Drawing;
using System.Windows.Forms;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public partial class GameForm : Form
    {
        private Label title;
        private Button btnStart;
        private Button btnOptions;
        private Button btnExit;

        private int musicVolume = 50;

        public GameForm()
        {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.DoubleBuffered = true;

            CreateUI();
            LayoutUI();

            this.Resize += (s, e) => LayoutUI();
        }

        private void CreateUI()
        {
            title = new Label();
            title.Text = "BlockAdventures";
            title.AutoSize = true;
            title.Font = Theme.MainTitleFont;
            StyleHelper.ApplyTitleStyle(title);

            btnStart = CreateMenuButton("Начать игру");
            btnOptions = CreateMenuButton("Опции");
            btnExit = CreateMenuButton("Выход");

            btnStart.Click += (s, e) =>
            {
                PlayForm playForm = new PlayForm(this, musicVolume);
                playForm.BackgroundImage = this.BackgroundImage;
                playForm.BackgroundImageLayout = this.BackgroundImageLayout;
                playForm.Show();
                this.Hide();
            };

            btnOptions.Click += (s, e) =>
            {
                OptionsForm optionsForm = new OptionsForm(this, musicVolume);
                optionsForm.BackgroundImage = this.BackgroundImage;
                optionsForm.BackgroundImageLayout = this.BackgroundImageLayout;

                optionsForm.FormClosed += (sender, args) =>
                {
                    musicVolume = optionsForm.MusicVolume;
                };

                optionsForm.Show();
                this.Hide();
            };

            btnExit.Click += (s, e) =>
            {
                this.Close();
            };

            this.Controls.Add(title);
            this.Controls.Add(btnStart);
            this.Controls.Add(btnOptions);
            this.Controls.Add(btnExit);
        }

        private Button CreateMenuButton(string text)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(390, 88);

            StyleHelper.ApplyMenuButtonStyle(button);

            return button;
        }

        private void LayoutUI()
        {
            int centerX = this.ClientSize.Width / 2;
            int centerY = this.ClientSize.Height / 2;

            int buttonHeight = btnStart.Height;

            int titleY = this.ClientSize.Height / 8;
            int startY = centerY - 70 - buttonHeight;
            int gap = 40;
            int exitY = this.ClientSize.Height - 180 - buttonHeight;

            title.Location = new Point(centerX - title.Width / 2, titleY);
            btnStart.Location = new Point(centerX - btnStart.Width / 2, startY);
            btnOptions.Location = new Point(centerX - btnOptions.Width / 2, btnStart.Bottom + gap);
            btnExit.Location = new Point(centerX - btnExit.Width / 2, exitY);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}