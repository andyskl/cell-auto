using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CellularAutomatonSimulation
{
    enum CellState
    {
        Empty,
        Alive,
        Dead
    }

    enum AutomatonType
    {
        Conway,
        Maze,
        Cave
    }

    static class Grid
    {
        static int w;
        static int h;

        static int genCounter;

        static CellState[,] grid;
        static CellState[,] gridCopy;
        static CellState[,] initialState;
        static int[,] temps;
        static int max;

        static Dictionary<AutomatonType, Func<int, bool>> comparingFunctionsForBecoming = new Dictionary<AutomatonType, Func<int, bool>>()
        {
            {AutomatonType.Conway, count => count == 3},
            {AutomatonType.Cave, count => count >= 5},
            {AutomatonType.Maze, count=>count==3}
        };
        static Dictionary<AutomatonType, Func<int, bool>> comparingFunctionsForDying = new Dictionary<AutomatonType, Func<int, bool>>()
        {
            {AutomatonType.Conway, count => (count < 2 || count > 3)},
            {AutomatonType.Cave, count => count < 4},
            {AutomatonType.Maze, count => (count == 0 || count > 5)}
        };

        static Dictionary<AutomatonType, int> randomizerValues = new Dictionary<AutomatonType, int>()
        {
            {AutomatonType.Conway, 12},
            {AutomatonType.Cave, 52},
            {AutomatonType.Maze, 12}
        };

        static Func<int, bool> currentComparingFunctionForBecoming;
        static Func<int, bool> currentComparingFunctionForDying;
        static int currentRandomizerValue;

        public static void Initialize(int width = 100, int height = 100)
        {
            w = width;
            h = height;
            grid = new CellState[h, w];
            gridCopy = new CellState[h, w];
            initialState = new CellState[h, w];
            temps = new int[h, w];
            CurrentType = AutomatonType.Conway;
        }

        public static AutomatonType CurrentType
        {
            set
            {
                currentComparingFunctionForBecoming = comparingFunctionsForBecoming[value];
                currentComparingFunctionForDying = comparingFunctionsForDying[value];
                currentRandomizerValue = randomizerValues[value];
            }
        }

        public static void Draw(Graphics graphics, int fieldWidth, int fieldHeight)
        {
            float tileWidth = ((float)fieldWidth) / w;
            float tileHeight = ((float)fieldHeight) / h;
            Pen p = Pens.LightGray;
            for (int i = 0; i < h - 1; i++)
            {
                int y = (int)(tileHeight * (i + 1));
                graphics.DrawLine(
                        p,
                        new Point(0, y),
                        new Point(fieldWidth, y));

            }
            for (int j = 0; j < w - 1; j++)
            {
                int x = (int)(tileWidth * (j + 1));
                graphics.DrawLine(
                        p,
                        new Point(x, 0),
                        new Point(x, fieldHeight));
            }
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    CellState cellState = grid[i, j];
                    if (cellState == CellState.Alive || cellState == CellState.Dead)
                    {
                        int x = (int)(j * tileWidth);
                        int y = (int)(i * tileHeight);
                        float maxVal = max;
                        int colorCoef = 0;
                        int currentValue = temps[i, j];
                        colorCoef =
                                maxVal == 0 ?
                                255 :
                                (int)((1 - (((float)currentValue) / maxVal)) * 255);
                        colorCoef = Math.Min(colorCoef, 255);
                        Brush b = new SolidBrush(Color.FromArgb(255, colorCoef / 2, colorCoef / 2, colorCoef));
                        graphics.FillRectangle(
                                b,
                                new Rectangle(x, y, (int)tileWidth + 1, (int)tileHeight + 1));
                        if (cellState == CellState.Alive)
                        {
                            b =
                                Brushes.Crimson;
                            graphics.FillRectangle(
                                    b,
                                    new Rectangle(x, y, (int)tileWidth + 1, (int)tileHeight + 1));
                        }
                    }
                }
            }
        }

        private static void TurnCell(int i, int j)
        {
            if (i >= 0 && i < h && j >= 0 && j < w)
            {
                if (grid[i, j] == CellState.Empty)
                {
                    grid[i, j] = CellState.Alive;
                }
                else if (grid[i, j] == CellState.Alive)
                {
                    grid[i, j] = CellState.Dead;
                }
                else
                {
                    grid[i, j] = CellState.Alive;
                }
            }
        }

        public static void TurnCell(int x, int y, int fieldWidth, int fieldHeight)
        {
            float tileWidth = ((float)fieldWidth) / w;
            float tileHeight = ((float)fieldHeight) / h;
            int i = (int)(y / tileHeight);
            int j = (int)(x / tileWidth);
            TurnCell(i, j);
        }

        private static int GetNeighboursCount(int i, int j)
        {
            int count = 0;
            if (i > 0)
            {
                if (gridCopy[i - 1, j] == CellState.Alive) count++;
                if (j > 0 && gridCopy[i - 1, j - 1] == CellState.Alive) count++;
                if (j < w - 1 && gridCopy[i - 1, j + 1] == CellState.Alive) count++;
            }
            if (i < h - 1)
            {
                if (gridCopy[i + 1, j] == CellState.Alive) count++;
                if (j > 0 && gridCopy[i + 1, j - 1] == CellState.Alive) count++;
                if (j < w - 1 && gridCopy[i + 1, j + 1] == CellState.Alive) count++;
            }
            if (j > 0 && gridCopy[i, j - 1] == CellState.Alive) count++;
            if (j < w - 1 && gridCopy[i, j + 1] == CellState.Alive) count++;
            return count;
        }

        private static void CopyGrid()
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    gridCopy[i, j] = grid[i, j];
                }
            }
        }

        private static void SaveState()
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    initialState[i, j] = grid[i, j];
                }
            }
        }

        private static void RestoreState()
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    grid[i, j] = initialState[i, j];
                }
            }
        }

        private static void ClearGrid()
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    grid[i, j] = CellState.Empty;
                }
            }
        }

        public static int Generation
        {
            get
            {
                return genCounter;
            }
        }

        public static void Reset()
        {
            RestoreState();
            max = 0;
            ClearTemps();
            CopyGrid();
            genCounter = 0;
        }

        public static void Clear()
        {
            ClearGrid();
            max = 0;
            ClearTemps();
            CopyGrid();
            SaveState();
            genCounter = 0;
        }

        public static void Step()
        {
            CopyGrid();
            if (genCounter == 0)
            {
                SaveState();
            }
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    int nc = GetNeighboursCount(i, j);
                    if (currentComparingFunctionForBecoming(nc) &&
                        grid[i, j] != CellState.Alive) //become
                    {
                        grid[i, j] = CellState.Alive;
                        temps[i, j]++;
                        if (temps[i, j] > max)
                        {
                            max = temps[i, j];
                        }
                    }
                    if (currentComparingFunctionForDying(nc) && 
                        grid[i, j] == CellState.Alive) //die
                    {
                        grid[i, j] = CellState.Dead;
                    }
                }
            }
            genCounter++;
        }

        public static void Randomize()
        {
            RandomizeGrid();
        }

        private static void RandomizeGrid()
        {
            Clear();
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    grid[i, j] =
                        Randomizer.DecisionByPercent(currentRandomizerValue) ?
                        CellState.Alive :
                        CellState.Empty;
                }
            }
        }

        private static void ClearTemps()
        {
            max = 0;
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    temps[i, j] = 0;
                }
            }
        }
    }
}
