using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using BlockAdventures.Styles;

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

        private PictureBox archaeologistPicture;

        private Button settingsButton;
        private ContextMenuStrip settingsMenu;

        private int musicVolume = 50;
        private int score = 0;
        private int scoreBarMax = 300;

        private int fieldCols = 10;
        private int fieldRows = 8;
        private int cellSize = 68;
        private int fieldOffsetX = 90;

        private FieldManager fieldManager;
        private BonusManager bonusManager;

        private FigureModel currentFigure;
        private TaskModel currentTask;

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

            fieldManager = new FieldManager(fieldCols, fieldRows);
            bonusManager = new BonusManager();

            currentFigure = FigureGenerator.Generate();
            currentTask = TaskGenerator.Generate();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();
            CreateDragSystem();

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

            archaeologistPicture = new PictureBox();
            archaeologistPicture.Size = new Size(380, 520);
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

            var mainMenuItem = new ToolStripMenuItem("В главное меню");
            var optionsItem = new ToolStripMenuItem("Опции");

            StyleMenuItem(mainMenuItem);
            StyleMenuItem(optionsItem);

            mainMenuItem.Click += (s, e) =>
            {
                menuForm.Show();
                Close();
            };

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

            settingsMenu.Items.Add(mainMenuItem);
            settingsMenu.Items.Add(optionsItem);

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

        private void StyleMenuItem(ToolStripMenuItem item)
        {
            item.BackColor = Color.FromArgb(86, 72, 46);
            item.ForeColor = Color.FromArgb(236, 227, 198);
        }

        private void LoadArchaeologistImage()
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, @"..\..\Resources\archaeologist.png");
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

            archaeologistPicture.Location = new Point(
                fieldLeft - archaeologistPicture.Width - 30,
                fieldTop - 145
            );

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
                fieldLeft - bonusPanel.Width - 30,
                fieldTop + fieldHeight - 170
            );
            bonusTitle.Location = new Point(bonusPanel.Width / 2 - bonusTitle.Width / 2, 12);

            scoreBar.Invalidate();
            bonusPanel.Invalidate();
        }

        private void UpdateTaskPanel()
        {
            taskText.Text = currentTask.Text;
            taskChangeCostLabel.Text = GetTaskChangeCost().ToString();
            UpdateCosts();
        }

        private void UpdateCosts()
        {
            figureChangeCostLabel.Text = GetFigureChangeCost().ToString();
            taskChangeCostLabel.Text = GetTaskChangeCost().ToString();
        }

        private void UpdateScoreView()
        {
            scoreBar.Invalidate();
            UpdateCosts();
        }

        private void ScoreBar_Paint(object sender, PaintEventArgs e)
        {
            var fillWidth = score * scoreBar.Width / scoreBarMax;

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

            var text = score.ToString();
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

        private int GetFigureChangeCost()
        {
            if (score <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling(score * 0.10);
        }

        private int GetTaskChangeCost()
        {
            if (score <= 0)
            {
                return 0;
            }

            return (int)Math.Ceiling(score * 0.20);
        }

        private void AddScore(int points)
        {
            score += points;
            UpdateScoreView();
        }

        private bool TrySpendScore(int points)
        {
            if (score < points)
            {
                return false;
            }

            score -= points;
            UpdateScoreView();
            return true;
        }

        private void ChangeFigure()
        {
            var changeCost = GetFigureChangeCost();

            if (!TrySpendScore(changeCost))
            {
                MessageBox.Show("Недостаточно очков");
                return;
            }

            currentFigure = FigureGenerator.Generate();
            blocksPanel.Invalidate();
            Invalidate();

            CheckGameOver();
        }

        private void ChangeTask()
        {
            var changeCost = GetTaskChangeCost();

            if (!TrySpendScore(changeCost))
            {
                MessageBox.Show("Недостаточно очков");
                return;
            }

            currentTask = TaskGenerator.Generate();
            UpdateTaskPanel();
            Invalidate();
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

                        var color = fieldManager.GetCellColor(col, row, Theme.FieldCellColor);

                        using (var cellBrush = new SolidBrush(color))
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
            if (currentFigure == null)
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

            using (var brush = new SolidBrush(currentFigure.Color))
            using (var pen = new Pen(Color.FromArgb(70, 55, 25), 2))
            {
                for (var row = 0; row < 3; row++)
                {
                    for (var col = 0; col < 3; col++)
                    {
                        if (!currentFigure.Cells[col, row])
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
                    if (!currentFigure.Cells[col, row])
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

            dragGhost.SetFigure(currentFigure, cellSize);
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

            canPlaceFigureHere = fieldManager.CanPutFigure(currentFigure, previewStartX, previewStartY);
        }

        private void PutFigureOnField(int startCol, int startRow)
        {
            fieldManager.PutFigure(currentFigure, startCol, startRow);

            var taskIsDone = TaskChecker.CheckTask(
                currentTask,
                fieldManager.Cells,
                fieldCols,
                fieldRows
            );

            if (taskIsDone)
            {
                AddScore(currentTask.Reward);
                bonusManager.AddProgress(currentTask.BonusColor, 25);

                currentTask = TaskGenerator.Generate();
                UpdateTaskPanel();

                bonusPanel.Invalidate();
            }

            fieldManager.ClearFilledRows();
            fieldManager.ClearFilledColumns();

            currentFigure = FigureGenerator.Generate();

            blocksPanel.Invalidate();
            bonusPanel.Invalidate();

            CheckGameOver();
            Invalidate();
        }

        private void CheckGameOver()
        {
            if (fieldManager.HasAnyPlaceForFigure(currentFigure))
            {
                return;
            }

            MessageBox.Show("Больше нет места для фигуры. Игра окончена.");
            menuForm.Show();
            Close();
        }

        private void BonusPanel_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedColor = bonusManager.GetClickedBonusColor(e.Location, bonusPanel.Width, bonusPanel.Height);

            if (!bonusManager.IsReady(clickedColor))
            {
                return;
            }

            var color = TaskChecker.GetColorByBonus(clickedColor);

            fieldManager.ClearColor(color);
            bonusManager.Reset(clickedColor);

            bonusPanel.Invalidate();
            Invalidate();
        }

        private void BonusPanel_Paint(object sender, PaintEventArgs e)
        {
            bonusManager.Draw(e.Graphics, bonusPanel.Width, bonusPanel.Height);
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
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.ResizeRedraw, true);
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

                var drawColor = Figure.Color;

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