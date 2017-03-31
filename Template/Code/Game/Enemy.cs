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
            health = 100;
            CollisionActive = true;
        }

    }
}
