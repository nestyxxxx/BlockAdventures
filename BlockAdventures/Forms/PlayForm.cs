using BlockAdventures.Controllers;
using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using BlockAdventures.Styles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace BlockAdventures
{
    public partial class PlayForm : Form
    {
        private Form menuForm;

        private Label scoreTitle;
        private Panel scoreBar;

        private Panel blocksPanel;
        private Label blocksTitle;
        private Label figureChangeCostLabel;
        private Button changeFigureButton;

        private Panel taskPanel;
        private Label taskTitle;
        private Label taskText;
        private Label taskChangeCostLabel;
        private Button changeTaskButton;

        private Panel bonusPanel;
        private Label bonusTitle;

        private Panel leaderboardPanel;
        private Label leaderboardTitle;
        private Label leaderboardPlaceHeader;
        private Label leaderboardNameHeader;
        private Label leaderboardScoreHeader;

        private List<Label> leaderboardPlaceLabels = new List<Label>();
        private List<Label> leaderboardNameLabels = new List<Label>();
        private List<Label> leaderboardScoreLabels = new List<Label>();

        private PictureBox archaeologistPicture;

        private Button settingsButton;
        private ContextMenuStrip settingsMenu;

        private int musicVolume = 50;
        private int scoreBarMax = 300;

        private int fieldCols = 10;
        private int fieldRows = 8;
        private int cellSize = 68;
        private int fieldOffsetX = 90;

        private PlayController controller;

        private bool isDraggingFigure;
        private Point mousePointOnForm;
        private Point dragOffset;

        private int grabbedFigureCellX = 0;
        private int grabbedFigureCellY = 0;

        private int previewStartX = 0;
        private int previewStartY = 0;
        private bool canPlaceFigureHere;

        private Timer dragTimer;
        private FigureGhostControl dragGhost;

        public PlayForm(Form menu, int currentVolume)
        {
            InitializeComponent();

            menuForm = menu;
            musicVolume = currentVolume;
            controller = new PlayController(fieldCols, fieldRows);

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();
            CreateDragSystem();

            UpdateLeaderboardPanel();
            UpdateLayout();
            UpdateTaskPanel();
            UpdateScoreView();

            Resize += (s, e) =>
            {
                UpdateLayout();
                UpdateScoreView();
                Invalidate();
            };

            CheckGameOver();
        }

        private void CreateDragSystem()
        {
            dragGhost = new FigureGhostControl();
            dragGhost.Visible = false;
            dragGhost.Enabled = false;
            Controls.Add(dragGhost);

            dragTimer = new Timer();
            dragTimer.Interval = 10;
            dragTimer.Tick += DragTimer_Tick;
        }

        private void DragTimer_Tick(object sender, EventArgs e)
        {
            if (!isDraggingFigure)
            {
                return;
            }

            UpdateDragFromCursor();

            if ((Control.MouseButtons & MouseButtons.Left) == 0)
            {
                FinishFigureDrag();
            }
        }

        private void CreateControls()
        {
            scoreTitle = new Label();
            scoreTitle.Text = "Очки";
            scoreTitle.AutoSize = true;
            scoreTitle.Font = Theme.PanelTitleFont;
            StyleHelper.ApplyTitleStyle(scoreTitle);

            scoreBar = new Panel();
            scoreBar.Size = new Size(470, 36);
            scoreBar.BackColor = Theme.ScoreBarColor;
            scoreBar.BorderStyle = BorderStyle.FixedSingle;
            scoreBar.Paint += ScoreBar_Paint;

            blocksPanel = CreateStonePanel(new Size(300, 270));
            blocksPanel.Paint += BlocksPanel_Paint;
            blocksPanel.MouseDown += BlocksPanel_MouseDown;

            blocksTitle = CreatePanelTitle("Блоки", 24);
            figureChangeCostLabel = CreatePanelText("0", 20);
            changeFigureButton = CreateArrowButton();
            changeFigureButton.Click += (s, e) => ChangeFigure();

            blocksPanel.Controls.Add(blocksTitle);
            blocksPanel.Controls.Add(figureChangeCostLabel);
            blocksPanel.Controls.Add(changeFigureButton);

            taskPanel = CreateStonePanel(new Size(300, 340));

            taskTitle = CreatePanelTitle("Задание", 24);

            taskText = new Label();
            taskText.Size = new Size(240, 120);
            taskText.TextAlign = ContentAlignment.MiddleCenter;
            taskText.Font = new Font("Georgia", 16, FontStyle.Bold);
            StyleHelper.ApplyTextStyle(taskText);

            taskChangeCostLabel = CreatePanelText("0", 20);
            changeTaskButton = CreateArrowButton();
            changeTaskButton.Click += (s, e) => ChangeTask();

            taskPanel.Controls.Add(taskTitle);
            taskPanel.Controls.Add(taskText);
            taskPanel.Controls.Add(taskChangeCostLabel);
            taskPanel.Controls.Add(changeTaskButton);

            bonusPanel = CreateStonePanel(new Size(280, 220));
            bonusPanel.Paint += BonusPanel_Paint;
            bonusPanel.MouseClick += BonusPanel_MouseClick;

            bonusTitle = CreatePanelTitle("Бонусы", 22);
            bonusPanel.Controls.Add(bonusTitle);

            CreateLeaderboardPanel();

            archaeologistPicture = new PictureBox();
            archaeologistPicture.Size = new Size(220, 320);
            archaeologistPicture.SizeMode = PictureBoxSizeMode.Zoom;
            archaeologistPicture.BackColor = Color.Transparent;
            archaeologistPicture.Enabled = false;
            LoadArchaeologistImage();

            settingsButton = new Button();
            settingsButton.Text = "Настройки";
            settingsButton.Size = new Size(180, 55);
            StyleHelper.ApplyMenuButtonStyle(settingsButton);

            settingsMenu = new ContextMenuStrip();
            settingsMenu.Font = new Font("Georgia", 12, FontStyle.Bold);
            settingsMenu.Renderer = new ToolStripProfessionalRenderer(new JungleMenuColorTable());

            var optionsItem = new ToolStripMenuItem("Опции");
            var restartItem = new ToolStripMenuItem("Начать игру заново");
            var mainMenuItem = new ToolStripMenuItem("В главное меню");

            StyleMenuItem(optionsItem);
            StyleMenuItem(restartItem);
            StyleMenuItem(mainMenuItem);

            optionsItem.Click += (s, e) =>
            {
                var optionsForm = new OptionsForm(this, musicVolume);
                optionsForm.BackgroundImage = BackgroundImage;
                optionsForm.BackgroundImageLayout = BackgroundImageLayout;

                optionsForm.FormClosed += (sender2, args) =>
                {
                    musicVolume = optionsForm.MusicVolume;
                    Show();
                };

                optionsForm.Show();
                Hide();
            };

            restartItem.Click += (s, e) => RestartGame();

            mainMenuItem.Click += (s, e) =>
            {
                menuForm.Show();
                Close();
            };

            settingsMenu.Items.Add(optionsItem);
            settingsMenu.Items.Add(restartItem);
            settingsMenu.Items.Add(mainMenuItem);

            settingsButton.Click += (s, e) =>
            {
                settingsMenu.Show(settingsButton, 0, settingsButton.Height);
            };

            Controls.Add(scoreTitle);
            Controls.Add(scoreBar);
            Controls.Add(blocksPanel);
            Controls.Add(taskPanel);
            Controls.Add(bonusPanel);
            Controls.Add(archaeologistPicture);
            Controls.Add(settingsButton);
        }

        private void CreateLeaderboardPanel()
        {
            leaderboardPanel = new Panel();
            leaderboardPanel.Size = new Size(390, 430);
            leaderboardPanel.BackColor = Color.FromArgb(85, 72, 46);
            leaderboardPanel.Paint += LeaderboardPanel_Paint;

            leaderboardTitle = new Label();
            leaderboardTitle.Text = "Топ";
            leaderboardTitle.Size = new Size(leaderboardPanel.Width, 46);
            leaderboardTitle.Location = new Point(0, 10);
            leaderboardTitle.TextAlign = ContentAlignment.MiddleCenter;
            leaderboardTitle.Font = new Font("Georgia", 22, FontStyle.Bold);
            leaderboardTitle.ForeColor = Theme.TitleColor;
            leaderboardTitle.BackColor = Color.Transparent;

            leaderboardPlaceHeader = new Label();
            leaderboardPlaceHeader.Text = "№";
            leaderboardPlaceHeader.Size = new Size(50, 30);
            leaderboardPlaceHeader.Location = new Point(0, 64);
            leaderboardPlaceHeader.TextAlign = ContentAlignment.MiddleCenter;
            leaderboardPlaceHeader.Font = new Font("Georgia", 13, FontStyle.Bold);
            leaderboardPlaceHeader.ForeColor = Theme.TextColor;
            leaderboardPlaceHeader.BackColor = Color.Transparent;

            leaderboardNameHeader = new Label();
            leaderboardNameHeader.Text = "Игрок";
            leaderboardNameHeader.Size = new Size(220, 30);
            leaderboardNameHeader.Location = new Point(50, 64);
            leaderboardNameHeader.TextAlign = ContentAlignment.MiddleCenter;
            leaderboardNameHeader.Font = new Font("Georgia", 13, FontStyle.Bold);
            leaderboardNameHeader.ForeColor = Theme.TextColor;
            leaderboardNameHeader.BackColor = Color.Transparent;

            leaderboardScoreHeader = new Label();
            leaderboardScoreHeader.Text = "Баллы";
            leaderboardScoreHeader.Size = new Size(120, 30);
            leaderboardScoreHeader.Location = new Point(270, 64);
            leaderboardScoreHeader.TextAlign = ContentAlignment.MiddleCenter;
            leaderboardScoreHeader.Font = new Font("Georgia", 13, FontStyle.Bold);
            leaderboardScoreHeader.ForeColor = Theme.TextColor;
            leaderboardScoreHeader.BackColor = Color.Transparent;

            leaderboardPanel.Controls.Add(leaderboardTitle);
            leaderboardPanel.Controls.Add(leaderboardPlaceHeader);
            leaderboardPanel.Controls.Add(leaderboardNameHeader);
            leaderboardPanel.Controls.Add(leaderboardScoreHeader);

            for (var i = 0; i < 10; i++)
            {
                var rowTop = 98 + i * 32;

                var placeLabel = new Label();
                placeLabel.Size = new Size(50, 30);
                placeLabel.Location = new Point(0, rowTop);
                placeLabel.TextAlign = ContentAlignment.MiddleCenter;
                placeLabel.Font = new Font("Georgia", 12, FontStyle.Bold);
                placeLabel.ForeColor = Theme.TitleColor;
                placeLabel.BackColor = Color.Transparent;
                placeLabel.Text = (i + 1).ToString();

                var nameLabel = new Label();
                nameLabel.Size = new Size(220, 30);
                nameLabel.Location = new Point(50, rowTop);
                nameLabel.TextAlign = ContentAlignment.MiddleLeft;
                nameLabel.Font = new Font("Georgia", 12, FontStyle.Bold);
                nameLabel.ForeColor = Theme.TextColor;
                nameLabel.BackColor = Color.Transparent;
                nameLabel.Text = "---";

                var scoreLabel = new Label();
                scoreLabel.Size = new Size(120, 30);
                scoreLabel.Location = new Point(270, rowTop);
                scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
                scoreLabel.Font = new Font("Georgia", 12, FontStyle.Bold);
                scoreLabel.ForeColor = Theme.TextColor;
                scoreLabel.BackColor = Color.Transparent;
                scoreLabel.Text = "---";

                leaderboardPlaceLabels.Add(placeLabel);
                leaderboardNameLabels.Add(nameLabel);
                leaderboardScoreLabels.Add(scoreLabel);

                leaderboardPanel.Controls.Add(placeLabel);
                leaderboardPanel.Controls.Add(nameLabel);
                leaderboardPanel.Controls.Add(scoreLabel);
            }

            Controls.Add(leaderboardPanel);
        }

        private void LeaderboardPanel_Paint(object sender, PaintEventArgs e)
        {
            using (var outerPen = new Pen(Theme.TitleColor, 2))
            using (var linePen = new Pen(Color.FromArgb(190, 70, 40), 2))
            {
                e.Graphics.DrawRectangle(
                    outerPen,
                    1,
                    1,
                    leaderboardPanel.Width - 3,
                    leaderboardPanel.Height - 3
                );

                e.Graphics.DrawLine(linePen, 0, 58, leaderboardPanel.Width, 58);
                e.Graphics.DrawLine(linePen, 0, 94, leaderboardPanel.Width, 94);

                e.Graphics.DrawLine(linePen, 50, 58, 50, leaderboardPanel.Height);
                e.Graphics.DrawLine(linePen, 270, 58, 270, leaderboardPanel.Height);

                for (var i = 0; i < 10; i++)
                {
                    var y = 98 + i * 32;
                    e.Graphics.DrawLine(linePen, 0, y, leaderboardPanel.Width, y);
                }
            }
        }

        private void UpdateLeaderboardPanel()
        {
            var scores = LeaderboardManager.LoadScores();

            for (var i = 0; i < 10; i++)
            {
                leaderboardPlaceLabels[i].Text = (i + 1).ToString();

                if (i < scores.Count)
                {
                    leaderboardNameLabels[i].Text = scores[i].Name;
                    leaderboardScoreLabels[i].Text = scores[i].Score.ToString();
                }
                else
                {
                    leaderboardNameLabels[i].Text = "---";
                    leaderboardScoreLabels[i].Text = "---";
                }
            }

            leaderboardPanel.Invalidate();
        }

        private void StyleMenuItem(ToolStripMenuItem item)
        {
            item.BackColor = Color.FromArgb(86, 72, 46);
            item.ForeColor = Color.FromArgb(236, 227, 198);
        }

        private void LoadArchaeologistImage()
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, @"..\\..\\Resources\\archaeologist.png");
                path = Path.GetFullPath(path);

                if (File.Exists(path))
                {
                    archaeologistPicture.Image = Image.FromFile(path);
                }
            }
            catch
            {
            }
        }

        private Panel CreateStonePanel(Size size)
        {
            var panel = new Panel();
            panel.Size = size;
            StyleHelper.ApplyPanelStyle(panel);
            return panel;
        }

        private Label CreatePanelTitle(string text, int fontSize)
        {
            var label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Font = new Font("Georgia", fontSize, FontStyle.Bold);
            StyleHelper.ApplyTitleStyle(label);
            return label;
        }

        private Label CreatePanelText(string text, int fontSize)
        {
            var label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.Font = new Font("Georgia", fontSize, FontStyle.Bold);
            StyleHelper.ApplyTextStyle(label);
            return label;
        }

        private Button CreateArrowButton()
        {
            var button = new Button();
            button.Text = "=>";
            button.Size = new Size(60, 38);
            StyleHelper.ApplyMenuButtonStyle(button);
            return button;
        }

        private void UpdateLayout()
        {
            var fieldWidth = fieldCols * cellSize;
            var fieldHeight = fieldRows * cellSize;

            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();

            scoreTitle.Location = new Point(ClientSize.Width / 2 - scoreTitle.Width / 2, 18);
            scoreBar.Location = new Point(ClientSize.Width / 2 - scoreBar.Width / 2, 62);

            settingsButton.Location = new Point(ClientSize.Width - settingsButton.Width - 40, 30);

            leaderboardPanel.Location = new Point(
                Math.Max(30, fieldLeft - leaderboardPanel.Width - 60),
                fieldTop - 10
            );

            var archaeologistX = fieldLeft - archaeologistPicture.Width - 20;
            if (archaeologistX > leaderboardPanel.Right + 10)
            {
                archaeologistPicture.Visible = true;
                archaeologistPicture.Location = new Point(
                    archaeologistX,
                    leaderboardPanel.Top + 20
                );
            }
            else
            {
                archaeologistPicture.Visible = false;
            }

            blocksPanel.Location = new Point(fieldLeft + fieldWidth + 55, fieldTop - 10);
            blocksTitle.Location = new Point(blocksPanel.Width / 2 - blocksTitle.Width / 2, 14);
            figureChangeCostLabel.Location = new Point(blocksPanel.Width / 2 - figureChangeCostLabel.Width / 2, 215);
            changeFigureButton.Location = new Point(blocksPanel.Width - 70, blocksPanel.Height - 48);

            taskPanel.Location = new Point(fieldLeft + fieldWidth + 55, fieldTop + 290);
            taskTitle.Location = new Point(taskPanel.Width / 2 - taskTitle.Width / 2, 14);
            taskText.Location = new Point(taskPanel.Width / 2 - taskText.Width / 2, 80);
            taskChangeCostLabel.Location = new Point(taskPanel.Width / 2 - taskChangeCostLabel.Width / 2, 260);
            changeTaskButton.Location = new Point(taskPanel.Width - 70, taskPanel.Height - 48);

            bonusPanel.Location = new Point(
                leaderboardPanel.Left + leaderboardPanel.Width / 2 - bonusPanel.Width / 2,
                leaderboardPanel.Bottom + 25
            );
            bonusTitle.Location = new Point(bonusPanel.Width / 2 - bonusTitle.Width / 2, 12);

            scoreBar.Invalidate();
            bonusPanel.Invalidate();
            leaderboardPanel.Invalidate();
        }

        private void UpdateTaskPanel()
        {
            taskText.Text = controller.CurrentTask.Text;
            taskChangeCostLabel.Text = controller.GetTaskChangeCost().ToString();
            UpdateCosts();
        }

        private void UpdateCosts()
        {
            figureChangeCostLabel.Text = controller.GetFigureChangeCost().ToString();
            taskChangeCostLabel.Text = controller.GetTaskChangeCost().ToString();
        }

        private void UpdateScoreView()
        {
            scoreBar.Invalidate();
            UpdateCosts();
        }

        private void ScoreBar_Paint(object sender, PaintEventArgs e)
        {
            var fillWidth = controller.Score * scoreBar.Width / scoreBarMax;

            if (fillWidth < 0)
            {
                fillWidth = 0;
            }

            if (fillWidth > scoreBar.Width)
            {
                fillWidth = scoreBar.Width;
            }

            using (var fillBrush = new SolidBrush(Theme.ScoreFillColor))
            {
                e.Graphics.FillRectangle(fillBrush, 0, 0, fillWidth, scoreBar.Height);
            }

            var text = controller.Score.ToString();
            var font = Theme.ScoreFont;

            var textSize = TextRenderer.MeasureText(text, font);
            var textX = scoreBar.Width / 2 - textSize.Width / 2;
            var textY = scoreBar.Height / 2 - textSize.Height / 2 + 1;

            TextRenderer.DrawText(
                e.Graphics,
                text,
                font,
                new Point(textX, textY),
                Color.FromArgb(35, 25, 10)
            );
        }

        private void ChangeFigure()
        {
            if (!controller.ChangeFigure())
            {
                MessageBox.Show("Недостаточно очков");
                return;
            }

            UpdateTaskPanel();
            UpdateScoreView();
            blocksPanel.Invalidate();
            Invalidate();

            CheckGameOver();
        }

        private void ChangeTask()
        {
            if (!controller.ChangeTask())
            {
                MessageBox.Show("Недостаточно очков");
                return;
            }

            UpdateTaskPanel();
            UpdateScoreView();
            Invalidate();
        }

        private void RestartGame()
        {
            using (var confirmForm = new RestartConfirmForm())
            {
                if (confirmForm.ShowDialog(this) != DialogResult.Yes)
                {
                    return;
                }
            }

            var newPlayForm = new PlayForm(menuForm, musicVolume);
            newPlayForm.BackgroundImage = BackgroundImage;
            newPlayForm.BackgroundImageLayout = BackgroundImageLayout;
            newPlayForm.Show();

            Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawField(e.Graphics);
        }

        private void DrawField(Graphics graphics)
        {
            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();

            using (var borderPen = new Pen(Theme.FieldBorderColor, 2))
            using (var outerPen = new Pen(Theme.FieldOuterBorderColor, 4))
            {
                for (var row = 0; row < fieldRows; row++)
                {
                    for (var col = 0; col < fieldCols; col++)
                    {
                        var cellLeft = fieldLeft + col * cellSize;
                        var cellTop = fieldTop + row * cellSize;
                        var cellRect = new Rectangle(cellLeft, cellTop, cellSize, cellSize);

                        var modelColor = controller.Cells[col, row];
                        var viewColor = modelColor.HasValue
                            ? GetUiColor(modelColor.Value)
                            : Theme.FieldCellColor;

                        using (var cellBrush = new SolidBrush(viewColor))
                        {
                            graphics.FillRectangle(cellBrush, cellRect);
                        }

                        graphics.DrawRectangle(borderPen, cellRect);
                    }
                }

                graphics.DrawRectangle(
                    outerPen,
                    fieldLeft - 2,
                    fieldTop - 2,
                    fieldCols * cellSize + 3,
                    fieldRows * cellSize + 3
                );
            }
        }

        private void BlocksPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawFigurePreview(e.Graphics);
        }

        private void DrawFigurePreview(Graphics graphics)
        {
            if (controller.CurrentFigure == null)
            {
                return;
            }

            if (isDraggingFigure)
            {
                return;
            }

            var previewCellSize = 42;
            var previewLeft = 85;
            var previewTop = 72;

            using (var brush = new SolidBrush(GetUiColor(controller.CurrentFigure.Color)))
            using (var pen = new Pen(Color.FromArgb(70, 55, 25), 2))
            {
                for (var row = 0; row < 3; row++)
                {
                    for (var col = 0; col < 3; col++)
                    {
                        if (!controller.CurrentFigure.Cells[col, row])
                        {
                            continue;
                        }

                        var rect = new Rectangle(
                            previewLeft + col * previewCellSize,
                            previewTop + row * previewCellSize,
                            previewCellSize,
                            previewCellSize
                        );

                        graphics.FillRectangle(brush, rect);
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        private void BlocksPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            var previewCellSize = 42;
            var previewLeft = 85;
            var previewTop = 72;

            var clickedCellX = -1;
            var clickedCellY = -1;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!controller.CurrentFigure.Cells[col, row])
                    {
                        continue;
                    }

                    var rect = new Rectangle(
                        previewLeft + col * previewCellSize,
                        previewTop + row * previewCellSize,
                        previewCellSize,
                        previewCellSize
                    );

                    if (rect.Contains(e.Location))
                    {
                        clickedCellX = col;
                        clickedCellY = row;
                    }
                }
            }

            if (clickedCellX == -1 || clickedCellY == -1)
            {
                return;
            }

            StartFigureDrag(e, clickedCellX, clickedCellY, previewCellSize, previewLeft, previewTop);
        }

        private void StartFigureDrag(
            MouseEventArgs e,
            int clickedCellX,
            int clickedCellY,
            int previewCellSize,
            int previewLeft,
            int previewTop)
        {
            isDraggingFigure = true;

            grabbedFigureCellX = clickedCellX;
            grabbedFigureCellY = clickedCellY;

            var insideCellX = e.X - (previewLeft + clickedCellX * previewCellSize);
            var insideCellY = e.Y - (previewTop + clickedCellY * previewCellSize);

            dragOffset = new Point(
                clickedCellX * cellSize + insideCellX * cellSize / previewCellSize,
                clickedCellY * cellSize + insideCellY * cellSize / previewCellSize
            );

            dragGhost.SetFigure(controller.CurrentFigure, cellSize);
            dragGhost.Visible = true;
            dragGhost.BringToFront();

            blocksPanel.Invalidate();

            UpdateDragFromCursor();
            dragTimer.Start();
        }

        private void FinishFigureDrag()
        {
            var shouldPutFigure = canPlaceFigureHere;
            var startCol = previewStartX;
            var startRow = previewStartY;

            isDraggingFigure = false;
            canPlaceFigureHere = false;

            dragTimer.Stop();
            dragGhost.Visible = false;

            blocksPanel.Invalidate();
            Invalidate();

            if (shouldPutFigure)
            {
                PutFigureOnField(startCol, startRow);
            }
        }

        private void UpdateDragFromCursor()
        {
            if (!isDraggingFigure)
            {
                return;
            }

            mousePointOnForm = PointToClient(Cursor.Position);

            UpdatePreviewPosition();
            UpdateGhostPosition();

            dragGhost.CanPlace = canPlaceFigureHere;
            dragGhost.Invalidate();
        }

        private void UpdateGhostPosition()
        {
            var drawLeft = mousePointOnForm.X - dragOffset.X;
            var drawTop = mousePointOnForm.Y - dragOffset.Y;

            dragGhost.Location = new Point(drawLeft, drawTop);
        }

        private int GetFieldLeft()
        {
            var fieldWidth = fieldCols * cellSize;
            return (ClientSize.Width - fieldWidth) / 2 + fieldOffsetX;
        }

        private int GetFieldTop()
        {
            var fieldHeight = fieldRows * cellSize;
            return (ClientSize.Height - fieldHeight) / 2 + 30;
        }

        private bool IsCursorInsideField()
        {
            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();
            var fieldWidth = fieldCols * cellSize;
            var fieldHeight = fieldRows * cellSize;

            return
                mousePointOnForm.X >= fieldLeft &&
                mousePointOnForm.X < fieldLeft + fieldWidth &&
                mousePointOnForm.Y >= fieldTop &&
                mousePointOnForm.Y < fieldTop + fieldHeight;
        }

        private void UpdatePreviewPosition()
        {
            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();

            var cellUnderMouseX = (int)Math.Floor((double)(mousePointOnForm.X - fieldLeft) / cellSize);
            var cellUnderMouseY = (int)Math.Floor((double)(mousePointOnForm.Y - fieldTop) / cellSize);

            previewStartX = cellUnderMouseX - grabbedFigureCellX;
            previewStartY = cellUnderMouseY - grabbedFigureCellY;

            if (!IsCursorInsideField())
            {
                canPlaceFigureHere = false;
                return;
            }

            canPlaceFigureHere = controller.CanPlaceCurrentFigure(previewStartX, previewStartY);
        }

        private void PutFigureOnField(int startCol, int startRow)
        {
            if (!controller.PlaceCurrentFigure(startCol, startRow))
            {
                return;
            }

            UpdateTaskPanel();
            UpdateScoreView();

            blocksPanel.Invalidate();
            bonusPanel.Invalidate();

            CheckGameOver();
            Invalidate();
        }

        private void CheckGameOver()
        {
            if (controller.HasAnyPlaceForCurrentFigure())
            {
                return;
            }

            FinishGame();
        }

        private void FinishGame()
        {
            var isHighScore = LeaderboardManager.IsHighScore(controller.Score);

            using (var gameOverForm = new GameOverForm(controller.Score, isHighScore))
            {
                gameOverForm.ShowDialog(this);

                if (gameOverForm.ScoreWasSaved)
                {
                    UpdateLeaderboardPanel();
                }
            }

            menuForm.Show();
            Close();
        }

        private void BonusPanel_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedColor = GetClickedBonusColor(e.Location, bonusPanel.Width);

            if (!controller.UseBonus(clickedColor))
            {
                return;
            }

            bonusPanel.Invalidate();
            Invalidate();
        }

        private void BonusPanel_Paint(object sender, PaintEventArgs e)
        {
            DrawBonusPanel(e.Graphics, bonusPanel.Width);
        }

        private BonusColor GetClickedBonusColor(Point point, int panelWidth)
        {
            var centerX = panelWidth / 2;
            var topY = 72;
            var size = 105;

            var top = new Point(centerX, topY);
            var left = new Point(centerX - size / 2, topY + size / 2);
            var right = new Point(centerX + size / 2, topY + size / 2);
            var bottom = new Point(centerX, topY + size);
            var center = new Point(centerX, topY + size / 2);

            if (IsPointInsideTriangle(point, top, left, center))
            {
                return BonusColor.Red;
            }

            if (IsPointInsideTriangle(point, top, right, center))
            {
                return BonusColor.Yellow;
            }

            if (IsPointInsideTriangle(point, left, bottom, center))
            {
                return BonusColor.Green;
            }

            if (IsPointInsideTriangle(point, right, bottom, center))
            {
                return BonusColor.Blue;
            }

            return BonusColor.None;
        }

        private void DrawBonusPanel(Graphics graphics, int panelWidth)
        {
            var centerX = panelWidth / 2;
            var topY = 72;
            var size = 105;

            var top = new Point(centerX, topY);
            var left = new Point(centerX - size / 2, topY + size / 2);
            var right = new Point(centerX + size / 2, topY + size / 2);
            var bottom = new Point(centerX, topY + size);
            var center = new Point(centerX, topY + size / 2);

            DrawBonusPart(
                graphics,
                center,
                top,
                left,
                BonusColor.Red,
                controller.GetBonusProgress(BonusColor.Red),
                controller.IsBonusReady(BonusColor.Red)
            );

            DrawBonusPart(
                graphics,
                center,
                top,
                right,
                BonusColor.Yellow,
                controller.GetBonusProgress(BonusColor.Yellow),
                controller.IsBonusReady(BonusColor.Yellow)
            );

            DrawBonusPart(
                graphics,
                center,
                left,
                bottom,
                BonusColor.Green,
                controller.GetBonusProgress(BonusColor.Green),
                controller.IsBonusReady(BonusColor.Green)
            );

            DrawBonusPart(
                graphics,
                center,
                right,
                bottom,
                BonusColor.Blue,
                controller.GetBonusProgress(BonusColor.Blue),
                controller.IsBonusReady(BonusColor.Blue)
            );

            using (var pen = new Pen(Color.FromArgb(60, 50, 30), 3))
            {
                graphics.DrawPolygon(pen, new[] { top, left, bottom, right });
                graphics.DrawLine(pen, top, bottom);
                graphics.DrawLine(pen, left, right);
            }

            if (HasAnyReadyBonus())
            {
                DrawBonusReadyText(graphics, panelWidth);
            }
        }

        private void DrawBonusPart(
            Graphics graphics,
            Point center,
            Point p1,
            Point p2,
            BonusColor bonusColor,
            int progress,
            bool isReady)
        {
            if (progress <= 0)
            {
                return;
            }

            var k = progress / 100f;
            var newP1 = GetScaledPoint(center, p1, k);
            var newP2 = GetScaledPoint(center, p2, k);

            using (var brush = new SolidBrush(GetUiColor(bonusColor)))
            {
                graphics.FillPolygon(brush, new[] { center, newP1, newP2 });
            }

            if (isReady)
            {
                using (var readyPen = new Pen(Color.FromArgb(255, 245, 225, 150), 5))
                {
                    graphics.DrawPolygon(readyPen, new[] { center, p1, p2 });
                }
            }
        }

        private void DrawBonusReadyText(Graphics graphics, int panelWidth)
        {
            var text = "Нажми на бонус";
            using (var font = new Font("Georgia", 11, FontStyle.Bold))
            {
                var textSize = TextRenderer.MeasureText(text, font);

                var x = panelWidth / 2 - textSize.Width / 2;
                var y = bonusPanel.Height - 34;

                using (var backBrush = new SolidBrush(Color.FromArgb(170, 70, 55, 30)))
                using (var borderPen = new Pen(Color.FromArgb(230, 232, 206, 110), 1))
                {
                    graphics.FillRectangle(backBrush, x - 8, y - 2, textSize.Width + 16, textSize.Height + 4);
                    graphics.DrawRectangle(borderPen, x - 8, y - 2, textSize.Width + 16, textSize.Height + 4);
                }

                TextRenderer.DrawText(
                    graphics,
                    text,
                    font,
                    new Point(x, y),
                    Color.White
                );
            }
        }

        private bool HasAnyReadyBonus()
        {
            return controller.IsBonusReady(BonusColor.Red) ||
                   controller.IsBonusReady(BonusColor.Green) ||
                   controller.IsBonusReady(BonusColor.Yellow) ||
                   controller.IsBonusReady(BonusColor.Blue);
        }

        private Point GetScaledPoint(Point center, Point target, float k)
        {
            var x = center.X + (int)((target.X - center.X) * k);
            var y = center.Y + (int)((target.Y - center.Y) * k);

            return new Point(x, y);
        }

        private bool IsPointInsideTriangle(Point p, Point p1, Point p2, Point p3)
        {
            var d1 = GetTriangleSign(p, p1, p2);
            var d2 = GetTriangleSign(p, p2, p3);
            var d3 = GetTriangleSign(p, p3, p1);

            var hasNegative = d1 < 0 || d2 < 0 || d3 < 0;
            var hasPositive = d1 > 0 || d2 > 0 || d3 > 0;

            return !(hasNegative && hasPositive);
        }

        private float GetTriangleSign(Point p1, Point p2, Point p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        private static Color GetUiColor(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                return Color.FromArgb(196, 72, 56);
            }

            if (bonusColor == BonusColor.Green)
            {
                return Color.FromArgb(92, 176, 78);
            }

            if (bonusColor == BonusColor.Yellow)
            {
                return Color.FromArgb(222, 198, 68);
            }

            return Color.FromArgb(78, 180, 220);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                menuForm.Show();
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private class FigureGhostControl : Control
        {
            public FigureModel Figure { get; private set; }
            public int CellSize { get; private set; }
            public bool CanPlace { get; set; }

            public FigureGhostControl()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            public void SetFigure(FigureModel figure, int cellSize)
            {
                Figure = figure;
                CellSize = cellSize;

                Size = new Size(3 * cellSize, 3 * cellSize);
                UpdateFigureRegion();
                Invalidate();
            }

            private void UpdateFigureRegion()
            {
                if (Figure == null)
                {
                    return;
                }

                using (var path = new GraphicsPath())
                {
                    for (var row = 0; row < 3; row++)
                    {
                        for (var col = 0; col < 3; col++)
                        {
                            if (!Figure.Cells[col, row])
                            {
                                continue;
                            }

                            path.AddRectangle(new Rectangle(
                                col * CellSize,
                                row * CellSize,
                                CellSize,
                                CellSize));
                        }
                    }

                    if (Region != null)
                    {
                        Region.Dispose();
                    }

                    Region = new Region(path);
                }
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (Figure == null)
                {
                    return;
                }

                var drawColor = GetUiColor(Figure.Color);

                if (!CanPlace)
                {
                    drawColor = Color.FromArgb(140, 140, 140);
                }

                using (var brush = new SolidBrush(Color.FromArgb(180, drawColor)))
                using (var pen = new Pen(Color.FromArgb(70, 55, 25), 2))
                {
                    for (var row = 0; row < 3; row++)
                    {
                        for (var col = 0; col < 3; col++)
                        {
                            if (!Figure.Cells[col, row])
                            {
                                continue;
                            }

                            var rect = new Rectangle(
                                col * CellSize,
                                row * CellSize,
                                CellSize,
                                CellSize
                            );

                            e.Graphics.FillRectangle(brush, rect);
                            e.Graphics.DrawRectangle(pen, rect);
                        }
                    }
                }
            }
        }

        private class JungleMenuColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground
            {
                get { return Color.FromArgb(86, 72, 46); }
            }

            public override Color MenuBorder
            {
                get { return Color.FromArgb(140, 130, 90); }
            }

            public override Color MenuItemBorder
            {
                get { return Color.FromArgb(150, 135, 85); }
            }

            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(110, 95, 60); }
            }

            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(110, 95, 60); }
            }

            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(110, 95, 60); }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(120, 100, 65); }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get { return Color.FromArgb(120, 100, 65); }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(120, 100, 65); }
            }

            public override Color ImageMarginGradientBegin
            {
                get { return Color.FromArgb(86, 72, 46); }
            }

            public override Color ImageMarginGradientMiddle
            {
                get { return Color.FromArgb(86, 72, 46); }
            }

            public override Color ImageMarginGradientEnd
            {
                get { return Color.FromArgb(86, 72, 46); }
            }
        }
    }
}