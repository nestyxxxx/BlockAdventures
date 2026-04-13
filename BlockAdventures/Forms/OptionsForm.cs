using System.Drawing;
using System.Windows.Forms;
using BlockAdventures.Styles;

namespace BlockAdventures
{
    public partial class OptionsForm : Form
    {
        private Form previousForm;

        private Label titleLabel;
        private Label volumeLabel;
        private Label volumeValueLabel;
        private Button backButton;

        private Panel sliderBack;
        private Panel sliderFill;
        private Panel sliderThumb;

        private bool isDragging;
        private int musicVolume = 50;

        public int MusicVolume
        {
            get { return musicVolume; }
        }

        public OptionsForm(Form previous, int currentVolume)
        {
            InitializeComponent();

            previousForm = previous;
            musicVolume = currentVolume;

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;

            CreateControls();
            UpdateLayout();
            SetVolume(musicVolume);

            Resize += (s, e) => UpdateLayout();
        }

        private void CreateControls()
        {
            titleLabel = new Label();
            titleLabel.Text = "Опции";
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Georgia", 36, FontStyle.Bold);
            StyleHelper.ApplyTitleStyle(titleLabel);

            volumeLabel = new Label();
            volumeLabel.Text = "Громкость музыки";
            volumeLabel.AutoSize = true;
            volumeLabel.Font = new Font("Georgia", 22, FontStyle.Bold);
            StyleHelper.ApplyTextStyle(volumeLabel);

            volumeValueLabel = new Label();
            volumeValueLabel.Text = "50%";
            volumeValueLabel.AutoSize = true;
            volumeValueLabel.Font = new Font("Georgia", 18, FontStyle.Bold);
            volumeValueLabel.ForeColor = Theme.TitleColor;
            volumeValueLabel.BackColor = Color.Transparent;

            sliderBack = new Panel();
            sliderBack.Size = new Size(420, 24);
            sliderBack.BackColor = Color.FromArgb(82, 73, 52);
            sliderBack.BorderStyle = BorderStyle.FixedSingle;
            sliderBack.MouseDown += SliderBack_MouseDown;

            sliderFill = new Panel();
            sliderFill.Height = 24;
            sliderFill.BackColor = Color.FromArgb(106, 140, 72);
            sliderBack.Controls.Add(sliderFill);

            sliderThumb = new Panel();
            sliderThumb.Size = new Size(28, 28);
            sliderThumb.BackColor = Color.FromArgb(176, 157, 97);
            sliderThumb.Cursor = Cursors.Hand;
            sliderThumb.Paint += SliderThumb_Paint;
            sliderThumb.MouseDown += (s, e) => isDragging = true;
            sliderThumb.MouseMove += SliderThumb_MouseMove;
            sliderThumb.MouseUp += (s, e) => isDragging = false;

            backButton = new Button();
            backButton.Text = "Назад";
            backButton.Size = new Size(250, 70);
            StyleHelper.ApplyMenuButtonStyle(backButton);
            backButton.Click += (s, e) =>
            {
                previousForm.Show();
                Close();
            };

            Controls.Add(titleLabel);
            Controls.Add(volumeLabel);
            Controls.Add(volumeValueLabel);
            Controls.Add(sliderBack);
            Controls.Add(sliderThumb);
            Controls.Add(backButton);
        }

        private void UpdateLayout()
        {
            var centerX = ClientSize.Width / 2;

            titleLabel.Location = new Point(centerX - titleLabel.Width / 2, 110);
            volumeLabel.Location = new Point(centerX - volumeLabel.Width / 2, 250);
            volumeValueLabel.Location = new Point(centerX - volumeValueLabel.Width / 2, 305);
            sliderBack.Location = new Point(centerX - sliderBack.Width / 2, 370);
            backButton.Location = new Point(centerX - backButton.Width / 2, 500);

            UpdateSliderView();
        }

        private void SetVolume(int value)
        {
            if (value < 0) value = 0;
            if (value > 100) value = 100;

            musicVolume = value;
            volumeValueLabel.Text = musicVolume + "%";
            UpdateSliderView();
        }

        private void UpdateSliderView()
        {
            var fillWidth = sliderBack.Width * musicVolume / 100;
            sliderFill.Width = fillWidth;

            var thumbX = sliderBack.Left + fillWidth - sliderThumb.Width / 2;
            var minX = sliderBack.Left - sliderThumb.Width / 2;
            var maxX = sliderBack.Right - sliderThumb.Width / 2;

            if (thumbX < minX) thumbX = minX;
            if (thumbX > maxX) thumbX = maxX;

            sliderThumb.Location = new Point(
                thumbX,
                sliderBack.Top + sliderBack.Height / 2 - sliderThumb.Height / 2
            );

            volumeValueLabel.Left = ClientSize.Width / 2 - volumeValueLabel.Width / 2;
        }

        private void SliderBack_MouseDown(object sender, MouseEventArgs e)
        {
            var value = e.X * 100 / sliderBack.Width;
            SetVolume(value);
        }

        private void SliderThumb_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging)
                return;

            var mouseX = PointToClient(Cursor.Position).X;
            var xInsideSlider = mouseX - sliderBack.Left;
            var value = xInsideSlider * 100 / sliderBack.Width;

            SetVolume(value);
        }

        private void SliderThumb_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (var brush = new SolidBrush(Color.FromArgb(176, 157, 97)))
            {
                e.Graphics.FillEllipse(brush, 0, 0, sliderThumb.Width - 1, sliderThumb.Height - 1);
            }

            using (var pen = new Pen(Color.FromArgb(110, 90, 50), 2))
            {
                e.Graphics.DrawEllipse(pen, 1, 1, sliderThumb.Width - 3, sliderThumb.Height - 3);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                previousForm.Show();
                Close();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}