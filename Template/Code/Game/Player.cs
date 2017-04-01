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
            //Set health
            Health = 100;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Triangle);

            //set position and colour
            Position2D = startPos;
            Wash = col;
            SY = 1.25f;

            //setup controls
            UpdateCallBack += Move;
            UpdateCallBack += Display;

            //set wrapping
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.wrap);

            //flash then turn on collisions (invincibility)
            TimerInitialise();
            Timer.FlashStopAfter(2f, 0.1f, 0.05f);
            Timer.TimingDoneCallBack += MakeVunerable;

            //Add friction
            Friction = 10f;

            //set check for wall collision
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            EpilogueCallBack += Stop;

            //Start a shooting timer
            GM.eventM.AddTimer(tiShootCooldown = new Event(0.1f, "Shoot Cooldown"));

            
        }

        //Stopping collisions with bullets
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
                GM.textM.Draw(FontBank.arcadePixel, "DODGE~COOLDOWN~" + Math.Round(0.5f - tiBoostDelay.ElapsedSoFar, 1), boostX, boostY, TextAtt.TopLeft);
        }

        private void Stop(Sprite hit)
        {
            //stop if hit wall
            if (hit is wall)
                Velocity = Vector3.Zero;
        }

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
            Vector2 currentPosition = Position2D;

            if (GM.inputM.KeyDown(Left))
            {
                Vector3 d = RotationHelper.MyDirection(this, 270);
                Velocity += d * 20;// * GM.eventM.Delta;
            }
            if (GM.inputM.KeyDown(Right))
            {
                Vector3 d = RotationHelper.MyDirection(this, 90);
                Velocity += d * 20;// * GM.eventM.Delta;
            }
            if (GM.inputM.KeyDown(Forward))
            {
                Vector3 d = RotationHelper.MyDirection(this, 0);
                Velocity += d * 20;// * GM.eventM.Delta;
            }
            if (GM.inputM.KeyDown(Backward))
            {
                Vector3 d = RotationHelper.MyDirection(this, 180);
                Velocity += d * 20;// * GM.eventM.Delta;
            }

            //For firing:
            
            if ((GM.inputM.KeyDown(Shoot) || GM.inputM.MouseLeftButtonHeld()) && GM.eventM.Elapsed(tiShootCooldown))
            {
                Vector2 fireAngle = GM.inputM.MouseLocation;
                //create bullet and pass reference to player and angle
                new Bullet(this, fireAngle);
            }

            if (GM.inputM.KeyPressed(Boost) && (tiBoostDelay == null || GM.eventM.Elapsed(tiBoostDelay)))
            {
                if (tiBoostDelay == null)
                    GM.eventM.AddTimer(tiBoostDelay = new Event(0.5f, "dodge delay"));

                //Change this- currently adds more velocity if moving at angle.
                Vector3 b = Velocity;
                Velocity += b * 2;
            }
        }
    }
}