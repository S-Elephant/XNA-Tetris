using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNALib;

namespace Tetris
{
    internal class Credit
    {
        private bool m_IsTitle;

        public bool IsTitle
        {
            get { return m_IsTitle; }
            set { m_IsTitle = value; }
        }
        private string m_Person;

        public string Person
        {
            get { return m_Person; }
            set { m_Person = value; }
        }
        private Vector2 m_Location;

        public Vector2 Location
        {
            get { return m_Location; }
            set { m_Location = value; }
        }

        public Credit(bool isTitle, string person)
        {
            IsTitle = isTitle;
            Person = person;
            //Location = location;
        }
    }

    public class Credits : IActiveState
    {
        #region Members
        private SpriteFont m_Font;
        private SpriteFont Font
        {
            get { return m_Font; }
            set { m_Font = value; }
        }
        private SpriteFont m_FontTitle;
        private SpriteFont FontTitle
        {
            get { return m_FontTitle; }
            set { m_FontTitle = value; }
        }

        private Color m_FontColor = Color.Gold;
        private Color FontColor
        {
            get { return m_FontColor; }
            set { m_FontColor = value; }
        }

        private List<Credit> m_AllCredits = new List<Credit>();
        private List<Credit> AllCredits
        {
            get { return m_AllCredits; }
            set { m_AllCredits = value; }
        }
        #endregion

        public Credits()
        {
            Font = Common.str2Font("Credit");
            FontTitle = Common.str2Font("CreditTitle");
            AllCredits.Add(new Credit(true, "Tetris"));
            AllCredits.Add(new Credit(true, "Created on August (9th & 10th) 2011"));
            AllCredits.Add(new Credit(true, ""));
            AllCredits.Add(new Credit(true, ""));
            AllCredits.Add(new Credit(true, "Programming & Design"));
            AllCredits.Add(new Credit(false, "C.a.r.l.o v.o.n R.a.n.z.o.w"));
            AllCredits.Add(new Credit(true, ""));
            AllCredits.Add(new Credit(true, "Audio"));
            AllCredits.Add(new Credit(false, "grunz (http://www.freesound.org) - level up sound"));
            AllCredits.Add(new Credit(false, "cfork & acclivity & (http://www.freesound.org) - plopp sounds"));
            AllCredits.Add(new Credit(false, "Destructavator (www.opengameart.org) - countdown sound"));
            AllCredits.Add(new Credit(true, ""));
            AllCredits.Add(new Credit(true, "Graphics"));
            AllCredits.Add(new Credit(false, "Azoris (www.opengameart.org) - cartoony backgrounds"));
            AllCredits.Add(new Credit(false, "C.a.r.l.o v.o.n R.a.n.z.o.w - Everything else"));
            AllCredits.Add(new Credit(true, ""));
            AllCredits.Add(new Credit(true, "Special Thanks & Tools"));
            AllCredits.Add(new Credit(false, "The Gimp"));

            int y = Engine.Instance.Height;
            foreach (Credit credit in AllCredits)
            {
                SpriteFont measureFont;
                if (credit.IsTitle)
                    measureFont = FontTitle;
                else
                    measureFont = Font;
                credit.Location = Common.CenterStringX(measureFont, credit.Person, Engine.Instance.Width, y);
                y += (int)measureFont.MeasureString(Common.MeasureString).Y;
            }
        }

        public void Update(GameTime gameTime)
        {
            // Scroll down
            foreach (Credit credit in AllCredits)
                credit.Location = new Vector2(credit.Location.X, credit.Location.Y - 1);

            // Input
            if (Engine.Instance.KB.KeyIsReleased(Keys.Enter) || Engine.Instance.KB.KeyIsReleased(Keys.Space))
            {
                Engine.Instance.ActiveState = new MainMenu();
            }
            if (Engine.Instance.KB.KeyIsReleased(Keys.Escape))
                Engine.Instance.Game.Exit();
        }

        public void Draw()
        {
            Engine.Instance.Game.GraphicsDevice.Clear(Color.Black);
            foreach (Credit credit in AllCredits)
            {
                if (credit.IsTitle)
                    Engine.Instance.SpriteBatch.DrawString(FontTitle, credit.Person, credit.Location, Color.Yellow);
                else
                    Engine.Instance.SpriteBatch.DrawString(Font, credit.Person, credit.Location, Color.Yellow);
            }
        }
    }
}
