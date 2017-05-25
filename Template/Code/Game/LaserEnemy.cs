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
        private Sprite laserLeft;
        private bool fourLasers;

        /// <summary>
        /// Constructor for LaserEnemy
        /// </summary>
        /// <param name="startPos">Where enemy spawns</param>
        /// <param name="doubleLasers">False for 2 lasers, true for 4</param>
        public LaserEnemy(Vector2 startPos, bool doubleLasers)
        {
            fourLasers = doubleLasers;

            Random setRotation = new Random();
            RotationAngle = setRotation.Next(0, 360);

            //Set properties
            KillPoints = 10;
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
            GM.eventM.AddTimer(tiDamageCooldown = new Event(0.05f, "Damage Cooldown"));

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
            laserTop.SY = 100f;
            laserTop.Visible = false;
            laserTop.Wash = Color.OrangeRed;
            laserTop.Shape = Shape.rectangle;
            laserTop.Static = false;
            laserTop.PrologueCallBack += CollisionLaser;

            //4 lasers
            if (fourLasers)
            {
                laserLeft = new Sprite();
                GM.engineM.AddSprite(laserLeft);
                laserLeft.Frame.Define(Tex.Rectangle50by50);
                laserLeft.SX = 0.1f;
                laserLeft.SY = 100f;
                laserLeft.Visible = false;
                laserLeft.Wash = Color.OrangeRed;
                laserLeft.Shape = Shape.rectangle;
                laserLeft.Static = false;
                laserLeft.PrologueCallBack += CollisionLaser;
            }

            //Prevent moving off screen
            Moving = true;
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.bounce);

            //Callbacks
            UpdateCallBack += Move;
            FuneralCallBack += KillSprites;
            PrologueCallBack += Collision;
        }

        private void CollisionLaser(Sprite hit)
        {
            //laserTop.Velocity = Vector3.Zero;
            //laserTop.Position = Position;

            //if (fourLasers)
            //{
            //    laserLeft.Velocity = Vector3.Zero;
            //    laserLeft.Position = Position;
            //}
        }

        private void Collision(Sprite hit)
        {
            Velocity = Vector3.Zero;
        }

        private void Move()
        {
            //Do this for first 0.5 seconds of life
            if (tiBirth.ElapsedSoFar < 0.5)
            {
                RotationHelper.FaceDirection(this, direction, DirectionAccuracy.free, 10);
                RotationHelper.VelocityInCurrentDirection(this, 500, 0);
            }
            //Then do this
            else
            {
                if (tiBirth.ElapsedSoFar < 10)
                {
                    if (fourLasers) RotationVelocity = 22.5f;
                    else RotationVelocity = 45f;
                }
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
                if (fourLasers)
                {
                    laserLeft.Position2D = Position2D;
                    laserLeft.RotationAngle = RotationAngle + 90;
                    laserLeft.Visible = true;
                }
            }

            //Every 5+1 seconds
            if (tiBirth.ElapsedSoFar > 6)
            {
                //1 laser
                laserTop.Wash = Color.Red;
                laserTop.SX = 0.2f;

                //Checking for collisions
                Ray rayTop = new Ray(Position, RotationHelper.MyDirection(this, 0));
                Ray rayBottom = new Ray(Position, RotationHelper.MyDirection(this, 180));
                
                BoundingSphere playerSphere = new BoundingSphere(GameSetup.PlayerChar.Position, (GameSetup.PlayerChar.Right - GameSetup.PlayerChar.Left));

                if ((rayTop.Intersects(playerSphere) != null || rayBottom.Intersects(playerSphere) != null) && GM.eventM.Elapsed(tiDamageCooldown))
                {
                    GameSetup.PlayerChar.Health -= 2;
                    if (GameSetup.PlayerChar.Health <= 0) GameSetup.PlayerChar.Kill();
                }
                //4 lasers
                if (fourLasers)
                {
                    laserLeft.Wash = Color.Red;
                    laserLeft.SX = 0.2f;

                    Ray rayLeft = new Ray(Position, RotationHelper.MyDirection(this, 90));
                    Ray rayRight = new Ray(Position, RotationHelper.MyDirection(this, 270));

                    if ((rayLeft.Intersects(playerSphere) != null || rayRight.Intersects(playerSphere) != null) && GM.eventM.Elapsed(tiDamageCooldown))
                    {
                        GameSetup.PlayerChar.Health -= 2;
                        if (GameSetup.PlayerChar.Health <= 0) GameSetup.PlayerChar.Kill();
                    }
                }
            }

            //Every 10 seconds
            if(tiBirth.ElapsedSoFar >= 10)
            {
                GM.eventM.AddEventRaiseOnce(tiBirth = new Event(10f, "Birth timer"));

                direction = Vector3.Zero;

                if (Position.X <= GM.screenSize.Center.X) direction.X += 1;
                else direction.X -= 1;
                if (Position.Y <= GM.screenSize.Center.Y) direction.Y += 1;
                else direction.Y -= 1;
                direction.Normalize();

                //1 laser
                laserTop.Wash = Color.OrangeRed;
                laserTop.Visible = false;
                laserTop.SX = 0.1f;

                //4 lasers
                if (fourLasers)
                {
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
            if (fourLasers)
            {
                laserLeft.Kill();
            }
        }
    }
}