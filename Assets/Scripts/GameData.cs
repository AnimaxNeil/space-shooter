namespace NMX.SpaceShooter.Data
{
    public static class GameStats
    {
        public static bool running;
        public static int lives, score, bestScore, totalKills, levelKills;
    }
    public static class GameConfig
    {
        public static bool music = true, sfx = true;
    }
    public static class Names
    {
        public const string
            bestScoreSaveKey = "best-score",
            audioMainMusic = "main-music",
            audioPlayerBullet = "player-bullet",
            audioEnemyBullet = "enemy-bullet",
            audioShipExplosion = "ship-explosion",
            audioBulletExplosion = "bullet-explosion";
    }
}
