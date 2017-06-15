using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class Bullet : Sprite
    {
        private Sprite player;
        private int damage;

        internal Sprite Player
        {
            get
            {
                return player;
            }

            //set
            //{
            //    player = value;
            //}
        }

        /// <summary>
        /// Constructor for bullet class
        /// </summary>
        /// <param name="player">The object that fired the bullet</param>
        /// <param name="fireAngle">2D vector to travel towards</param>
        /// <param name="bulletSpeed">Speed of bullet</param>
        /// <param name="bulletDamage">Damage on hit</param>
        public Bullet(Sprite player, Vector2 fireAngle, float bulletSpeed, int bulletDamage)
        {
            damage = bulletDamage;
            
            this.player = player;
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SX = 4;
            SY = 24;



            //Sound effects
            GM.audioM.PlayEffect("shoot");

            //get player attributes
            Wash = player.Wash;
            //RotationAngle = player.RotationAngle;

            //set postion of bullet and give velocity
            X = player.Centre.X;
            Y = player.Centre.Y;

            //Set rotation at player
            Position2D = RotationHelper.RotateAround(player.Centre2D, player.Centre2D, 0);

            //Create direction vector and normalise
            Vector2 direction = fireAngle - Position2D;
            direction = Vector2.Normalize(direction);

            //Face direction vector
            RotationHelper.FaceDirection(this, direction, DirectionAccuracy.free, 0);
            RotationHelper.VelocityInCurrentDirection(this, bulletSpeed, 0);
            Position += RotationHelper.MyDirection(this, 0) * 32;

            //collision setup
            CollisionActive = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
            EpilogueCallBack += AfterHit;
            Moving = true;

            //kill after 5 seconds
            TimerInitialise();
            Timer.KillAfter(5f);

            //allow to wrap
            //LimitInitialise();
            //Limit.ViewPortAction(LimitAction.wrapExact);
        }

        private void Hit(Sprite hit)
        {
            if (hit is Bullet)
            {
                CollisionAbandonResponse = true;
            }
            if (hit is Player)
            {
                if (player != hit && GameSetup.PlayerChar.Invulnerable == false)
                {
                    GameSetup.PlayerChar.Health -= damage;
                    GM.audioM.PlayEffect("explode");

                    Kill();
                    if (GameSetup.PlayerChar.Health <= 0)
                    {
                        GameSetup.PlayerChar.Kill();
                    }
                }
                else
                {
                    CollisionAbandonResponse = true;
                    //Kill();
                }
            }
            if(hit is Enemy)
            {
                Enemy enemy = (Enemy)hit;
                enemy.Health -= damage;
                Kill();
                if (enemy.Health <= 0)
                {
                    enemy.Kill();
                    GM.audioM.PlayEffect("enemykilled");
                }
            }
        }
        private void AfterHit(Sprite hit)
        {
            RotationHelper.FaceVelocity(this, DirectionAccuracy.free, false, 0f);
        }
    }
}