using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace MazeGeneration
{
    [Flags]
    public enum CellState
    {
        Top     = 1 << 0,
        Right   = 1 << 1,
        Bottom  = 1 << 2,
        Left    = 1 << 3,
        Visited = 1 << 4,
        Initial = Top | Left | Bottom | Right,
    }

    public struct Point
    {
        public int x, y;
    }

    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng)
        {
            var e = source.ToArray();
            for (var i = e.Length - 1; i >= 0; i--)
            {
                var swapIndex = rng.Next(i + 1);
                yield return e[swapIndex];
                e[swapIndex] = e[i];
            }
        }

        public static CellState OppositeWall(this CellState orig)
        {
            return (CellState)(((int) orig >> 2) | ((int) orig << 2)) & CellState.Initial;
        }

        public static bool HasFlag(this CellState cs, CellState flag)
        {
            return ((int)cs & (int)flag) != 0;
        }

        public static bool HasFlags(this CellState cs, CellState flag)
        {
            return ((int)cs & (int)flag) == (int)flag;
        }
    }

    public class Maze
    {
        private readonly CellState[,] cells;
        public readonly int width;
        public readonly int height;
        private readonly System.Random rng;

        public Maze(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new CellState[width, height];
            
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    cells[x, y] = CellState.Initial;

            rng = new System.Random();

            VisitCell(rng.Next(width), rng.Next(height));
        }
        
        public struct CellNeighbour
        {
            public int x, y;
            public CellState side;
        }

        public IEnumerable<CellNeighbour> GetNeighbours(int x, int y)
        {
            if (x > 0)
                yield return new CellNeighbour { x = x - 1, y = y,      side = CellState.Left }; 
            if (y > 0)
                yield return new CellNeighbour { x = x,     y = y - 1,  side = CellState.Top };
            if (x < width - 1)
                yield return new CellNeighbour { x = x + 1, y = y,      side = CellState.Right };
            if (y < height - 1)
                yield return new CellNeighbour { x = x,     y = y + 1,  side = CellState.Bottom };
        }

        private void VisitCell(int x, int y)
        {
            // start with a random cell and mark it as visited
            this[x, y] |= CellState.Visited;
            foreach(var n in GetNeighbours(x, y).Shuffle(rng).Where(z => !(this[z.x, z.y].HasFlag(CellState.Visited))))
            {
                cells[x, y]     &= ~n.side;
                cells[n.x, n.y] &= ~n.side.OppositeWall();
                VisitCell(n.x, n.y);
            }
        }

        public CellState this[int x, int y]
        {
            get { return cells[x, y]; }
            set { cells[x, y] = value; }
        }
    }
}