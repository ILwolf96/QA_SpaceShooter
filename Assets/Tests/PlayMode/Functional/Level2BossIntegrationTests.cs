using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Level2BossIntegrationTests
{
    private GameObject flowObject;
    private GameObject level2Object;

    private LevelFlowController levelFlow;
    private Level2Controller level2;

    private GameObject[] wavePrefabs;

    private GameObject shieldedEnemyPrefab;
    private GameObject bossPrefab;

    private readonly List<GameObject> spawnedTestObjects =
        new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        flowObject =
            new GameObject("Test_LevelFlowController");

        levelFlow =
            flowObject.AddComponent<LevelFlowController>();

        level2Object =
            new GameObject("Test_Level2Controller");

        level2 =
            level2Object.AddComponent<Level2Controller>();

        CreateWavePool();

        // ------------------------------------------------------------
        // Shielded enemy prefab
        // ------------------------------------------------------------

        shieldedEnemyPrefab =
            new GameObject("Test_ShieldedEnemy");

        Enemy shieldedEnemy =
            shieldedEnemyPrefab.AddComponent<Enemy>();

        shieldedEnemy.health = 20;
        shieldedEnemy.shotChance = 0;
        shieldedEnemy.shotTimeMin = 999f;
        shieldedEnemy.shotTimeMax = 999f;

        shieldedEnemyPrefab.AddComponent<FollowThePath>();

        EnemyShield shield =
            shieldedEnemyPrefab.AddComponent<EnemyShield>();

        shield.shieldHealth = 50;

        shieldedEnemyPrefab.SetActive(false);

        spawnedTestObjects.Add(
            shieldedEnemyPrefab);

        // ------------------------------------------------------------
        // Boss prefab
        // ------------------------------------------------------------

        bossPrefab =
            new GameObject("Test_Boss");

        Boss boss =
            bossPrefab.AddComponent<Boss>();

        boss.maxHealth = 5000;
        boss.ResetHealth();

        boss.movementSpeed = 2.5f;
        boss.movementDirection = Vector2.right;

        boss.minX = -10f;
        boss.maxX = 10f;
        boss.minY = -5f;
        boss.maxY = 5f;

        bossPrefab.SetActive(false);

        spawnedTestObjects.Add(
            bossPrefab);

        // ------------------------------------------------------------
        // Level 2 configuration
        // ------------------------------------------------------------

        level2.wavePool =
            wavePrefabs;

        level2.difficultyMultiplier = 1.5f;
        level2.additionalShotChance = 0;
        level2.useShieldedEnemies = true;

        level2.shieldedEnemyPrefab =
            shieldedEnemyPrefab;

        level2.bossPrefab =
            bossPrefab;

        level2.deterministicTestMode = true;
        level2.testWaveIndex = 0;
    }

    [TearDown]
    public void TearDown()
    {
        if (level2 != null)
            level2.StopLevel();

        if (levelFlow != null)
            Object.DestroyImmediate(
                levelFlow.gameObject);

        foreach (GameObject obj in spawnedTestObjects)
        {
            if (obj != null)
                Object.DestroyImmediate(obj);
        }

        spawnedTestObjects.Clear();

        wavePrefabs = null;
        shieldedEnemyPrefab = null;
        bossPrefab = null;

        level2 = null;
        levelFlow = null;
        level2Object = null;
        flowObject = null;
    }

    // ================================================================
    // BOS-FT-001
    // ================================================================

    [UnityTest]
    public IEnumerator BOS_FT_001_BossCanBeEncounteredInLevel2()
    {
        level2.numberOfWaves = 0;

        level2.StartLevel();

        yield return null;

        Boss bossComponent =
            Object.FindFirstObjectByType<Boss>();

        Assert.IsNotNull(
            bossComponent,
            "Level 2 should spawn the Boss when the Level 2 wave sequence is complete.");

        Object.DestroyImmediate(
            bossComponent.gameObject);

        level2.StopLevel();
    }

    // ================================================================
    // BOS-FT-002
    // ================================================================

    [UnityTest]
    public IEnumerator BOS_FT_002_BossCanBeDefeated()
    {
        level2.numberOfWaves = 0;

        level2.StartLevel();

        yield return null;

        Boss boss =
            Object.FindFirstObjectByType<Boss>();

        Assert.IsNotNull(
            boss,
            "Level 2 should spawn a Boss.");

        int startingHealth =
            boss.health;

        boss.GetDamage(startingHealth);

        yield return null;

        Assert.IsTrue(
            boss == null,
            "Boss should be destroyed after receiving lethal damage.");

        level2.StopLevel();
    }

    // ================================================================
    // L2-FT-002
    // ================================================================

    [Test]
    public void L2_FT_002_Level1CanTransitionToLevel2()
    {
        levelFlow.CompleteLevel1();

        Assert.IsTrue(
            levelFlow.Level2Started,
            "Completing Level 1 should start Level 2.");

        Assert.AreEqual(
            2,
            levelFlow.CurrentLevel,
            "Current level should become Level 2.");
    }

    // ================================================================
    // L2-FT-003
    // ================================================================

    [Test]
    public void L2_FT_003_Level2UsesHarderSettings()
    {
        float baseDifficulty = 1f;

        bool harder =
            level2.IsHarderThanNormal(
                baseDifficulty);

        Assert.IsTrue(
            harder,
            "Level 2 should use harder settings than the normal baseline.");
    }

    // ================================================================
    // L2-FT-004
    // ================================================================

    [UnityTest]
    public IEnumerator L2_FT_004_ShieldedEnemiesActuallyAppearInLevel2()
    {
        level2.numberOfWaves = 1;
        level2.deterministicTestMode = true;
        level2.testWaveIndex = 0;
        level2.useShieldedEnemies = true;

        level2.StartLevel();

        yield return null;
        yield return null;

        EnemyShield[] shields =
            Object.FindObjectsByType<EnemyShield>(
                FindObjectsSortMode.None);

        Assert.Greater(
            shields.Length,
            0,
            "Level 2 should actually spawn at least one shielded enemy.");

        foreach (EnemyShield shield in shields)
        {
            Assert.IsNotNull(
                shield.GetComponent<Enemy>(),
                "A shielded Level 2 enemy must also contain Enemy.");
        }

        level2.StopLevel();

        foreach (EnemyShield shield in shields)
        {
            if (shield != null)
                Object.DestroyImmediate(
                    shield.gameObject);
        }
    }

    // ================================================================
    // L2-FT-005
    // ================================================================

    [UnityTest]
    public IEnumerator L2_FT_005_BossActuallyAppearsInLevel2()
    {
        level2.numberOfWaves = 0;

        level2.StartLevel();

        yield return null;

        Boss boss =
            Object.FindFirstObjectByType<Boss>();

        Assert.IsNotNull(
            boss,
            "Boss should actually appear in Level 2.");

        level2.StopLevel();

        if (boss != null)
            Object.DestroyImmediate(
                boss.gameObject);
    }

    // ================================================================
    // L2-FT-006
    // ================================================================

    [UnityTest]
    public IEnumerator L2_FT_006_BossDefeatIsRecognized()
    {
        bool completed = false;

        System.Action completedHandler =
            () => completed = true;

        level2.LevelCompleted +=
            completedHandler;

        level2.numberOfWaves = 0;

        level2.StartLevel();

        yield return null;

        Boss boss =
            Object.FindFirstObjectByType<Boss>();

        Assert.IsNotNull(
            boss,
            "Level 2 should spawn the Boss.");

        boss.GetDamage(
            boss.health);

        yield return null;
        yield return null;

        Assert.IsTrue(
            completed,
            "Level 2 should recognize Boss defeat.");

        level2.LevelCompleted -=
            completedHandler;

        level2.StopLevel();
    }

    // ================================================================
    // L2-FT-007
    // ================================================================

    [UnityTest]
    public IEnumerator L2_FT_007_Level2VictoryConditionWorks()
    {
        GameObject testFlowObject =
            new GameObject("Test_LevelFlowController_Victory");

        LevelFlowController testFlow =
            testFlowObject.AddComponent<LevelFlowController>();

        testFlow.level2Controller = level2;

        level2.numberOfWaves = 0;
        level2.StopLevel();

        // Allow LevelFlowController.Start() to run first.
        yield return null;

        // Now start Level 2 after initialization has completed.
        testFlow.StartLevel2();

        Boss boss = null;

        for (int i = 0; i < 10 && boss == null; i++)
        {
            yield return null;

            boss =
                Object.FindFirstObjectByType<Boss>();
        }

        Assert.IsNotNull(
            boss,
            "Level 2 should spawn the Boss.");

        boss.GetDamage(boss.health);

        for (int i = 0; i < 10 && !level2.IsCompleted; i++)
        {
            yield return null;
        }

        Assert.IsTrue(
            level2.IsCompleted,
            "Level 2 should complete after its Boss is defeated.");

        testFlow.CompleteLevel2();

        Assert.IsTrue(
            testFlow.GameCompleted,
            "Game should be completed after Level 2 victory.");

        level2.StopLevel();

        Object.DestroyImmediate(testFlowObject);
    }

    // ================================================================
    // L2-IT-001
    // ================================================================

    [UnityTest]
    public IEnumerator L2_IT_001_Level2ControllerWaveSpawnsShieldedEnemy()
    {
        level2.numberOfWaves = 1;
        level2.deterministicTestMode = true;
        level2.testWaveIndex = 0;
        level2.useShieldedEnemies = true;

        level2.StartLevel();

        yield return null;
        yield return null;

        EnemyShield shield =
            Object.FindFirstObjectByType<EnemyShield>();

        Assert.IsNotNull(
            shield,
            "Wave spawned by Level2Controller should create an EnemyShield.");

        Enemy enemy =
            shield.GetComponent<Enemy>();

        Assert.IsNotNull(
            enemy,
            "Shielded enemy spawned through Wave must retain Enemy.");

        level2.StopLevel();

        if (shield != null)
            Object.DestroyImmediate(
                shield.gameObject);
    }

    // ================================================================
    // L2-IT-002
    // ================================================================

    [Test]
    public void L2_IT_002_Level2CanProvideBossForLevelIntegration()
    {
        GameObject configuredBoss =
            level2.GetBossPrefab();

        Assert.IsNotNull(
            configuredBoss);

        Boss boss =
            configuredBoss.GetComponent<Boss>();

        Assert.IsNotNull(
            boss,
            "Level 2 must expose a Boss that can be integrated with the level flow.");
    }

    // ================================================================
    // L2-UT-001
    // ================================================================

    [Test]
    public void L2_UT_001_Level2SelectsWaveFromConfiguredWavePool()
    {
        Assert.IsNotNull(
            level2.wavePool,
            "Level 2 wave pool must be configured.");

        Assert.GreaterOrEqual(
            level2.wavePool.Length,
            1,
            "Level 2 must contain at least one selectable Wave prefab.");

        const int selectionCount = 100;

        for (int i = 0;
             i < selectionCount;
             i++)
        {
            GameObject selected =
                level2.SelectRandomWave();

            Assert.IsNotNull(
                selected,
                "Random wave selection returned null.");

            bool belongsToPool = false;

            foreach (GameObject wave in level2.wavePool)
            {
                if (selected == wave)
                {
                    belongsToPool = true;
                    break;
                }
            }

            Assert.IsTrue(
                belongsToPool,
                "Selected wave must come from the configured Level 2 wave pool.");
        }
    }

    // ================================================================
    // L2-UT-002
    // ================================================================

    [Test]
    public void L2_UT_002_DeterministicTestMode_SelectsConfiguredWave()
    {
        Assert.Greater(
            level2.wavePool.Length,
            0,
            "Wave pool must contain at least one Wave.");

        level2.deterministicTestMode = true;
        level2.testWaveIndex = 0;

        GameObject expected =
            level2.wavePool[0];

        GameObject selected =
            level2.SelectWaveForTest();

        Assert.AreSame(
            expected,
            selected,
            "Deterministic test mode should select the configured test wave.");
    }

    // ================================================================
    // Test Fixture Helpers
    // ================================================================

    private void CreateWavePool()
    {
        wavePrefabs =
            new GameObject[6];

        for (int i = 0;
             i < wavePrefabs.Length;
             i++)
        {
            // --------------------------------------------------------
            // Enemy prefab
            // --------------------------------------------------------

            GameObject enemyPrefab =
                new GameObject(
                    $"Test_Level2Enemy_{i}");

            Enemy enemy =
                enemyPrefab.AddComponent<Enemy>();

            enemy.health = 20;
            enemy.shotChance = 0;
            enemy.shotTimeMin = 999f;
            enemy.shotTimeMax = 999f;

            enemyPrefab.AddComponent<FollowThePath>();

            enemyPrefab.SetActive(false);

            spawnedTestObjects.Add(
                enemyPrefab);

            // --------------------------------------------------------
            // Wave prefab
            // --------------------------------------------------------

            GameObject waveObject =
                new GameObject(
                    $"Test_Level2Wave_{i + 1}");

            Wave wave =
                waveObject.AddComponent<Wave>();

            wave.enemy =
                enemyPrefab;

            wave.count = 1;
            wave.speed = 10f + i;
            wave.timeBetween = 0.01f;

            wave.rotationByPath = false;
            wave.Loop = false;
            wave.testMode = false;

            wave.shooting =
                new Shooting
                {
                    shotChance = 0,
                    shotTimeMin = 999f,
                    shotTimeMax = 999f
                };

            // --------------------------------------------------------
            // Valid path points
            // --------------------------------------------------------

            GameObject point1 =
                new GameObject(
                    $"Test_Wave_{i + 1}_Point1");

            GameObject point2 =
                new GameObject(
                    $"Test_Wave_{i + 1}_Point2");

            GameObject point3 =
                new GameObject(
                    $"Test_Wave_{i + 1}_Point3");

            GameObject point4 =
                new GameObject(
                    $"Test_Wave_{i + 1}_Point4");

            point1.transform.position =
                new Vector3(0f, 5f, 0f);

            point2.transform.position =
                new Vector3(2f, 2f, 0f);

            point3.transform.position =
                new Vector3(-2f, -2f, 0f);

            point4.transform.position =
                new Vector3(0f, -5f, 0f);

            wave.pathPoints =
                new[]
                {
                    point1.transform,
                    point2.transform,
                    point3.transform,
                    point4.transform
                };

            waveObject.SetActive(false);

            wavePrefabs[i] =
                waveObject;

            spawnedTestObjects.Add(
                waveObject);

            spawnedTestObjects.Add(
                point1);

            spawnedTestObjects.Add(
                point2);

            spawnedTestObjects.Add(
                point3);

            spawnedTestObjects.Add(
                point4);
        }
    }
}