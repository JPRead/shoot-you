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
            Health = 100;

            //set management of sprite and graphic
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.Triangle);

            //set position and colour
            Position2D = startPos;
            Wash = Color.OrangeRed;
            SY = 1.25f;

        }
    }
}