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
        private bool increasedRate;

        //Spawn score thresholds
        private int straferSpawn = 4;
        private int turretSpawn = 16;
        private int laserSpawn = 64;

        //Spawn chances
        private int chargerChance = 1200;
        private int straferChance = 2400;
        private int turretChance = 2500;
        private int doubleTurretChance = 2550;
        private int laserChance = 2650;
        private int doubleLaserChance = 2700;
        private Event tiDoubleSpawnCooldown;

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

        public bool IncreasedRate
        {
            get
            {
                return increasedRate;
            }

            set
            {
                increasedRate = value;
            }
        }

        public EnemySpawnSystem()
        {
            increasedRate = false;

            r = new Random();
            GM.engineM.AddSprite(this);
            GM.eventM.AddTimer(tiSpawnTimer = new Event(600, "Round timer"));
            GM.eventM.AddTimer(tiDoubleSpawnCooldown = new Event(5, "Double spawn cooldown"));
            TimerInitialise();
            Timer.EventContinous(1, Spawn);
            Timer.EventContinous(0.5f, Spawn);
            
            UpdateCallBack += Tick;
        }

        private void Tick()
        {

        }

        private void Spawn()
        {
            if (increasedRate)
            {
                //Deciding edge to spawn on
                Vector2 spawnPosI = new Vector2();
                //Generate int between 0 and 3
                int spawnSideI = r.Next(0, 3);
                //Left
                if (spawnSideI == 0)
                {
                    spawnX = 0;
                    spawnY = r.Next(0, GM.screenSize.Bottom);
                    spawnPosI = new Vector2(spawnX, spawnY);
                }
                //Right
                if (spawnSideI == 1)
                {
                    spawnX = GM.screenSize.Right;
                    spawnY = r.Next(0, GM.screenSize.Bottom);
                    spawnPosI = new Vector2(spawnX, spawnY);
                }
                //Top
                if (spawnSideI == 2)
                {
                    spawnY = 0;
                    spawnX = r.Next(0, GM.screenSize.Right);
                    spawnPosI = new Vector2(spawnX, spawnY);
                }
                //Bottom
                if (spawnSideI == 3)
                {
                    spawnY = GM.screenSize.Bottom;
                    spawnX = r.Next(0, GM.screenSize.Right);
                    spawnPosI = new Vector2(spawnX, spawnY);
                }

                enemySelector = new Random();
                EnemySpawner(spawnPosI);
            }
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

            //Start double spawns every 20 seconds
            if (((int)(tiSpawnTimer.ElapsedSoFar)) % 20 == 0)
            {
                increasedRate = true;
                GM.eventM.AddTimer(tiDoubleSpawnCooldown = new Event(5, "Double spawn cooldown"));
            }
            //And end them after 5 seconds
            if (GM.eventM.Elapsed(tiDoubleSpawnCooldown))
            {
                increasedRate = false;
                GM.eventM.AddTimer(tiDoubleSpawnCooldown = new Event(5, "Double spawn cooldown"));
            }

        }

        private void EnemySpawner(Vector2 spawnPos)
        {
            int totalChance = 100;
            if (score > straferSpawn) totalChance = straferChance;
            if (score > turretSpawn) totalChance = turretChance;
            if (score > doubleTurretChance) totalChance = doubleTurretChance;
            if (score > laserSpawn) totalChance = doubleLaserChance;

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
                            else
                            {
                                if (chance <= doubleLaserChance) { LaserEnemy laserEnemy = new LaserEnemy(spawnPos, true); }
                            }
                        }
                    }
                }
            }
        }
    }
}