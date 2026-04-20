using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public partial class OptionsForm : Form
    {
        private Label titleLabel;
        private Panel volumePanel;
        private Label volumeLabel;
        private Label volumeValueLabel;
        private Label volumeLeftLabel;
        private Label volumeRightLabel;
        private VolumeSlider volumeSlider;
        private Button backButton;

        private Form menuForm;

        public int MusicVolume
        {
            get { return volumeSlider.Value; }
        }

        public OptionsForm(Form menu, int currentVolume)
        {
            InitializeComponent();

            menuForm = menu;

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();

            volumeSlider.Value = currentVolume;
            UpdateVolumeText();
            UpdateLayout();

            Resize += (s, e) => UpdateLayout();
        }

        private void CreateControls()
        {
            titleLabel = new Label();
            titleLabel.Text = "Опции";
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Georgia", 34, FontStyle.Bold);
            StyleHelper.ApplyTitleStyle(titleLabel);

            volumePanel = new Panel();
            volumePanel.Size = new Size(520, 180);
            volumePanel.BackColor = Color.FromArgb(118, 105, 74);
            volumePanel.BorderStyle = BorderStyle.FixedSingle;
            volumePanel.Paint += VolumePanel_Paint;

            volumeLabel = new Label();
            volumeLabel.Text = "Громкость музыки";
            volumeLabel.AutoSize = true;
            volumeLabel.Font = new Font("Georgia", 18, FontStyle.Bold);
            volumeLabel.ForeColor = Theme.TextColor;
            volumeLabel.BackColor = volumePanel.BackColor;

            volumeValueLabel = new Label();
            volumeValueLabel.AutoSize = true;
            volumeValueLabel.Font = new Font("Georgia", 18, FontStyle.Bold);
            volumeValueLabel.ForeColor = Theme.TitleColor;
            volumeValueLabel.BackColor = volumePanel.BackColor;

            volumeSlider = new VolumeSlider();
            volumeSlider.Size = new Size(400, 50);
            volumeSlider.ValueChanged += (s, e) =>
            {
                UpdateVolumeText();
                MusicManager.SetVolume(volumeSlider.Value);
            };

            volumeLeftLabel = new Label();
            volumeLeftLabel.Text = "Тише";
            volumeLeftLabel.AutoSize = true;
            volumeLeftLabel.Font = new Font("Georgia", 11, FontStyle.Bold);
            volumeLeftLabel.ForeColor = Color.FromArgb(220, 210, 180);
            volumeLeftLabel.BackColor = volumePanel.BackColor;

            volumeRightLabel = new Label();
            volumeRightLabel.Text = "Громче";
            volumeRightLabel.AutoSize = true;
            volumeRightLabel.Font = new Font("Georgia", 11, FontStyle.Bold);
            volumeRightLabel.ForeColor = Color.FromArgb(220, 210, 180);
            volumeRightLabel.BackColor = volumePanel.BackColor;

            backButton = new Button();
            backButton.Text = "В главное меню";
            backButton.Size = new Size(320, 80);
            backButton.Font = new Font("Georgia", 16, FontStyle.Bold);
            StyleHelper.ApplyMenuButtonStyle(backButton);

            backButton.Click += (s, e) =>
            {
                menuForm.Show();
                Close();
            };

            volumePanel.Controls.Add(volumeLabel);
            volumePanel.Controls.Add(volumeValueLabel);
            volumePanel.Controls.Add(volumeSlider);
            volumePanel.Controls.Add(volumeLeftLabel);
            volumePanel.Controls.Add(volumeRightLabel);

            Controls.Add(titleLabel);
            Controls.Add(volumePanel);
            Controls.Add(backButton);
        }

        private void VolumePanel_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(150, 135, 95), 2))
            {
                var rect = new Rectangle(6, 6, volumePanel.Width - 13, volumePanel.Height - 13);
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void UpdateVolumeText()
        {
            volumeValueLabel.Text = volumeSlider.Value + "%";

            if (volumePanel != null)
            {
                volumeValueLabel.Location = new Point(
                    volumePanel.Width - volumeValueLabel.Width - 28,
                    24
                );
            }
        }

        private void UpdateLayout()
        {
            var centerX = ClientSize.Width / 2;

            titleLabel.Location = new Point(
                centerX - titleLabel.Width / 2,
                110
            );

            volumePanel.Location = new Point(
                centerX - volumePanel.Width / 2,
                255
            );

            volumeLabel.Location = new Point(28, 24);

            volumeSlider.Location = new Point(60, 82);

            volumeLeftLabel.Location = new Point(
                volumeSlider.Left,
                volumeSlider.Bottom + 10
            );

            volumeRightLabel.Location = new Point(
                volumeSlider.Right - volumeRightLabel.Width,
                volumeSlider.Bottom + 10
            );

            backButton.Location = new Point(
                centerX - backButton.Width / 2,
                volumePanel.Bottom + 65
            );

            UpdateVolumeText();
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

        private class VolumeSlider : Control
        {
            private int value = 50;
            private bool isDragging;

            public event EventHandler ValueChanged;

            public int Value
            {
                get { return value; }
                set
                {
                    var newValue = value;

                    if (newValue < 0)
                    {
                        newValue = 0;
                    }

                    if (newValue > 100)
                    {
                        newValue = 100;
                    }

                    if (this.value == newValue)
                    {
                        return;
                    }

                    this.value = newValue;
                    Invalidate();

                    if (ValueChanged != null)
                    {
                        ValueChanged(this, EventArgs.Empty);
                    }
                }
            }

            public VolumeSlider()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);

                BackColor = Color.FromArgb(118, 105, 74);
                Cursor = Cursors.Hand;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.Clear(BackColor);

                var trackRect = new Rectangle(10, Height / 2 - 6, Width - 20, 12);
                var fillWidth = (int)(trackRect.Width * (Value / 100f));
                var fillRect = new Rectangle(trackRect.X, trackRect.Y, fillWidth, trackRect.Height);

                using (var trackBrush = new SolidBrush(Color.FromArgb(92, 80, 54)))
                using (var fillBrush = new SolidBrush(Color.FromArgb(190, 166, 88)))
                using (var trackPen = new Pen(Color.FromArgb(70, 58, 38), 2))
                using (var knobBrush = new SolidBrush(Color.FromArgb(225, 205, 130)))
                using (var knobPen = new Pen(Color.FromArgb(95, 78, 45), 2))
                {
                    FillRoundedRect(e.Graphics, trackBrush, trackRect, 6);

                    if (fillRect.Width > 0)
                    {
                        FillRoundedRect(e.Graphics, fillBrush, fillRect, 6);
                    }

                    DrawRoundedRect(e.Graphics, trackPen, trackRect, 6);

                    var knobSize = 20;
                    var knobX = trackRect.X + fillWidth - knobSize / 2;

                    if (knobX < trackRect.X - knobSize / 2)
                    {
                        knobX = trackRect.X - knobSize / 2;
                    }

                    if (knobX > trackRect.Right - knobSize / 2)
                    {
                        knobX = trackRect.Right - knobSize / 2;
                    }

                    var knobRect = new Rectangle(
                        knobX,
                        Height / 2 - knobSize / 2,
                        knobSize,
                        knobSize
                    );

                    e.Graphics.FillEllipse(knobBrush, knobRect);
                    e.Graphics.DrawEllipse(knobPen, knobRect);
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button != MouseButtons.Left)
                {
                    return;
                }

                isDragging = true;
                UpdateValueFromMouse(e.X);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (!isDragging)
                {
                    return;
                }

                UpdateValueFromMouse(e.X);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                isDragging = false;
            }

            private void UpdateValueFromMouse(int mouseX)
            {
                var left = 10;
                var width = Width - 20;

                var x = mouseX;

                if (x < left)
                {
                    x = left;
                }

                if (x > left + width)
                {
                    x = left + width;
                }

                var percent = (x - left) / (float)width;
                Value = (int)Math.Round(percent * 100);
            }

            private void FillRoundedRect(Graphics graphics, Brush brush, Rectangle rect, int radius)
            {
                using (var path = CreateRoundedRect(rect, radius))
                {
                    graphics.FillPath(brush, path);
                }
            }

            private void DrawRoundedRect(Graphics graphics, Pen pen, Rectangle rect, int radius)
            {
                using (var path = CreateRoundedRect(rect, radius))
                {
                    graphics.DrawPath(pen, path);
                }
            }

            private GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
            {
                var path = new GraphicsPath();
                var size = radius * 2;

                path.AddArc(rect.X, rect.Y, size, size, 180, 90);
                path.AddArc(rect.Right - size, rect.Y, size, size, 270, 90);
                path.AddArc(rect.Right - size, rect.Bottom - size, size, size, 0, 90);
                path.AddArc(rect.X, rect.Bottom - size, size, size, 90, 90);
                path.CloseFigure();

                return path;
            }
        }
    }
}