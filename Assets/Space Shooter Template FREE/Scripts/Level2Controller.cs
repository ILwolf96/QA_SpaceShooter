using System;
using System.Collections;
using UnityEngine;

public class Level2Controller : MonoBehaviour
{
    [Header("Wave Pool")]
    [Tooltip("Existing Wave prefabs that Level 2 can randomly select.")]
    public GameObject[] wavePool;

    [Header("Wave Settings")]
    [Tooltip("Number of waves to run before the Boss.")]
    [Min(1)]
    public int numberOfWaves = 6;

    [Tooltip("Multiplier applied to enemy count and speed.")]
    [Min(1f)]
    public float difficultyMultiplier = 1.5f;

    [Tooltip("Additional shooting chance added to Level 2 waves.")]
    [Range(0, 100)]
    public int additionalShotChance = 10;

    [Tooltip("Use ShieldedEnemy for Level 2 waves.")]
    public bool useShieldedEnemies = true;

    [Header("Shielded Enemy")]
    public GameObject shieldedEnemyPrefab;

    [Header("Boss")]
    public GameObject bossPrefab;

    public Transform bossSpawnPoint;

    [Header("Testing")]
    [Tooltip("When enabled, Level 2 uses testWaveIndex instead of random selection.")]
    public bool deterministicTestMode;

    [Tooltip("Wave index used when deterministic test mode is enabled.")]
    [Min(0)]
    public int testWaveIndex;

    private Coroutine levelRoutine;
    private GameObject activeWave;
    private GameObject activeBoss;

    private bool levelRunning;

    public bool IsRunning =>
        levelRunning;

    public bool IsCompleted { get; private set; }

    public event Action LevelCompleted;

    public event Action BossSpawned;

    public void StartLevel()
    {
        StopLevel();

        IsCompleted = false;
        levelRunning = true;

        levelRoutine =
            StartCoroutine(
                RunLevel2());
    }

    public void StopLevel()
    {
        levelRunning = false;

        if (levelRoutine != null)
        {
            StopCoroutine(levelRoutine);
            levelRoutine = null;
        }

        activeWave = null;
    }

    private IEnumerator RunLevel2()
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            if (!levelRunning)
                yield break;

            yield return StartCoroutine(
                SpawnLevel2Wave());
        }

        if (!levelRunning)
            yield break;

        yield return StartCoroutine(
            SpawnAndWaitForBoss());

        if (!levelRunning)
            yield break;

        IsCompleted = true;
        levelRunning = false;

        LevelCompleted?.Invoke();
    }

    private IEnumerator SpawnLevel2Wave()
    {
        GameObject wavePrefab =
            SelectWave();

        if (wavePrefab == null)
        {
            Debug.LogWarning(
                "Level2Controller: No valid Wave prefab available.");

            yield break;
        }

        activeWave =
            Instantiate(
                wavePrefab,
                transform.position,
                Quaternion.identity);

        Wave wave =
            activeWave.GetComponent<Wave>();

        if (wave == null)
        {
            Debug.LogWarning(
                "Level2Controller: Selected prefab does not contain Wave.");

            Destroy(activeWave);
            activeWave = null;

            yield break;
        }

        ConfigureWaveForLevel2(wave);

        // Wave destroys itself when Loop is false and all configured
        // enemies have been spawned.
        while (activeWave != null)
        {
            yield return null;
        }
    }

    private void ConfigureWaveForLevel2(Wave wave)
    {
        wave.Loop = false;
        wave.testMode = false;

        if (useShieldedEnemies &&
            shieldedEnemyPrefab != null)
        {
            wave.enemy =
                shieldedEnemyPrefab;
        }

        wave.count =
            Mathf.Max(
                1,
                Mathf.CeilToInt(
                    wave.count *
                    difficultyMultiplier));

        wave.speed *=
            difficultyMultiplier;

        if (difficultyMultiplier > 0f)
        {
            wave.timeBetween =
                Mathf.Max(
                    0.05f,
                    wave.timeBetween /
                    difficultyMultiplier);
        }

        wave.shooting.shotChance =
            Mathf.Clamp(
                wave.shooting.shotChance +
                additionalShotChance,
                0,
                100);
    }

    private GameObject SelectWave()
    {
        if (wavePool == null ||
            wavePool.Length == 0)
            return null;

        if (deterministicTestMode)
        {
            int index =
                Mathf.Clamp(
                    testWaveIndex,
                    0,
                    wavePool.Length - 1);

            return wavePool[index];
        }

        return wavePool[
            UnityEngine.Random.Range(
                0,
                wavePool.Length)];
    }

    public GameObject SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning(
                "Level2Controller: Boss prefab is not assigned.");

            return null;
        }

        if (activeBoss != null)
            return activeBoss;

        Vector3 spawnPosition =
            bossSpawnPoint != null
                ? bossSpawnPoint.position
                : transform.position;

        activeBoss =
            Instantiate(
                bossPrefab,
                spawnPosition,
                Quaternion.identity);

        BossSpawned?.Invoke();

        StartCoroutine(
            WaitForBossDefeat(
                activeBoss));

        return activeBoss;
    }

    private IEnumerator SpawnAndWaitForBoss()
    {
        SpawnBoss();

        while (activeBoss != null)
            yield return null;
    }

    private IEnumerator WaitForBossDefeat(
        GameObject bossObject)
    {
        while (bossObject != null)
            yield return null;

        activeBoss = null;
    }

    public GameObject GetBossPrefab()
    {
        return bossPrefab;
    }

    public GameObject GetShieldedEnemyPrefab()
    {
        return shieldedEnemyPrefab;
    }

    public bool IsHarderThanNormal(
        float baseValue)
    {
        return
            difficultyMultiplier *
            baseValue >
            baseValue;
    }

    public GameObject SelectRandomWave()
    {
        if (wavePool == null ||
            wavePool.Length == 0)
            return null;

        return wavePool[
            UnityEngine.Random.Range(
                0,
                wavePool.Length)];
    }
}