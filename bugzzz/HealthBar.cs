﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Bugzzz
{
    class HealthBar
    {
        #region Fields (Backgrounds, Bar, current, max, destination)
        private Texture2D background;
        private Texture2D bar;
        private Vector2 position;

        float current;
        const float max = 100f;
        float destination;

        const float increment = 0.5f;

        #endregion




        #region Main Methods (Constructor, Update, Decrement, Refresh, Draw)

        public HealthBar(Texture2D back, Texture2D front, Vector2 pos)
        {
            this.background = back;
            this.bar = front;
            this.position = pos;
            this.Initialize();

        }
        private void Initialize()
        {
            current = max;
            destination = max;
        }

        public void Update()
        {
            if (current != destination)
                current -= increment;
            if (current <= 0f)
                this.Initialize();
        }

        public void Decrement()
        {
            this.destination -= 25;
        }


        public void Draw(SpriteBatch s)
        {
            s.Draw(background, new Rectangle((int)position.X, (int)position.Y, (int)background.Width, (int)background.Height), Color.White);
            s.Draw(bar, new Rectangle((int)position.X + 2, (int)position.Y + 2, (int)((bar.Width * current / 100)-4), (int)(bar.Height*.8)), Color.White);
        }

        #endregion
    }
}
