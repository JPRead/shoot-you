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
    internal class TurretEnemy : Enemy
    {
        private Event tiShootCooldown;
        private Event tiBirth;
        private Vector3 direction;
        private bool fourBeams;

        /// <summary>
        /// Constructor for LaserEnemy
        /// </summary>
        /// <param name="startPos">Where enemy spawns</param>
        /// <param name="doubleTurret">False for 2 beams, true for 4</param>
        public TurretEnemy(Vector2 startPos, bool doubleTurret)
        {
            fourBeams = doubleTurret;

            Random setRotation = new Random();
            RotationAngle = setRotation.Next(0, 360);

            //Set properties
            KillPoints = 5;
            Health = 100;

            //Set collisions
            CollisionActive = true;
            CollisionPrimary = true;

            Friction = 10f;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Rectangle50by50);
            ScaleBoth = 0.75f;

            //set position and colour
            Position2D = startPos;
            Wash = Color.OrangeRed;
            Layer++;

            //Timers
            GM.eventM.AddTimer(tiShootCooldown = new Event(0.05f, "Shoot Cooldown"));
            GM.eventM.AddEventRaiseOnce(tiBirth = new Event(10f, "Birth timer"));

            //Determining initial movement direction
            if (startPos.X <= GM.screenSize.Center.X) direction.X += 1;
            else direction.X -= 1;
            if (startPos.Y <= GM.screenSize.Center.Y) direction.Y += 1;
            else direction.Y -= 1;
            direction.Normalize();

            //Prevent moving off screen
            Moving = true;
            LimitInitialise();
            Limit.ViewPortAction(LimitAction.bounce);

            //Callbacks
            UpdateCallBack += Move;
            PrologueCallBack += Collision;
            EpilogueCallBack += AfterCollision;
        }

        private void AfterCollision(Sprite hit)
        {
            Velocity = Vector3.Zero;
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
                RotationHelper.VelocityInThisDirection(this, direction, 500);
            }
            //Then do this
            else
            {
                if (fourBeams) RotationVelocity = 22.5f;
                else RotationVelocity = 45f;
            }
            
            //For shooting
            //Every 5 seconds
            if(tiBirth.ElapsedSoFar > 2.5)
            {
                if (GM.eventM.Elapsed(tiShootCooldown))
                {
                    //1 laser
                    Vector3 front3d = Position + RotationHelper.MyDirection(this, 0);
                    Vector3 bottom3d = Position + RotationHelper.MyDirection(this, 180);
                    Vector2 front = new Vector2(front3d.X, front3d.Y);
                    Vector2 bottom = new Vector2(bottom3d.X, bottom3d.Y);


                    new Bullet(this, front, 1500f, 2);
                    new Bullet(this, bottom, 1500f, 2);
                    //4 lasers
                    if (fourBeams)
                    {
                        Vector3 left3d = Position + RotationHelper.MyDirection(this, 90);
                        Vector3 right3d = Position + RotationHelper.MyDirection(this, 270);
                        Vector2 left = new Vector2(left3d.X, left3d.Y);
                        Vector2 right = new Vector2(right3d.X, right3d.Y);

                        new Bullet(this, left, 1000f, 2);
                        new Bullet(this, right, 1000f, 2);
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
            }
        }
    }
}