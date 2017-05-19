using Engine7;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Template.Game
{
    internal class Enemy : Sprite
    {
        private int health;
        private int killPoints;
        private Sprite healthSprite;
        private float heightAbove;

        internal int Health
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

        public int KillPoints
        {
            get
            {
                return killPoints;
            }

            set
            {
                killPoints = value;
            }
        }

        public Enemy()
        {
            //Create healthSprite
            healthSprite = new Sprite();
            GM.engineM.AddSprite(healthSprite);
            healthSprite.Frame.Define(Tex.Rectangle50by50);
            healthSprite.SX = 0.01f * health;
            healthSprite.SY = 0.1f;
            //healthSprite.SY = 50;
            heightAbove = Convert.ToSingle(Math.Sqrt(Width * Width + Height * Height)) / 2;
            healthSprite.X = X;
            healthSprite.Y = Y + heightAbove - Height;
            healthSprite.Layer += 2;

            //Callbacks
            PrologueCallBack += HealthBar;
            UpdateCallBack += Update;
            FuneralCallBack += Funeral;
        }

        private void Update()
        {
            //For healthSprite:
            healthSprite.Wash = Wash;
            healthSprite.X = X;
            healthSprite.Y = Y + heightAbove - 50;
        }

        /// <summary>
        /// Called after being hit, currently used for changing health bar size
        /// </summary>
        /// <param name="hit"></param>
        private void HealthBar(Sprite hit)
        {
            if (hit is Bullet)
            {
                Bullet bullet = (Bullet)hit;
                if (bullet.Player == GameSetup.PlayerChar)
                {
                    if (health > 0)
                    {
                        healthSprite.SX = 0.01f * health;
                    }
                }
            }
        }

        /// <summary>
        /// Called on funeral
        /// </summary>
        private void Funeral()
        {
            GameSetup.EnemySpawnSystem.Score += killPoints;
            healthSprite.Kill();
        }
    }
}
