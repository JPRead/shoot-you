using System;
using Engine7;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Collections.Generic;

namespace Template.Game
{
    internal class Player : Sprite
    {
        //keys for controlling player
        private Keys MoveRight;
        private Keys MoveLeft;
        private Keys Fire;
        private Keys SpecialFire;
        private Keys Forward;
        private Keys Backward;
        private Keys Boost;
        private Event tiBoostDelay;
        private float boostX;
        private float boostY;
        private Event tiFireCooldown;
        private Event tiSpecialFireCooldown;
        private Event tiAltFireCooldown;
        private Event tiLockingDelay;
        private int health;
        private Sprite gunSprite;
        private Sprite directionSprite;
        private Sprite lockSprite;
        private Sprite attack;
        private int playerScore;
        private bool invulnerable;
        private bool debugMode;
        private float lockAmount;

        
        public int Health
        {
            get
            {
                return health;
            }

            set
            {
                health = value;
            }
        }

        public int PlayerScore
        {
            get
            {
                return playerScore;
            }

            set
            {
                playerScore = value;
            }
        }

        public bool Invulnerable
        {
            get
            {
                return invulnerable;
            }

            set
            {
                invulnerable = value;
            }
        }

        public bool DebugMode
        {
            get
            {
                return debugMode;
            }

            set
            {
                debugMode = value;
            }
        }

        public Player(Vector2 startPos, Color col)
        {
            debugMode = false;

            //Creating gun sprite
            gunSprite = new Sprite();
            GM.engineM.AddSprite(gunSprite);
            gunSprite.Frame.Define(Tex.Rectangle50by50);
            gunSprite.Wash = col;
            gunSprite.SX = 0.1f;
            gunSprite.SY = 0.6f;

            // Creating lock on sprite
            lockSprite = new Sprite();
            GM.engineM.AddSprite(lockSprite);
            lockSprite.Visible = false;
            lockSprite.Frame.Define(GM.txSprite, new Rectangle(71, 160, 36, 36));
            Wash = Color.LimeGreen;
            Layer = RenderLayer.hud;

            //Creating direction sprite
            directionSprite = new Sprite();
            GM.engineM.AddSprite(directionSprite);
            directionSprite.Frame.Define(Tex.Triangle);
            directionSprite.Wash = col;
            directionSprite.SX = 0.5f;
            directionSprite.SY = 0.5f;

            //Set health
            Health = 100;
            invulnerable = false;

            //Set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Circle32by32);

            //Set position and colour
            Position2D = startPos;
            Wash = col;

            //Setup controls
            UpdateCallBack += Move;
            UpdateCallBack += Display;

            //Prevent moving off screen
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.bounce);

            //flash then turn on collisions (invincibility)
            TimerInitialise();
            gunSprite.TimerInitialise();
            directionSprite.TimerInitialise();
            gunSprite.Timer.FlashStopAfter(2f, 0.1f, 0.05f);
            directionSprite.Timer.FlashStopAfter(2f, 0.1f, 0.05f);
            Timer.FlashStopAfter(2f, 0.1f, 0.05f);
            Timer.TimingDoneCallBack += MakeVunerable;

            //Add friction
            Friction = 10f;

            //Setting up collisions
            Moving = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            //EpilogueCallBack += Stop;

            //Timers
            GM.eventM.AddTimer(tiFireCooldown = new Event(0.1f, "Fire Cooldown"));
            GM.eventM.AddTimer(tiLockingDelay = new Event(0.01f, "Locking Cooldown"));
            
            //GM.eventM.AddTimer(tiAltFireCooldown = new Event(10f, "Alt Fire Cooldown"));


            //Funeral
            FuneralCallBack += Funeral;
        }

        //Update score on death
        private void Funeral()
        {
            gunSprite.Kill();
            directionSprite.Kill();
            //if(playerScore > GM.BestScore)
            //{
            //    GM.BestScore = playerScore;
            //}
        }

        //Stopping collisions with own bullets
        private void Hit(Sprite hit)
        {
            if (hit is Bullet)
            {
                Bullet bullet = (Bullet)hit;
                if (bullet.Player == this) CollisionAbandonResponse = true;
            }

            if (hit is Missile)
            {
                Missile missile = (Missile)hit;
                if (missile.Owner == this) CollisionAbandonResponse = true;
            }
        }

