using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Engine7;
using Template.Title;

namespace Template
{
    internal class EnemySpawnSystem : Sprite
    {
        private Event tiSpawnTimer;
        private Random r;
        private int spawnX;
        private int spawnY;
        //private Random rY;

        public EnemySpawnSystem()
        {
            r = new Random();
            //rY = new Random();
            GM.engineM.AddSprite(this);
            GM.eventM.AddTimer(tiSpawnTimer = new Event(600, "Round timer"));
            TimerInitialise();
            Timer.EventContinous(1, Spawning);
        }


        private void Spawning()
        {
            //Deciding edge to spawn on
            Vector2 spawnPos = new Vector2();
            //Generate int between 0 and 3
            int spawnSide = r.Next(0, 3);
            //Left
            if (spawnSide == 0)
            {
                spawnX = 0;
                spawnY = r.Next(0, GM.screenSize.Bottom);
                spawnPos = new Vector2(spawnX, spawnY);
            }
            //Right
            if (spawnSide == 1)
            {
                spawnX = GM.screenSize.Right;
                spawnY = r.Next(0, GM.screenSize.Bottom);
                spawnPos = new Vector2(spawnX, spawnY);
            }
            //Top
            if (spawnSide == 2)
            {
                spawnY = 0;
                spawnX = r.Next(0, GM.screenSize.Right);
                spawnPos = new Vector2(spawnX, spawnY);
            }
            //Bottom
            if (spawnSide == 3)
            {
                spawnY = GM.screenSize.Bottom;
                spawnX = r.Next(0, GM.screenSize.Right);
                spawnPos = new Vector2(spawnX, spawnY);
            }

            //Every 40 seconds
            if (((int)tiSpawnTimer.ElapsedSoFar % 40) == 0)
            {
                LaserEnemy laserEnemy = new LaserEnemy(spawnPos, true);
            }
            else
            {
                //Every 20 seconds
                if (((int)tiSpawnTimer.ElapsedSoFar % 20) == 0)
                {
                    LaserEnemy laserEnemy = new LaserEnemy(spawnPos, false);
                }
                else
                {
                    //Every 5 seconds
                    if (((int)tiSpawnTimer.ElapsedSoFar % 5) == 0)
                    {
                        //StraferEnemy straferEnemy = new StraferEnemy(spawnPos);
                    }
                    else
                    {
                        //Every 1 second
                        if (((int)tiSpawnTimer.ElapsedSoFar % 1) == 0)
                        {
                            //ChargerEnemy chargerEnemy = new ChargerEnemy(spawnPos);
                        }
                    }
                }
            }
        }
    }
}