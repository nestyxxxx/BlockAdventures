using System.Drawing;
using BlockAdventures.GameLogic;
using BlockAdventures.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlockAdventures.Tests
{
    [TestClass]
    public class FieldManagerTests
    {
        private FigureModel CreateOneCellFigure(Color color)
        {
            var cells = new bool[3, 3];
            cells[0, 0] = true;

            return new FigureModel(cells, color);
        }

        [TestMethod]
        public void CanPutFigure_EmptyField_ReturnsTrue()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            var result = fieldManager.CanPutFigure(figure, 2, 2);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanPutFigure_BusyCell_ReturnsFalse()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            fieldManager.PutFigure(figure, 2, 2);

            var result = fieldManager.CanPutFigure(figure, 2, 2);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPutFigure_OutsideLeft_ReturnsFalse()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            var result = fieldManager.CanPutFigure(figure, -1, 0);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanPutFigure_OutsideBottom_ReturnsFalse()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            var result = fieldManager.CanPutFigure(figure, 0, 5);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PutFigure_OneCellFigure_PaintsNeededCell()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Blue);

            fieldManager.PutFigure(figure, 1, 3);

            Assert.IsTrue(fieldManager.Cells[1, 3].HasValue);
            Assert.AreEqual(Color.Blue.ToArgb(), fieldManager.Cells[1, 3].Value.ToArgb());
        }

        [TestMethod]
        public void ClearFilledRows_FullRow_ReturnsOneAndClearsRow()
        {
            var fieldManager = new FieldManager(3, 2);
            var figure = CreateOneCellFigure(Color.Green);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 1, 0);
            fieldManager.PutFigure(figure, 2, 0);

            var clearedRows = fieldManager.ClearFilledRows();

            Assert.AreEqual(1, clearedRows);
            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[1, 0]);
            Assert.IsNull(fieldManager.Cells[2, 0]);
        }

        [TestMethod]
        public void ClearFilledRows_NotFullRow_ReturnsZero()
        {
            var fieldManager = new FieldManager(3, 2);
            var figure = CreateOneCellFigure(Color.Green);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 1, 0);

            var clearedRows = fieldManager.ClearFilledRows();

            Assert.AreEqual(0, clearedRows);
            Assert.IsNotNull(fieldManager.Cells[0, 0]);
            Assert.IsNotNull(fieldManager.Cells[1, 0]);
        }

        [TestMethod]
        public void ClearFilledColumns_FullColumn_ReturnsOneAndClearsColumn()
        {
            var fieldManager = new FieldManager(2, 3);
            var figure = CreateOneCellFigure(Color.Yellow);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 0, 1);
            fieldManager.PutFigure(figure, 0, 2);

            var clearedColumns = fieldManager.ClearFilledColumns();

            Assert.AreEqual(1, clearedColumns);
            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[0, 1]);
            Assert.IsNull(fieldManager.Cells[0, 2]);
        }

        [TestMethod]
        public void ClearFilledColumns_NotFullColumn_ReturnsZero()
        {
            var fieldManager = new FieldManager(2, 3);
            var figure = CreateOneCellFigure(Color.Yellow);

            fieldManager.PutFigure(figure, 0, 0);
            fieldManager.PutFigure(figure, 0, 1);

            var clearedColumns = fieldManager.ClearFilledColumns();

            Assert.AreEqual(0, clearedColumns);
            Assert.IsNotNull(fieldManager.Cells[0, 0]);
            Assert.IsNotNull(fieldManager.Cells[0, 1]);
        }

        [TestMethod]
        public void ClearColor_RemovesOnlyNeededColor()
        {
            var fieldManager = new FieldManager(4, 4);
            var redFigure = CreateOneCellFigure(Color.Red);
            var blueFigure = CreateOneCellFigure(Color.Blue);

            fieldManager.PutFigure(redFigure, 0, 0);
            fieldManager.PutFigure(redFigure, 1, 1);
            fieldManager.PutFigure(blueFigure, 2, 2);

            fieldManager.ClearColor(Color.Red);

            Assert.IsNull(fieldManager.Cells[0, 0]);
            Assert.IsNull(fieldManager.Cells[1, 1]);
            Assert.IsNotNull(fieldManager.Cells[2, 2]);
            Assert.AreEqual(Color.Blue.ToArgb(), fieldManager.Cells[2, 2].Value.ToArgb());
        }

        [TestMethod]
        public void ClampFigurePosition_LeftTopOutside_ReturnsZeroZero()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            var point = fieldManager.ClampFigurePosition(figure, -10, -10);

            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
        }

        [TestMethod]
        public void ClampFigurePosition_RightBottomOutside_ReturnsLastCell()
        {
            var fieldManager = new FieldManager(5, 5);
            var figure = CreateOneCellFigure(Color.Red);

            var point = fieldManager.ClampFigurePosition(figure, 20, 20);

            Assert.AreEqual(4, point.X);
            Assert.AreEqual(4, point.Y);
        }

        [TestMethod]
        public void HasAnyPlaceForFigure_EmptyField_ReturnsTrue()
        {
            var fieldManager = new FieldManager(3, 3);
            var figure = CreateOneCellFigure(Color.Red);

            var result = fieldManager.HasAnyPlaceForFigure(figure);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasAnyPlaceForFigure_FullField_ReturnsFalse()
        {
            var fieldManager = new FieldManager(1, 1);
            var figure = CreateOneCellFigure(Color.Red);

            fieldManager.PutFigure(figure, 0, 0);

            var result = fieldManager.HasAnyPlaceForFigure(figure);

            Assert.IsFalse(result);
        }
    }
}