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
    class Player
    {
        public float rotation = 0.0f;
        public float rotation_b = 0.0f;
        public Texture2D spriteT = null;
        public Texture2D spriteB = null;

        public Vector2 velocity = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        private Vector2 startPos;

        public HealthBar healthBar;

        public int type;

        public bool fire;
        public bool move1;
        public bool move2;
        public bool move3;

        private int id;
        public bool deploy;
        
        public int energy;

        public int score;
        public Weapons weapon;
        public int activeWeapon;
        public Statistics stat;
        public SpellMenu spellMenu;
        public NarcolepsyEffect narc;

        public bool isAlive;

        public int ID
        {
            get
            {
                return id;
            }
        }

        public Player(int i, Texture2D t, Texture2D b, Weapons w, Vector2 pos, int ty, Statistics s, Texture2D[] sMenu, Viewport viewport, SpriteFont f, Texture2D healthBack, Texture2D healthFront, Texture2D narcMenu, Texture2D[] narcTexArray)
        {
            energy = 100;
            deploy = false;
            id = i;
            isAlive = true;
 
            fire = false;
            move1 = false;
            move2 = false;
            move3 = false;

            position = pos;
            startPos = pos;
            velocity = Vector2.Zero;
            spriteT = t;
            spriteB = b;
            rotation = 0.0f;
            rotation_b = 0.0f;
            weapon = w;
            activeWeapon = 0;
            stat = s;

            type = ty;
            spellMenu = new SpellMenu(sMenu, viewport, id);
            narc = new NarcolepsyEffect(f,narcMenu,narcTexArray, viewport, id);
            Vector2 healthPos;
            if (id == 1)
                healthPos = new Vector2(viewport.Width / 15, viewport.Height / 15);
            else
                healthPos = new Vector2(viewport.Width * 12 / 16, viewport.Height / 15);
            healthBar = new HealthBar(healthBack, healthFront, healthPos);
        }
        public void Restart()
        {
            energy = 100;
            deploy = false;
            isAlive = true;

            fire = false;
            move1 = false;
            move2 = false;
            move3 = false;

            position = startPos;
            velocity = Vector2.Zero;
            rotation = 0.0f;
            rotation_b = 0.0f;
            activeWeapon = 0;
            healthBar.Restart();
        }
    }
}
