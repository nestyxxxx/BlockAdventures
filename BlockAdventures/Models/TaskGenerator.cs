using System;

namespace BlockAdventures.Models
{
    public static class TaskGenerator
    {
        private static readonly Random random = new Random();

        public static TaskModel Generate()
        {
            var value = random.Next(14);

            if (value == 0)
            {
                return new TaskModel(
                    TaskType.FillTopRowRed,
                    "Заполни верхний ряд\nкрасными блоками",
                    80,
                    BonusColor.Red
                );
            }

            if (value == 1)
            {
                return new TaskModel(
                    TaskType.FillBottomRowGreen,
                    "Заполни нижний ряд\nзелёными блоками",
                    80,
                    BonusColor.Green
                );
            }

            if (value == 2)
            {
                return new TaskModel(
                    TaskType.FillAnyRowYellow,
                    "Заполни любой ряд\nжёлтыми блоками",
                    70,
                    BonusColor.Yellow
                );
            }

            if (value == 3)
            {
                return new TaskModel(
                    TaskType.FillAnyRowBlue,
                    "Заполни любой ряд\nголубыми блоками",
                    70,
                    BonusColor.Blue
                );
            }

            if (value == 4)
            {
                return new TaskModel(
                    TaskType.FillLeftColumnRed,
                    "Заполни левый\nстолбец красными\nблоками",
                    80,
                    BonusColor.Red
                );
            }

            if (value == 5)
            {
                return new TaskModel(
                    TaskType.FillRightColumnBlue,
                    "Заполни правый\nстолбец голубыми\nблоками",
                    80,
                    BonusColor.Blue
                );
            }

            if (value == 6)
            {
                return new TaskModel(
                    TaskType.FillAnyColumnGreen,
                    "Заполни любой\nстолбец зелёными\nблоками",
                    70,
                    BonusColor.Green
                );
            }

            if (value == 7)
            {
                return new TaskModel(
                    TaskType.FillCenterColumnYellow,
                    "Заполни центральный\nстолбец жёлтыми\nблоками",
                    85,
                    BonusColor.Yellow
                );
            }

            if (value == 8)
            {
                return new TaskModel(
                    TaskType.FillCornersRed,
                    "Заполни углы\nкрасными блоками",
                    100,
                    BonusColor.Red
                );
            }

            if (value == 9)
            {
                return new TaskModel(
                    TaskType.FillCornersBlue,
                    "Заполни углы\nголубыми блоками",
                    100,
                    BonusColor.Blue
                );
            }

            if (value == 10)
            {
                return new TaskModel(
                    TaskType.FillTopCornersYellow,
                    "Заполни верхние углы\nжёлтыми блоками",
                    60,
                    BonusColor.Yellow
                );
            }

            if (value == 11)
            {
                return new TaskModel(
                    TaskType.FillBottomCornersGreen,
                    "Заполни нижние углы\nзелёными блоками",
                    60,
                    BonusColor.Green
                );
            }

            if (value == 12)
            {
                return new TaskModel(
                    TaskType.FillCenterGreen,
                    "Заполни центральные\n4 клетки зелёными\nблоками",
                    90,
                    BonusColor.Green
                );
            }

            return new TaskModel(
                TaskType.FillCenterYellow,
                "Заполни центральные\n4 клетки жёлтыми\nблоками",
                90,
                BonusColor.Yellow
            );
        }
    }
}