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

            //Timers
            GM.eventM.AddTimer(tiShootCooldown = new Event(5f, "Shoot Cooldown"));
            GM.eventM.AddEventRaiseOnce(tiBirth = new Event(10f, "Birth timer"));

            //Callbacks
            UpdateCallBack += Move;
            FuneralCallBack += KillSprites;

            //Determining initial movement direction
            if (startPos.X <= GM.screenSize.Center.X) direction.X += 1;
            else direction.X -= 1;
            if (startPos.Y <= GM.screenSize.Center.Y) direction.Y += 1;
            else direction.Y -= 1;
            direction.Normalize();

            //Creating lasers for use
            if (laserAmount >= 1)
            {
                laserTop = new Sprite();
                GM.engineM.AddSprite(laserTop);
                laserTop.Frame.Define(Tex.Rectangle50by50);
                laserTop.SX = 0.1f;
                laserTop.SY = 1000f;
                laserTop.Visible = false;
            }
            if (laserAmount >= 2)
            {
                laserBottom = new Sprite();
                GM.engineM.AddSprite(laserBottom);
                laserBottom.Frame.Define(Tex.Rectangle50by50);
                laserBottom.SX = 0.1f;
                laserBottom.SY = 1000f;
                laserBottom.Visible = false;
            }
            if (laserAmount == 4)
            {
                laserRight = new Sprite();
                GM.engineM.AddSprite(laserRight);
                laserRight.Frame.Define(Tex.Rectangle50by50);
                laserRight.SX = 0.1f;
                laserRight.SY = 1000f;
                laserRight.Visible = false;

                laserLeft = new Sprite();
                GM.engineM.AddSprite(laserLeft);
                laserLeft.Frame.Define(Tex.Rectangle50by50);
                laserLeft.SX = 0.1f;
                laserLeft.SY = 1000f;
                laserLeft.Visible = false;
            }

            //Prevent moving off screen
            Moving = true;
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.bounce);
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
                RotationVelocity = 22.5f;
            }
            
            //For shooting
            if(tiBirth.ElapsedSoFar > 5)
            {
                if(laserAmount >= 1)
                {
                    laserTop.Position2D = Position2D;
                    laserTop.RotationAngle = RotationAngle;
                    laserTop.Visible = true;
                }
                if (laserAmount >= 2)
                {
                    laserBottom.Position2D = Position2D;
                    laserBottom.RotationAngle = RotationAngle + 180;
                    laserBottom.Visible = true;
                }
                if (laserAmount == 4)
                {
                    laserRight.Position2D = Position2D;
                    laserRight.RotationAngle = RotationAngle + 90;
                    laserRight.Visible = true;

                    laserLeft.Position2D = Position2D;
                    laserLeft.RotationAngle = RotationAngle + 270;
                    laserLeft.Visible = true;

                }
            }
        }

        private void KillSprites()
        {

        }
    }
}