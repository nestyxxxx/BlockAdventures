using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public static class TaskChecker
    {
        public static bool CheckTask(TaskModel task, BonusColor?[,] fieldCells, int fieldCols, int fieldRows)
        {
            if (task.Type == TaskType.FillTopRowRed)
            {
                return IsFullRow(fieldCells, 0, fieldCols, BonusColor.Red);
            }

            if (task.Type == TaskType.FillBottomRowGreen)
            {
                return IsFullRow(fieldCells, fieldRows - 1, fieldCols, BonusColor.Green);
            }

            if (task.Type == TaskType.FillAnyRowYellow)
            {
                for (var row = 0; row < fieldRows; row++)
                {
                    if (IsFullRow(fieldCells, row, fieldCols, BonusColor.Yellow))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (task.Type == TaskType.FillAnyRowBlue)
            {
                for (var row = 0; row < fieldRows; row++)
                {
                    if (IsFullRow(fieldCells, row, fieldCols, BonusColor.Blue))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (task.Type == TaskType.FillLeftColumnRed)
            {
                return IsFullColumn(fieldCells, 0, fieldRows, BonusColor.Red);
            }

            if (task.Type == TaskType.FillRightColumnBlue)
            {
                return IsFullColumn(fieldCells, fieldCols - 1, fieldRows, BonusColor.Blue);
            }

            if (task.Type == TaskType.FillAnyColumnGreen)
            {
                for (var col = 0; col < fieldCols; col++)
                {
                    if (IsFullColumn(fieldCells, col, fieldRows, BonusColor.Green))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (task.Type == TaskType.FillCenterColumnYellow)
            {
                if (fieldCols % 2 == 1)
                {
                    return IsFullColumn(fieldCells, fieldCols / 2, fieldRows, BonusColor.Yellow);
                }

                var leftCenterCol = fieldCols / 2 - 1;
                var rightCenterCol = fieldCols / 2;

                return IsFullColumn(fieldCells, leftCenterCol, fieldRows, BonusColor.Yellow) ||
                       IsFullColumn(fieldCells, rightCenterCol, fieldRows, BonusColor.Yellow);
            }

            if (task.Type == TaskType.FillCornersRed)
            {
                return IsCellColor(fieldCells, 0, 0, BonusColor.Red) &&
                       IsCellColor(fieldCells, fieldCols - 1, 0, BonusColor.Red) &&
                       IsCellColor(fieldCells, 0, fieldRows - 1, BonusColor.Red) &&
                       IsCellColor(fieldCells, fieldCols - 1, fieldRows - 1, BonusColor.Red);
            }

            if (task.Type == TaskType.FillCornersBlue)
            {
                return IsCellColor(fieldCells, 0, 0, BonusColor.Blue) &&
                       IsCellColor(fieldCells, fieldCols - 1, 0, BonusColor.Blue) &&
                       IsCellColor(fieldCells, 0, fieldRows - 1, BonusColor.Blue) &&
                       IsCellColor(fieldCells, fieldCols - 1, fieldRows - 1, BonusColor.Blue);
            }

            if (task.Type == TaskType.FillTopCornersYellow)
            {
                return IsCellColor(fieldCells, 0, 0, BonusColor.Yellow) &&
                       IsCellColor(fieldCells, fieldCols - 1, 0, BonusColor.Yellow);
            }

            if (task.Type == TaskType.FillBottomCornersGreen)
            {
                return IsCellColor(fieldCells, 0, fieldRows - 1, BonusColor.Green) &&
                       IsCellColor(fieldCells, fieldCols - 1, fieldRows - 1, BonusColor.Green);
            }

            if (task.Type == TaskType.FillCenterGreen)
            {
                return IsCenterSquare(fieldCells, fieldCols, fieldRows, BonusColor.Green);
            }

            if (task.Type == TaskType.FillCenterYellow)
            {
                return IsCenterSquare(fieldCells, fieldCols, fieldRows, BonusColor.Yellow);
            }

            return false;
        }

        private static bool IsFullRow(BonusColor?[,] fieldCells, int row, int fieldCols, BonusColor neededColor)
        {
            for (var col = 0; col < fieldCols; col++)
            {
                if (!IsCellColor(fieldCells, col, row, neededColor))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFullColumn(BonusColor?[,] fieldCells, int col, int fieldRows, BonusColor neededColor)
        {
            for (var row = 0; row < fieldRows; row++)
            {
                if (!IsCellColor(fieldCells, col, row, neededColor))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsCenterSquare(BonusColor?[,] fieldCells, int fieldCols, int fieldRows, BonusColor neededColor)
        {
            var leftCenterCol = fieldCols / 2 - 1;
            var rightCenterCol = fieldCols / 2;
            var topCenterRow = fieldRows / 2 - 1;
            var bottomCenterRow = fieldRows / 2;

            return IsCellColor(fieldCells, leftCenterCol, topCenterRow, neededColor) &&
                   IsCellColor(fieldCells, rightCenterCol, topCenterRow, neededColor) &&
                   IsCellColor(fieldCells, leftCenterCol, bottomCenterRow, neededColor) &&
                   IsCellColor(fieldCells, rightCenterCol, bottomCenterRow, neededColor);
        }

        public static bool IsCellColor(BonusColor?[,] fieldCells, int col, int row, BonusColor neededColor)
        {
            if (!fieldCells[col, row].HasValue)
            {
                return false;
            }

            return fieldCells[col, row].Value == neededColor;
        }
    }
}