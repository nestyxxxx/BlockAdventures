using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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

        private Color?[,] fieldCells;
        private FigureModel currentFigure;
        private TaskModel currentTask;

        private bool isDraggingFigure;
        private Point mousePointOnForm;
        private Point dragOffset;

        private int grabbedFigureCellX = 0;
        private int grabbedFigureCellY = 0;

        private int previewStartX = -1;
        private int previewStartY = -1;
        private bool canPlaceFigureHere;

        private int redBonus = 0;
        private int greenBonus = 0;
        private int yellowBonus = 0;
        private int blueBonus = 0;

        public PlayForm(Form menu, int currentVolume)
        {
            InitializeComponent();

            menuForm = menu;
            musicVolume = currentVolume;

            fieldCells = new Color?[fieldCols, fieldRows];
            currentFigure = FigureGenerator.Generate();
            currentTask = TaskGenerator.Generate();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();
            UpdateLayout();
            UpdateTaskPanel();
            UpdateScoreView();

            Resize += (s, e) =>
            {
                UpdateLayout();
                UpdateScoreView();
                Invalidate();
            };

            MouseMove += PlayForm_MouseMove;
            MouseUp += PlayForm_MouseUp;

            CheckGameOver();
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

                optionsForm.FormClosed += (sender, args) =>
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

            archaeologistPicture.Location = new Point(fieldLeft - 310, fieldTop - 145);

            blocksPanel.Location = new Point(fieldLeft + fieldWidth + 55, fieldTop - 10);
            blocksTitle.Location = new Point(blocksPanel.Width / 2 - blocksTitle.Width / 2, 14);
            figureChangeCostLabel.Location = new Point(blocksPanel.Width / 2 - figureChangeCostLabel.Width / 2, 215);
            changeFigureButton.Location = new Point(blocksPanel.Width - 70, blocksPanel.Height - 48);

            taskPanel.Location = new Point(fieldLeft + fieldWidth + 55, fieldTop + 290);
            taskTitle.Location = new Point(taskPanel.Width / 2 - taskTitle.Width / 2, 14);
            taskText.Location = new Point(taskPanel.Width / 2 - taskText.Width / 2, 80);
            taskChangeCostLabel.Location = new Point(taskPanel.Width / 2 - taskChangeCostLabel.Width / 2, 260);
            changeTaskButton.Location = new Point(taskPanel.Width - 70, taskPanel.Height - 48);

            bonusPanel.Location = new Point(fieldLeft - 330, fieldTop + fieldHeight - 170);
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

            if (isDraggingFigure)
            {
                DrawDraggedFigure(e.Graphics);
            }
        }

        private void DrawField(Graphics graphics)
        {
            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();
            var fieldWidth = fieldCols * cellSize;
            var fieldHeight = fieldRows * cellSize;

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

                        var color = fieldCells[col, row] ?? Theme.FieldCellColor;

                        using (var cellBrush = new SolidBrush(color))
                        {
                            graphics.FillRectangle(cellBrush, cellRect);
                        }

                        graphics.DrawRectangle(borderPen, cellRect);
                    }
                }

                graphics.DrawRectangle(outerPen, fieldLeft - 2, fieldTop - 2, fieldWidth + 3, fieldHeight + 3);
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

        private void DrawDraggedFigure(Graphics graphics)
        {
            if (currentFigure == null)
            {
                return;
            }

            var previewColor = currentFigure.Color;

            if (!canPlaceFigureHere)
            {
                previewColor = Color.FromArgb(140, 140, 140);
            }

            var drawLeft = mousePointOnForm.X - dragOffset.X;
            var drawTop = mousePointOnForm.Y - dragOffset.Y;

            using (var brush = new SolidBrush(Color.FromArgb(180, previewColor)))
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
                            drawLeft + col * cellSize,
                            drawTop + row * cellSize,
                            cellSize,
                            cellSize
                        );

                        graphics.FillRectangle(brush, rect);
                        graphics.DrawRectangle(pen, rect);
                    }
                }
            }
        }

        private void BlocksPanel_MouseDown(object sender, MouseEventArgs e)
        {
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

            isDraggingFigure = true;
            mousePointOnForm = PointToClient(Cursor.Position);

            grabbedFigureCellX = clickedCellX;
            grabbedFigureCellY = clickedCellY;

            var minCol = GetFigureMinCol();
            var minRow = GetFigureMinRow();
            var maxCol = GetFigureMaxCol();
            var maxRow = GetFigureMaxRow();

            var figureWidthCells = maxCol - minCol + 1;
            var figureHeightCells = maxRow - minRow + 1;

            var figureWidth = figureWidthCells * cellSize;
            var figureHeight = figureHeightCells * cellSize;

            dragOffset = new Point(figureWidth / 2, figureHeight / 2);

            previewStartX = -1;
            previewStartY = -1;
            canPlaceFigureHere = false;

            UpdatePreviewPosition();
            Invalidate();
        }

        private void PlayForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingFigure)
            {
                return;
            }

            mousePointOnForm = PointToClient(Cursor.Position);
            UpdatePreviewPosition();
            Invalidate();
        }

        private void PlayForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDraggingFigure)
            {
                return;
            }

            if (canPlaceFigureHere)
            {
                PutFigureOnField(previewStartX, previewStartY);
            }

            isDraggingFigure = false;
            previewStartX = -1;
            previewStartY = -1;
            canPlaceFigureHere = false;

            Invalidate();
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

        private int GetFigureMinCol()
        {
            var minCol = 3;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (currentFigure.Cells[col, row] && col < minCol)
                    {
                        minCol = col;
                    }
                }
            }

            return minCol;
        }

        private int GetFigureMinRow()
        {
            var minRow = 3;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (currentFigure.Cells[col, row] && row < minRow)
                    {
                        minRow = row;
                    }
                }
            }

            return minRow;
        }

        private int GetFigureMaxCol()
        {
            var maxCol = 0;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (currentFigure.Cells[col, row] && col > maxCol)
                    {
                        maxCol = col;
                    }
                }
            }

            return maxCol;
        }

        private int GetFigureMaxRow()
        {
            var maxRow = 0;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (currentFigure.Cells[col, row] && row > maxRow)
                    {
                        maxRow = row;
                    }
                }
            }

            return maxRow;
        }

        private bool IsMouseNearField()
        {
            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();
            var fieldWidth = fieldCols * cellSize;
            var fieldHeight = fieldRows * cellSize;

            return
                mousePointOnForm.X >= fieldLeft - cellSize * 5 &&
                mousePointOnForm.X <= fieldLeft + fieldWidth + cellSize * 5 &&
                mousePointOnForm.Y >= fieldTop - cellSize * 3 &&
                mousePointOnForm.Y <= fieldTop + fieldHeight + cellSize * 3;
        }

        private void UpdatePreviewPosition()
        {
            if (!IsMouseNearField())
            {
                previewStartX = -1;
                previewStartY = -1;
                canPlaceFigureHere = false;
                return;
            }

            var fieldLeft = GetFieldLeft();
            var fieldTop = GetFieldTop();

            var cellUnderMouseX = (int)Math.Floor((double)(mousePointOnForm.X - fieldLeft) / cellSize);
            var cellUnderMouseY = (int)Math.Floor((double)(mousePointOnForm.Y - fieldTop) / cellSize);

            var startCol = cellUnderMouseX - grabbedFigureCellX;
            var startRow = cellUnderMouseY - grabbedFigureCellY;

            var minCol = GetFigureMinCol();
            var minRow = GetFigureMinRow();
            var maxCol = GetFigureMaxCol();
            var maxRow = GetFigureMaxRow();

            var minStartCol = -minCol;
            var minStartRow = -minRow;
            var maxStartCol = fieldCols - 1 - maxCol;
            var maxStartRow = fieldRows - 1 - maxRow;

            if (startCol < minStartCol)
            {
                startCol = minStartCol;
            }

            if (startRow < minStartRow)
            {
                startRow = minStartRow;
            }

            if (startCol > maxStartCol)
            {
                startCol = maxStartCol;
            }

            if (startRow > maxStartRow)
            {
                startRow = maxStartRow;
            }

            previewStartX = startCol;
            previewStartY = startRow;
            canPlaceFigureHere = CanPutFigure(previewStartX, previewStartY);
        }

        private void PutFigureOnField(int startCol, int startRow)
        {
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!currentFigure.Cells[col, row])
                    {
                        continue;
                    }

                    fieldCells[startCol + col, startRow + row] = currentFigure.Color;
                }
            }

            var taskIsDone = CheckTaskByField();

            if (taskIsDone)
            {
                AddScore(currentTask.Reward);
                AddBonusProgress(currentTask.BonusColor, 25);

                currentTask = TaskGenerator.Generate();
                UpdateTaskPanel();
            }

            ClearFilledRows();
            ClearFilledColumns();

            currentFigure = FigureGenerator.Generate();
            blocksPanel.Invalidate();

            CheckGameOver();
            Invalidate();
        }

        private void AddBonusProgress(BonusColor bonusColor, int value)
        {
            if (bonusColor == BonusColor.Red)
            {
                redBonus += value;
                if (redBonus > 100)
                {
                    redBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Green)
            {
                greenBonus += value;
                if (greenBonus > 100)
                {
                    greenBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Yellow)
            {
                yellowBonus += value;
                if (yellowBonus > 100)
                {
                    yellowBonus = 100;
                }
            }

            if (bonusColor == BonusColor.Blue)
            {
                blueBonus += value;
                if (blueBonus > 100)
                {
                    blueBonus = 100;
                }
            }

            bonusPanel.Invalidate();
        }

        private bool CheckTaskByField()
        {
            if (currentTask.Type == TaskType.FillCornersRed)
            {
                var red = GetRedColor();

                return IsCellColor(0, 0, red) &&
                       IsCellColor(fieldCols - 1, 0, red) &&
                       IsCellColor(0, fieldRows - 1, red) &&
                       IsCellColor(fieldCols - 1, fieldRows - 1, red);
            }

            if (currentTask.Type == TaskType.FillRowRed)
            {
                var red = GetRedColor();

                for (var row = 0; row < fieldRows; row++)
                {
                    var fullRedRow = true;

                    for (var col = 0; col < fieldCols; col++)
                    {
                        if (!IsCellColor(col, row, red))
                        {
                            fullRedRow = false;
                            break;
                        }
                    }

                    if (fullRedRow)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (currentTask.Type == TaskType.FillRightColumnBlue)
            {
                var blue = GetBlueColor();
                var lastCol = fieldCols - 1;

                for (var row = 0; row < fieldRows; row++)
                {
                    if (!IsCellColor(lastCol, row, blue))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (currentTask.Type == TaskType.FillCornersBlue)
            {
                var blue = GetBlueColor();

                return IsCellColor(0, 0, blue) &&
                       IsCellColor(fieldCols - 1, 0, blue) &&
                       IsCellColor(0, fieldRows - 1, blue) &&
                       IsCellColor(fieldCols - 1, fieldRows - 1, blue);
            }

            if (currentTask.Type == TaskType.FillCenterGreen)
            {
                var green = GetGreenColor();

                return IsCellColor(4, 3, green) &&
                       IsCellColor(5, 3, green) &&
                       IsCellColor(4, 4, green) &&
                       IsCellColor(5, 4, green);
            }

            if (currentTask.Type == TaskType.FillColumnGreen)
            {
                var green = GetGreenColor();

                for (var col = 0; col < fieldCols; col++)
                {
                    var fullGreenColumn = true;

                    for (var row = 0; row < fieldRows; row++)
                    {
                        if (!IsCellColor(col, row, green))
                        {
                            fullGreenColumn = false;
                            break;
                        }
                    }

                    if (fullGreenColumn)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (currentTask.Type == TaskType.FillTopRowYellow)
            {
                var yellow = GetYellowColor();

                for (var col = 0; col < fieldCols; col++)
                {
                    if (!IsCellColor(col, 0, yellow))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (currentTask.Type == TaskType.FillCenterYellow)
            {
                var yellow = GetYellowColor();

                return IsCellColor(4, 3, yellow) &&
                       IsCellColor(5, 3, yellow) &&
                       IsCellColor(4, 4, yellow) &&
                       IsCellColor(5, 4, yellow);
            }

            return false;
        }

        private Color GetRedColor()
        {
            return Color.FromArgb(196, 72, 56);
        }

        private Color GetGreenColor()
        {
            return Color.FromArgb(92, 176, 78);
        }

        private Color GetYellowColor()
        {
            return Color.FromArgb(222, 198, 68);
        }

        private Color GetBlueColor()
        {
            return Color.FromArgb(78, 180, 220);
        }

        private bool IsCellColor(int col, int row, Color neededColor)
        {
            if (!fieldCells[col, row].HasValue)
            {
                return false;
            }

            return fieldCells[col, row].Value.ToArgb() == neededColor.ToArgb();
        }

        private void BonusPanel_MouseClick(object sender, MouseEventArgs e)
        {
            var clickedColor = GetClickedBonusColor(e.Location);

            if (clickedColor == BonusColor.Red && redBonus == 100)
            {
                ClearColorFromField(GetRedColor());
                redBonus = 0;
            }

            if (clickedColor == BonusColor.Yellow && yellowBonus == 100)
            {
                ClearColorFromField(GetYellowColor());
                yellowBonus = 0;
            }

            if (clickedColor == BonusColor.Green && greenBonus == 100)
            {
                ClearColorFromField(GetGreenColor());
                greenBonus = 0;
            }

            if (clickedColor == BonusColor.Blue && blueBonus == 100)
            {
                ClearColorFromField(GetBlueColor());
                blueBonus = 0;
            }

            bonusPanel.Invalidate();
            Invalidate();
        }

        private BonusColor GetClickedBonusColor(Point point)
        {
            var centerX = bonusPanel.Width / 2;
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

        private void ClearColorFromField(Color color)
        {
            for (var row = 0; row < fieldRows; row++)
            {
                for (var col = 0; col < fieldCols; col++)
                {
                    if (IsCellColor(col, row, color))
                    {
                        fieldCells[col, row] = null;
                    }
                }
            }
        }

        private int ClearFilledRows()
        {
            var clearedRows = 0;

            for (var row = 0; row < fieldRows; row++)
            {
                var rowIsFull = true;

                for (var col = 0; col < fieldCols; col++)
                {
                    if (!fieldCells[col, row].HasValue)
                    {
                        rowIsFull = false;
                        break;
                    }
                }

                if (!rowIsFull)
                {
                    continue;
                }

                clearedRows++;

                for (var col = 0; col < fieldCols; col++)
                {
                    fieldCells[col, row] = null;
                }
            }

            return clearedRows;
        }

        private int ClearFilledColumns()
        {
            var clearedColumns = 0;

            for (var col = 0; col < fieldCols; col++)
            {
                var columnIsFull = true;

                for (var row = 0; row < fieldRows; row++)
                {
                    if (!fieldCells[col, row].HasValue)
                    {
                        columnIsFull = false;
                        break;
                    }
                }

                if (!columnIsFull)
                {
                    continue;
                }

                clearedColumns++;

                for (var row = 0; row < fieldRows; row++)
                {
                    fieldCells[col, row] = null;
                }
            }

            return clearedColumns;
        }

        private bool CanPutFigure(int startCol, int startRow)
        {
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!currentFigure.Cells[col, row])
                    {
                        continue;
                    }

                    var fieldCol = startCol + col;
                    var fieldRow = startRow + row;

                    if (fieldCol < 0 || fieldCol >= fieldCols || fieldRow < 0 || fieldRow >= fieldRows)
                    {
                        return false;
                    }

                    if (fieldCells[fieldCol, fieldRow].HasValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool HasAnyPlaceForFigure()
        {
            var minCol = GetFigureMinCol();
            var minRow = GetFigureMinRow();
            var maxCol = GetFigureMaxCol();
            var maxRow = GetFigureMaxRow();

            var minStartCol = -minCol;
            var minStartRow = -minRow;
            var maxStartCol = fieldCols - 1 - maxCol;
            var maxStartRow = fieldRows - 1 - maxRow;

            for (var row = minStartRow; row <= maxStartRow; row++)
            {
                for (var col = minStartCol; col <= maxStartCol; col++)
                {
                    if (CanPutFigure(col, row))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void CheckGameOver()
        {
            if (HasAnyPlaceForFigure())
            {
                return;
            }

            MessageBox.Show("Больше нет места для фигуры. Игра окончена.");
            menuForm.Show();
            Close();
        }

        private void BonusPanel_Paint(object sender, PaintEventArgs e)
        {
            var graphics = e.Graphics;

            var centerX = bonusPanel.Width / 2;
            var topY = 72;
            var size = 105;

            var top = new Point(centerX, topY);
            var left = new Point(centerX - size / 2, topY + size / 2);
            var right = new Point(centerX + size / 2, topY + size / 2);
            var bottom = new Point(centerX, topY + size);
            var center = new Point(centerX, topY + size / 2);

            DrawBonusPart(graphics, center, top, left, GetRedColor(), redBonus);
            DrawBonusPart(graphics, center, top, right, GetYellowColor(), yellowBonus);
            DrawBonusPart(graphics, center, left, bottom, GetGreenColor(), greenBonus);
            DrawBonusPart(graphics, center, right, bottom, GetBlueColor(), blueBonus);

            using (var pen = new Pen(Color.FromArgb(60, 50, 30), 3))
            {
                graphics.DrawPolygon(pen, new[] { top, left, bottom, right });
                graphics.DrawLine(pen, top, bottom);
                graphics.DrawLine(pen, left, right);
            }
        }

        private void DrawBonusPart(Graphics graphics, Point center, Point p1, Point p2, Color color, int progress)
        {
            if (progress <= 0)
            {
                return;
            }

            var k = progress / 100f;

            var newP1 = GetScaledPoint(center, p1, k);
            var newP2 = GetScaledPoint(center, p2, k);

            using (var brush = new SolidBrush(color))
            {
                graphics.FillPolygon(brush, new[] { center, newP1, newP2 });
            }
        }

        private Point GetScaledPoint(Point center, Point target, float k)
        {
            var x = center.X + (int)((target.X - center.X) * k);
            var y = center.Y + (int)((target.Y - center.Y) * k);

            return new Point(x, y);
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
                get { return Color.FromArgb(110, 95, 60); }ё
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