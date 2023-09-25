namespace NMX.SpaceShooter.Data
{
    public struct Level
    {
        public int enemyDronesSpawnLimit;
        public float enemyDronesSpawnDelay;
        public int targetKills;
    }
    public static class LevelData
    {
        public static Level[] levels = {
            //0
            new Level { 
                enemyDronesSpawnLimit = 3, 
                enemyDronesSpawnDelay = 10,
                targetKills = 12,
            },
            //1
            new Level {
                enemyDronesSpawnLimit = 4,
                enemyDronesSpawnDelay = 8,
                targetKills = 20,
            },
            //2
            new Level {
                enemyDronesSpawnLimit = 5,
                enemyDronesSpawnDelay = 5,
                targetKills = 30,
            },
            //3
            new Level {
                enemyDronesSpawnLimit = 6,
                enemyDronesSpawnDelay = 3,
                targetKills = 50,
            },
            //4
            new Level {
                enemyDronesSpawnLimit = 8,
                enemyDronesSpawnDelay = 3,
                targetKills = 100,
            },
        };
    }
}
