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
using Template.Title;

namespace Template.Game
{
    class GameSetup : BasicSetup
    {
        int score1 = 0;
        int score2 = 0;
        private Event tiGameTimer;

        public GameSetup() : base(false)
        {
            GM.engineM.DebugDisplay = Debug.none;// Debug.fps | Debug.version | Debug.sprite;
            GM.engineM.ScreenColour = Color.Black;

            //create players and generate level layout
            new PlayerOne();

            //Remove when enemy generation implemented
            new ChargerEnemy(new Vector2(100, 100));

            //Create cursor and places centre on mouse position
            Cursor cursor = new Cursor();

            //new PlayerTwo();
            GenerateLevel();

            //create a timer to track game time
            GM.eventM.AddTimer(tiGameTimer = new Event(0.01f, "timer"));

            //listen out for player death to manage lives and score
            MessageBus.Instance.Subscribe(ExtraMessageTypes.PlayerDestroyed, PlayerKilled);
        }

        /// <summary>
        /// generate level walls
        /// </summary>
        private void GenerateLevel()
        {
            new wall(GM.screenSize.Center.X - 20, GM.screenSize.Center.Y - 200, 40, 400);
        }

        /// <summary>
        /// update scores and decide if game is over
        /// </summary>
        /// <param name="message"></param>
        private void PlayerKilled(Message message)
        {
            if (message.ObjectValue is PlayerOne)
            {
                //player 1 shot so points for player 2
                score2++;

                //if game not won create another player one
                if (score2 < 10)
                    new PlayerOne();
                else
                    //player 2 won
                    GameOver(2);
            }
            else
            {
                //player 1 shot so points for player 2
                score1++;

                //if game not won create another player two
                if (score1 < 10)
                    //new PlayerTwo();
                    ;
                else
                    //player 1 won
                    GameOver(1);
            }
        }

        /// <summary>
        /// do a display showing who won and how quick
        /// update best score if time is quicker
        /// </summary>
        /// <param name="winner"></param>
        private void GameOver(int winner)
        {
            //remove subscriptions
            MessageBus.Instance.Unsubscribe(ExtraMessageTypes.PlayerDestroyed, PlayerKilled);

            //get game time and check for high score (quickest time)
            float timeTaken = tiGameTimer.ElapsedSoFar;
            if (timeTaken < GM.BestScore)
                GM.BestScore = timeTaken;

            //stop timer
            GM.eventM.Remove(tiGameTimer);

            //create a dummy sprite to kill display text after 10 seconds
            Sprite dummy = new Sprite();
            dummy.TimerInitialise();
            dummy.Timer.KillAfter(10);

            //concatenate the message to display
            // ~ forces a line break
            string message = "GAME OVER~PLAYER " + winner + "~YOU ARE AWESOME~GAME WON IN " + Math.Round(timeTaken, 2) + " seconds";

            GM.textM.DrawAsSprites(FontBank.arcadeLarge, message ,
                GM.screenSize.Center.X, GM.screenSize.Center.Y, TextAtt.Centred, 3000, dummy);

            //run BackToTitle subroutine after 10 seconds
            GM.eventM.DelayCall(10, BackToTitle);

        }

        public override void Logic()
        {
            //display code
            GM.textM.Draw(FontBank.arcadeLarge, "1 UP~" + score1, 30, 30, TextAtt.TopLeft);
            GM.textM.Draw(FontBank.arcadeLarge, "2 UP~" + score2, GM.screenSize.Right - 30, 30, TextAtt.TopRight);
            GM.textM.Draw(FontBank.arcadeLarge, "QUICKEST~" + Math.Round(GM.BestScore,2), GM.screenSize.Center.X, 10, TextAtt.Top);

            //let player quit
            if (GM.inputM.KeyPressed(Keys.Escape))
            {
                BackToTitle();
            }
        }


        private static void BackToTitle()
        {
            GM.ClearAllManagedObjects();
            GM.active = new TitleSetup();
        }
    }
}
