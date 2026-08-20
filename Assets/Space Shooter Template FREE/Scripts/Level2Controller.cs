using System;
using System.Collections;
using UnityEngine;

public class Level2Controller : MonoBehaviour
{
    [Header("Wave Pool")]
    [Tooltip("Wave prefabs available for Level 2.")]
    public GameObject[] wavePool;

    [Header("Wave Settings")]
    [Min(1)]
    public int numberOfWaves = 6;

    [Min(1f)]
    public float difficultyMultiplier = 1.5f;

    [Range(0, 100)]
    public int additionalShotChance = 10;

    public bool useShieldedEnemies = true;

    [Header("Shielded Enemy")]
    public GameObject shieldedEnemyPrefab;

    [Header("Boss")]
    public GameObject bossPrefab;

    public Transform bossSpawnPoint;

    [Header("Testing")]
    [Tooltip("Use testWaveIndex instead of random selection.")]
    public bool deterministicTestMode;

    [Min(0)]
    public int testWaveIndex;

    public bool IsRunning { get; private set; }

    public bool IsCompleted { get; private set; }

    public int CurrentWaveIndex { get; private set; }

    public event Action LevelCompleted;

    public event Action<GameObject> WaveStarted;

    public event Action<GameObject> BossSpawned;

    private Coroutine levelRoutine;

    private GameObject activeWave;

    private GameObject activeBoss;

    public void StartLevel()
    {
        StopLevel();

        IsRunning = true;
        IsCompleted = false;
        CurrentWaveIndex = 0;

        levelRoutine =
            StartCoroutine(
                RunLevel2());
    }

    public void StopLevel()
    {
        IsRunning = false;

        if (levelRoutine != null)
        {
            StopCoroutine(
                levelRoutine);

            levelRoutine = null;
        }

        if (activeWave != null)
        {
            Destroy(
                activeWave);

            activeWave = null;
        }

        if (activeBoss != null)
        {
            Destroy(
                activeBoss);

            activeBoss = null;
        }
    }

    private IEnumerator RunLevel2()
    {
        for (int i = 0;
             i < numberOfWaves;
             i++)
        {
            if (!IsRunning)
                yield break;

            CurrentWaveIndex = i;

            yield return StartCoroutine(
                RunSingleWave());
        }

        if (!IsRunning)
            yield break;

        yield return StartCoroutine(
            RunBossEncounter());

        if (!IsRunning)
            yield break;

        IsCompleted = true;
        IsRunning = false;

        LevelCompleted?.Invoke();
    }

    private IEnumerator RunSingleWave()
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

        activeWave.SetActive(true);

        Wave wave =
            activeWave.GetComponent<Wave>();

        if (wave == null)
        {
            Debug.LogWarning(
                "Level2Controller: Selected prefab does not contain Wave.");

            Destroy(
                activeWave);

            activeWave = null;

            yield break;
        }

        ConfigureWaveForLevel2(
            wave);

        WaveStarted?.Invoke(
            activeWave);

        bool completed = false;

        void HandleWaveCompleted(
            Wave completedWave)
        {
            if (completedWave == wave)
                completed = true;
        }

        wave.WaveCompleted +=
            HandleWaveCompleted;

        while (IsRunning && !completed)
            yield return null;

        wave.WaveCompleted -=
            HandleWaveCompleted;

        activeWave = null;
    }

    private void ConfigureWaveForLevel2(
        Wave wave)
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

        wave.timeBetween =
            Mathf.Max(
                0.05f,
                wave.timeBetween /
                difficultyMultiplier);

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
        {
            return null;
        }

        if (deterministicTestMode)
        {
            return SelectWaveForTest();
        }

        return wavePool[
            UnityEngine.Random.Range(
                0,
                wavePool.Length)];
    }

    /// <summary>
    /// Explicit deterministic wave selection for automated tests.
    /// </summary>
    public GameObject SelectWaveForTest()
    {
        if (wavePool == null ||
            wavePool.Length == 0)
        {
            return null;
        }

        int index =
            Mathf.Clamp(
                testWaveIndex,
                0,
                wavePool.Length - 1);

        return wavePool[index];
    }

    private IEnumerator RunBossEncounter()
    {
        activeBoss = SpawnBoss();

        if (activeBoss == null)
            yield break;

        bool bossDefeated = false;

        Boss boss =
            activeBoss.GetComponent<Boss>();

        if (boss == null)
            yield break;

        System.Action bossDefeatedHandler =
            () => bossDefeated = true;

        boss.BossDefeated += bossDefeatedHandler;

        while (IsRunning && !bossDefeated)
            yield return null;

        boss.BossDefeated -= bossDefeatedHandler;

        activeBoss = null;
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

        if (!activeBoss.activeSelf)
            activeBoss.SetActive(true);

        Boss boss =
            activeBoss.GetComponent<Boss>();

        if (boss == null)
        {
            Debug.LogError(
                "Level2Controller: Boss prefab does not contain Boss.");

            Destroy(activeBoss);
            activeBoss = null;

            return null;
        }

        BossSpawned?.Invoke(activeBoss);

        return activeBoss;
    }

    public GameObject SelectRandomWave()
    {
        if (wavePool == null ||
            wavePool.Length == 0)
        {
            return null;
        }

        return wavePool[
            UnityEngine.Random.Range(
                0,
                wavePool.Length)];
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
}