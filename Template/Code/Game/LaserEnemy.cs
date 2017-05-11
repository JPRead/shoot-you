using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine7;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Template.Game;

namespace Template
{
    internal class LaserEnemy : Enemy
    {
        private Event tiShootCooldown;
        private Event tiBirth;
        private Event tiDamageCooldown;
        private Vector3 direction;
        private Sprite laserTop;
        private Sprite laserBottom;
        private Sprite laserLeft;
        private Sprite laserRight;
        private int laserAmount;

        /// <summary>
        /// Constructor for LaserEnemy
        /// </summary>
        /// <param name="startPos">Where enemy spawns</param>
        /// <param name="numLasers">How many lasers it fires (1, 2 or 4)</param>
        public LaserEnemy(Vector2 startPos, int numLasers)
        {
            laserAmount = numLasers;

            Random setRotation = new Random();
            RotationAngle = setRotation.Next(0, 360);

            //Set health
            Health = 100;

            //Set collisions
            CollisionActive = true;
            CollisionPrimary = true;

            Friction = 10f;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Rectangle50by50);

            //set position and colour
            Position2D = startPos;
            Wash = Color.OrangeRed;
            Layer++;

            //Timers
            GM.eventM.AddTimer(tiShootCooldown = new Event(5f, "Shoot Cooldown"));
            GM.eventM.AddEventRaiseOnce(tiBirth = new Event(10f, "Birth timer"));
            GM.eventM.AddTimer(tiDamageCooldown = new Event(0.1f, "Damage Cooldown"));

            //Callbacks
            UpdateCallBack += Move;
            FuneralCallBack += KillSprites;
            CollisionCallBack += Collision;

            //Determining initial movement direction
            if (startPos.X <= GM.screenSize.Center.X) direction.X += 1;
            else direction.X -= 1;
            if (startPos.Y <= GM.screenSize.Center.Y) direction.Y += 1;
            else direction.Y -= 1;
            direction.Normalize();

            //Creating lasers for use
            //1 laser
            laserTop = new Sprite();
            GM.engineM.AddSprite(laserTop);
            laserTop.Frame.Define(Tex.Rectangle50by50);
            laserTop.SX = 0.1f;
            laserTop.SY = 1000f;
            laserTop.Visible = false;
            laserTop.Wash = Color.OrangeRed;
            laserTop.CollisionActive = false;
            laserTop.CollisionPrimary = true;
            laserTop.PrologueCallBack += CollisionLaser;
            //laserTop.Moving = true;

            //4 lasers
            if (laserAmount == 4)
            {
                laserLeft = new Sprite();
                GM.engineM.AddSprite(laserLeft);
                laserLeft.Frame.Define(Tex.Rectangle50by50);
                laserLeft.SX = 0.1f;
                laserLeft.SY = 1000f;
                laserLeft.Visible = false;
                laserLeft.Wash = Color.OrangeRed;
                laserLeft.CollisionActive = false;
                laserLeft.CollisionPrimary = true;
                laserLeft.PrologueCallBack += CollisionLaser;
                //laserLeft.Moving = true;
            }

            //Prevent moving off screen
            Moving = true;
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.bounce);
        }

        private void Collision(Sprite hit)
        {
            Velocity = Vector3.Zero;
        }

        private void CollisionLaser(Sprite hit)
        {
            if (hit == GameSetup.PlayerChar)
            {
                if (GM.eventM.Elapsed(tiDamageCooldown))
                {
                    GameSetup.PlayerChar.Health -= 1;
                }
            }
            if(hit is Bullet)
            {
                //1 laser
                laserTop.CollisionAbandonResponse = true;

                //4 lasers
                if (laserAmount == 4)
                {
                    laserLeft.CollisionAbandonResponse = true;
                }
            }

            //1 laser
            laserTop.Velocity = Vector3.Zero;

            //4 lasers
            if (laserAmount == 4)
            {
                laserLeft.Velocity = Vector3.Zero;
            }
        }

        private void Move()
        {
            //Do this for first 0.5 seconds of life
            if(tiBirth.ElapsedSoFar < 0.5)
            {
                RotationHelper.VelocityInThisDirection(this, direction, 500);
            }
            //Then do this
            else
            {
                RotationVelocity = 45f;
            }
            
            //For shooting
            //Every 5 seconds
            if(tiBirth.ElapsedSoFar > 5)
            {
                //1 laser
                laserTop.Position2D = Position2D;
                laserTop.RotationAngle = RotationAngle;
                laserTop.Visible = true;

                //4 lasers
                if (laserAmount == 4)
                {
                    laserLeft.Position2D = Position2D;
                    laserLeft.RotationAngle = RotationAngle + 270;
                    laserLeft.Visible = true;
                }
            }

            //Every 5+1 seconds
            if (tiBirth.ElapsedSoFar > 6)
            {
                //1 laser
                laserTop.Wash = Color.Red;
                laserTop.CollisionActive = true;
                laserTop.SX = 0.2f;

                //4 lasers
                if (laserAmount == 4)
                {
                    laserLeft.Wash = Color.Red;
                    laserLeft.CollisionActive = true;
                    laserTop.SX = 0.2f;
                }
            }

            //Every 10 seconds
            if(tiBirth.ElapsedSoFar >= 10)
            {
                GM.eventM.AddEventRaiseOnce(tiBirth = new Event(10f, "Birth timer"));

                //1 laser
                laserTop.CollisionActive = false;
                laserTop.Wash = Color.OrangeRed;
                laserTop.Visible = false;
                laserTop.SX = 0.1f;

                //4 lasers
                if (laserAmount == 4)
                {
                    laserLeft.CollisionActive = false;
                    laserLeft.Wash = Color.OrangeRed;
                    laserLeft.Visible = false;
                    laserLeft.SX = 0.1f;
                }
            }
        }

        private void KillSprites()
        {
            //1 laser
            laserTop.Kill();

            //4 lasers
            if (laserAmount == 4)
            {
                laserLeft.Kill();
            }
        }
    }
}