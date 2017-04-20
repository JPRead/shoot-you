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
    internal class StraferEnemy : Enemy
    {
        private Sprite gunSprite;
        private Event tiShootCooldown;
        private Event tiSwitchDirection;

        public StraferEnemy(Vector2 startPos)
        {
            //Set health
            Health = 30;

            //Set collisions
            CollisionActive = true;
            CollisionPrimary = true;

            Friction = 10f;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Circle32by32);

            //set position and colour
            Position2D = startPos;
            Wash = Color.OrangeRed;

            gunSprite = new Sprite();
            GM.engineM.AddSprite(gunSprite);
            gunSprite.Frame.Define(Tex.Rectangle50by50);
            gunSprite.Wash = Wash;
            gunSprite.SX = 0.1f;
            gunSprite.SY = 0.6f;

            UpdateCallBack += Move;
            PrologueCallBack += Hit;
            FuneralCallBack += KillSprites;

            GM.eventM.AddTimer(tiSwitchDirection = new Event(4f, "Switch Direction"));
            GM.eventM.AddTimer(tiShootCooldown = new Event(1f, "Shoot Cooldown"));

            Moving = true;
        }

        private void Move()
        {
            //For gunSprite
            Vector2 playerPos = GameSetup.PlayerChar.Position2D;
            Vector2 direction = playerPos - Position2D;
            direction = Vector2.Normalize(direction);
            RotationHelper.FaceDirection(gunSprite, direction, DirectionAccuracy.free, 0);
            gunSprite.Position2D = Position2D + (direction * 15);
            Vector2 currentPosition = Position2D;

            float distanceFromPlayer = Vector2.Distance(Position2D, playerPos);

            if (distanceFromPlayer > 200)
            {
                RotationHelper.FacePosition(this, GameSetup.PlayerChar.Position, DirectionAccuracy.free, 0, false);
                RotationHelper.VelocityInCurrentDirection(this, 600, 0);
            }
            if(distanceFromPlayer < 400)
            {
                int dirMultiplier = 1;
                if (tiSwitchDirection.ElapsedSoFar > 2) dirMultiplier = -1;
                RotationHelper.VelocityInThisDirection(this, RotationHelper.MyDirection(this, dirMultiplier * 60), 300);
                //For firing
                if (GM.eventM.Elapsed(tiShootCooldown))
                {
                    //create bullet and pass reference to player and angle
                    new Bullet(this, playerPos, 1000f);
                }
            }
        }

        private void Hit(Sprite hit)
        {

        }

        private void KillSprites()
        {
            if (Health <= 0)
            {
                gunSprite.Kill();
            }
        }
    }
}