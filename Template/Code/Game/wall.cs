using Engine7;

namespace Template.Game
{
    internal class wall : Sprite
    {
        /// <summary>
        /// create a simple rectangle with collision active
        /// </summary>
        /// <param name="x">left hand side</param>
        /// <param name="y">top of rectangle</param>
        /// <param name="width">pixel width</param>
        /// <param name="height">pixel height</param>
        public wall(int x, int y, int width, int height)
        {
            GM.engineM.AddSprite(this);
            Frame.Define(Tex.SingleWhitePixel);

            //set alignment to top left corner and set rectangle size using scaling
            Align = Align.topLeft;
            X = x;
            Y = y;
            SX = width;
            SY = height;

            //make collidable and rectangular
            CollisionActive = true;
            Shape = Shape.rectangle;
        }
    }
}