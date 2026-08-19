using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WaveFunctionalTests
{
    private GameObject waveObject;
    private GameObject enemyPrefab;
    private GameObject projectilePrefab;
    private GameObject destructionVfx;
    private GameObject hitEffect;
    private GameObject[] pathObjects;

    [SetUp]
    public void SetUp()
    {
        destructionVfx =
            new GameObject("Test_Enemy_DestructionVFX");

        hitEffect =
            new GameObject("Test_Enemy_HitEffect");

        projectilePrefab =
            new GameObject("Test_Enemy_Projectile");

        Projectile projectile =
            projectilePrefab.AddComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = true;

        enemyPrefab = new GameObject("Test_Enemy");

        enemyPrefab.tag = "Enemy";

        Enemy enemy =
            enemyPrefab.AddComponent<Enemy>();

        enemy.health = 10;
        enemy.Projectile = projectilePrefab;
        enemy.destructionVFX = destructionVfx;
        enemy.hitEffect = hitEffect;

        enemyPrefab.AddComponent<FollowThePath>();

        pathObjects = new GameObject[4];

        for (int i = 0; i < pathObjects.Length; i++)
        {
            pathObjects[i] =
                new GameObject($"WavePathPoint_{i}");

            pathObjects[i].transform.position =
                new Vector3(i * 2f, i, 0f);
        }

        waveObject =
            new GameObject("Test_Wave");

        Wave wave =
            waveObject.AddComponent<Wave>();

        wave.enemy = enemyPrefab;
        wave.count = 1;
        wave.speed = 25f;
        wave.timeBetween = 0f;
        wave.rotationByPath = false;
        wave.Loop = true;
        wave.testMode = false;

        wave.shooting = new Shooting
        {
            shotChance = 75,
            shotTimeMin = 2f,
            shotTimeMax = 4f
        };

        wave.pathPoints = GetPathTransforms();
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(waveObject);
        DestroyIfExists(enemyPrefab);
        DestroyIfExists(projectilePrefab);
        DestroyIfExists(destructionVfx);
        DestroyIfExists(hitEffect);

        CleanupSpawnedEnemies();

        if (pathObjects != null)
        {
            foreach (GameObject pathPoint in pathObjects)
                DestroyIfExists(pathPoint);
        }
    }

    [UnityTest]
    public IEnumerator Wave_SpawnsConfiguredEnemyCount()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        wave.count = 1;

        yield return new WaitForSeconds(0.2f);

        int spawnedCount = CountSpawnedEnemies();

        Assert.AreEqual(
            1,
            spawnedCount,
            "Wave should spawn the configured number of enemies.");
    }

    [UnityTest]
    public IEnumerator Wave_AppliesSpeedToFollowThePath()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        wave.speed = 42f;

        yield return new WaitForSeconds(0.2f);

        FollowThePath follow =
            FindSpawnedEnemyFollowThePath();

        Assert.IsNotNull(
            follow,
            "Wave should create an enemy with FollowThePath.");

        Assert.AreEqual(
            42f,
            follow.speed);
    }

    [UnityTest]
    public IEnumerator Wave_AppliesRotationSettingToFollowThePath()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        wave.rotationByPath = true;

        yield return new WaitForSeconds(0.2f);

        FollowThePath follow =
            FindSpawnedEnemyFollowThePath();

        Assert.IsNotNull(follow);
        Assert.IsTrue(follow.rotationByPath);
    }

    [UnityTest]
    public IEnumerator Wave_AppliesLoopSettingToFollowThePath()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        wave.Loop = false;

        yield return new WaitForSeconds(0.2f);

        FollowThePath follow =
            FindSpawnedEnemyFollowThePath();

        Assert.IsNotNull(follow);
        Assert.IsFalse(follow.loop);
    }

    [UnityTest]
    public IEnumerator Wave_AppliesPathToFollowThePath()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        yield return new WaitForSeconds(0.2f);

        FollowThePath follow =
            FindSpawnedEnemyFollowThePath();

        Assert.IsNotNull(follow);
        Assert.IsNotNull(follow.path);

        Assert.AreEqual(
            wave.pathPoints.Length,
            follow.path.Length);
    }

    [UnityTest]
    public IEnumerator Wave_AppliesShootingConfigurationToEnemy()
    {
        Wave wave =
            waveObject.GetComponent<Wave>();

        wave.shooting.shotChance = 65;
        wave.shooting.shotTimeMin = 1.5f;
        wave.shooting.shotTimeMax = 3.5f;

        yield return new WaitForSeconds(0.2f);

        GameObject spawnedEnemy =
            FindSpawnedEnemy();

        Assert.IsNotNull(spawnedEnemy);

        Enemy enemy =
            spawnedEnemy.GetComponent<Enemy>();

        Assert.AreEqual(
            65,
            enemy.shotChance);

        Assert.AreEqual(
            1.5f,
            enemy.shotTimeMin,
            0.001f);

        Assert.AreEqual(
            3.5f,
            enemy.shotTimeMax,
            0.001f);
    }

    [UnityTest]
    public IEnumerator TestMode_RegeneratesWaveAfterThreeSeconds()
    {
        Wave wave = waveObject.GetComponent<Wave>();
        wave.testMode = true; // Enable test mode specifically for this test

        yield return new WaitForSeconds(0.25f);

        int firstWaveCount = CountSpawnedEnemies();

        Assert.AreEqual(
            1,
            firstWaveCount,
            "The initial test-mode wave should spawn one enemy.");

        yield return new WaitForSeconds(3.1f);

        int secondWaveCount = CountSpawnedEnemies();

        Assert.GreaterOrEqual(
            secondWaveCount,
            1,
            "Test mode should generate another wave after its three-second delay.");
    }

    private Transform[] GetPathTransforms()
    {
        Transform[] result =
            new Transform[pathObjects.Length];

        for (int i = 0; i < pathObjects.Length; i++)
            result[i] = pathObjects[i].transform;

        return result;
    }

    private GameObject FindSpawnedEnemy()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_Enemy(Clone)"))
                return obj;
        }

        return null;
    }

    private FollowThePath FindSpawnedEnemyFollowThePath()
    {
        GameObject enemy =
            FindSpawnedEnemy();

        if (enemy == null)
            return null;

        return enemy.GetComponent<FollowThePath>();
    }

    private int CountSpawnedEnemies()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        int count = 0;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_Enemy(Clone)"))
                count++;
        }

        return count;
    }

    private void CleanupSpawnedEnemies()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_Enemy(Clone)"))
                Object.DestroyImmediate(obj);
        }
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}