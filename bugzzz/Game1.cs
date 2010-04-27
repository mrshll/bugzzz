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

namespace Bugzzz
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //arrays containing bullet entities and enemies
        GameObject[] bullets;
        GameObject[] bullets2;
        GameObject[] turretBullets1;
        GameObject[] turretBullets2;
        GameObject[] enemies;
        ArrayList score;
        Turret turret1; //Player 1 turret
        Turret turret2; //Player 2 turret
        const int maxEnemies = 5;
        const int maxBullets = 15;
        int[] enemies_level;
        int level;
        int enemies_killed;

        // Score display time on screen as it fades out
        const int SCORE_TIME = 80;
        const int fade_length = 150;
        const int fade_increment = 5;
        int current_fade;
        bool fade_in;
        
        Player player1;
        Player player2;
        SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewport;
        Rectangle viewportRect;

        Random rand;
        //for fire delay
        float elapsedTime = 0;
        float elapsedTime2 = 0;
        float t_elapsedTime = 0;
        float t2_elapsedTime = 0;
        float fireDelay = 0.25f;
        Texture2D healthBar;

        //rotation increment
        float angle_rot = .18f;
        float turret_rot = .23f;

        KeyboardState previousKeyboardState = Keyboard.GetState();
        MouseState previousMouseState = Mouse.GetState();


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //player's stating position

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            turret1 = new Turret(Content.Load<Texture2D>("sprites\\cannon"));
            turret2 = new Turret(Content.Load<Texture2D>("sprites\\cannon"));

            healthBar = Content.Load<Texture2D>("sprites\\healthBar");
            current_fade=0;
            fade_in = true;
            rand= new Random();
            enemies_killed = 0;
            enemies_level  = new int[4];
            enemies_level[0] = 25;
            enemies_level[1] = 50;
            enemies_level[2] = 75;
            enemies_level[3] = 100;

            level = 0;
            viewport = GraphicsDevice.Viewport;
            viewportRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            //Initializes all of the bullets, enemies, etc.
            bullets = new GameObject[maxBullets];
            bullets2 = new GameObject[maxBullets];
            for (int i = 0; i < maxBullets; i++)
            {
                bullets2[i] = new GameObject(Content.Load<Texture2D>("sprites\\cannonball"));
                bullets[i] = new GameObject(Content.Load<Texture2D>("sprites\\cannonball"));
            }
            turretBullets1 = new GameObject[maxBullets];
            turretBullets2 = new GameObject[maxBullets];
            for (int i = 0; i < maxBullets; i++)
            {
                turretBullets1[i] = new GameObject(Content.Load<Texture2D>("sprites\\cannonball"));
                turretBullets2[i] = new GameObject(Content.Load<Texture2D>("sprites\\cannonball"));
            }
            enemies = new GameObject[maxEnemies];
            for (int j = 0; j < maxEnemies; j++)
            {
                enemies[j] = new GameObject(Content.Load<Texture2D>("sprites\\enemy"));
            }

            // The maximum amount of scores to display on screen is the maximum number of dead enemies
            score = new ArrayList();

            player1 = new Player(1, Content.Load<Texture2D>("sprites\\cannon"), Content.Load<Texture2D>("sprites\\smiley1"));
            player2 = new Player(2, Content.Load<Texture2D>("sprites\\cannon"), Content.Load<Texture2D>("sprites\\smiley1"));

            // Loading in the font we will use for showing the killed enemies score value
            font = Content.Load<SpriteFont>("ScoreFont");
            
            spriteBatch = new SpriteBatch(GraphicsDevice);

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

        public void fireP1Bullets()
        {
            //firing command
            foreach (GameObject bullet in bullets)
            {
                if (!bullet.alive)
                {
                    bullet.alive = true;
                    bullet.position = player1.p_position - bullet.center;
                    bullet.velocity = new Vector2((float)Math.Cos(player1.p_rotation + Math.PI / 2), (float)Math.Sin(player1.p_rotation + Math.PI / 2)) * 15.0f;
                    return;
                }
            }
        }
        public void fireP2Bullets()
        {
            //firing command
            foreach (GameObject bullet in bullets2)
            {
                if (!bullet.alive)
                {
                    bullet.alive = true;
                    bullet.position = player2.p_position - bullet.center;
                    bullet.velocity = new Vector2((float)Math.Cos(player2.p_rotation + Math.PI / 2), (float)Math.Sin(player2.p_rotation + Math.PI / 2)) * 15.0f;
                    return;
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

   
        public void updateEnemies()
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {
                    //checks for collision with the player
                    Rectangle playerRect = new Rectangle((int)player1.p_position.X - player1.p_spriteB.Width / 2, (int)player1.p_position.Y - player1.p_spriteB.Height/2, player1.p_spriteB.Width, player1.p_spriteB.Height);
                    Rectangle enemyRect = new Rectangle((int)enemy.position.X,(int)enemy.position.Y,enemy.sprite.Width,enemy.sprite.Height);
                    if (playerRect.Intersects(enemyRect))
                    {
                        //p_alive = false;
                        enemy.alive = false;
                        player1.health -= 25;
                        enemies_killed++;
                        
                        break;
                    }
                    playerRect = new Rectangle((int)player2.p_position.X - player2.p_spriteB.Width / 2, (int)player2.p_position.Y - player2.p_spriteB.Height / 2, player2.p_spriteB.Width, player2.p_spriteB.Height);
                    
                    if (playerRect.Intersects(enemyRect))
                    {
                        //p_alive = false;
                        enemy.alive = false;
                        player2.health -= 25;
                        enemies_killed++;
                        
                        break;
                    }
                    if (!viewportRect.Contains(new Point((int)enemy.position.X, (int)enemy.position.Y)))
                    {
                        enemy.alive = false;
                    }
                }
                else
                {
                   // Console.WriteLine("made an enemy");
                    enemy.alive = true;
                    int side = rand.Next(4);
                    if (side == 0)
                    {
                        enemy.position = new Vector2(viewportRect.Left, MathHelper.Lerp(0.0f, (float)viewportRect.Height, (float)rand.NextDouble()));
                    }
                    else if (side == 1)
                    {
                        enemy.position = new Vector2(viewportRect.Top, MathHelper.Lerp(0.0f, (float)viewportRect.Width, (float)rand.NextDouble()));
                    }
                    else if (side == 2)
                    {
                        enemy.position = new Vector2(viewportRect.Right, MathHelper.Lerp(0.0f, (float)viewportRect.Height, (float)rand.NextDouble()));
                    }
                    else
                    {
                        enemy.position = new Vector2(viewportRect.Bottom, MathHelper.Lerp(0.0f, (float)viewportRect.Width, (float)rand.NextDouble()));
                    }
                }
                //makes enemies move towards the player
                Vector2 target;

                if (MathFns.Distance(enemy.position, player1.p_position) > MathFns.Distance(enemy.position, player2.p_position))
                    target = player2.p_position;
                else
                    target = player1.p_position;

                enemy.velocity = target - enemy.position;
                enemy.velocity.Normalize();
                enemy.position += enemy.velocity * 2;
            }
        }

        public void updateBullets()
        {
            #region Player 1 Bullets
            foreach (GameObject bullet in bullets)
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
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        if (bulletRect.Intersects(enemyRect))
                        {
                            bullet.alive = false;
                            enemy.alive = false;
                            score.Add(new Score(20,SCORE_TIME,enemy.position,true,1));
                            player1.score += 20;
                            enemies_killed++;
                            break;
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
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        if (bulletRect.Intersects(enemyRect))
                        {
                            bullet.alive = false;
                            enemy.alive = false;
                            score.Add(new Score(20, SCORE_TIME, enemy.position, true,2));
                            player2.score += 20;
                            break;
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
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        if (bulletRect.Intersects(enemyRect))
                        {
                            bullet.alive = false;
                            enemy.alive = false;
                            score.Add(new Score(10, SCORE_TIME, enemy.position, true,1));
                            player1.score += 10;
                            break;
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
                        Rectangle enemyRect = new Rectangle(
                            (int)enemy.position.X,
                            (int)enemy.position.Y,
                            enemy.sprite.Width,
                            enemy.sprite.Height);

                        if (bulletRect.Intersects(enemyRect))
                        {
                            bullet.alive = false;
                            enemy.alive = false;
                            score.Add(new Score(10, SCORE_TIME, enemy.position, true,2));
                            player2.score += 10;
                            break;
                        }
                    }
                }
            }
            #endregion
        }
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed)
                player1.deploy = true;
            if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed)
                player2.deploy = true;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += elapsed;
            elapsedTime2 += elapsed;
            t_elapsedTime += elapsed;
            t2_elapsedTime += elapsed;
            // TODO: Add your update logic here
            UpdateTurret(turret1,player1);
            UpdateTurret(turret2,player2);
            UpdateInput();
            updateBullets();
            updateEnemies();

            player1.p_position += player1.p_velocity;
            player2.p_position += player2.p_velocity;

            if (player1.p_fire && elapsedTime >= fireDelay)
            {
                elapsedTime = 0.0f;
                fireP1Bullets();
            }
            if (player2.p_fire && elapsedTime2 >= fireDelay)
            {
                elapsedTime2 = 0.0f;
                fireP2Bullets();
            }
            if (turret1.fire && t_elapsedTime >= fireDelay+.5 && turret1.placed)
            {
                t_elapsedTime = 0.0f;
                fireTurretBullets1();
            }
            if (turret2.fire && t2_elapsedTime >= fireDelay + .5 && turret2.placed)
            {
                t2_elapsedTime = 0.0f;
                fireTurretBullets2();
            }
            base.Update(gameTime);
        }


        private void UpdateTurret(Turret turret, Player player)
        {
            if (turret.bulletsLeft > 0)
            {

                if (player.deploy)
                {
                    turret.position = player.p_position;
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
                player1.p_velocity.X = currentState.ThumbSticks.Left.X * 5;
                player1.p_velocity.Y = -currentState.ThumbSticks.Left.Y * 5;
                //player1.p_rotation = -(float)((Math.Tan(currentState.ThumbSticks.Right.Y / currentState.ThumbSticks.Right.X)*2*Math.PI)/180);
                const float DEADZONE = 0.2f;
                const float FIREDEADZONE = 0.3f;

                Vector2 direction = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                float magnitude = direction.Length();
                player1.p_fire = false;
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Length() > DEADZONE)
                {
                    player1.p_rotation_b = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Left.Y, currentState.ThumbSticks.Left.X)));
                }

                if (magnitude > DEADZONE)
                {
                    //Smooth Rotation
                    float angle = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Right.Y, currentState.ThumbSticks.Right.X)));

                    if (angle != player1.p_rotation)
                        player1.p_rotation = MathFns.Clerp(player1.p_rotation, angle, angle_rot);

                    if (magnitude > FIREDEADZONE)
                    {
                        player1.p_fire = true;
                    }
                }
                #endregion

            }
            //keyboard controls
            else
            {
                #region Keyboard Controls Player1
                //player1.p_fire = false;
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                float XDistance = player1.p_position.X - mouse.X;
                float YDistance = player1.p_position.Y - mouse.Y;
                float xdim = 800;
                float ydim = 800;

                float rotation = (float)(Math.Atan2(YDistance, XDistance) + Math.PI / 2);
                player1.p_rotation = rotation;

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    player1.p_fire = true;
                    xdim = mouse.X;
                    ydim = mouse.Y;
                }
                else
                {
                    player1.p_fire = false;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    player1.p_velocity.X = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    player1.p_velocity.X = 5;
                }
                else
                {
                    player1.p_velocity.X = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    player1.p_velocity.Y = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.Down))
                {
                    player1.p_velocity.Y = 5;
                }
                else
                {
                    player1.p_velocity.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Z))
                {
                    player1.deploy = true;
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                    this.Exit();



                
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
                player2.p_velocity.X = currentState.ThumbSticks.Left.X * 5;
                player2.p_velocity.Y = -currentState.ThumbSticks.Left.Y * 5;
                //player2.p_rotation = -(float)((Math.Tan(currentState.ThumbSticks.Right.Y / currentState.ThumbSticks.Right.X)*2*Math.PI)/180);
                const float DEADZONE = 0.2f;
                const float FIREDEADZONE = 0.3f;

                Vector2 direction = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;
                float magnitude = direction.Length();
                player2.p_fire = false;
                if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Length() > DEADZONE)
                {
                    player2.p_rotation_b = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Left.Y, currentState.ThumbSticks.Left.X)));
                }

                if (magnitude > DEADZONE)
                {
                    //Smooth Rotation
                    float angle = (float)(-1 * (3.14 / 2 + Math.Atan2(currentState.ThumbSticks.Right.Y, currentState.ThumbSticks.Right.X)));

                    if (angle != player2.p_rotation)
                        player2.p_rotation = MathFns.Clerp(player2.p_rotation, angle, angle_rot);

                    if (magnitude > FIREDEADZONE)
                    {
                        player2.p_fire = true;
                    }
                }
                #endregion

            }
            //keyboard controls
            else
            {
                #region Keyboard Controls Player 2

                //player2.p_fire = false;
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouse = Mouse.GetState();
                float XDistance = player2.p_position.X - mouse.X;
                float YDistance = player2.p_position.Y - mouse.Y;
                float xdim = 800;
                float ydim = 800;

                float rotation = (float)(Math.Atan2(YDistance, XDistance) + Math.PI / 2);
                player2.p_rotation = rotation;

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    player2.p_fire = true;
                    xdim = mouse.X;
                    ydim = mouse.Y;
                }
                else
                {
                    player2.p_fire = false;
                }
                if (keyboardState.IsKeyDown(Keys.A))
                {
                    player2.p_velocity.X = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    player2.p_velocity.X = 5;
                }
                else
                {
                    player2.p_velocity.X = 0;
                }
                if (keyboardState.IsKeyDown(Keys.W))
                {
                    player2.p_velocity.Y = -5;
                }
                else if (keyboardState.IsKeyDown(Keys.S))
                {
                    player2.p_velocity.Y = 5;
                }
                else
                {
                    player2.p_velocity.Y = 0;
                }
                if (keyboardState.IsKeyDown(Keys.Q))
                {
                    player2.deploy = true;
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                    this.Exit();




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
            if (enemies_level[level] == enemies_killed)
            {
                //Fade to Black
                float elapsed = gameTime.ElapsedGameTime.Milliseconds;
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                if (current_fade <= 255 && fade_in == true)
                {
                    if (elapsed >= (float)fade_length / 24)
                    {
                        spriteBatch.Draw(healthBar, new Rectangle(0, 0, this.viewport.Width, this.viewport.Height), new Color(Color.Black, (byte)(255 - current_fade)));
                        current_fade += fade_increment;
                    }
                    //Fade out
                }
                else if (fade_in = false)
                {
                    if (elapsed >= (float)fade_length / 24)
                    {
                        spriteBatch.Draw(healthBar, new Rectangle(0, 0, this.viewport.Width, this.viewport.Height), new Color(Color.Black, (byte)(255 - current_fade)));
                        current_fade -= fade_increment;
                    }
                    if (current_fade == 255)
                    {
                        level++; //TODO: Will crash after 5 levels. 
                        enemies_killed = 0;
                    }
                }
                else
                {
                    Console.Out.WriteLine("Fade in/out error");
                }


            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(healthBar, new Rectangle(this.viewport.Width / 15, this.viewport.Height / 15, (int)this.viewport.Width * player1.health / 600, this.viewport.Height / 30), Color.Red);
                spriteBatch.Draw(healthBar, new Rectangle(this.viewport.Width * 12 / 16, this.viewport.Height / 15, (int)this.viewport.Width * player2.health / 600, this.viewport.Height / 30), Color.Red);
                spriteBatch.DrawString(font, "Player 1 Score: " + player1.score.ToString(), new Vector2(this.viewport.Width / 15, this.viewport.Height / 60), new Color(Color.White, (byte)130));
                spriteBatch.DrawString(font, "Player 2 Score: " + player2.score.ToString(), new Vector2(this.viewport.Width * 12 / 16, this.viewport.Height / 60), new Color(Color.White, (byte)130));
                spriteBatch.Draw(player1.p_spriteB, new Rectangle((int)player1.p_position.X, (int)player1.p_position.Y, player1.p_spriteB.Width, player1.p_spriteB.Height), null, Color.White, player1.p_rotation_b, new Vector2(player1.p_spriteB.Width / 2, player1.p_spriteB.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player1.p_spriteT, new Rectangle((int)player1.p_position.X, (int)player1.p_position.Y, player1.p_spriteT.Width, player1.p_spriteT.Height), null, Color.White, (float)(player1.p_rotation + .5 * Math.PI), new Vector2(player1.p_spriteT.Width / 2, player1.p_spriteT.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player2.p_spriteB, new Rectangle((int)player2.p_position.X, (int)player2.p_position.Y, player2.p_spriteB.Width, player2.p_spriteB.Height), null, Color.Red, player2.p_rotation_b, new Vector2(player2.p_spriteB.Width / 2, player2.p_spriteB.Height / 2), SpriteEffects.None, 0);
                spriteBatch.Draw(player2.p_spriteT, new Rectangle((int)player2.p_position.X, (int)player2.p_position.Y, player2.p_spriteT.Width, player2.p_spriteT.Height), null, Color.Red, (float)(player2.p_rotation + .5 * Math.PI), new Vector2(player2.p_spriteT.Width / 2, player2.p_spriteT.Height / 2), SpriteEffects.None, 0);

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
                        spriteBatch.Draw(enemy.sprite, enemy.position, Color.White);
                    }
                }
                ArrayList deadScores = new ArrayList(); //Used for determing what scores need to be deleted. 
                //Output Scores
                foreach (Score s in score)
                {
                    if (s.alive)
                    {
                        if (s.time > 0)
                        {
                            if (s.player == 1)
                                spriteBatch.DrawString(font, s.pointVal.ToString(), s.position, new Color(Color.Red, (byte)(s.time * 2.5)));
                            else
                                spriteBatch.DrawString(font, s.pointVal.ToString(), s.position, new Color(Color.Green, (byte)(s.time * 2.5)));
                            s.time--;
                        }
                        else
                        {
                            s.alive = false;
                            deadScores.Add(s);
                        }
                    }
                }
                //Remove Scores
                foreach (Score s in deadScores)
                {
                    score.Remove(s);
                }
                #endregion

                spriteBatch.End();
                base.Draw(gameTime);
            }
        }
    }
}