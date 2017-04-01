using System;
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
    /// defines a cursor sprite
    /// </summary>
    class Cursor : Sprite
    {
        /// <summary>
        /// an event to control operational logic for this sprite
        /// </summary>
        private Event evLogic;

        /// <summary>
        /// creates a new cursor that tracks the mouse
        /// </summary>
        public Cursor()
        {
            GM.engineM.AddSprite(this);

            //Loading crosshair to frame ad colouring red
            Frame.Define(GM.txSprite, new Rectangle(34, 160, 36, 36));
            Wash = Color.LimeGreen;
            Align = Engine7.Align.topLeft;
            Layer = RenderLayer.hud;

            //Placing crosshair at mouse
            Vector2 mouseposition = new Vector2(Mouse.GetState().X - this.Width / 2, Mouse.GetState().Y - this.Height / 2);
            Position2D = mouseposition;
            WorldCoordinates = false;

            //add mouse logic to fire as fast as engine is updating
            GM.eventM.AddEvent(evLogic = new Event(GM.eventM.MaximumRate, "mouse cursor", Logic));
        }
        /// <summary>
        /// make sure the event is removed when this object is destroyed
        /// </summary>
        public override void CleanUp()
        {
            GM.eventM.Remove(evLogic);
        }

        /// <summary>
        /// allows a starting position to be specified
        /// </summary>
        /// <param name="start">the start position</param>
        public Cursor(Point start)
            :this()
        { 
            GM.inputM.MouseReset(start);
        }

        /// <summary>
        /// update logic that needs to be performed
        /// </summary>
        private void Logic()
        {
            //add on mouse movement to sprites position
            Position2D += GM.inputM.MouseDistance;
        }
        /// <summary>
        /// set the new position of the cursor
        /// </summary>
        /// <param name="newPosition">position to set cursor</param>
        public static void Reset(Vector2 newPosition)
        {
            GM.inputM.MouseReset(newPosition);
        }

    }
}
