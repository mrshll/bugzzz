using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

namespace Bugzzz
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>z
    /// 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //arrays containing bullet entities and enemies
        GameObject[] bullets;
        GameObject[] bullets2;
        GameObject[] turretBullets1;
        GameObject[] turretBullets2;
        GameObject[] enemies;
        ArrayList pickups;
        ArrayList score;
        Turret turret1; //Player 1 turret
        Turret turret2; //Player 2 turret
        const int maxEnemies = 20;
        const int maxBullets = 30;
        int[] enemies_level;
        int level;
        int enemies_killed;

        // Score display time on screen as it fades out
        const int SCORE_TIME = 80;
        const int fade_length = 150;
        const float fade_increment = 0.5f;
        float current_fade;
        bool fade_in, fade_out, scoreScreen, act_fade;
        
        Player player1;
        Player player2;
        SpriteFont scorefont;
        SpriteFont levelfont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewport;
        Rectangle viewportRect;
        LevelScore ls;

        Random rand;
        //for fire delay
        float elapsedTime = 0;
        float elapsedTime2 = 0;
        float t_elapsedTime = 0;
        float t2_elapsedTime = 0;
        float fireDelay = 0.15f;
        Texture2D healthBar;

        Weapons p1_w;
        Weapons p2_w;

        //rotation increment
        float angle_rot = .18f;
        float turret_rot = .23f;

        KeyboardState previousKeyboardState = Keyboard.GetState();
        MouseState previousMouseState = Mouse.GetState();

        //particle effects zomg!
        ParticleEffect particleEffect;
        PointSpriteRenderer particleRenderer;

        GameTime gt;

        Texture2D[] level_backgrounds;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Attempt to set the display mode to the desired resolution.  Itterates through the display
        /// capabilities of the default graphics adapter to determine if the graphics adapter supports the
        /// requested resolution.  If so, the resolution is set and the function returns true.  If not,
        /// no change is made and the function returns false.
        /// </summary>
        /// <param name="iWidth">Desired screen width.</param>
        /// <param name="iHeight">Desired screen height.</param>
        /// <param name="bFullScreen">True if you wish to go to Full Screen, false for Windowed Mode.</param>
        private bool InitGraphicsMode(int iWidth, int iHeight, bool bFullScreen)
        {
            // If we aren't using a full screen mode, the height and width of the window can
            // be set to anything equal to or smaller than the actual screen size.
            if (bFullScreen == false)
            {
                if ((iWidth <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                    && (iHeight <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height))
                {
                    graphics.PreferredBackBufferWidth = iWidth;
                    graphics.PreferredBackBufferHeight = iHeight;
                    graphics.IsFullScreen = bFullScreen;
                    graphics.ApplyChanges();
                    return true;
                }
            }
            else
            {
                // If we are using full screen mode, we should check to make sure that the display
                // adapter can handle the video mode we are trying to set.  To do this, we will
                // iterate thorugh the display modes supported by the adapter and check them against
                // the mode we want to set.
                foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
                {
                    // Check the width and height of each mode against the passed values
                    if ((dm.Width == iWidth) && (dm.Height == iHeight))
                    {
                        // The mode is supported, so set the buffer formats, apply changes and return
                        graphics.PreferredBackBufferWidth = iWidth;
                        graphics.PreferredBackBufferHeight = iHeight;
                        graphics.IsFullScreen = bFullScreen;
                        graphics.ApplyChanges();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Infitialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //player's stating position
            InitGraphicsMode(1280, 720, false);
            current_fade = 0;
            fade_in = true;
            rand = new Random();
            enemies_killed = 0;
            enemies_level = new int[4];
            enemies_level[0] = 100;
            enemies_level[1] = 250;
            enemies_level[2] = 500;
            enemies_level[3] = 1000;


            level = 0;
            viewport = GraphicsDevice.Viewport;
            viewportRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            bullets = new GameObject[maxBullets];
            bullets2 = new GameObject[maxBullets];

            turretBullets1 = new GameObject[maxBullets];
            turretBullets2 = new GameObject[maxBullets];

            enemies = new GameObject[maxEnemies];
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            Texture2D temp = Content.Load<Texture2D>("sprites\\cannon");
            turret1 = new Turret(temp);
            turret2 = new Turret(temp);
            
            healthBar = Content.Load<Texture2D>("sprites\\healthBar");
            temp = null;
            temp = Content.Load<Texture2D>("sprites\\cannonball");

            p1_w = new Weapons(temp, temp, temp);
            p2_w = new Weapons(temp, temp, temp);


            pickups = new ArrayList();
            //Initializes all of the bullets, enemies, etc.

            level_backgrounds = new Texture2D[5];
            level_backgrounds[0] = Content.Load<Texture2D>("sprites\\level1_background");

            for (int i = 0; i < maxBullets; i++)
            {
                bullets2[i] = new GameObject(temp);
                bullets[i] = new GameObject(temp);
            }

            for (int i = 0; i < maxBullets; i++)
            {
                turretBullets1[i] = new GameObject(temp);
                turretBullets2[i] = new GameObject(temp);
            }

            temp = Content.Load<Texture2D>("sprites\\roach");

            for (int j = 0; j < maxEnemies; j++)
            {
                enemies[j] = new GameObject(temp);
            }

            temp = Content.Load<Texture2D>("sprites\\cannon");
            //spell menu textures
            Texture2D[] sMenu = new Texture2D[4];
            sMenu[0] = Content.Load<Texture2D>("SpellMenus\\SpiderBar1Active");
            sMenu[1] = Content.Load<Texture2D>("SpellMenus\\SpiderBar2Active");
            sMenu[2] = Content.Load<Texture2D>("SpellMenus\\SpiderBar3Active");
            sMenu[3] = Content.Load<Texture2D>("SpellMenus\\SpiderBar4Active");

            player1 = new Player(1, temp, Content.Load<Texture2D>("sprites\\smiley1"), p1_w, new Vector2(viewport.Width*7/15,viewport.Height/2), 1, new Statistics(true), sMenu, viewport);
            player2 = new Player(2, temp, Content.Load<Texture2D>("sprites\\smiley1"), p2_w, new Vector2(viewport.Width*8/15, viewport.Height/2), 2, new Statistics(true), sMenu, viewport);

            
            // The maximum amount of scores to display on screen is the maximum number of dead enemies
            score = new ArrayList();

            // Loading in the font we will use for showing the killed enemies score value
            scorefont = Content.Load<SpriteFont>("ScoreFont");
            levelfont = Content.Load<SpriteFont>("LevelFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            particleEffect = new ParticleEffect
            {
                new Emitter
                {
                    Budget = 1000,
                    Term = 3f,

                    Name = "TestEmitter",
                    BlendMode = BlendMode.Alpha,
                    ReleaseQuantity = 3,
                    ReleaseRotation = new VariableFloat { Value = 0f, Variation = MathHelper.Pi },
                    ReleaseScale = 64f,
                    ReleaseSpeed = new VariableFloat { Value = 64f, Variation = 32f },
                    ParticleTextureAssetName = "Particle003",
                    Modifiers = new ModifierCollection
                    {
                        new OpacityModifier
                        {
                            Initial = 1f,
                            Ultimate = 0f,
                        },
                        new ColourModifier
                        {
                            InitialColour = Color.Tomato.ToVector3(),
                            UltimateColour = Color.Lime.ToVector3(),
                        },
                    },
                },
            };

            particleRenderer = new PointSpriteRenderer
            {
                GraphicsDeviceService = graphics
            };

            particleEffect.Initialise();

            particleEffect.LoadContent(Content);
            particleRenderer.LoadContent(Content);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 


        public void fireBullets(Player p)
        {
            //firing command
            if (p.activeWeapon == 0)
            {
                foreach (GameObject bullet in bullets)
                {
                    if (!bullet.alive)
                    {
                        bullet.alive = true;
                        bullet.position = p.position - bullet.center;
                        bullet.velocity = new Vector2((float)Math.Cos(p.rotation + Math.PI / 2), (float)Math.Sin(p.rotation + Math.PI / 2)) * 8.0f;
                        return;
                    }
                }
            }
            if (p.activeWeapon == 1)
            {
                double spread = -0.2094;
                foreach (GameObject bullet in bullets)
                {
                    if (!bullet.alive)
                    {
                        bullet.alive = true;
                        bullet.position = p.position - bullet.center;
                        bullet.velocity = new Vector2((float)Math.Cos(p.rotation + Math.PI / 2 + spread), (float)Math.Sin(p.rotation + Math.PI / 2 + spread)) * 8.0f;
                        spread += .1047;
                        if (spread > .21)
                            return;
                    }
                }
            }
            if (p.activeWeapon == 2)
            {
                foreach (GameObject bullet in bullets)
                {
                    if (!bullet.alive)
                    {
                        bullet.alive = true;
                        bullet.position = p.position - bullet.center;
                        bullet.velocity = new Vector2((float)Math.Cos(p.rotation + Math.PI / 2 + rand.NextDouble()), (float)Math.Sin(p.rotation + Math.PI / 2 + rand.NextDouble())) * 3.0f;
                        return;
                    }
                }
            }
        }



        public void fireTurretBullets1()
        {
            //firing command

            foreach (GameObject bullet in turretBullets1)
            {
                if (!bullet.alive)
                {
                    bullet.alive = true;
                    bullet.position = turret1.position - bullet.center;
                    bullet.velocity = new Vector2((float)Math.Cos(turret1.rotation + Math.PI / 2), (float)Math.Sin(turret1.rotation + Math.PI / 2)) * 15.0f;
                    turret1.bulletsLeft -= 1;
                    return;
                }
            }
        }
        public void fireTurretBullets2()
        {
            foreach (GameObject bullet in turretBullets2)
            {
                if (!bullet.alive)
                {
                    bullet.alive = true;
                    bullet.position = turret2.position - bullet.center;
                    bullet.velocity = new Vector2((float)Math.Cos(turret2.rotation + Math.PI / 2), (float)Math.Sin(turret2.rotation + Math.PI / 2)) * 15.0f;
                    turret2.bulletsLeft -= 1;
                    return;
                }
            }
        }

        public void updateBullets()
        {
            #region Player 1 Bullets
            foreach (GameObject bullet in bullets)
            {
                if (bullet.alive)
                {
                    if (player1.activeWeapon == 2)
                    {
                        Rectangle flameRect = new Rectangle((int)player1.position.X-100, (int)player1.position.Y-100, 200, 200);
                        Rectangle flameBulletRect = new Rectangle((int)bullet.position.X, (int)bullet.position.Y, bullet.sprite.Width, bullet.sprite.Height);
                        if (!flameRect.Intersects(flameBulletRect))
                            bullet.alive = false;
                    }

                    bullet.position += bullet.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet.position.X, (int)bullet.position.Y)))
                    {
                        bullet.alive = false;
                        continue;
                    }
                    Rectangle bulletRect = new Rectangle(
                        (int)bullet.position.X,
                        (int)bullet.position.Y,
                        bullet.sprite.Width,
                        bullet.sprite.Height);
                    //enemy-bullet collision detection.
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.alive)
                        {
                            Rectangle enemyRect = new Rectangle(
                                (int)enemy.position.X,
                                (int)enemy.position.Y,
                                enemy.sprite.Width,
                                enemy.sprite.Height);

                            if (MathFns.broadPhaseCollision(bulletRect, enemyRect, enemy.rotation))
                            {
                                bullet.alive = false;
                                enemy.alive = false;

                                // particle test
                                particleEffect.Trigger(new Vector2(enemy.position.X, enemy.position.Y));

                                score.Add(new ScoreDisplay(20, SCORE_TIME, enemy.position, true, 1));
                                player1.score += 20;
                                enemies_killed++;

                                // Possibly Generate a new WeaponPickup
                                int wpn_pickup_prob = rand.Next(100);
                                if (wpn_pickup_prob < 10)
                                {
                                    generateWeaponPickup(enemy.position);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Player 2 Bullets
            foreach (GameObject bullet in bullets2)
            {
                if (bullet.alive)
                {
                    bullet.position += bullet.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet.position.X, (int)bullet.position.Y)))
                    {
                        bullet.alive = false;
                        continue;
                    }
                    Rectangle bulletRect = new Rectangle(
                        (int)bullet.position.X,
                        (int)bullet.position.Y,
                        bullet.sprite.Width,
                        bullet.sprite.Height);
                    //enemy-bullet collision detection.
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.alive)
                        {
                            Rectangle enemyRect = new Rectangle(
                                (int)enemy.position.X,
                                (int)enemy.position.Y,
                                enemy.sprite.Width,
                                enemy.sprite.Height);

                            if (MathFns.broadPhaseCollision(bulletRect, enemyRect, enemy.rotation))
                            {
                                bullet.alive = false;
                                enemy.alive = false;
                                score.Add(new ScoreDisplay(20, SCORE_TIME, enemy.position, true, 2));
                                player2.score += 20;
                                enemies_killed++;

                                // Possibly Generate a new WeaponPickup
                                int wpn_pickup_prob = rand.Next(100);
                                if (wpn_pickup_prob < 10)
                                {
                                    generateWeaponPickup(enemy.position);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Turret 1 Bullets
            foreach (GameObject bullet in turretBullets1)
            {
                if (bullet.alive)
                {
                    bullet.position += bullet.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet.position.X, (int)bullet.position.Y)))
                    {
                        bullet.alive = false;
                        continue;
                    }
                    Rectangle bulletRect = new Rectangle(
                        (int)bullet.position.X,
                        (int)bullet.position.Y,
                        bullet.sprite.Width,
                        bullet.sprite.Height);
                    //enemy-bullet collision detection.
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.alive)
                        {
                            Rectangle enemyRect = new Rectangle(
                                (int)enemy.position.X,
                                (int)enemy.position.Y,
                                enemy.sprite.Width,
                                enemy.sprite.Height);

                            if (MathFns.broadPhaseCollision(bulletRect, enemyRect, enemy.rotation))
                            {
                                bullet.alive = false;
                                enemy.alive = false;
                                score.Add(new ScoreDisplay(10, SCORE_TIME, enemy.position, true, 1));
                                player1.score += 10;
                                enemies_killed++;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
            #region Turret 2 Tullets
            foreach (GameObject bullet in turretBullets2)
            {
                if (bullet.alive)
                {
                    bullet.position += bullet.velocity;
                    if (!viewportRect.Contains(new Point((int)bullet.position.X, (int)bullet.position.Y)))
                    {
                        bullet.alive = false;
                        continue;
                    }
                    Rectangle bulletRect = new Rectangle(
                        (int)bullet.position.X,
                        (int)bullet.position.Y,
                        bullet.sprite.Width,
                        bullet.sprite.Height);
                    //enemy-bullet collision detection.
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.alive)
                        {
                            Rectangle enemyRect = new Rectangle(
                                (int)enemy.position.X,
                                (int)enemy.position.Y,
                                enemy.sprite.Width,
                                enemy.sprite.Height);

                            if (MathFns.broadPhaseCollision(bulletRect, enemyRect, (float)(enemy.rotation + Math.PI / 2)))
                            {
                                bullet.alive = false;
                                enemy.alive = false;
                                score.Add(new ScoreDisplay(10, SCORE_TIME, enemy.position, true, 2));
                                player2.score += 10;
                                enemies_killed++;
                                break;
                            }
                        }
                    }
                }
            }
            #endregion
        }
   
        public void updateEnemies()
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    //checks for collision with the player
                    
                    Rectangle playerRect = new Rectangle((int)player1.position.X - player1.spriteB.Width / 2, (int)player1.position.Y - player1.spriteB.Height/2, player1.spriteB.Width, player1.spriteB.Height);
                    Rectangle enemyRect = new Rectangle((int)enemy.position.X,(int)enemy.position.Y,enemy.sprite.Width,enemy.sprite.Height);

                    //detect p1 collision
                   if (MathFns.broadPhaseCollision(playerRect, enemyRect, (float)(enemy.rotation+Math.PI/2)))
                   {
                            //alive = false;
                            enemy.alive = false;
                            if (player1.health > 0)
                            {
                                player1.health -= 25;
                            }
                            else
                            {
                                if (player1.livesLeft > 0)
                                {
                                    player1.livesLeft--;
                                    player1.stat.playerDied();
                                    player1.health = 100;
                                }
                                else
                                {
                                    // Game over
                                }
                            }
                            enemies_killed++;

                            break;
                   }
                    playerRect = new Rectangle((int)player2.position.X - player2.spriteB.Width / 2, (int)player2.position.Y - player2.spriteB.Height / 2, player2.spriteB.Width, player2.spriteB.Height);
                    
                    //detect p2 collision
                    if (MathFns.broadPhaseCollision(playerRect, enemyRect, (float)(enemy.rotation + Math.PI / 2)))
                    {
                        //alive = false;
                        enemy.alive = false;
                        if (player2.health > 0)
                        {
                            player2.health -= 25;
                        }
                        else
                        {
                            if (player2.livesLeft > 0)
                            {
                                player2.livesLeft--;
                                player2.stat.playerDied();
                                player2.health = 100;
                            }
                            else
                            {
                                // Game over
                            }
                        }
                        enemies_killed++;

                        break;
                    }
                }
                else 
                { 
                    //limits number of enemies to number per level
                    if (enemies_killed  < (enemies_level[level]-maxEnemies+1))
                    {
                        enemy.alive = true;

                        int rand1 = rand.Next(100);
                        int rand2 = rand.Next(100);
                        if (rand1 < 33)
                        {
                            enemy.position.X = -enemy.sprite.Width - 5;
                            enemy.position.Y = rand.Next(viewport.Height);
                        }
                        else if (rand1 < 66)
                        {
                            enemy.position.X = rand.Next(viewport.Width);
                            if (rand2 < 50)
                                enemy.position.Y = -enemy.sprite.Height + 5;
                            else
                                enemy.position.Y = viewport.Height + 4;
                        }
                        else
                        {
                            enemy.position.X = viewport.Width + 4;
                            enemy.position.Y = rand.Next(viewport.Height);
                        }
                    }

                }
                //only update movement if enemy is alive
                if (enemy.alive)
                {
                    //makes enemies move towards the player
                    Vector2 target;

                    if (MathFns.Distance(enemy.position, player1.position) > MathFns.Distance(enemy.position, player2.position))
                        target = player2.position;
                    else
                        target = player1.position;

                    enemy.velocity = target - enemy.position;
                    enemy.velocity.Normalize();
                    enemy.position += enemy.velocity * 2;
                    float angle = (float)(-1 * (Math.PI / 2 + Math.Atan2(enemy.velocity.X, enemy.velocity.Y)));

                    if (angle != enemy.rotation)
                        enemy.rotation = MathFns.Clerp(enemy.rotation, angle, angle_rot);
                }
            }
        }

        private void generateWeaponPickup(Vector2 pos)
        {
            Console.WriteLine("A new pickup was created"+pos);
            Texture2D sample_weapon = Content.Load<Texture2D>("sprites\\cannonball");
            pickups.Add(new WeaponPickup(sample_weapon, pos, rand.Next(2)));
        }

        private void updatePickups(Player p)
        {
            WeaponPickup destroyedPickup = null;

            Rectangle playerRect = new Rectangle(
                    (int)p.position.X,
                    (int)p.position.Y,
                    p.spriteB.Width,
                    p.spriteB.Height);
            foreach (WeaponPickup pickup in pickups)
            {
                    Rectangle pickupRect = new Rectangle(
                      (int)pickup.position.X,
                      (int)pickup.position.Y,
                      pickup.sprite.Width,
                      pickup.sprite.Height);

                    if (playerRect.Intersects(pickupRect))
                    {
                        destroyedPickup = pickup;
                        p.activeWeapon = pickup.weaponIndex;
                        break;
                    }
                } 
            if (destroyedPickup != null)
                pickups.Remove(destroyedPickup);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (!act_fade)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                    player1.deploy = true;
                if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
                    player2.deploy = true;
                
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                gt = gameTime;
                elapsedTime += elapsed;
                elapsedTime2 += elapsed;
                t_elapsedTime += elapsed;
                t2_elapsedTime += elapsed;
                // TODO: Add your update logic here
                UpdateTurret(turret1, player1);
                UpdateTurret(turret2, player2);
                UpdateInput();
                updateBullets();
                updateEnemies();
                updatePickups(player1);
                updatePickups(player2);

                //player1 spell menu
                if (player1.spellMenu.Active)
                    player1.spellMenu.moveOn();
                else if (!player1.spellMenu.Active)
                    player1.spellMenu.moveOff();

                //player2 spell menu

                if (player2.spellMenu.Active)
                    player2.spellMenu.moveOn();
                else if (!player2.spellMenu.Active)
                    player2.spellMenu.moveOff();


                // Border detection keeps players on the window
                float xmove = player1.position.X + player1.velocity.X;
                float ymove = player1.position.Y + player1.velocity.Y;
                if( (xmove>(player1.spriteB.Width/2) && xmove<(viewport.Width-player1.spriteB.Width/2)) && (ymove>(player1.spriteB.Height/2) && ymove<(viewport.Height-player1.spriteB.Height/2)) )
                    player1.position += player1.velocity;
                xmove = player2.position.X + player2.velocity.X;
                ymove = player2.position.Y + player2.velocity.Y;
                if ((xmove > (player2.spriteB.Width / 2) && xmove < (viewport.Width - player2.spriteB.Width / 2)) && (ymove > (player2.spriteB.Height / 2) && ymove < (viewport.Height - player2.spriteB.Height / 2)))
                    player2.position += player2.velocity;
                
                if (player1.energy < 100)
                {
                    player1.energy += 1;
                }
                if (player2.energy < 100)
                {
                    player2.energy += 1;
                }

                #region Fire Delay
                if ((elapsedTime >= player1.weapon.delays[player1.activeWeapon]) && player1.fire)
                {

                    elapsedTime = 0.0f;
                    fireBullets(player1);
                }
                if ((elapsedTime2 >= player2.weapon.delays[player2.activeWeapon]) && player2.fire)
                {
                    elapsedTime2 = 0.0f;
                    fireBullets(player2);
                }

                if (turret1.fire && t_elapsedTime >= fireDelay + .5 && turret1.placed)
                {
                    t_elapsedTime = 0.0f;
                    fireTurretBullets1();
                }
                if (turret2.fire && t2_elapsedTime >= fireDelay + .5 && turret2.placed)
                {
                    t2_elapsedTime = 0.0f;
                    fireTurretBullets2();
                }
                #endregion


            }

            // Update the statistical time used to calculate player statistics
            player1.stat.updateStatisticsTime(gameTime);
            player2.stat.updateStatisticsTime(gameTime);

            // particle test
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEffect.Update(deltaSeconds);

            base.Update(gameTime);
        }


        private void UpdateTurret(Turret turret, Player player)
        {
            if (turret.bulletsLeft > 0)
            {

                if (player.deploy)
                {
                    turret.position = player.position;
                    turret.rotation = 0f;
                    turret.placed = true;
                    player.deploy = false;
                }
                if (turret.placed)
                {
                    double temp = MathFns.Distance(turret.position, enemies[0].position);
                    turret.closestEnemy = 0;
                    Vector2 offset = new Vector2(); //allows for aiming at center of target

                    for (int i = 1; i < enemies.Length; i++)
                    {
                        if (MathFns.Distance(enemies[i].position,turret.position) < temp)
                        {
                           turret.closestEnemy = i;
                           offset.X = enemies[i].sprite.Width/2;
                           offset.Y = enemies[i].sprite.Height/2;
                        }
                    }

                    Vector2 aim = new Vector2(enemies[turret.closestEnemy].position.X + offset.X, enemies[turret.closestEnemy].position.Y + offset.Y);
                    Vector2 direction = turret.position - aim;
                    
                    direction.Normalize();
                    float desiredAngle = (float)Math.Acos((double)direction.X);

                    if (direction.Y < 0)
                    {
                        desiredAngle = (float)(2.0f * Math.PI) - (float)desiredAngle;
                    }

                    //Smooth Rotation
                    if (desiredAngle != turret.rotation)
                    {
                      // Console.WriteLine("Turret: " + turret.rotation + "Desired: " + desiredAngle);
                       turret.rotation = MathFns.Clerp(turret.rotation, desiredAngle + (float)Math.PI / 2, this.turret_rot);
                    }
                    
                    //previous rotation function
                    //turret.rotation = desiredAngle + (float)Math.PI / 2;
                    turret.fire = true;
                }
            }
            else
            {
                turret.placed = false;
                player.deploy = false;
                turret.bulletsLeft = 25;

            }
        }
        protected void UpdateInput()
        {


            GamePadState currentState;

            #region Player 1 Control Scheme
            currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                #region XBox Controls Player1
                player1.velocity.X = currentState.ThumbSticks.Left.X * 5;
                player1.velocity.Y = -currentState.ThumbSticks.Left.Y * 5;
                //player1.rotation = -(float)((Math.Tan(currentState.ThumbSticks.Right.Y / currentState.ThumbSticks.Right.X)*2*Math.PI)/180);
                const float DEADZONE = 0.2f;
                const float FIREDEADZONE = 0.3f;

                Vector2 direction = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                float magnitude = direction.Length();
                player1.fire = false;
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Length() > DEADZONE)
                {
                    player1.rotation_b = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Left.Y, currentState.ThumbSticks.Left.X)));
                }

                if (magnitude > DEADZONE)
                {
                    //Smooth Rotation
                    float angle = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Right.Y, currentState.ThumbSticks.Right.X)));

                    if (angle != player1.rotation)
                        player1.rotation = MathFns.Clerp(player1.rotation, angle, angle_rot);

                    if (magnitude > FIREDEADZONE)
                    {
                        player1.fire = true;
                    }
                    
                }
                if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    player1.spellMenu.Active = true;
                if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    player2.spellMenu.Active = false;

                if (player1.spellMenu.Active)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                        player1.spellMenu.stateInc();
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                        player1.spellMenu.stateDec();
                }
                #endregion

            }
            //keyboard controls
            else
            {
                #region Keyboard Controls Player1
                //player1.fire = false;
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                float XDistance = player1.position.X - mouse.X;
                float YDistance = player1.position.Y - mouse.Y;
                float xdim = 800;
                float ydim = 800;

                float rotation = (float)(Math.Atan2(YDistance, XDistance) + Math.PI / 2);
                player1.rotation = rotation;

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    player1.fire = true;
                    xdim = mouse.X;
                    ydim = mouse.Y;
                }
                else
                {
                    player1.fire = false;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    player1.velocity.X = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    player1.velocity.X = 5;
                }
                else
                {
                    player1.velocity.X = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    player1.velocity.Y = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    player1.velocity.Y = 5;
                }
                else
                {
                    player1.velocity.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    player1.deploy = true;
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                    this.Exit();

                if (keyboardState.IsKeyDown(Keys.I))
                    player1.spellMenu.Active = true;
                if (keyboardState.IsKeyDown(Keys.K))
                    player1.spellMenu.Active = false;


                if (player1.spellMenu.Active)
                {
                    if (keyboardState.IsKeyDown(Keys.U))
                        //TODO:: Fix from rotating through too fast
                        player1.spellMenu.stateDec();
                    else if (keyboardState.IsKeyDown(Keys.O))
                        player1.spellMenu.stateInc();
                }



                
                //cannon.rotation = MathHelper.Clamp(cannon.rotation, MathHelper.PiOver2, 0);
                // TODO: Add your update logic here
                previousKeyboardState = keyboardState;
                previousMouseState = mouse;

                #endregion
            }
            #endregion
            
            currentState = GamePad.GetState(PlayerIndex.Two);

            #region Player 2 Control Scheme

            if (currentState.IsConnected)
            {
                #region XBox Controller Controls Player 2
                player2.velocity.X = currentState.ThumbSticks.Left.X * 5;
                player2.velocity.Y = -currentState.ThumbSticks.Left.Y * 5;
                //player2.rotation = -(float)((Math.Tan(currentState.ThumbSticks.Right.Y / currentState.ThumbSticks.Right.X)*2*Math.PI)/180);
                const float DEADZONE = 0.2f;
                const float FIREDEADZONE = 0.3f;

                Vector2 direction = GamePad.GetState(PlayerIndex.Two).ThumbSticks.Right;
                float magnitude = direction.Length();
                player2.fire = false;
                if (GamePad.GetState(PlayerIndex.Two).ThumbSticks.Left.Length() > DEADZONE)
                {
                    player2.rotation_b = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Left.Y, currentState.ThumbSticks.Left.X)));
                }

                if (magnitude > DEADZONE)
                {
                    //Smooth Rotation
                    float angle = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Right.Y, currentState.ThumbSticks.Right.X)));

                    if (angle != player2.rotation)
                        player2.rotation = MathFns.Clerp(player2.rotation, angle, angle_rot);

                    if (magnitude > FIREDEADZONE)
                    {
                        player2.fire = true;
                    }
                }

                if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                    player1.spellMenu.Active = true;
                if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                    player2.spellMenu.Active = false;



                if (player2.spellMenu.Active)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.RightShoulder == ButtonState.Pressed)
                        player2.spellMenu.stateInc();
                    if (GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
                        player2.spellMenu.stateDec();
                }

                #endregion

            }
            //keyboard controls
            else
            {
                #region Keyboard Controls Player 2

                //player2.fire = false;
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                float XDistance = player2.position.X - mouse.X;
                float YDistance = player2.position.Y - mouse.Y;
                float xdim = 800;
                float ydim = 800;

                float rotation = (float)(Math.Atan2(YDistance, XDistance) + Math.PI / 2);
                player2.rotation = rotation;

                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    player2.fire = true;
                    xdim = mouse.X;
                    ydim = mouse.Y;
                }
                else
                {
                    player2.fire = false;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    player2.velocity.X = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    player2.velocity.X = 5;
                }
                else
                {
                    player2.velocity.X = 0;
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    player2.velocity.Y = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.S))
                {
                    player2.velocity.Y = 5;
                }
                else
                {
                    player2.velocity.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    player2.deploy = true;
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                    this.Exit();


                if (keyboardState.IsKeyDown(Keys.NumPad8))
                    player2.spellMenu.Active = true;
                if (keyboardState.IsKeyDown(Keys.NumPad5))
                    player2.spellMenu.Active = false;

                if (player2.spellMenu.Active){
                    if (keyboardState.IsKeyDown(Keys.NumPad7))
                        //TODO:: Fix from rotating through too fast
                        player2.spellMenu.stateDec();
                    else if (keyboardState.IsKeyDown(Keys.NumPad9))
                        player2.spellMenu.stateInc();
                }






                //cannon.rotation = MathHelper.Clamp(cannon.rotation, MathHelper.PiOver2, 0);
                // TODO: Add your update logic here
                previousKeyboardState = keyboardState;
                previousMouseState = mouse;
                #endregion Keyboard Controls

            }
            #endregion
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (enemies_level[level] == enemies_killed && !act_fade)
            {
                ls = new LevelScore(this.level+1, player1, player2, true, 200, levelfont, GraphicsDevice, this.healthBar);
                act_fade = true;
                enemies_killed = 0;
            }

            if (act_fade)
            {
                //Fade to Black
                float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                #region Fade In Logic
                if (fade_in)
                {
                    
                    spriteBatch.Draw(healthBar, new Rectangle(0, 0, this.viewport.Width, this.viewport.Height), new Color(Color.DarkBlue, (byte)(int)(current_fade)));
                    if (elapsed >= fade_length/12)
                    {
                        current_fade += fade_increment;
                    }
                    //Console.Out.WriteLine(current_fade);
                    if (current_fade == 22)
                    {
                        scoreScreen = true;
                        fade_in = false;
                    }
                    //Fade out
                }
                #endregion

                #region Score Screen Logic
                if (scoreScreen)
                {
                    //TODO: Add Score Screen Here
                    ls.Draw(this.viewport);
                    if (!GamePad.GetState(PlayerIndex.One).IsConnected)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Z))
                        {
                            fade_out = true;
                            scoreScreen = false;
                        }
                    }
                    else
                    {
                        if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                        {
                            fade_out = true;
                            scoreScreen = false;
                        }
                    }

                    if (!GamePad.GetState(PlayerIndex.Two).IsConnected)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Q))
                        {
                            fade_out = true;
                            scoreScreen = false;
                        }
                    }
                    else
                    {
                        if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
                        {
                            fade_out = true;
                            scoreScreen = false;
                        }
                    }


                }
                #endregion

                #region Fade Out Logic
                if (fade_out)
                {
                    spriteBatch.Draw(healthBar, new Rectangle(0, 0, this.viewport.Width, this.viewport.Height), new Color(Color.Black, (byte)(current_fade)));
                    if (elapsed >= fade_length/10)
                        current_fade -= fade_increment;

                    if (current_fade <= 0)
                    {
                        level++; //TODO: Will crash after 5 levels.
                        enemies_killed = 0;
                        fade_out = false;
                        act_fade = false;
                    }
                }
                #endregion

                spriteBatch.End();
            }
            else
            {
                current_fade = 0;
                fade_in = true;
                fade_out = false;
                scoreScreen = false;
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                //
                spriteBatch.Draw(level_backgrounds[0], new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
                spriteBatch.Draw(healthBar, new Rectangle(this.viewport.Width / 15, this.viewport.Height / 15, (int)this.viewport.Width * player1.health / 600, this.viewport.Height / 30), Color.Red);
                spriteBatch.Draw(healthBar, new Rectangle(this.viewport.Width * 12 / 16, this.viewport.Height / 15, (int)this.viewport.Width * player2.health / 600, this.viewport.Height / 30), Color.Red);
                spriteBatch.DrawString(scorefont, "Player 1 Score: " + player1.score.ToString(), new Vector2(this.viewport.Width / 15, this.viewport.Height / 60), new Color(Color.White, (byte)130));
                spriteBatch.DrawString(scorefont, "Player 2 Score: " + player2.score.ToString(), new Vector2(this.viewport.Width * 12 / 16, this.viewport.Height / 60), new Color(Color.White, (byte)130));
                spriteBatch.DrawString(scorefont, "Enemies Killed: " + enemies_killed.ToString(), new Vector2(this.viewport.Width * 7 / 16, this.viewport.Height / 60), new Color(Color.Beige, (byte)130));
                spriteBatch.Draw(player1.spriteB, new Rectangle((int)player1.position.X, (int)player1.position.Y, player1.spriteB.Width, player1.spriteB.Height), null, Color.White, player1.rotation_b, new Vector2(player1.spriteB.Width / 2, player1.spriteB.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player1.spriteT, new Rectangle((int)player1.position.X, (int)player1.position.Y, player1.spriteT.Width, player1.spriteT.Height), null, Color.White, (float)(player1.rotation + .5 * Math.PI), new Vector2(player1.spriteT.Width / 2, player1.spriteT.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player2.spriteB, new Rectangle((int)player2.position.X, (int)player2.position.Y, player2.spriteB.Width, player2.spriteB.Height), null, Color.Red, player2.rotation_b, new Vector2(player2.spriteB.Width / 2, player2.spriteB.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player2.spriteT, new Rectangle((int)player2.position.X, (int)player2.position.Y, player2.spriteT.Width, player2.spriteT.Height), null, Color.Red, (float)(player2.rotation + .5 * Math.PI), new Vector2(player2.spriteT.Width / 2, player2.spriteT.Height / 2), SpriteEffects.None, 0);
                
                //draw spell menus
                spriteBatch.Draw(player1.spellMenu.State, new Rectangle((int)player1.spellMenu.Position.X, (int)player1.spellMenu.Position.Y, player1.spellMenu.Width, player1.spellMenu.Height), new Color(Color.White, (byte)210));
                spriteBatch.Draw(player2.spellMenu.State, new Rectangle((int)player2.spellMenu.Position.X, (int)player2.spellMenu.Position.Y, player2.spellMenu.Width, player2.spellMenu.Height), new Color(Color.White, (byte)210));
                if (turret1.placed)
                {
                    spriteBatch.Draw(turret1.sprite, new Rectangle((int)turret1.position.X, (int)turret1.position.Y, turret1.sprite.Width, turret1.sprite.Height), null, Color.White, (float)(turret1.rotation + .5 * Math.PI), new Vector2(turret1.sprite.Width / 2, turret1.sprite.Height / 2), SpriteEffects.None, 0);

                }
                if (turret2.placed)
                {
                    spriteBatch.Draw(turret2.sprite, new Rectangle((int)turret2.position.X, (int)turret2.position.Y, turret2.sprite.Width, turret2.sprite.Height), null, Color.White, (float)(turret2.rotation + .5 * Math.PI), new Vector2(turret2.sprite.Width / 2, turret2.sprite.Height / 2), SpriteEffects.None, 0);

                }
                // TODO: Add your drawing code here
                #region Drawing Code:Bullets, TurretBullets, Enemies, Scores
                //player 1
                foreach (WeaponPickup pickup in pickups)
                {
                        spriteBatch.Draw(pickup.sprite, pickup.position, Color.White);
                }
                foreach (GameObject bullet in bullets)
                {
                    if (bullet.alive)
                    {
                        spriteBatch.Draw(bullet.sprite, bullet.position, Color.White);
                    }
                }
                //player 2
                foreach (GameObject bullet in bullets2)
                {
                    if (bullet.alive)
                    {
                        spriteBatch.Draw(bullet.sprite, bullet.position, Color.White);
                    }
                }
                //turret 1
                foreach (GameObject bullet in turretBullets1)
                {
                    if (bullet.alive)
                    {
                        spriteBatch.Draw(bullet.sprite, bullet.position, Color.White);
                    }
                }
                //turret 2
                foreach (GameObject bullet in turretBullets2)
                {
                    if (bullet.alive)
                    {
                        spriteBatch.Draw(bullet.sprite, bullet.position, Color.White);
                    }
                }
                foreach (GameObject enemy in enemies)
                {
                    if (enemy.alive)
                    {
                        spriteBatch.Draw(enemy.sprite, new Rectangle((int)enemy.position.X, (int)enemy.position.Y, enemy.sprite.Width, enemy.sprite.Height), null, Color.White, (float)(enemy.rotation+Math.PI/2), new Vector2(enemy.sprite.Width/2, enemy.sprite.Height/2),SpriteEffects.None, 0);
                    }
                }
                ArrayList deadScores = new ArrayList(); //Used for determing what scores need to be deleted. 
                //Output Scores
                foreach (ScoreDisplay s in score)
                {
                    if (s.Alive)
                    {
                        if (s.Time > 0)
                        {
                            if (s.Player == 1)
                                spriteBatch.DrawString(scorefont, s.PointVal.ToString(), s.Position, new Color(Color.Red, (byte)(s.Time * 2.5)));
                            else
                                spriteBatch.DrawString(scorefont, s.PointVal.ToString(), s.Position, new Color(Color.Green, (byte)(s.Time * 2.5)));
                            s.Time--;
                        }
                        else
                        {
                            s.Alive = false;
                            deadScores.Add(s);
                        }
                    }
                }
                //Remove Scores
                foreach (ScoreDisplay s in deadScores)
                {
                    score.Remove(s);
                }
                #endregion
                //
                spriteBatch.End();

               // Rectangle playerRect = new Rectangle((int)player1.position.X - player1.spriteB.Width / 2, (int)player1.position.Y - player1.spriteB.Height / 2, player1.spriteB.Width, player1.spriteB.Height);
                //Rectangle enemyRect = new Rectangle((int)enemies[0].position.X, (int)enemies[0].position.Y, enemies[0].sprite.Width, enemies[0].sprite.Height);
                //bool wer = MathFns.broadPhaseCollision(playerRect, 0, enemyRect, (float)(enemies[0].rotation+Math.PI/2), spriteBatch, healthBar);
                
                //
                
                

                particleRenderer.RenderEffect(particleEffect);

                base.Draw(gameTime);
            }
        }
    }
}
