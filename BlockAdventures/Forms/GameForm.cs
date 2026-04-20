using System;
using System.Drawing;
using System.Windows.Forms;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public partial class GameForm : Form
    {
        private Label titleLabel;
        private Button startButton;
        private Button optionsButton;
        private Button exitButton;

        private int musicVolume = 50;

        public GameForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();
            MusicManager.Play("tribal.mp3", musicVolume);

            UpdateLayout();

            Resize += (s, e) => UpdateLayout();
        }

        private void CreateControls()
        {
            titleLabel = new Label();
            titleLabel.Text = "BlockAdventures";
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Georgia", 42, FontStyle.Bold);
            StyleHelper.ApplyTitleStyle(titleLabel);

            startButton = CreateMenuButton("Начать игру");
            optionsButton = CreateMenuButton("Опции");
            exitButton = CreateMenuButton("Выход");

            startButton.Click += StartButton_Click;
            optionsButton.Click += OptionsButton_Click;
            exitButton.Click += (s, e) =>
            {
                MusicManager.Stop();
                Close();
            };

            Controls.Add(titleLabel);
            Controls.Add(startButton);
            Controls.Add(optionsButton);
            Controls.Add(exitButton);
        }

        private Button CreateMenuButton(string text)
        {
            var button = new Button();
            button.Text = text;
            button.Size = new Size(390, 88);
            button.Font = new Font("Georgia", 18, FontStyle.Bold);
            StyleHelper.ApplyMenuButtonStyle(button);
            button.TabStop = false;
            return button;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var playForm = new PlayForm(this, musicVolume);
            playForm.BackgroundImage = BackgroundImage;
            playForm.BackgroundImageLayout = BackgroundImageLayout;
            playForm.Show();
            Hide();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            var optionsForm = new OptionsForm(this, musicVolume);
            optionsForm.BackgroundImage = BackgroundImage;
            optionsForm.BackgroundImageLayout = BackgroundImageLayout;

            optionsForm.FormClosed += (s, args) =>
            {
                musicVolume = optionsForm.MusicVolume;
                Show();
            };

            optionsForm.Show();
            Hide();
        }

        private void UpdateLayout()
        {
            var centerX = ClientSize.Width / 2;
            var centerY = ClientSize.Height / 2;

            var titleY = ClientSize.Height / 10;
            var gap = 35;

            titleLabel.Location = new Point(
                centerX - titleLabel.Width / 2,
                titleY
            );

            startButton.Location = new Point(
                centerX - startButton.Width / 2,
                centerY - 120
            );

            optionsButton.Location = new Point(
                centerX - optionsButton.Width / 2,
                startButton.Bottom + gap
            );

            exitButton.Location = new Point(
                centerX - exitButton.Width / 2,
                optionsButton.Bottom + gap
            );
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                MusicManager.Stop();
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}