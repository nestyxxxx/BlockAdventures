using System;
using System.Collections.Generic;
using System.Drawing;

namespace BlockAdventures.Models
{
    public static class FigureGenerator
    {
        private static readonly Random random = new Random();

        private static readonly List<bool[,]> shapes = new List<bool[,]>
        {
            new bool[,]
            {
                { true, false, false },
                { false, false, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, false },
                { false, false, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, true },
                { false, false, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, false },
                { true, true, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, false, false },
                { true, false, false },
                { true, false, false }
            },

            new bool[,]
            {
                { true, false, false },
                { true, true, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, true },
                { false, true, false },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, false },
                { false, true, true },
                { false, false, false }
            },

            new bool[,]
            {
                { true, true, true },
                { true, false, false },
                { false, false, false }
            }
        };

        private static readonly Color[] colors =
        {
            Color.FromArgb(196, 72, 56),
            Color.FromArgb(92, 176, 78),
            Color.FromArgb(222, 198, 68),
            Color.FromArgb(78, 180, 220)
        };

        public static FigureModel Generate()
        {
            bool[,] shape = shapes[random.Next(shapes.Count)];
            Color color = colors[random.Next(colors.Length)];

            bool[,] copy = new bool[3, 3];

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    copy[x, y] = shape[x, y];
                }
            }

            return new FigureModel(copy, color);
        }
    }
}