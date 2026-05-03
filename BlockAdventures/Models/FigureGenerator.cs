using System;
using System.Collections.Generic;

namespace BlockAdventures.Models
{
    public static class FigureGenerator
    {
        private static readonly Random random = new Random();

        private static readonly List<bool[,]> baseShapes = new List<bool[,]>
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

        private static readonly List<bool[,]> allShapes = CreateAllShapes();

        private static readonly BonusColor[] colors =
        {
            BonusColor.Red,
            BonusColor.Green,
            BonusColor.Yellow,
            BonusColor.Blue
        };

        public static FigureModel Generate()
        {
            var shape = allShapes[random.Next(allShapes.Count)];
            var color = colors[random.Next(colors.Length)];

            return new FigureModel(CopyShape(shape), color);
        }

        private static List<bool[,]> CreateAllShapes()
        {
            var result = new List<bool[,]>();

            foreach (var baseShape in baseShapes)
            {
                AddUniqueRotations(result, baseShape);
            }

            return result;
        }

        private static void AddUniqueRotations(List<bool[,]> result, bool[,] shape)
        {
            var current = CopyShape(shape);

            for (var i = 0; i < 4; i++)
            {
                if (!ContainsShape(result, current))
                {
                    result.Add(CopyShape(current));
                }

                current = RotateClockwise(current);
            }
        }

        private static bool ContainsShape(List<bool[,]> shapes, bool[,] newShape)
        {
            foreach (var shape in shapes)
            {
                if (AreEqual(shape, newShape))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AreEqual(bool[,] first, bool[,] second)
        {
            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (first[x, y] != second[x, y])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool[,] CopyShape(bool[,] source)
        {
            var copy = new bool[3, 3];

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    copy[x, y] = source[x, y];
                }
            }

            return copy;
        }

        private static bool[,] RotateClockwise(bool[,] source)
        {
            var rotated = new bool[3, 3];

            for (var x = 0; x < 3; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    rotated[x, y] = source[y, 2 - x];
                }
            }

            return rotated;
        }
    }
}