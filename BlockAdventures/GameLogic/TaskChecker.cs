using System.Drawing;
using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public static class TaskChecker
    {
        public static bool CheckTask(TaskModel task, Color?[,] fieldCells, int fieldCols, int fieldRows)
        {
            if (task.Type == TaskType.FillCornersRed)
            {
                var red = GetRedColor();

                return IsCellColor(fieldCells, 0, 0, red) &&
                       IsCellColor(fieldCells, fieldCols - 1, 0, red) &&
                       IsCellColor(fieldCells, 0, fieldRows - 1, red) &&
                       IsCellColor(fieldCells, fieldCols - 1, fieldRows - 1, red);
            }

            if (task.Type == TaskType.FillRowRed)
            {
                var red = GetRedColor();

                for (var row = 0; row < fieldRows; row++)
                {
                    var fullRedRow = true;

                    for (var col = 0; col < fieldCols; col++)
                    {
                        if (!IsCellColor(fieldCells, col, row, red))
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

            if (task.Type == TaskType.FillRightColumnBlue)
            {
                var blue = GetBlueColor();
                var lastCol = fieldCols - 1;

                for (var row = 0; row < fieldRows; row++)
                {
                    if (!IsCellColor(fieldCells, lastCol, row, blue))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (task.Type == TaskType.FillCornersBlue)
            {
                var blue = GetBlueColor();

                return IsCellColor(fieldCells, 0, 0, blue) &&
                       IsCellColor(fieldCells, fieldCols - 1, 0, blue) &&
                       IsCellColor(fieldCells, 0, fieldRows - 1, blue) &&
                       IsCellColor(fieldCells, fieldCols - 1, fieldRows - 1, blue);
            }

            if (task.Type == TaskType.FillCenterGreen)
            {
                var green = GetGreenColor();

                return IsCellColor(fieldCells, 4, 3, green) &&
                       IsCellColor(fieldCells, 5, 3, green) &&
                       IsCellColor(fieldCells, 4, 4, green) &&
                       IsCellColor(fieldCells, 5, 4, green);
            }

            if (task.Type == TaskType.FillColumnGreen)
            {
                var green = GetGreenColor();

                for (var col = 0; col < fieldCols; col++)
                {
                    var fullGreenColumn = true;

                    for (var row = 0; row < fieldRows; row++)
                    {
                        if (!IsCellColor(fieldCells, col, row, green))
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

            if (task.Type == TaskType.FillTopRowYellow)
            {
                var yellow = GetYellowColor();

                for (var col = 0; col < fieldCols; col++)
                {
                    if (!IsCellColor(fieldCells, col, 0, yellow))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (task.Type == TaskType.FillCenterYellow)
            {
                var yellow = GetYellowColor();

                return IsCellColor(fieldCells, 4, 3, yellow) &&
                       IsCellColor(fieldCells, 5, 3, yellow) &&
                       IsCellColor(fieldCells, 4, 4, yellow) &&
                       IsCellColor(fieldCells, 5, 4, yellow);
            }

            return false;
        }

        public static bool IsCellColor(Color?[,] fieldCells, int col, int row, Color neededColor)
        {
            if (!fieldCells[col, row].HasValue)
            {
                return false;
            }

            return fieldCells[col, row].Value.ToArgb() == neededColor.ToArgb();
        }

        public static Color GetColorByBonus(BonusColor bonusColor)
        {
            if (bonusColor == BonusColor.Red)
            {
                return GetRedColor();
            }

            if (bonusColor == BonusColor.Green)
            {
                return GetGreenColor();
            }

            if (bonusColor == BonusColor.Yellow)
            {
                return GetYellowColor();
            }

            return GetBlueColor();
        }

        public static Color GetRedColor()
        {
            return Color.FromArgb(196, 72, 56);
        }

        public static Color GetGreenColor()
        {
            return Color.FromArgb(92, 176, 78);
        }

        public static Color GetYellowColor()
        {
            return Color.FromArgb(222, 198, 68);
        }

        public static Color GetBlueColor()
        {
            return Color.FromArgb(78, 180, 220);
        }
    }
}