        //Displaying dodge cooldown
        private void Display()
        {
            if (tiBoostDelay != null && tiBoostDelay.ElapsedSoFar < 0.5f)
                GM.textM.Draw(FontBank.arcadePixel, "DODGE~COOLDOWN~" + Math.Round(0.5f - tiBoostDelay.ElapsedSoFar, 1), GM.screenSize.Left + 40, GM.screenSize.Bottom - 40, TextAtt.BottomLeft);
            if (tiSpecialFireCooldown != null && tiSpecialFireCooldown.ElapsedSoFar < 10)
                GM.textM.Draw(FontBank.arcadePixel, "SPECIAL FIRE~COOLDOWN~" + (10 - (int)(tiSpecialFireCooldown.ElapsedSoFar + 0.5)), GM.screenSize.Left + 40, GM.screenSize.Bottom - 120, TextAtt.BottomLeft);
            if (tiAltFireCooldown != null && tiAltFireCooldown.ElapsedSoFar < 1)
                GM.textM.Draw(FontBank.arcadePixel, "ALTERNATE FIRE~COOLDOWN~" + Math.Round(1 - tiAltFireCooldown.ElapsedSoFar, 1), GM.screenSize.Left + 40, GM.screenSize.Bottom - 80, TextAtt.BottomLeft);
        }

        //Activate collisions
        private void MakeVunerable()
        {
            CollisionActive = true;
        }

        /// <summary>
        /// apply key set
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="forward"></param>
        /// <param name="backward"></param>
        /// <param name="shoot"></param>
        /// <param name="specialfire"></param>
        /// <param name="boost"></param>
        public void SetKeys(Keys left, Keys right, Keys forward, Keys backward, Keys shoot, Keys specialfire, Keys boost)
        {
            MoveRight = right;
            MoveLeft = left;
            Forward = forward;
            Backward = backward;
            Fire = shoot;
            SpecialFire = specialfire;
            Boost = boost;
        }

        public void SetBoot(float x, float y)
        {
            boostX = x;
            boostY = y;
        }

