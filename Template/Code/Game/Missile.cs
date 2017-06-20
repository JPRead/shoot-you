using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine7;
using Microsoft.Xna.Framework;

namespace Template.Game
{

    internal class Missile : Sprite
    {
        private Sprite owner;
        private int damage;
        private Sprite target;
        private float turnAmount;
        private float speed;
        private Event tiLifetimeCounter;
        private Event tiSmokeCounter;

        public Sprite Owner
        {
            get
            {
                return owner;
            }

            set
            {
                owner = value;
            }
        }

        /// <summary>
        /// Constructor for missile
        /// </summary>
        /// <param name="spawnPos">Start position</param>
        /// <param name="initialAim">Position missile is initally aimed at</param>
        /// <param name="ownerSprite">Sprite that fired this missile</param>
        /// <param name="targetSprite">Target for missile</param>
        /// <param name="missileSpeed">Velocity of missile</param>
        /// <param name="maxTurn">Maximum turning angle for missile</param>
        /// <param name="lifetime">How many seconds this missile lives for</param>
        /// <param name="missileDamage">Damage on impact</param>
        public Missile(Vector2 spawnPos, Vector2 initialAim, Sprite ownerSprite, Sprite targetSprite, float missileSpeed, float maxTurn, float lifetime, int missileDamage)
        {
            //Assigning variables
            owner = ownerSprite;
            target = targetSprite;
            damage = missileDamage;
            turnAmount = maxTurn;
            speed = missileSpeed;

            //Events
            GM.eventM.AddTimer(tiLifetimeCounter = new Event(lifetime, "Lifetime Counter"));
            GM.eventM.AddTimer(tiSmokeCounter = new Event(0.5f, "Smoke Counter"));

            //Graphics
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);
            Wash = Color.OrangeRed;
            SX = 8;
            SY = 32;

            //Sound effect


            Moving = true;
            CollisionActive = true;
            CollisionPrimary = true;

            UpdateCallBack += Move;
            PrologueCallBack += Hit;

            RotationHelper.FacePosition(this, new Vector3(initialAim, 0), DirectionAccuracy.free, 0, false);

            Position2D = spawnPos;
        }

        private void Hit(Sprite hit)
        {
            if(hit is Enemy)
            {
                if(owner is Enemy)
                {
                    Kill();
                }
                else
                {
                    Enemy enemy = (Enemy)hit;
                    enemy.Health -= damage;
                    if(enemy.Health <= 0)
                    {
                        enemy.Kill();
                    }
                    Kill();
                }
            }
            if(hit is Player && GameSetup.PlayerChar.Invulnerable == false)
            {
                if(owner is Player)
                {
                    
                }
                else
                {
                    GameSetup.PlayerChar.Health -= damage;
                    if(GameSetup.PlayerChar.Health <= 0)
                    {
                        GameSetup.PlayerChar.Kill();
                        Kill();
                    }
                }
            }
            if(hit is Bullet)
            {
                Bullet bullet = (Bullet)hit;
                if(bullet.Player == GameSetup.PlayerChar)
                {
                    bullet.Kill();
                    Kill();
                }
            }
        }

        private void Move()
        {
            //Trailing smoke particles
            if (tiLifetimeCounter.ElapsedSoFar > 0.5 - GM.r.FloatBetween(0.1f, 0.3f))
            {
                Vector2 offset = new Vector2(20 * RotationHelper.MyDirection(this, 180).X, 20 * RotationHelper.MyDirection(this, 180).Y);
                new SmokeParticle(Position2D + offset, Vector3.Zero, Vector2.Zero, 0.2f);
                GM.eventM.AddTimer(tiSmokeCounter = new Event(0.5f, "Smoke Counter"));
            }

            if (target.Dead)
            {
                RotationHelper.VelocityInCurrentDirection(this, speed, 0);
            }
            else
            {
                //turnDir = -1, anticlockwise; 0, none; 1, clockwise
                int turnDir = (int)RotationHelper.AngularDirectionTo(this, target.Position, 0, false);

                //Direction to add velocity to, additional angle is 90 * turnDir so -90 for anticlockwise, 90 for clockwise, straight ahead for none
                Vector3 velDir = RotationHelper.MyDirection(this, 90 * turnDir);

                Velocity = RotationHelper.MyDirection(this, 0) * speed;
                Velocity += velDir * (turnAmount / 5);
                RotationHelper.FaceVelocity(this, DirectionAccuracy.free, false, 0);
            }
        }
    }
}