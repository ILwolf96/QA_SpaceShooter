using System.Collections;
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

        shieldedEnemyPrefab =
            new GameObject("Test_ShieldedEnemy");

        shieldedEnemyPrefab.AddComponent<Enemy>();
        shieldedEnemyPrefab.AddComponent<EnemyShield>();

        bossPrefab =
            new GameObject("Test_Boss");

        Boss boss =
            bossPrefab.AddComponent<Boss>();

        boss.maxHealth = 5000;

        level2.wavePool = wavePrefabs;
        level2.difficultyMultiplier = 1.5f;
        level2.shieldedEnemyPrefab = shieldedEnemyPrefab;
        level2.bossPrefab = bossPrefab;
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(flowObject);
        DestroyIfExists(level2Object);

        if (wavePrefabs != null)
        {
            foreach (GameObject wave in wavePrefabs)
                DestroyIfExists(wave);
        }

        DestroyIfExists(shieldedEnemyPrefab);
        DestroyIfExists(bossPrefab);
    }

    // ================================================================
    // BOS-FT-001
    // ================================================================

    [Test]
    public void BOS_FT_001_BossCanBeEncounteredInLevel2()
    {
        GameObject selectedBoss =
            level2.GetBossPrefab();

        Assert.IsNotNull(
            selectedBoss,
            "Level 2 should provide a Boss prefab for the Boss encounter.");
    }

    // ================================================================
    // BOS-FT-002
    // ================================================================

    [Test]
    public void BOS_FT_002_BossCanBeDefeated()
    {
        GameObject bossObject =
            Object.Instantiate(level2.GetBossPrefab());

        Boss boss =
            bossObject.GetComponent<Boss>();

        Assert.IsNotNull(
            boss,
            "The Level 2 Boss prefab must contain Boss.");

        boss.maxHealth = 500;
        bossObject.SetActive(false);
        bossObject.SetActive(true);

        boss.GetDamage(boss.health);

        Assert.AreEqual(
            0,
            boss.health,
            "Boss health should reach zero after lethal damage.");

        Assert.IsFalse(
            boss.IsAlive,
            "Defeated Boss should no longer report itself as alive.");

        Object.DestroyImmediate(bossObject);
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
            level2.IsHarderThanNormal(baseDifficulty);

        Assert.IsTrue(
            harder,
            "Level 2 should use harder settings than the normal baseline.");
    }

    // ================================================================
    // L2-FT-004
    // ================================================================

    [Test]
    public void L2_FT_004_ShieldedEnemiesAreConfiguredForLevel2()
    {
        GameObject shieldedEnemy =
            level2.GetShieldedEnemyPrefab();

        Assert.IsNotNull(
            shieldedEnemy,
            "Level 2 must have a shielded enemy prefab.");

        Assert.IsNotNull(
            shieldedEnemy.GetComponent<EnemyShield>(),
            "The Level 2 shielded enemy must contain EnemyShield.");
    }

    // ================================================================
    // L2-FT-005
    // ================================================================

    [Test]
    public void L2_FT_005_BossIsConfiguredForLevel2()
    {
        GameObject configuredBoss =
            level2.GetBossPrefab();

        Assert.IsNotNull(
            configuredBoss,
            "Level 2 must contain a configured Boss prefab.");

        Assert.IsNotNull(
            configuredBoss.GetComponent<Boss>(),
            "The configured Level 2 Boss must contain Boss.");
    }

    // ================================================================
    // L2-FT-006
    // ================================================================

    [Test]
    public void L2_FT_006_BossDefeatCanBeRecognized()
    {
        GameObject bossObject =
            Object.Instantiate(level2.GetBossPrefab());

        Boss boss =
            bossObject.GetComponent<Boss>();

        boss.maxHealth = 100;

        bossObject.SetActive(false);
        bossObject.SetActive(true);

        boss.GetDamage(boss.health);

        Assert.IsFalse(
            boss.IsAlive,
            "Level 2 should be able to recognize that the Boss has been defeated.");

        Object.DestroyImmediate(bossObject);
    }

    // ================================================================
    // L2-FT-007
    // ================================================================

    [Test]
    public void L2_FT_007_Level2VictoryConditionCanBeTriggered()
    {
        levelFlow.CompleteLevel1();

        Assert.IsTrue(
            levelFlow.Level2Started,
            "Level 2 must be active before testing its victory condition.");

        levelFlow.CompleteLevel2();

        Assert.IsTrue(
            levelFlow.Level2Completed,
            "Level 2 should report completion after its Boss encounter.");

        Assert.IsTrue(
            levelFlow.GameCompleted,
            "Completing Level 2 should complete the game.");
    }

    // ================================================================
    // L2-IT-001
    // ================================================================

    [Test]
    public void L2_IT_001_Level2CanProvideShieldedEnemyForWaveIntegration()
    {
        GameObject shieldedEnemy =
            level2.GetShieldedEnemyPrefab();

        Assert.IsNotNull(shieldedEnemy);

        Enemy enemy =
            shieldedEnemy.GetComponent<Enemy>();

        EnemyShield shield =
            shieldedEnemy.GetComponent<EnemyShield>();

        Assert.IsNotNull(
            enemy,
            "Shielded Level 2 enemies must still use Enemy.");

        Assert.IsNotNull(
            shield,
            "Shielded Level 2 enemies must use EnemyShield.");
    }

    // ================================================================
    // L2-IT-002
    // ================================================================

    [Test]
    public void L2_IT_002_Level2CanProvideBossForLevelIntegration()
    {
        GameObject configuredBoss =
            level2.GetBossPrefab();

        Assert.IsNotNull(configuredBoss);

        Boss boss =
            configuredBoss.GetComponent<Boss>();

        Assert.IsNotNull(
            boss,
            "Level 2 must expose a Boss that can be integrated with the level flow.");
    }

    // ================================================================
    // Helpers
    // ================================================================

    private void CreateWavePool()
    {
        wavePrefabs = new GameObject[6];

        for (int i = 0; i < wavePrefabs.Length; i++)
        {
            GameObject waveObject =
                new GameObject($"Test_Level2Wave_{i + 1}");

            Wave wave =
                waveObject.AddComponent<Wave>();

            wave.count = 1;
            wave.speed = 10f + i;
            wave.timeBetween = 1f;

            wavePrefabs[i] = waveObject;
        }
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}