        /// <summary>
        /// act on key sets to move player and shoot
        /// </summary>
        private void Move()
        {
            //For gunSprite
            Vector2 aimAngle = GM.inputM.MouseLocation;
            Vector2 direction = aimAngle - Position2D;
            direction = Vector2.Normalize(direction);
            RotationHelper.FaceDirection(gunSprite, direction, DirectionAccuracy.free, 0);
            gunSprite.Position2D = Position2D + (direction * 15);
            Vector2 currentPosition = Position2D;

            //For console
            if (GM.inputM.KeyPressed(Keys.C))
            {
                debugMode = true;
            }
            if (debugMode == true)
            {
                GM.textM.Draw(FontBank.arcadePixel, "Debug", GM.screenSize.Left + 175, GM.screenSize.Top + 40, TextAtt.TopLeft);

                if (GM.inputM.KeyPressed(Keys.F5))
                {
                    LaserEnemy laserEnemy = new LaserEnemy(GM.inputM.MouseLocation, false);
                }
                if (GM.inputM.KeyPressed(Keys.F12))
                {
                    GameSetup.EnemySpawnSystem.Kill();
                }
            }

            //For dodging
            Vector3 d = new Vector3(0, 0, 0);
            if (tiBoostDelay == null || tiBoostDelay.ElapsedSoFar > 0.25f)
            {
                if (GM.inputM.KeyDown(MoveLeft))
                {
                    d += Vector3.Left;
                }
                if (GM.inputM.KeyDown(MoveRight))
                {
                    d += Vector3.Right;
                }
                if (GM.inputM.KeyDown(Forward))
                {
                    d += Vector3.Down;
                }
                if (GM.inputM.KeyDown(Backward))
                {
                    d += Vector3.Up;
                }
                if (d == new Vector3(0, 0, 0)) RotationHelper.FaceDirection(this, Vector3.Down, DirectionAccuracy.free, 0);
                else
                {
                    RotationHelper.FaceDirection(this, d, DirectionAccuracy.free, 0);
                    RotationHelper.VelocityInCurrentDirection(this, 500, 0);
                }
            }

            //For directionSprite
            Vector3 dNorm = d;
            dNorm.Normalize();
            RotationHelper.FaceDirection(directionSprite, dNorm, DirectionAccuracy.free, 0);
            directionSprite.Position = Position + (dNorm * 20);

            //For firing
            if ((GM.inputM.KeyDown(Fire) || GM.inputM.MouseLeftButtonHeld()) && GM.eventM.Elapsed(tiFireCooldown))
            {
                //create bullet and pass reference to player and angle
                new Bullet(this, aimAngle, 1500f, 10);
            }

            if (GM.inputM.KeyDown(SpecialFire) && (tiSpecialFireCooldown == null || GM.eventM.Elapsed(tiSpecialFireCooldown)))
            {
                for (int i = 0; i < 360; i += 5)
                {
                    Vector3 v3ShootAngle = RotationHelper.MyDirection(this, i);
                    Vector2 v2ShootAngle = new Vector2(v3ShootAngle.X, v3ShootAngle.Y);

                    RotationHelper.Direction2DFromAngle(i, 0);

                    //create bullet and pass reference to player and angle
                    new Bullet(this, Position2D + v2ShootAngle, 750, 50);
                }

                for (int i = 0; i < 360; i += 10)
                {
                    Vector3 v3ShootAngle = RotationHelper.MyDirection(this, i);
                    Vector2 v2ShootAngle = new Vector2(v3ShootAngle.X, v3ShootAngle.Y);

                    RotationHelper.Direction2DFromAngle(i, 0);

                    //create bullet and pass reference to player and angle
                    new Bullet(this, Position2D + v2ShootAngle, 500, 50);
                }

                if (tiSpecialFireCooldown == null)
                    GM.eventM.AddTimer(tiSpecialFireCooldown = new Event(10f, "Special Fire Cooldown"));
            }

            ////Missiles
            //if (GM.inputM.MouseRightButtonHeld() && (tiAltFireCooldown == null || GM.eventM.Elapsed(tiAltFireCooldown)))
            //{
            //    Vector2 dir = new Vector2(RotationHelper.MyDirection(this, 0).X, RotationHelper.MyDirection(this, 0).Y);
            //    //Find target based on cursor placement
            //    new Missile(Position2D + (24 * dir), dir, this, this, 750, 200, 20, 50);

            //    if(tiAltFireCooldown == null)
            //        GM.eventM.AddTimer(tiAltFireCooldown = new Event(1, "Alternate Fire Cooldown"));
            //}

            //For boosting
            if (GM.inputM.KeyPressed(Boost) && (tiBoostDelay == null || GM.eventM.Elapsed(tiBoostDelay)) && d != Vector3.Zero)
            {
                if (tiBoostDelay == null)
                    GM.eventM.AddTimer(tiBoostDelay = new Event(0.5f, "dodge delay"));

                //Raycasting to check for collisions and offset to avoid collision
                //Position += d * 100;
                //Ray findHits = new Ray(Position, d);

                Friction = 0f;
                Velocity += dNorm * 800;
                invulnerable = true;
                Wash = Color.LightGreen;
            }

            //Resetting dodge delay for displaying
            if (tiBoostDelay != null && tiBoostDelay.ElapsedSoFar > 0.25f)
            {
                Friction = 10f;
                invulnerable = false;
                Wash = Color.LimeGreen;
            }

            //Locking sequence
            if (GM.inputM.MouseRightButtonPressed())
            {
                lockSprite.Visible = true;

                attack = null;
                float bestDistance = int.MaxValue;
                
                foreach (Sprite s in GM.engineM.SpriteList)
                {
                    if (s is Enemy)
                    {
                        if(attack == null || Vector2.DistanceSquared(Centre2D, s.Centre2D) < bestDistance)
                        {
                            attack = s;
                            bestDistance = Vector2.DistanceSquared(Centre2D, s.Centre2D);
                        }
                    }
                }
            }

            if (GM.inputM.MouseRightButtonHeld() && GM.eventM.Elapsed(tiLockingDelay) && attack != null)
            {
                Color color = new Color();
                if (lockAmount < 1)
                {
                    lockAmount += tiLockingDelay.Interval;
                    lockSprite.RotationAngle = 360 * lockAmount;
                    //lockSprite.Wash = new Color(255, 255, 255);
                    
                }
                else
                {
                    lockSprite.RotationAngle = 0;
                    //lockSprite.Wash = new Color(255, 0, 0);
                }

                color.R = (byte)(255 * lockAmount);
                color.B = (byte)(255 - color.B);

                lockSprite.Position2D = attack.Position2D;
                lockSprite.ScaleBoth = 1.5f + (1 - lockAmount);
                lockSprite.Wash = color;
            }

            if (GM.inputM.MouseRightButtonReleased())
            {
                if (lockAmount >= 1 && (tiAltFireCooldown == null || GM.eventM.Elapsed(tiAltFireCooldown)))
                {
                    FireMissile(attack);
                }
                lockSprite.Visible = false;
                lockAmount = 0;
            }
        }

        private void FireMissile(Sprite lockedSprite)
        {
            Sprite target = lockedSprite;

            Vector2 dir = new Vector2(RotationHelper.MyDirection(this, 0).X, RotationHelper.MyDirection(this, 0).Y);
            new Missile(Position2D + (24 * dir), dir, this, target, 750, 500, 20, 50);

            if (tiAltFireCooldown == null)
                GM.eventM.AddTimer(tiAltFireCooldown = new Event(1, "Alternate Fire Cooldown"));
        }
    }
}