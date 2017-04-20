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
    internal class ChargerEnemy : Enemy
    {
        public ChargerEnemy(Vector2 startPos)
        {
            //Set health
            Health = 50;

            //Set collisions
            CollisionActive = true;
            CollisionPrimary = true;

            Friction = 10f;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Triangle);

            //set position and colour
            Position2D = startPos;
            Wash = Color.OrangeRed;
            SY = 1.25f;

            UpdateCallBack += Move;
            PrologueCallBack += Hit;
            EpilogueCallBack += Stop;
        }

        private void Move()
        {
            RotationHelper.FacePosition(this, GameSetup.PlayerChar.Position, DirectionAccuracy.free, 0, false);
            RotationHelper.VelocityInCurrentDirection(this, 400, 0);
        }

        private void Stop(Sprite hit)
        {
            //stop if hit wall
            if (hit is wall)
                Velocity = Vector3.Zero;
        }

        private void Hit(Sprite hit)
        {
            if(hit == GameSetup.PlayerChar)
            {
                GameSetup.PlayerChar.Health -= 10;
                if (GameSetup.PlayerChar.Health <= 0) GameSetup.PlayerChar.Kill();
                Kill();
                GM.audioM.PlayEffect("explode");
            }
        }

    }
}