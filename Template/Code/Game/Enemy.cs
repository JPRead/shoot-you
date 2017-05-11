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

        public Enemy()
        {
            //Create healthSprite
            healthSprite = new Sprite();
            GM.engineM.AddSprite(healthSprite);
            healthSprite.Frame.Define(Tex.Rectangle50by50);
            healthSprite.SX = 0.01f * health;
            healthSprite.SY = 0.1f;
            heightAbove = Convert.ToSingle(Math.Sqrt(Width * Width + Height * Height)) / 2;
            healthSprite.X = X;
            healthSprite.Y = Y + heightAbove;

            //Callbacks
            EpilogueCallBack += HealthBar;
            UpdateCallBack += Update;
            FuneralCallBack += KillHealthBar;
        }

        private void Update()
        {
            //For healthSprite:
            healthSprite.X = X;
            healthSprite.Y = Y + heightAbove;
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
                        healthSprite.Wash = Wash;
                        healthSprite.SX = 0.01f * health;
                    }
                }
            }
        }

        /// <summary>
        /// Called on funeral
        /// </summary>
        private void KillHealthBar()
        {
            healthSprite.Kill();
        }
    }
}
