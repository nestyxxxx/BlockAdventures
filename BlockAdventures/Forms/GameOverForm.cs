using System;
using System.Drawing;
using System.Windows.Forms;
using BlockAdventures.GameLogic;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public class GameOverForm : Form
    {
        private Label titleLabel;
        private Label scoreLabel;
        private Label infoLabel;
        private TextBox nameTextBox;
        private Button saveButton;
        private Button exitButton;
        private Label statusLabel;

        private int finalScore;
        private bool isHighScore;
        private bool scoreWasSaved;

        public bool ScoreWasSaved
        {
            get { return scoreWasSaved; }
        }

        public GameOverForm(int score, bool isHighScoreResult)
        {
            finalScore = score;
            isHighScore = isHighScoreResult;

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(520, 330);
            BackColor = Color.FromArgb(86, 72, 46);
            DoubleBuffered = true;

            CreateControls();
            UpdateView();
        }

        private void CreateControls()
        {
            titleLabel = new Label();
            titleLabel.Text = "Игра окончена";
            titleLabel.Size = new Size(ClientSize.Width, 50);
            titleLabel.Location = new Point(0, 20);
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Font = new Font("Georgia", 24, FontStyle.Bold);
            titleLabel.ForeColor = Theme.TitleColor;
            titleLabel.BackColor = Color.Transparent;

            scoreLabel = new Label();
            scoreLabel.Text = "Очки: " + finalScore;
            scoreLabel.Size = new Size(ClientSize.Width, 40);
            scoreLabel.Location = new Point(0, 80);
            scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            scoreLabel.Font = new Font("Georgia", 18, FontStyle.Bold);
            scoreLabel.ForeColor = Theme.TextColor;
            scoreLabel.BackColor = Color.Transparent;

            infoLabel = new Label();
            infoLabel.Size = new Size(420, 55);
            infoLabel.Location = new Point(50, 130);
            infoLabel.TextAlign = ContentAlignment.MiddleCenter;
            infoLabel.Font = new Font("Georgia", 13, FontStyle.Bold);
            infoLabel.ForeColor = Theme.TextColor;
            infoLabel.BackColor = Color.Transparent;

            nameTextBox = new TextBox();
            nameTextBox.Size = new Size(240, 32);
            nameTextBox.Location = new Point(140, 195);
            nameTextBox.Font = new Font("Georgia", 14, FontStyle.Bold);
            nameTextBox.MaxLength = 12;

            saveButton = new Button();
            saveButton.Text = "Сохранить";
            saveButton.Size = new Size(150, 48);
            saveButton.Location = new Point(90, 245);
            StyleHelper.ApplyMenuButtonStyle(saveButton);

            exitButton = new Button();
            exitButton.Text = "Выйти";
            exitButton.Size = new Size(170, 48);
            exitButton.Location = new Point(260, 245);
            StyleHelper.ApplyMenuButtonStyle(exitButton);

            statusLabel = new Label();
            statusLabel.Size = new Size(420, 30);
            statusLabel.Location = new Point(50, 285);
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Font = new Font("Georgia", 11, FontStyle.Bold);
            statusLabel.ForeColor = Theme.TitleColor;
            statusLabel.BackColor = Color.Transparent;

            saveButton.Click += SaveButton_Click;
            exitButton.Click += ExitButton_Click;

            Controls.Add(titleLabel);
            Controls.Add(scoreLabel);
            Controls.Add(infoLabel);
            Controls.Add(nameTextBox);
            Controls.Add(saveButton);
            Controls.Add(exitButton);
            Controls.Add(statusLabel);
        }

        private void UpdateView()
        {
            if (isHighScore)
            {
                infoLabel.Text = "Новый рекорд!\nВведи имя и нажми Сохранить";
                nameTextBox.Visible = true;
                saveButton.Visible = true;
                exitButton.Location = new Point(260, 245);
            }
            else
            {
                infoLabel.Text = "В этот раз без рекорда.\nПопробуй ещё";
                nameTextBox.Visible = false;
                saveButton.Visible = false;
                exitButton.Location = new Point(
                    ClientSize.Width / 2 - exitButton.Width / 2,
                    245
                );
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var playerName = nameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                statusLabel.Text = "Введите имя";
                return;
            }

            LeaderboardManager.AddOrUpdateScore(playerName, finalScore);

            scoreWasSaved = true;
            nameTextBox.Enabled = false;
            saveButton.Enabled = false;
            statusLabel.Text = "Результат сохранён";
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (isHighScore && !scoreWasSaved)
            {
                statusLabel.Text = "Сначала сохрани результат";
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var outerPen = new Pen(Theme.TitleColor, 3))
            using (var innerPen = new Pen(Color.FromArgb(150, 60, 40), 2))
            {
                e.Graphics.DrawRectangle(
                    outerPen,
                    6,
                    6,
                    ClientSize.Width - 13,
                    ClientSize.Height - 13
                );

                e.Graphics.DrawRectangle(
                    innerPen,
                    16,
                    16,
                    ClientSize.Width - 33,
                    ClientSize.Height - 33
                );
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}