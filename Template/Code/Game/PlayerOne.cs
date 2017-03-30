using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Engine7;

namespace Template.Game
{
    /// <summary>
    /// this inherits from player - takes on all its features and functions
    /// but has these specific keys, colour and start position
    /// </summary>
    internal class PlayerOne : Player
    {
        /// <summary>
        /// create a new player one and set its keys
        /// </summary>
        public PlayerOne()
            :base(new Vector2(1000, GM.screenSize.Center.Y), Color.Red)
        {
            SetKeys(Keys.A,Keys.D,Keys.W,Keys.S,Keys.LeftControl,Keys.Space);
            SetBoot(100, GM.screenSize.Bottom - 50);

        }

    }
}