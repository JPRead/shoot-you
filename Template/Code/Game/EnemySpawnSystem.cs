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
        private Random enemySelector;
        private int spawnX;
        private int spawnY;
        private int score;

        //Spawn score thresholds
        private int straferSpawn = 4;
        private int turretSpawn = 16;
        private int laserSpawn = 64;

        //Spawn chances
        private int chargerChance = 200;
        private int straferChance = 300;
        private int turretChance = 350;
        private int doubleTurretChance = 400;
        private int laserChance = 425;

        public int Score
        {
            get
            {
                return score;
            }

            set
            {
                score = value;
            }
        }

        public EnemySpawnSystem()
        {
            r = new Random();
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

            enemySelector = new Random();
            EnemySpawner(spawnPos);
            
            ////Every 40 seconds
            //if (((int)tiSpawnTimer.ElapsedSoFar % 40) == 0)
            //{
            //    //LaserEnemy laserEnemy = new LaserEnemy(spawnPos, false);
            //}
            //else
            //{
            //    //Every 20 seconds
            //    if (((int)tiSpawnTimer.ElapsedSoFar % 20) == 0)
            //    {
            //        TurretEnemy turretEnemy = new TurretEnemy(spawnPos, true);
            //    }
            //    else
            //    {
            //        //Every 10 seconds
            //        if (((int)tiSpawnTimer.ElapsedSoFar % 15) == 0)
            //        {
            //            TurretEnemy turretEnemy = new TurretEnemy(spawnPos, false);
                        
            //        }
            //        else
            //        {
            //            {
            //                //Every 5 seconds
            //                if (((int)tiSpawnTimer.ElapsedSoFar % 5) == 0)
            //                {
            //                    StraferEnemy straferEnemy = new StraferEnemy(spawnPos);
            //                }
            //                else
            //                {
            //                    //Every 1 second
            //                    if (((int)tiSpawnTimer.ElapsedSoFar % 1) == 0)
            //                    {
            //                        ChargerEnemy chargerEnemy = new ChargerEnemy(spawnPos);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }

        private void EnemySpawner(Vector2 spawnPos)
        {
            int totalChance = 100;
            if (score > straferSpawn) totalChance = straferChance;
            if (score > turretSpawn) totalChance = turretChance;
            if (score > doubleTurretChance) totalChance = doubleTurretChance;
            if (score > laserSpawn) totalChance = laserChance;

            int chance = enemySelector.Next(totalChance);

            if (chance <= chargerChance) { ChargerEnemy chargerEnemy = new ChargerEnemy(spawnPos); }
            else
            {
                if (chance <= straferChance) { StraferEnemy straferEnemy = new StraferEnemy(spawnPos); }
                else
                {
                    if (chance <= turretChance) { TurretEnemy turretEnemy = new TurretEnemy(spawnPos, false); }
                    else
                    {
                        if (chance <= doubleTurretChance) { TurretEnemy turretEnemy = new TurretEnemy(spawnPos, true); }
                        else
                        {
                            if (chance <= laserChance) { LaserEnemy laserEnemy = new LaserEnemy(spawnPos, false); }
                        }
                    }
                }
            }
        }
    }
}