namespace BlockAdventures.Models
{
    public class FigureModel
    {
        public bool[,] Cells { get; }
        public BonusColor Color { get; }

        public int Width
        {
            get { return Cells.GetLength(0); }
        }

        public int Height
        {
            get { return Cells.GetLength(1); }
        }

        public FigureModel(bool[,] cells, BonusColor color)
        {
            Cells = cells;
            Color = color;
        }
    }
}