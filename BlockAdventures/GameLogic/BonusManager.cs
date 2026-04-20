using System.Drawing;
using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public class BonusManager
    {
        private int redBonus = 0;
        private int greenBonus = 0;
        private int yellowBonus = 0;
        private int blueBonus = 0;

        public void AddProgress(BonusColor bonusColor, int value)
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
        }

        public bool IsReady(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                return redBonus == 100;
            }

            if (bonusColor == BonusColor.Green)
            {
                return greenBonus == 100;
            }

            if (bonusColor == BonusColor.Yellow)
            {
                return yellowBonus == 100;
            }

            if (bonusColor == BonusColor.Blue)
            {
                return blueBonus == 100;
            }

            return false;
        }

        public void Reset(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                redBonus = 0;
            }

            if (bonusColor == BonusColor.Green)
            {
                greenBonus = 0;
            }

            if (bonusColor == BonusColor.Yellow)
            {
                yellowBonus = 0;
            }

            if (bonusColor == BonusColor.Blue)
            {
                blueBonus = 0;
            }
        }

        public BonusColor GetClickedBonusColor(Point point, int panelWidth, int panelHeight)
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

        public void Draw(Graphics graphics, int panelWidth, int panelHeight)
        {
            var centerX = panelWidth / 2;
            var topY = 72;
            var size = 105;

            var top = new Point(centerX, topY);
            var left = new Point(centerX - size / 2, topY + size / 2);
            var right = new Point(centerX + size / 2, topY + size / 2);
            var bottom = new Point(centerX, topY + size);
            var center = new Point(centerX, topY + size / 2);

            DrawBonusPart(graphics, center, top, left, TaskChecker.GetRedColor(), redBonus);
            DrawBonusPart(graphics, center, top, right, TaskChecker.GetYellowColor(), yellowBonus);
            DrawBonusPart(graphics, center, left, bottom, TaskChecker.GetGreenColor(), greenBonus);
            DrawBonusPart(graphics, center, right, bottom, TaskChecker.GetBlueColor(), blueBonus);

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
    }
}