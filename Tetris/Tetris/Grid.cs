using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNALib;

namespace Tetris
{
    public class Grid
    {
        #region Members
        public static Grid Instance;
        static readonly Texture2D BG = Common.White1px50Trans;

        /// <summary>
        /// The blocks that are locked in place
        /// </summary>
        public Block[,] Blocks;

        public const int BLOCK_SPACING = 1;
        
        private Rectangle m_GridArea = new Rectangle();
        public Rectangle GridArea
        {
            get { return m_GridArea; }
            private set { m_GridArea = value; }
        }
       
        public const int RowCnt = 17;
        public const int ColCnt = 23;

        private readonly Dictionary<int, int> ScoreLookup = new Dictionary<int, int>()
        {
            {0,0},
            {1,100},
            {2,250},
            {3,425},
            {4,550},
            {5,750}
        };

        private Block BorderBlock = new Block(Point.Zero, Point.Zero);
        #endregion

        public Grid()
        {
            GridArea = new Rectangle(Block.SIZE, -Block.SIZE*2, RowCnt * (Block.SIZE + BLOCK_SPACING), ColCnt * (Block.SIZE + BLOCK_SPACING));
            Blocks = new Block[RowCnt, ColCnt];
        }

        public Vector2 IdxToLoc(Point gridIdx)
        {
            return new Vector2(gridIdx.X * (Block.SIZE + BLOCK_SPACING)+GridArea.X, gridIdx.Y * (Block.SIZE + BLOCK_SPACING)+GridArea.Y);
        }

        public void HandleCompleteRows()
        {
            int rowsCompleted = 0;

            for (int y = 0; y < ColCnt; y++)
            {
                bool rowIsComplete = true;
                for (int x = 0; x < RowCnt; x++)
                {
                    if (Blocks[x, y] == null)
                    {
                        rowIsComplete = false;
                        break;
                    }
                }

                if (rowIsComplete)
                {
                    rowsCompleted++;

                    // Push all blocks above one down
                    for (int rowPusher = y; rowPusher >= 0; rowPusher--)
                    {
                        // Clear the row
                        for (int x = 0; x < RowCnt; x++)
                            Blocks[x, rowPusher] = null;

                        if (rowPusher > 0)
                        {
                            // Move the row above one down
                            for (int x = 0; x < RowCnt; x++)
                            {
                                if (Blocks[x, rowPusher - 1] != null)
                                {
                                    Blocks[x, rowPusher] = Blocks[x, rowPusher - 1];
                                    Blocks[x, rowPusher].GridIndex.Y++; // adjust the y-index on the block
                                }
                                else
                                    Blocks[x, rowPusher] = null;
                            }
                        }
                    }
                }
            }

            // Add score based on the rowsCompleted
            Level.Instance.Score += ScoreLookup[rowsCompleted] * Level.Instance.LevelNr;

            if(rowsCompleted > 0)
                Engine.Instance.Audio.PlaySound("plopp");
        }

        public void Draw()
        {
            //Engine.Instance.SpriteBatch.Draw(BG, new Rectangle(GridArea.X - Block.SIZE, GridArea.Y, GridArea.Width + 2 * Block.SIZE, GridArea.Height + Block.SIZE), Color.Black);
            foreach (Block b in Blocks)
            {
                if(b != null)
                    b.Draw();
            }

            // Draw border
            for (int y = 0; y <= ColCnt; y++)
            {
                BorderBlock.DrawForBorder(new Vector2(GridArea.X - Block.SIZE, GridArea.Y + y * (Block.SIZE + BLOCK_SPACING)));
                BorderBlock.DrawForBorder(new Vector2(GridArea.Right, GridArea.Y + y * (Block.SIZE + BLOCK_SPACING)));
            }
            for (int x = 0; x < RowCnt; x++)
            {
                BorderBlock.DrawForBorder(new Vector2(GridArea.X + x * (Block.SIZE + BLOCK_SPACING), GridArea.Bottom));
            }
        }
    }
}
