using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using XNALib;

namespace Tetris
{
    public class Shape : IXNADispose
    {
        private bool m_IsDisposed = false;
        public bool IsDisposed
        {
            get { return m_IsDisposed; }
            set { m_IsDisposed = value; }
        }

        /// <summary>
        /// The blocks that are still movable by the player.
        /// </summary>
        public Block[,] Blocks = new Block[BlocksRowCnt, BlocksRowCnt];
        const int BlocksRowCnt = 5;

        bool PlantNextTick = false;

        /// <summary>
        /// Shape's index in the grid.
        /// </summary>
        public Point GridIdx;

        SimpleTimer UpdateTimer = new SimpleTimer(Level.Instance.FallDelayInMS);
        eShape ShapeType;
        bool IsShiftingDown = false;

        public int BlockCnt;

        SimpleTimer MoveLeftDelayTimer = new SimpleTimer(75);
        SimpleTimer MoveDownDelayTimer = new SimpleTimer(40);
        SimpleTimer MoveRightDelayTimer = new SimpleTimer(75);

        public Shape(eShape shape)
        {
            ShapeType = shape;
            GridIdx = new Point((Grid.RowCnt / 2) - BlocksRowCnt/2, 0);
        }

        public void SetColor(Color newColor)
        {
            foreach (Block b in Blocks)
            {
                if (b != null)
                    b.DrawColor = newColor;
            }
        }

        /*
#warning rotateleft works but it very very slow
        Block[,] RotateLeft()
        {
            Block[,] result = new Block[BlocksRowCnt, BlocksRowCnt];
            for (int i = BlocksRowCnt-1; i >= 0; --i)
            {
                for (int j = 0; j < BlocksRowCnt; ++j)
                {
                    result[i, BlocksRowCnt-1 - j] = Blocks[j, i];
                    if (result[i, BlocksRowCnt-1 - j] != null)
                        result[i, BlocksRowCnt - 1 - j].ShapeIndex = new Point(i, BlocksRowCnt - 1 - j);
                }
            }
            return result;
        }*/

        // works
        Block[,] RotateRight()
        {
            if (ShapeType != eShape.O)
            {
                Block[,] result = new Block[BlocksRowCnt, BlocksRowCnt];
                for (int i = 0; i < BlocksRowCnt; ++i)
                {
                    for (int j = BlocksRowCnt - 1; j >= 0; --j)
                    {
                        if (Blocks[i, j] != null)
                        {
                            result[j, BlocksRowCnt - 1 - i] = (Block)Blocks[i, j].CloneDeep();
                            result[j, BlocksRowCnt - 1 - i].ShapeIndex = new Point(j, BlocksRowCnt - 1 - i);
                            result[j, BlocksRowCnt - 1 - i].GridIndex = result[j, BlocksRowCnt - 1 - i].ShapeIndex.Add(GridIdx);
                        }
                    }
                }
                return result;
            }
            else
                return Blocks;
        }

        /// <summary>
        /// Checks if all blocks are not colliding with any block in the grid
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        public bool IsAllowed(Block[,] blocks)
        {
            foreach (Block b in blocks)
            {
                if (b != null) // don't test empty blocks)
                {
                    if (b.GridIndex.X < 0 || b.GridIndex.X >= Grid.RowCnt || b.GridIndex.Y >= Grid.ColCnt) // Check if it is within the level boundaries
                        return false;
                    else if (Grid.Instance.Blocks[b.GridIndex.X, b.GridIndex.Y] != null) // Check if there isn't already a block there
                        return false;
                }
            }
            return true;
        }

        public bool CollidesWithOtherBlocks()
        {
            return !IsAllowed(Blocks);
        }

