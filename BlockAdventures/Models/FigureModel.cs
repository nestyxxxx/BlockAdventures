using System.Drawing;

namespace BlockAdventures.Models
{
    public class FigureModel
    {
        public bool[,] Cells { get; }
        public Color Color { get; }

        public int Width => Cells.GetLength(0);
        public int Height => Cells.GetLength(1);

        public FigureModel(bool[,] cells, Color color)
        {
            Cells = cells;
            Color = color;
        }
    }
}