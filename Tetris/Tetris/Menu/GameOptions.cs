using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNALib;
using XNALib.Menu;

namespace Tetris
{
    public class GameOptions : BaseMenuBarred
    {
        const string Title = "Set options";
        readonly SpriteFont Font = Common.str2Font("MenuTitle");

#if WINDOWS
        public GameOptions() :
            base(Engine.Instance, null, Engine.Instance.KB)
#elif XBOX
        public GameOptions(int playerCnt) :
            base(Engine.Instance, null)
#endif
        {
            ChoiceDrawColor = Color.White;
            AddChoice("play", "Play");
            AddChoice("level", "Level", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10");
            AddChoice("mode", "Game Mode: ", eGameMode.Normal,eGameMode.Extended, eGameMode.Insane);
            AddChoice("colors", "Use Original Colors: ", "Yes", "No");
            AddChoice("back", "Back");
            OnEnterChoice += new OnEnterChoiceEventHandler(MainMenu_OnEnterChoice);
        }

        void MainMenu_OnEnterChoice(ChoiceBarred choice)
        {
            if (choice.Name == "back")
            {
                Engine.Instance.Audio.PlaySound("levelUp");
                Engine.Instance.ActiveState = new MainMenu();
            }
            else if (choice.Name == "play")
            {
                Level.Instance = new Level((eGameMode)Enum.Parse(typeof(eGameMode), ChoiceValues["mode"], true));
                Level.Instance.Init();
                Level.Instance.OriginalColors = ChoiceValues["colors"] == "Yes";
                Level.Instance.SetLevel(int.Parse(ChoiceValues["level"]));
                Engine.Instance.ActiveState = Level.Instance;
                Engine.Instance.Audio.PlaySound("countdown");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
            Engine.Instance.SpriteBatch.DrawString(Font, Title, new Vector2(Engine.Instance.Width / 2 - Font.MeasureString(Title).X / 2, 40), Color.White);
        }
    }
}