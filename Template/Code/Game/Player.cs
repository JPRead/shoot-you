using System;
using Engine7;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Template.Game
{
    internal class Player : Sprite
    {
        //keys for controlling player
        private Keys Right;
        private Keys Left;
        private Keys Shoot;
        private Keys Forward;
        private Keys Backward;
        private Keys Boost;
        private Event tiBoostDelay;
        private float boostX;
        private float boostY;
        private Event tiShootCooldown;
        private int health;
        private Sprite gunSprite;
        private Sprite directionSprite;

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

        public Player(Vector2 startPos, Color col)
        {
            //Creating gun sprite
            gunSprite = new Sprite();
            GM.engineM.AddSprite(gunSprite);
            gunSprite.Frame.Define(Tex.Rectangle50by50);
            gunSprite.Wash = col;
            gunSprite.SX = 0.1f;
            gunSprite.SY = 0.6f;

            //Creating direction sprite
            directionSprite = new Sprite();
            GM.engineM.AddSprite(directionSprite);
            directionSprite.Frame.Define(Tex.Triangle);
            directionSprite.Wash = col;
            directionSprite.SX = 0.5f;
            directionSprite.SY = 0.5f;

            //Set health
            Health = 100;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Circle32by32);

            //set position and colour
            Position2D = startPos;
            Wash = col;
            //SY = 1.25f;

            //setup controls
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
            GM.eventM.AddTimer(tiShootCooldown = new Event(0.1f, "Shoot Cooldown"));
        }

        //Stopping collisions with own bullets
        private void Hit(Sprite hit)
        {
            if (hit is Bullet)
            {
                Bullet bullet = (Bullet)hit;
                if(bullet.Player == this)CollisionAbandonResponse = true;
                //CollisionAbandonResponse = true;
            }
        }

        private void Display()
        {
            if (tiBoostDelay != null && tiBoostDelay.ElapsedSoFar < 0.5f)
                GM.textM.Draw(FontBank.arcadePixel, "DODGE~COOLDOWN~" + Math.Round(0.5f - tiBoostDelay.ElapsedSoFar, 1), GM.screenSize.Left + 40, GM.screenSize.Bottom - 40, TextAtt.BottomLeft);
        }

        /// <summary>
        /// Unneeded at the moment
        /// </summary>
        /// <param name="hit"></param>
        //private void Stop(Sprite hit)
        //{
        //    //stop if hit wall
        //    //if (hit is wall)
        //    //    Velocity = Vector3.Zero;
        //}

        /// <summary>
        /// make collision active
        /// </summary>
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
        /// <param name="boost"></param>
        public void SetKeys(Keys left, Keys right, Keys forward, Keys backward, Keys shoot, Keys boost)
        {
            Right = right;
            Left = left;
            Forward = forward;
            Backward = backward;
            Shoot = shoot;
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

            Vector3 d = new Vector3(0, 0, 0);

            if (tiBoostDelay == null || tiBoostDelay.ElapsedSoFar > 0.25f)
            {
                if (GM.inputM.KeyDown(Left))
                {
                    d += Vector3.Left;
                }
                if (GM.inputM.KeyDown(Right))
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

            //Create a normalized vector from d
            Vector3 dNorm = d;
            dNorm.Normalize();
            RotationHelper.FaceDirection(directionSprite, dNorm, DirectionAccuracy.free, 0);
            directionSprite.Position = Position + (dNorm * 20);


            //For firing
            if ((GM.inputM.KeyDown(Shoot) || GM.inputM.MouseLeftButtonHeld()) && GM.eventM.Elapsed(tiShootCooldown))
            {
                //create bullet and pass reference to player and angle
                new Bullet(this, aimAngle, 1500f);
            }

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
            }

            if(tiBoostDelay != null && tiBoostDelay.ElapsedSoFar > 0.25f)
            {
                Friction = 10f;
            }
        }

    }
}