using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // --- Static Instance ---
    public static GameController instance;

    // --- Prefabs ---
    public GameObject player;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject explosionPrefab;
    public GameObject[] powerUpPrefabs;

    // --- Spawn Management ---
    public Vector2 spawnArea;
    public float minSpawnWaitTime = 1f;
    public float maxSpawnWaitTime = 3f;
    public float bossSpawnDelay = 60f;
    private bool bossSpawnInProgress = false;

    // --- Gameplay State ---
    public bool guaranteeNextPowerup = false;
    public int currentScore = 0;
    public int currentLives = 3;
    private float timeElapsed;

    // --- UI Elements ---
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI highScoreAchievedText;
    public GameObject gameOverUI;
    public GameObject gameUI;
    public GameObject mobileControlsParent;
    public GameObject pauseUI;

    // --- Audio ---
    public AudioSource backgroundMusicSource;
    public AudioClip originalBackgroundMusic;
    public AudioClip bossMusic;
    public AudioSource gameOverSoundSource;
    public float fadeTime = 2f;

    // --- Internal Mechanics ---
    private Camera mainCamera;
    public bool shouldUseMobileControls = Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform;
    public float respawnDelay = 2.0f;
    public Vector2 respawnPosition;
    public float immunityDuration = 5.0f;
    public float powerUpDropChance = 0.3f;
    public float explosionAnimDuration = 1.0f;
    public bool isPaused = false;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        if (mobileControlsParent != null)
        {
            mobileControlsParent.SetActive(shouldUseMobileControls);
        }

        if (PlayerPrefs.GetInt("KonamiBonus", 0) == 1)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.IncreaseSpread(15);
            playerController.EnableExtraGun();
            currentLives = 100;
            playerController.currentShields = 3;

            PlayerPrefs.DeleteKey("KonamiBonus");
        }

        timeElapsed = 0f;
        mainCamera = Camera.main;
        CalculateSpawnAreaWidth();
        StartCoroutine(SpawnEnemies());
        UpdateHighScoreDisplay();
        livesText.text = "LIVES: " + currentLives;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= bossSpawnDelay)
        {
            SpawnBoss();
            timeElapsed = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void SpawnBoss()
    {
        if (!bossSpawnInProgress)
        {
            bossSpawnInProgress = true;
            StartCoroutine(StartBossMusic());
            Vector3 spawnPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 15f, 10f));
            Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        }
    }

    void CalculateSpawnAreaWidth()
    {
        float worldWidth = mainCamera.ViewportToWorldPoint(Vector3.one).x * 2;
        spawnArea.x = worldWidth / 2;
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (GameObject.FindGameObjectWithTag("Boss") == null)
            {
                float waitTime = Random.Range(minSpawnWaitTime, maxSpawnWaitTime);
                yield return new WaitForSeconds(waitTime);

                Vector2 spawnPosition = GetRandomSpawnPosition();
                Instantiate(enemyPrefab, spawnPosition, Quaternion.Euler(0, 0, 180));
            }
            else
            {
                yield return null;
            }
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        float randomX = Random.Range(-spawnArea.x, spawnArea.x);

        Vector3 topScreenEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 1, 0));

        return new Vector2(randomX, topScreenEdge.y + 1);
    }

    public void AddScore(int score)
    {
        currentScore += score;
        scoreText.text = "SCORE: " + currentScore;
    }

    public void UpdateLives(int lives)
    {
        currentLives += lives;
        livesText.text = "LIVES: " + currentLives;
    }

    public void PlayerDestroyed(GameObject playerObject)
    {
        GameObject explosion = Instantiate(explosionPrefab, playerObject.transform.position, Quaternion.identity);
        playerObject.SetActive(false);
        playerObject.GetComponent<PlayerController>().ResetPlayer();
        UpdateLives(-1);
        playerObject.GetComponent<PlayerController>().ResetHealth();

        Destroy(explosion, explosionAnimDuration);

        if (currentLives > 0)
        {
            StartCoroutine(RespawnPlayer(playerObject));
        }
        else
        {
            int numExplosions = 7;
            float explosionRadius = 1.0f;
            float minDelay = 0.1f;
            float maxDelay = 0.3f;
            int score = 0;

            StartCoroutine(SpawnMultipleExplosions(playerObject, numExplosions, explosionRadius, minDelay, maxDelay, score));

            StartCoroutine(DelayedGameOver());
        }
    }

    IEnumerator DelayedGameOver()
    {
        yield return new WaitForSeconds(3f);
        GameOver();
    }

    IEnumerator RespawnPlayer(GameObject playerObject)
    {
        yield return new WaitForSeconds(respawnDelay);

        playerObject.SetActive(true);
        Vector3 worldRespawnPosition = mainCamera.ViewportToWorldPoint(respawnPosition, 0);
        float yOffset = .05f;
        worldRespawnPosition.y = mainCamera.transform.position.y + yOffset;
        playerObject.transform.position = worldRespawnPosition;
        playerObject.GetComponent<PlayerController>().GrantImmunity(immunityDuration);
    }

    public void EnemyDestroyed(GameObject enemyObject, int scoreValue)
    {
        GameObject explosion = Instantiate(explosionPrefab, enemyObject.transform.position, Quaternion.identity);

        Destroy(enemyObject);
        AddScore(scoreValue);
        Destroy(explosion, explosionAnimDuration);

        if (Random.value < powerUpDropChance || guaranteeNextPowerup)
        {
            int randomIndex = Random.Range(0, powerUpPrefabs.Length);
            GameObject selectedPowerUp = powerUpPrefabs[randomIndex];

            Instantiate(selectedPowerUp, enemyObject.transform.position, Quaternion.identity);

            guaranteeNextPowerup = false;
        }
    }

    public void BossGunDestroyed(GameObject bossGunObject)
    {
        int numExplosions = 5;
        float explosionRadius = 1.0f;
        float minDelay = 0.1f;
        float maxDelay = 0.3f;
        int score = 100;

        StartCoroutine(SpawnMultipleExplosions(bossGunObject, numExplosions, explosionRadius, minDelay, maxDelay, score));
    }

    public void BossDestroyed(GameObject bossObject)
    {
        int numExplosions = 15;
        float explosionRadius = 2.5f;
        float minDelay = 0.2f;
        float maxDelay = 0.5f;
        int score = 1000;

        StartCoroutine(SpawnMultipleExplosions(bossObject, numExplosions, explosionRadius, minDelay, maxDelay, score));
        StartCoroutine(StartBackgroundMusic());

        bossSpawnInProgress = false;
        timeElapsed = 0f;

    }

    IEnumerator SpawnMultipleExplosions(GameObject bossObject, int numExplosions, float explosionRadius, float minDelay, float maxDelay, int score = 0)
    {
        for (int i = 0; i < numExplosions; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * explosionRadius;
            Vector3 explosionPos = bossObject.transform.position + (Vector3)randomOffset;

            GameObject explosion = Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
            Destroy(explosion, explosionAnimDuration);

            float randomDelay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(randomDelay);
        }

        Destroy(bossObject);
        AddScore(score);
    }

    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyDestroyed(enemy, 10);
        }
    }

    IEnumerator StartBossMusic()
    {
        float startTime = Time.time;

        while (Time.time < startTime + fadeTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(1f, 0f, (Time.time - startTime) / fadeTime);
            yield return null;
        }

        backgroundMusicSource.clip = bossMusic;
        backgroundMusicSource.Play();

        startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(0f, 1f, (Time.time - startTime) / fadeTime);
            yield return null;
        }
    }
    IEnumerator StartBackgroundMusic()
    {
        float startTime = Time.time;

        while (Time.time < startTime + fadeTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(1f, 0f, (Time.time - startTime) / fadeTime);
            yield return null;
        }

        backgroundMusicSource.clip = originalBackgroundMusic;
        backgroundMusicSource.Play();

        startTime = Time.time;
        while (Time.time < startTime + fadeTime)
        {
            backgroundMusicSource.volume = Mathf.Lerp(0f, 1f, (Time.time - startTime) / fadeTime);
            yield return null;
        }
    }

    public void GameOver()
    {
        if (currentLives <= 0)
        {
            Debug.Log("Game Over");

            Time.timeScale = 0f;

            gameUI.gameObject.SetActive(false);
            gameOverUI.gameObject.SetActive(true);
            backgroundMusicSource.Stop();
            gameOverSoundSource.Play();

            DisplayFinalScore();
            CheckForHighScore();
        }
    }

    public void PauseGame()
    {
        gameUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(true);
        backgroundMusicSource.Pause();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        gameUI.gameObject.SetActive(true);
        pauseUI.gameObject.SetActive(false);
        backgroundMusicSource.Play();
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }

    public void DisplayFinalScore()
    {
        finalScoreText.text = currentScore.ToString();
    }

    void UpdateHighScoreDisplay()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        highScoreText.text = "HIGH SCORE: " + highScore;
    }
    public void CheckForHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            highScoreAchievedText.gameObject.SetActive(true);
        }
    }
}
