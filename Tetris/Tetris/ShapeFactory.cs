using Microsoft.Xna.Framework;
using XNALib;

namespace Tetris
{
    public enum eShape { None=0, I=1, J=2, L=3, O=4, Z=5, T=6, S=7,
        X1=8, Plus1=9, Rectangle=10,
        X2=11, Plus2=12 }
    public static class ShapeFactory
    {
        private static void AddBlock(ref Shape shape, int x, int y)
        {
            shape.Blocks[x, y] = new Block(new Point(x, y), shape.GridIdx.Add(new Point(x, y)));
        }

        public static Shape Create(eShape shape)
        {
            Shape result = new Shape(shape);
            switch (shape)
            {
                case eShape.I:
                    AddBlock(ref result, 3, 0);
                    AddBlock(ref result, 3, 1);
                    AddBlock(ref result, 3, 2);
                    AddBlock(ref result, 3, 3);
                    AddBlock(ref result, 2, 3);
                    result.SetColor(Color.Yellow);
                    result.BlockCnt = 5;
                    break;
                case eShape.J:
                    AddBlock(ref result, 2, 0);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 2, 3);
                    AddBlock(ref result, 1, 3);
                    result.SetColor(Color.Red);
                    result.BlockCnt = 5;
                    break;
                case eShape.L:
                    AddBlock(ref result, 2, 0);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 2, 3);
                    AddBlock(ref result, 3, 3);
                    result.SetColor(Color.LimeGreen);
                    result.BlockCnt = 5;
                    break;
                case eShape.O:
                    AddBlock(ref result, 1, 1);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 2, 2);
                    result.SetColor(Color.Purple);
                    result.BlockCnt = 4;
                    break;
                case eShape.Z:
                    AddBlock(ref result, 1, 1);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 3, 2);
                    result.SetColor(Color.Orange);
                    break;
                case eShape.T:
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 3, 2);
                    result.SetColor(Color.Blue);
                    result.BlockCnt = 4;
                    break;
                case eShape.S:
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 3, 1);
                    result.SetColor(Color.Pink);
                    result.BlockCnt = 4;
                    break;
                #region ExtendedShapes
                case eShape.X1:
                    AddBlock(ref result, 1, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 3, 3);
                    AddBlock(ref result, 1, 3);
                    AddBlock(ref result, 3, 1);
                    result.SetColor(Color.Silver);
                    result.BlockCnt = 5;
                    break;
                case eShape.Plus1:
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 2, 3);
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 3, 2);
                    result.SetColor(Color.SeaShell);
                    result.BlockCnt = 5;
                    break;
                case eShape.Rectangle:
                    AddBlock(ref result, 1, 1);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 3, 1);
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 3, 2);
                    result.SetColor(Color.Purple);
                    result.BlockCnt = 6;
                    break;
                #endregion
                #region Insane shapes
                case eShape.X2:
                    AddBlock(ref result, 0, 0);
                    AddBlock(ref result, 1, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 3, 3);
                    AddBlock(ref result, 4, 4);
                    AddBlock(ref result, 0, 4);
                    AddBlock(ref result, 1, 3);
                    AddBlock(ref result, 4, 0);
                    AddBlock(ref result, 3, 1);
                    result.SetColor(Color.Silver);
                    result.BlockCnt = 9;
                    break;
                case eShape.Plus2:
                    AddBlock(ref result, 2, 0);
                    AddBlock(ref result, 2, 1);
                    AddBlock(ref result, 2, 2);
                    AddBlock(ref result, 2, 3);
                    AddBlock(ref result, 2, 4);
                    AddBlock(ref result, 0, 2);
                    AddBlock(ref result, 1, 2);
                    AddBlock(ref result, 3, 2);
                    AddBlock(ref result, 4, 2);
                    result.SetColor(Color.SeaShell);
                    result.BlockCnt = 9;
                    break;
                #endregion
                default:
                    throw new CaseStatementMissingException();
            }

            // Random colors
            if (!Level.Instance.OriginalColors)
                result.SetColor(Misc.RandomColor());
            
            // Return
            return result;
        }

        public static Shape CreateRandom(eGameMode gameMode)
        {
            switch (gameMode)
            {
                case eGameMode.Normal:
                    return Create((eShape)Maths.RandomNr(1, 7));
                case eGameMode.Extended:
                    return Create((eShape)Maths.RandomNr(1, 10));
                case eGameMode.Insane:
                    return Create((eShape)Maths.RandomNr(1, 12));
                default:
                    throw new CaseStatementMissingException();
            }
        }
    }
}
