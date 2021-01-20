using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNALib;

namespace Tetris
{
    public class Block : IClone
    {
        public Point GridIndex;
        public Point ShapeIndex;
        public const int SIZE = 32;
        static readonly Texture2D Texture = Common.str2Tex("blockBG");
        static readonly Texture2D ShineTexture = Common.str2Tex("blockShine");
        public Color DrawColor = Color.White;

        public Block(Point shapeIdx, Point gridIdx)
        {
            ShapeIndex = shapeIdx;
            GridIndex = gridIdx;
        }

        /// <summary>
        /// For drawing the border
        /// </summary>
        /// <param name="blockLocation"></param>
        public void DrawForBorder(Vector2 blockLocation)
        {
            Engine.Instance.SpriteBatch.Draw(Texture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(blockLocation), Color.Black);
            Engine.Instance.SpriteBatch.Draw(ShineTexture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(blockLocation), Color.White);
        }

        /// <summary>
        /// For drawing the next block
        /// </summary>
        /// <param name="shapeLocation"></param>
        public void Draw(Vector2 shapeLocation)
        {
            Vector2 actualLocation = shapeLocation + new Vector2(ShapeIndex.X * SIZE + Grid.BLOCK_SPACING * ShapeIndex.X, ShapeIndex.Y * SIZE + Grid.BLOCK_SPACING * ShapeIndex.Y);
            Engine.Instance.SpriteBatch.Draw(Texture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(actualLocation), DrawColor);
            Engine.Instance.SpriteBatch.Draw(ShineTexture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(actualLocation), Color.White);
        }

        /// <summary>
        /// For drawing inside the Grid
        /// </summary>
        public void Draw()
        {
            Engine.Instance.SpriteBatch.Draw(Texture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(Grid.Instance.IdxToLoc(GridIndex)), DrawColor);
            Engine.Instance.SpriteBatch.Draw(ShineTexture, new Rectangle(0, 0, SIZE, SIZE).AddVector2(Grid.Instance.IdxToLoc(GridIndex)), Color.White);
        }

        public object CloneDeep()
        {
            return new Block(ShapeIndex, GridIndex) { DrawColor = this.DrawColor };
        }

        public object CloneShallow()
        {
            return CloneDeep();
        }
    }
}