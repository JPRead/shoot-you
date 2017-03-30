using System;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{
    internal class Bullet : Sprite
    {
        private Player player;

        public Bullet(Player player, Vector2 fireAngle)
        {
            //Bullet won't collide with player
            //player.CollisionAvoid = true;
            //Gets faster over time!
            //this.Friction = -0.5f;
            this.player = player;
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            SX = 3;
            SY = 24;

            //get player attributes
            Wash = player.Wash;
            RotationAngle = player.RotationAngle;

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
            RotationHelper.VelocityInCurrentDirection(this, 500f, 0);

            //collision setup
            CollisionActive = true;
            CollisionPrimary = true;
            PrologueCallBack += Hit;
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
            else
                if (hit is Player)
                {
                    //don't shoot yourself
                    if (player != hit)
                    {
                        hit.Kill();
                        Kill();
                        MessageBus.Instance.BroadcastMessage(ExtraMessageTypes.PlayerDestroyed, hit);
                    }
                    else
                    {
                        CollisionAbandonResponse = true;
                    }
                }
                else
                {
                    //Flip sprite along normal when hitting anything else
                }
        }
    }
}