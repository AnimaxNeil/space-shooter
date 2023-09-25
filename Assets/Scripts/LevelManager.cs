namespace NMX.SpaceShooter.Managers
{
    using Controllers;
    using Data;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

    public sealed class LevelManager : MonoBehaviour
    {
        [SerializeField]
        private RawImage background;
        [SerializeField]
        private RectTransform gameArea;
        [SerializeField]
        private ShipController playerPrefab, enemyDronePrefab;
        [SerializeField]
        private TextMeshProUGUI level, kills, score, bestScore, lives;
        [SerializeField]
        private RectTransform startScreen, pauseScreen, winScreen, looseScreen;
        private ShipController player;
        private List<ShipController> enemyDrones = new(10);
        private int currentLevelIndex;
        private Level currentLevel;
        private float backgroundMotionDelay, backgroundMotionTimer;
        private float enemySpawnTimer;


        private void Start()
        {
            InitiateGame();
            SetUpEvents();
            Application.targetFrameRate = 60;
        }
        private void Update()
        {
            CheckActions();
            CheckEffects();
            if (!GameStats.running) return;
            CheckSpawns();
        }

        private void LoadLevel(Level level)
        {
            currentLevel = level;
            GameStats.levelKills = 0;
            GameStats.lives++;
            SetStats();
        }
        private void CheckProgress()
        {
            if (GameStats.score >= GameStats.bestScore)
            {
                GameStats.bestScore = GameStats.score; 
                PlayerPrefs.SetInt(Names.bestScoreSaveKey, GameStats.bestScore);
            }
            if (GameStats.levelKills == currentLevel.targetKills)
            {
                if (currentLevelIndex == LevelData.levels.Length - 1) EndGame(true);
                else LoadLevel(LevelData.levels[++currentLevelIndex]);
            }
        }
        private void SetStats()
        {
            level.text = "Lvl " + (currentLevelIndex + 1);
            kills.text = "Kills\n" + GameStats.totalKills;
            score.text = "Score\n" + GameStats.score;
            lives.text = "Lives\n" + GameStats.lives;
            CheckProgress();
            bestScore.text = "Best\nScore\n" + GameStats.bestScore;
        }
        private void HandleShipDestroyed(ShipController ship)
        {
            //specific
            switch (ship.shipType)
            {
                case ShipController.ShipType.Player:
                    GameStats.lives--;
                    DestroyAllShipsAndBullets();
                    if (GameStats.lives < 0) GameStats.lives = 0;
                    if (GameStats.lives == 0) EndGame(false);
                    break;
                case ShipController.ShipType.EnemyDrone:
                    if (enemyDrones.Contains(ship)) enemyDrones.Remove(ship);
                    GameStats.score += 10;
                    GameStats.levelKills++;
                    GameStats.totalKills++;
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            //common
            SetStats();
            if (GameConfig.sfx) AudioManager.instance.PlayOneShot(Names.audioShipExplosion);
        }
        private void HandleBulletFired(ShipController ship)
        {
            //specifics
            switch (ship.shipType)
            {
                case ShipController.ShipType.Player:
                    if (GameConfig.sfx) AudioManager.instance.PlayOneShot(Names.audioPlayerBullet);
                    break;
                case ShipController.ShipType.EnemyDrone:
                    if (GameConfig.sfx) AudioManager.instance.PlayOneShot(Names.audioEnemyBullet);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
        private void HandleBulletDestroyed(BulletController bullet)
        {
            //specific
            switch (bullet.bulletType)
            {
                case BulletController.BulletType.PlayerStandard:
                    break;
                case BulletController.BulletType.EnemyDroneStandard:
                    break;
                default:
                    throw new System.NotImplementedException();
            }
            //common
            SetStats();
            if (GameConfig.sfx) AudioManager.instance.PlayOneShot(Names.audioBulletExplosion);
        }
        private void SetUpEvents()
        {
            ShipController.OnDestroyed += HandleShipDestroyed;
            ShipController.OnBulletFired += HandleBulletFired;
            BulletController.OnDestroyed += HandleBulletDestroyed;
        }

        private void InitiateGame()
        {
            currentLevelIndex = 0;
            backgroundMotionDelay = 0.005f;
            GameStats.running = false;
            GameStats.lives = 3;
            GameStats.score = 0;
            GameStats.totalKills = 0;
            GameStats.levelKills = 0;
            GameStats.bestScore = PlayerPrefs.GetInt(Names.bestScoreSaveKey, 0);
            startScreen.gameObject.SetActive(true);
            pauseScreen.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(false);
            looseScreen.gameObject.SetActive(false);
            LoadLevel(LevelData.levels[currentLevelIndex]);
        }

        private void DestroyAllShipsAndBullets()
        {
            foreach (ShipController ship in gameArea.GetComponentsInChildren<ShipController>(true)) Destroy(ship.gameObject);
            foreach (BulletController bullet in gameArea.GetComponentsInChildren<BulletController>(true)) Destroy(bullet.gameObject);
            enemyDrones.Clear();
        }
        private void SetAllShipsAndBulletsVisibility(bool visible)
        {
            foreach (ShipController ship in gameArea.GetComponentsInChildren<ShipController>(true)) ship.gameObject.SetActive(visible);
            foreach (BulletController bullet in gameArea.GetComponentsInChildren<BulletController>(true)) bullet.gameObject.SetActive(visible);
        }
        private void StartGame()
        {
            GameStats.running = true;
            startScreen.gameObject.SetActive(false);
            pauseScreen.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(false);
            looseScreen.gameObject.SetActive(false);
            if (!GameConfig.music) return;
            if (!AudioManager.instance.IsPlaying(Names.audioMainMusic)) AudioManager.instance.Play(Names.audioMainMusic);
        }
        private void PauseGame(bool pause)
        {
            GameStats.running = !pause;
            startScreen.gameObject.SetActive(false);
            pauseScreen.gameObject.SetActive(pause);
            winScreen.gameObject.SetActive(false);
            looseScreen.gameObject.SetActive(false);
            SetAllShipsAndBulletsVisibility(!pause);
            if (!GameConfig.music) return;
            if (pause && AudioManager.instance.IsPlaying(Names.audioMainMusic)) AudioManager.instance.Pause(Names.audioMainMusic);
            else if (!pause && !AudioManager.instance.IsPlaying(Names.audioMainMusic)) AudioManager.instance.UnPause(Names.audioMainMusic);
        }
        private void EndGame(bool win)
        {
            GameStats.running = false;
            List<ShipController> enemies = new(enemyDrones.Count);
            foreach (ShipController ship in enemyDrones) enemies.Add(ship);
            foreach (ShipController ship in enemies) ship.Destroy();
            startScreen.gameObject.SetActive(false);
            pauseScreen.gameObject.SetActive(false);
            winScreen.gameObject.SetActive(win);
            looseScreen.gameObject.SetActive(!win);
            SetAllShipsAndBulletsVisibility(false);
            DestroyAllShipsAndBullets();
            if (!GameConfig.music) return;
            if (AudioManager.instance.IsPlaying(Names.audioMainMusic)) AudioManager.instance.Stop(Names.audioMainMusic);
        }
        private void CheckActions()
        {
            if (!GameStats.running && (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Space)) && startScreen.gameObject.activeSelf) StartGame();
            else if (!GameStats.running && (Input.GetKeyDown(KeyCode.RightControl) || Input.GetKeyDown(KeyCode.Space)) && (winScreen.gameObject.activeSelf || looseScreen.gameObject.activeSelf))
            { InitiateGame(); StartGame(); }
            else if (GameStats.running && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) && !pauseScreen.gameObject.activeSelf) PauseGame(true);
            else if (!GameStats.running && (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) && pauseScreen.gameObject.activeSelf) PauseGame(false);
        }

        private void CheckBackgroundMotionEffect()
        {
            if (backgroundMotionTimer < backgroundMotionDelay) { backgroundMotionTimer += Time.deltaTime; return; }
            Rect uvRect = background.uvRect;
            if (uvRect.position.y >= 1) uvRect.position = Vector2.zero;
            else uvRect.position += (int)(backgroundMotionTimer / backgroundMotionDelay) * new Vector2(0, 0.002f);
            background.uvRect = uvRect;
            backgroundMotionTimer = 0;
        }
        private void CheckEffects()
        {
            CheckBackgroundMotionEffect();
        }

        private void SpawnPlayer()
        {
            if (player != null) return;
            player = Instantiate(playerPrefab, gameArea);
        }
        private void CheckPlayerSpawn()
        {
            if (player == null) SpawnPlayer();
        }
        private void SpawnEnemyDrone()
        {
            if (enemyDrones.Count == currentLevel.enemyDronesSpawnLimit) return;
            enemyDrones.Add(Instantiate(enemyDronePrefab, gameArea));
        }
        private void CheckEnemySpawns()
        {
            if (enemyDrones.Count > 0)
                if (enemySpawnTimer < currentLevel.enemyDronesSpawnDelay) { enemySpawnTimer += Time.deltaTime; return; }
            enemySpawnTimer = 0;
            SpawnEnemyDrone();
        }
        private void CheckSpawns()
        {
            CheckPlayerSpawn();
            CheckEnemySpawns();
        }


    }
}
