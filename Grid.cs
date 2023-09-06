using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tetris
{
    internal class Grid
    {
        private readonly int[,] grid;

        public int Rows { get;}
        public int Columns { get; }
        public int this[int rows, int columns]
        {
            get => grid[rows, columns];
            set => grid[rows, columns] = value;
        }

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        public bool IsInside(int rows, int columns)
        {
            if(rows >= 0 && rows < Rows && columns >= 0 && columns < Columns)
            {
                return true;
            }
            return false;
        }

        public bool IsEmpty(int rows, int columns)
        {
            return IsInside(rows, columns) && grid[rows, columns] == 0;
        }

        public bool IsRowFull(int row)
        {
            for ( int col = 0; col < Columns; col++ )
            {
                if (grid[row, col] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRowEmpty(int row)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (grid[row, col] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearRow(int row)
        {
            for( int col = 0; col < Columns; ++col)
            {
                grid[row, col] = 0;
            }
        }

        private void MoveRowDown(int row, int downRow)
        {
            for (int col = 0; col < Columns; ++col)
            {
                grid[row +downRow, col] = grid[row, col];
                grid[row, col] = 0;
            }
        }

        public int ClearFullRows()
        {
            int clearedRows = 0;

            for(int row = Rows -1; row >= 0; row--)
            {
                if (IsRowFull(row))
                {
                    ClearRow(row);
                    clearedRows++;
                }
                else if (clearedRows > 0)
                {
                    MoveRowDown(row, clearedRows);
                }
            }
            return clearedRows;
        }
    }
}
