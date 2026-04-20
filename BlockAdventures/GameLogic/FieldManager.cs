using System.Drawing;
using BlockAdventures.Models;

namespace BlockAdventures.GameLogic
{
    public class FieldManager
    {
        public Color?[,] Cells { get; private set; }

        private int cols;
        private int rows;

        public FieldManager(int fieldCols, int fieldRows)
        {
            cols = fieldCols;
            rows = fieldRows;
            Cells = new Color?[cols, rows];
        }

        public Color GetCellColor(int col, int row, Color emptyColor)
        {
            if (Cells[col, row].HasValue)
            {
                return Cells[col, row].Value;
            }

            return emptyColor;
        }

        public void PutFigure(FigureModel figure, int startCol, int startRow)
        {
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!figure.Cells[col, row])
                    {
                        continue;
                    }

                    Cells[startCol + col, startRow + row] = figure.Color;
                }
            }
        }

        public Point ClampFigurePosition(FigureModel figure, int startCol, int startRow)
        {
            var minCol = GetFigureMinCol(figure);
            var minRow = GetFigureMinRow(figure);
            var maxCol = GetFigureMaxCol(figure);
            var maxRow = GetFigureMaxRow(figure);

            var minStartCol = -minCol;
            var minStartRow = -minRow;
            var maxStartCol = cols - 1 - maxCol;
            var maxStartRow = rows - 1 - maxRow;

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

            return new Point(startCol, startRow);
        }

        public bool CanPutFigure(FigureModel figure, int startCol, int startRow)
        {
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (!figure.Cells[col, row])
                    {
                        continue;
                    }

                    var fieldCol = startCol + col;
                    var fieldRow = startRow + row;

                    if (fieldCol < 0 || fieldCol >= cols || fieldRow < 0 || fieldRow >= rows)
                    {
                        return false;
                    }

                    if (Cells[fieldCol, fieldRow].HasValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int ClearFilledRows()
        {
            var clearedRows = 0;

            for (var row = 0; row < rows; row++)
            {
                var rowIsFull = true;

                for (var col = 0; col < cols; col++)
                {
                    if (!Cells[col, row].HasValue)
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

                for (var col = 0; col < cols; col++)
                {
                    Cells[col, row] = null;
                }
            }

            return clearedRows;
        }

        public int ClearFilledColumns()
        {
            var clearedColumns = 0;

            for (var col = 0; col < cols; col++)
            {
                var columnIsFull = true;

                for (var row = 0; row < rows; row++)
                {
                    if (!Cells[col, row].HasValue)
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

                for (var row = 0; row < rows; row++)
                {
                    Cells[col, row] = null;
                }
            }

            return clearedColumns;
        }

        public void ClearColor(Color color)
        {
            for (var row = 0; row < rows; row++)
            {
                for (var col = 0; col < cols; col++)
                {
                    if (!Cells[col, row].HasValue)
                    {
                        continue;
                    }

                    if (Cells[col, row].Value.ToArgb() == color.ToArgb())
                    {
                        Cells[col, row] = null;
                    }
                }
            }
        }

        public bool HasAnyPlaceForFigure(FigureModel figure)
        {
            var minCol = GetFigureMinCol(figure);
            var minRow = GetFigureMinRow(figure);
            var maxCol = GetFigureMaxCol(figure);
            var maxRow = GetFigureMaxRow(figure);

            var minStartCol = -minCol;
            var minStartRow = -minRow;
            var maxStartCol = cols - 1 - maxCol;
            var maxStartRow = rows - 1 - maxRow;

            for (var row = minStartRow; row <= maxStartRow; row++)
            {
                for (var col = minStartCol; col <= maxStartCol; col++)
                {
                    if (CanPutFigure(figure, col, row))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public int GetFigureMinCol(FigureModel figure)
        {
            var minCol = 3;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (figure.Cells[col, row] && col < minCol)
                    {
                        minCol = col;
                    }
                }
            }

            return minCol;
        }

        public int GetFigureMinRow(FigureModel figure)
        {
            var minRow = 3;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (figure.Cells[col, row] && row < minRow)
                    {
                        minRow = row;
                    }
                }
            }

            return minRow;
        }

        public int GetFigureMaxCol(FigureModel figure)
        {
            var maxCol = 0;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (figure.Cells[col, row] && col > maxCol)
                    {
                        maxCol = col;
                    }
                }
            }

            return maxCol;
        }

        public int GetFigureMaxRow(FigureModel figure)
        {
            var maxRow = 0;

            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    if (figure.Cells[col, row] && row > maxRow)
                    {
                        maxRow = row;
                    }
                }
            }

            return maxRow;
        }
    }
}