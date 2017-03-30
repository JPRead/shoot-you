﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
//--engine import
using Engine7;

namespace Template
{
    /// <summary>
    /// hold base settings here for items needed during all containers
    /// </summary>
    public abstract class BasicSetup
    {
        /// <summary>
        /// holds the logic event
        /// </summary>
        private Event evLogic;
        /// <summary>
        /// contains items that all containers need to activate
        /// </summary>
        public BasicSetup(bool cursoron)
        {
            //all modes need a cursor?
            if (cursoron) new Cursor(GM.screenSize.Center);

            GM.eventM.AddEvent(evLogic = new Event(GM.eventM.MaximumRate, "container logic", Logic));
        }
        /// <summary>
        /// default logic for the container. You need to override this method
        /// </summary>
        public virtual void Logic()
        {
            
        }

    }
}
