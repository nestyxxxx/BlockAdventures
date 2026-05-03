using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockAdventures.Tests
{
    [TestClass]
    public class TaskCheckerTests
    {
        [TestMethod]
        public void CheckTask_FillTopRowRed_ReturnsTrue()
        {
            var field = new BonusColor?[10, 8];

            for (var col = 0; col < 10; col++)
            {
                field[col, 0] = BonusColor.Red;
            }

            var task = new TaskModel(
                TaskType.FillTopRowRed,
                "",
                80,
                BonusColor.Red
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillCornersBlue_ReturnsTrue()
        {
            var field = new BonusColor?[10, 8];

            field[0, 0] = BonusColor.Blue;
            field[9, 0] = BonusColor.Blue;
            field[0, 7] = BonusColor.Blue;
            field[9, 7] = BonusColor.Blue;

            var task = new TaskModel(
                TaskType.FillCornersBlue,
                "",
                100,
                BonusColor.Blue
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillTopCornersYellow_WithoutOneCorner_ReturnsFalse()
        {
            var field = new BonusColor?[10, 8];

            field[0, 0] = BonusColor.Yellow;

            var task = new TaskModel(
                TaskType.FillTopCornersYellow,
                "",
                60,
                BonusColor.Yellow
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CheckTask_FillCenterGreen_ReturnsTrue()
        {
            var field = new BonusColor?[10, 8];

            field[4, 3] = BonusColor.Green;
            field[5, 3] = BonusColor.Green;
            field[4, 4] = BonusColor.Green;
            field[5, 4] = BonusColor.Green;

            var task = new TaskModel(
                TaskType.FillCenterGreen,
                "",
                90,
                BonusColor.Green
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckTask_FillAnyColumnGreen_ReturnsTrue()
        {
            var field = new BonusColor?[10, 8];

            for (var row = 0; row < 8; row++)
            {
                field[2, row] = BonusColor.Green;
            }

            var task = new TaskModel(
                TaskType.FillAnyColumnGreen,
                "",
                70,
                BonusColor.Green
            );

            var result = TaskChecker.CheckTask(task, field, 10, 8);

            Assert.IsTrue(result);
        }
    }
}