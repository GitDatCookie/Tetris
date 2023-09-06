using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.BlockLogic;

namespace Tetris
{
    internal class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for(int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);
                    if (!BlockFits())
                    {
                        currentBlock.Move(-1,0);
                    }
                }
            }
        }

        public Grid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }

        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }


        public GameState()
        {
            GameGrid = new Grid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdateBlock();
        }

        private bool BlockFits()
        {
            foreach (Position pos in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(pos.Row, pos.Column))
                {
                    return false;
                }

            }
            return true;
        }

        public void HoldBlock()
        {
            if(!CanHold)
            {
                return;
            }
            if(HeldBlock == null)
            {
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdateBlock();
            }
            else
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }
            CanHold = false;
        }
        public void RotateBlockClockWise()
        {
            CurrentBlock.RotateClockWise();
            if (!BlockFits())
            {
                CurrentBlock.RotateCounterClockWise();
            }
        }
        public void RotateBlockCounterClockWise()
        {
            CurrentBlock.RotateCounterClockWise();
            if (!BlockFits())
            {
                CurrentBlock.RotateClockWise();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        public bool IsGameOver()
        {
            if (GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1))
            {
                return false;
            }
            return true;
        }

        private void PlaceBlock()
        {
            foreach (Position pos in CurrentBlock.TilePositions())
            {
                GameGrid[pos.Row, pos.Column] = CurrentBlock.ID;
            }
            Score += GameGrid.ClearFullRows() ;

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdateBlock();
                CanHold = true;
            }
        }
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position pos)
        {
            int drop = 0;
            while(GameGrid.IsEmpty(pos.Row + drop +1, pos.Column))
            {
                drop++;
            }
            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;
            foreach(Position pos in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(pos));
            }
            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }

}

 
