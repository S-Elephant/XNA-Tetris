using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNALib;

namespace Tetris
{
    public enum eGameMode { Normal, Extended, Insane }
    public class Level : IActiveState
    {
        #region Members
        enum eLevelState { Playing, CountingDown, QuitConfirm }

        eLevelState State = eLevelState.CountingDown;
        CountDown CountDown;

        public static Level Instance;

        public int FallDelayInMS = 750;
        const int LevelDelayDecrementer = 65;
        Shape ActiveShape;
        Shape NextShape;

        public int Score;
        public int LevelNr;
        public int Lives;
        static readonly SpriteFont GUIFont = Common.str2Font("GUI");
        Texture2D BG;
        static readonly Texture2D GUIBG = Common.str2Tex("guiBG");
        static readonly Texture2D QuitDlgBG = Common.str2Tex("quitMenu");
        public bool OriginalColors;
        static readonly SpriteFont CountDownFont = Common.str2Font("CountDown");

        readonly Dictionary<int, int> LevelUpLookup = new Dictionary<int, int>()
        {
            {1,1000},
            {2,2000},
            {3,4000},
            {4,10000},
            {5,25000},
            {6,50000},
            {7,15000},
            {8,200000},
            {9,400000},
            {10,int.MaxValue}
        };

        eGameMode GameMode;
        #endregion

        public Level(eGameMode gameMode)
        {
            GameMode = gameMode;
        }

        public void Init()
        {
            ActiveShape = ShapeFactory.CreateRandom(GameMode);
            NextShape = ShapeFactory.CreateRandom(GameMode);
            LevelNr = 1;
            Score = 0;
            Lives = 2;
            CountDown = new CountDown(3000, 1000);
            SetRandomBG();
            Grid.Instance = new Grid();
        }

        void SetRandomBG()
        {
            BG = Common.str2Tex("BG/bg0" + Maths.RandomNr(1, 1).ToString());
        }

        void GameOver()
        {
            Engine.Instance.ActiveState = new MainMenu();
        }

        void LoseLife()
        {
            Lives--;
            if (Lives < 0)
                GameOver();
            else
            {
                Grid.Instance = new Grid();
                ActiveShape = ShapeFactory.CreateRandom(GameMode);
                NextShape = ShapeFactory.CreateRandom(GameMode);
            }
        }

        public void SetLevel(int level)
        {
            LevelNr = level;
            FallDelayInMS -= (level-1) * LevelDelayDecrementer;
        }

        public void Update(GameTime gameTime)
        {
            switch (State)
            {
                case eLevelState.Playing:
                    ActiveShape.Update(gameTime);
                    if (ActiveShape.IsDisposed)
                    {
                        ActiveShape = NextShape;
                        NextShape = ShapeFactory.CreateRandom(GameMode);
                        if (ActiveShape.CollidesWithOtherBlocks())
                            LoseLife();
                    }

                    // Upgrade level when applicable
                    if (Score >= LevelUpLookup[LevelNr])
                    {
                        LevelNr++;
                        FallDelayInMS -= LevelDelayDecrementer;
                        Engine.Instance.Audio.PlaySound("levelUp");
                    }

                    if (Engine.Instance.KB.KeyIsReleased(Keys.Escape))
                        State = eLevelState.QuitConfirm;
                    if (Engine.Instance.KB.KeyIsReleased(Keys.B))
                        SetRandomBG();
                    break;
                case eLevelState.CountingDown:
                    CountDown.Update(gameTime);
                    if (CountDown.IsInterval)
                    {
                        if (CountDown.TimeLeftSec > 0)
                            Engine.Instance.Audio.PlaySound("countdown");
                        else
                        {
                            Engine.Instance.Audio.PlaySound("countdownFinished");
                            State = eLevelState.Playing;
                        }
                    }
                    break;
                case eLevelState.QuitConfirm:
                    if (Engine.Instance.KB.KeyIsReleased(Keys.Y))
                        Engine.Instance.ActiveState = new MainMenu();
                    else if (Engine.Instance.KB.KeyIsReleased(Keys.N))
                        State = eLevelState.Playing;
                    break;
                default:
                    throw new CaseStatementMissingException();
            }
        }

        public void Draw()
        {
            Vector2 guiOffset = new Vector2(700, 128);

            Engine.Instance.SpriteBatch.Draw(BG, Engine.Instance.ScreenArea, Color.White);
            Engine.Instance.SpriteBatch.Draw(GUIBG, guiOffset + new Vector2(-30, -50), Color.White);

            Grid.Instance.Draw();
            ActiveShape.Draw();

            Engine.Instance.SpriteBatch.DrawString(GUIFont, string.Format("Score: {0}", Score), guiOffset+new Vector2(0, 0), Color.White);
            Engine.Instance.SpriteBatch.DrawString(GUIFont, string.Format("Level: {0}", LevelNr), guiOffset+new Vector2(0, 50), Color.White);
            Engine.Instance.SpriteBatch.DrawString(GUIFont, string.Format("Lives: {0}", Lives), guiOffset + new Vector2(0, 75), Color.White);
            Engine.Instance.SpriteBatch.DrawString(GUIFont, "Next Block", guiOffset + new Vector2(0, 100), Color.White);

            NextShape.DrawInGUI(guiOffset + new Vector2(0, 150));

            if (State == eLevelState.CountingDown)
            {
                Engine.Instance.SpriteBatch.DrawString(CountDownFont, (CountDown.TimeLeftSec+1).ToString(), Grid.Instance.GridArea.CenterVector(), Color.White);
            }
            else if (State == eLevelState.QuitConfirm)
            {
                Engine.Instance.SpriteBatch.Draw(QuitDlgBG, Engine.Instance.ScreenArea.CenterVector() - new Vector2(QuitDlgBG.Width / 2, QuitDlgBG.Height / 2), Color.White);
            }
        }
    }
}