        private enum eCollType {None, Block, OutsideLevel}
        void Move(int moveX, int moveY)
        {
            eCollType collType = eCollType.None;

            // Check if movement is allowed
            foreach (Block b in Blocks)
            {
                if (b != null)
                {
                    Point testPoint = new Point(b.GridIndex.X + moveX, b.GridIndex.Y + moveY);
                    if (testPoint.X < 0 || testPoint.X >= Grid.RowCnt ||
                            testPoint.Y < 0 || testPoint.Y >= Grid.ColCnt)
                    {
                        // goes outside grid boundary
                        collType = eCollType.OutsideLevel;

                        // if we are moving down then plant it NEXT time it should move
                        if (moveY > 0)
                            PlantNextTick = true;

                        // Save cpu
                        break;
                    }
                    else if (Grid.Instance.Blocks[testPoint.X, testPoint.Y] != null)
                    {
                        // Collides with another block
                        collType = eCollType.Block;

                        // if we are moving down then plant it NEXT time it should move
                        if (moveY > 0)
                            PlantNextTick = true;

                        // Save cpu
                        break;
                    }
                }
            }

            if (collType == eCollType.None)
            {
                PlantNextTick = false;

                GridIdx.X += moveX;
                GridIdx.Y += moveY;

                // Update the grid-index of each block
                for (int y = 0; y < BlocksRowCnt; y++)
                {
                    for (int x = 0; x < BlocksRowCnt; x++)
                    {
                        if (Blocks[x, y] != null)
                            Blocks[x, y].GridIndex = Blocks[x, y].ShapeIndex.Add(GridIdx);
                    }
                }

                // Check if we reached the bottom
                foreach (Block b in Blocks)
                {
                    if (b != null)
                    {
                        if (b.GridIndex.Y == Grid.ColCnt - 1)
                        {
                            // a block reached the bottom
                            PlantNextTick = true;
                            return;
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDisposed)
            {
                // Get input from player
                if (Engine.Instance.KB.KeyIsReleased(Keys.Up))
                {
                    Block[,] rotatedBlocks = RotateRight();
                    if (IsAllowed(rotatedBlocks))
                        Blocks = rotatedBlocks;
                }

                // Update input delay
                MoveLeftDelayTimer.Update(gameTime);
                MoveRightDelayTimer.Update(gameTime);
                MoveDownDelayTimer.Update(gameTime);

                if (Engine.Instance.KB.CurrentKeyboardState.IsKeyDown(Keys.Left) && MoveLeftDelayTimer.IsDone)
                {
                    Move(-1, 0);
                    MoveLeftDelayTimer.Reset();
                }
                else if (Engine.Instance.KB.CurrentKeyboardState.IsKeyDown(Keys.Right) && MoveRightDelayTimer.IsDone)
                {
                    Move(1, 0);
                    MoveRightDelayTimer.Reset();
                }
                else if (Engine.Instance.KB.CurrentKeyboardState.IsKeyDown(Keys.Down) && MoveDownDelayTimer.IsDone)
                {
                    Move(0, 1);
                    MoveDownDelayTimer.Reset();
                }
                else if (Engine.Instance.KB.KeyIsReleased(Keys.Space))
                {
                    IsShiftingDown = true;
                }

                UpdateTimer.Update(gameTime);
                if (UpdateTimer.IsDone || IsShiftingDown)
                {
                    if (PlantNextTick)
                    {
                        Plant();
                        return;
                    }
                    else
                    {
                        UpdateTimer = new SimpleTimer(Level.Instance.FallDelayInMS);
                        // Fall down by 1
                        Move(0, 1);
                    }
                }
            }
        }

        void Plant()
        {
            // Add blocks to grid
            foreach (Block b in Blocks)
            {
                if (b != null)
                    Grid.Instance.Blocks[b.GridIndex.X, b.GridIndex.Y] = b;
            }

            // Mark disposed to the level an load a new shape
            IsDisposed = true;

            Grid.Instance.HandleCompleteRows();
            Level.Instance.Score += BlockCnt * Level.Instance.LevelNr;
        }

        public void DrawInGUI(Vector2 shapeLocation)
        {
            foreach (Block b in Blocks)
                if (b != null)
                    b.Draw(shapeLocation);
        }

        public void Draw()
        {
            foreach (Block b in Blocks)
            {
                if (b != null)
                    b.Draw();
            }
        }

    }
}
