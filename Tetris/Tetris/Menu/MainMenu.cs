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
    public class MainMenu : BaseMenuBarred
    {
#if WINDOWS
        public MainMenu() :
            base(Engine.Instance, null, Engine.Instance.KB)
#elif XBOX
        public MainMenu() :
            base(Engine.Instance, null)
#endif
        {
            //Texture = Common.str2Tex("Menu/MainBG01");
            //Engine.Instance.Audio.PlaySound("mainMenu");

            Choices.Add(new ChoiceBarred(new Vector2(150, 200), "Play", "play", Color.White));
            Choices.Add(new ChoiceBarred(new Vector2(150, 300), "Toggle full screen", "fullscreen", Color.White));
            Choices.Add(new ChoiceBarred(new Vector2(150, 400), "Credits", "credits", Color.White));
            Choices.Add(new ChoiceBarred(new Vector2(150, 500), "Exit", "exit", Color.White));
            OnEnterChoice += new OnEnterChoiceEventHandler(MainMenu_OnEnterChoice);
        }

        void MainMenu_OnEnterChoice(ChoiceBarred choice)
        {
            switch (choice.Name)
            {
                case "play":
                    //Engine.Instance.Audio.StopAllMusic();
                    Engine.Instance.ActiveState = new GameOptions();
                    Engine.Instance.Audio.PlaySound("levelUp");
                    break;
                case "fullscreen":
                    Engine.Instance.Graphics.IsFullScreen = !Engine.Instance.Graphics.IsFullScreen;;
                    Engine.Instance.Graphics.ApplyChanges();
                    break;
                case "credits":
                    //Engine.Instance.Audio.StopAllMusic();
                    Engine.Instance.ActiveState = new Credits();
                    Engine.Instance.Audio.PlaySound("levelUp");
                    break;
                case "exit":
                    Engine.Instance.Game.Exit();
                    break;
                default:
                    throw new CaseStatementMissingException();
            }
        }
    }
}