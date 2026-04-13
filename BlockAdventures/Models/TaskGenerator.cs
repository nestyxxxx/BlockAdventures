using System;

namespace BlockAdventures.Models
{
    public static class TaskGenerator
    {
        private static readonly Random random = new Random();

        public static TaskModel Generate()
        {
            var value = random.Next(8);

            if (value == 0)
            {
                return new TaskModel(
                    TaskType.FillCornersRed,
                    "Заполни углы\nкрасными блоками",
                    100,
                    BonusColor.Red
                );
            }

            if (value == 1)
            {
                return new TaskModel(
                    TaskType.FillRowRed,
                    "Заполни ряд\nкрасными блоками",
                    70,
                    BonusColor.Red
                );
            }

            if (value == 2)
            {
                return new TaskModel(
                    TaskType.FillRightColumnBlue,
                    "Заполни правый\nстолбец голубыми\nблоками",
                    70,
                    BonusColor.Blue
                );
            }

            if (value == 3)
            {
                return new TaskModel(
                    TaskType.FillCornersBlue,
                    "Заполни углы\nголубыми блоками",
                    100,
                    BonusColor.Blue
                );
            }

            if (value == 4)
            {
                return new TaskModel(
                    TaskType.FillCenterGreen,
                    "Заполни центральные\n4 клетки зелёными\nблоками",
                    80,
                    BonusColor.Green
                );
            }

            if (value == 5)
            {
                return new TaskModel(
                    TaskType.FillColumnGreen,
                    "Заполни столбец\nзелёными блоками",
                    70,
                    BonusColor.Green
                );
            }

            if (value == 6)
            {
                return new TaskModel(
                    TaskType.FillTopRowYellow,
                    "Заполни верхнюю\nстроку жёлтыми\nблоками",
                    70,
                    BonusColor.Yellow
                );
            }

            return new TaskModel(
                TaskType.FillCenterYellow,
                "Заполни центральные\n4 клетки жёлтыми\nблоками",
                80,
                BonusColor.Yellow
            );
        }
    }
}