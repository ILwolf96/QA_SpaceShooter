using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StressTests
{
    private readonly List<GameObject> spawnedObjects = new();

    private GameObject projectilePrefab;
    private GameObject enemyPrefab;

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
                Object.Destroy(obj);
        }

        spawnedObjects.Clear();

        if (projectilePrefab != null)
            Object.Destroy(projectilePrefab);

        if (enemyPrefab != null)
            Object.Destroy(enemyPrefab);

        yield return null;
    }

    [UnityTest]
    public IEnumerator ExtremeProjectileCount_RemainsStable()
    {
        const int projectileCount = 1000;

        projectilePrefab =
            new GameObject("StressProjectile");

        Projectile projectile =
            projectilePrefab.AddComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject instance =
                Object.Instantiate(
                    projectilePrefab,
                    new Vector3(
                        i % 50,
                        (i / 50) % 20,
                        0f),
                    Quaternion.identity);

            spawnedObjects.Add(instance);
        }

        yield return null;

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });

        Assert.AreEqual(
            projectileCount,
            CountObjectsWithPrefix(
                "StressProjectile(Clone)"));
    }

    [UnityTest]
    public IEnumerator ExtremeProjectileGeneration_DoesNotCrash()
    {
        const int iterations = 100;
        const int projectilesPerIteration = 20;

        GameObject prefab =
            new GameObject("ExtremeShootingProjectile");

        Projectile projectile =
            prefab.AddComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        spawnedObjects.Add(prefab);

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < projectilesPerIteration; j++)
            {
                GameObject instance =
                    Object.Instantiate(
                        prefab,
                        new Vector3(j, i, 0f),
                        Quaternion.identity);

                spawnedObjects.Add(instance);
            }

            if (i % 10 == 0)
                yield return null;
        }

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });

        Assert.Greater(
            CountObjectsWithPrefix(
                "ExtremeShootingProjectile(Clone)"),
            0,
            "Extreme projectile generation should leave the runtime in a valid state.");
    }

    [UnityTest]
    public IEnumerator ExtremeWaveGeneration_RemainsStable()
    {
        const int waveCount = 10;
        const int enemiesPerWave = 25;

        CreateEnemyPrefab();

        for (int i = 0; i < waveCount; i++)
        {
            CreateStressWave(
                $"StressWave_{i}",
                enemiesPerWave);
        }

        yield return new WaitForSeconds(2f);

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });

        int objectCount =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None).Length;

        Assert.Greater(
            objectCount,
            0);

        Assert.Less(
            objectCount,
            5000,
            "Stress configuration should not produce uncontrolled object growth.");
    }

    [UnityTest]
    public IEnumerator HeavyLevelConfiguration_RemainsStable()
    {
        const int waveCount = 20;
        const int planetCount = 20;

        GameObject controllerObject =
            new GameObject("Stress_LevelController");

        spawnedObjects.Add(controllerObject);

        LevelController controller =
            controllerObject.AddComponent<LevelController>();

        controller.powerUp =
            new GameObject("StressPowerUp");

        spawnedObjects.Add(controller.powerUp);

        controller.timeForNewPowerup = 9999f;
        controller.timeBetweenPlanets = 9999f;
        controller.planetsSpeed = 10f;

        controller.enemyWaves =
            new EnemyWaves[waveCount];

        for (int i = 0; i < waveCount; i++)
        {
            GameObject wave =
                new GameObject($"StressLevelWave_{i}");

            Wave waveComponent =
                wave.AddComponent<Wave>();

            waveComponent.count = 0;
            waveComponent.testMode = false;

            spawnedObjects.Add(wave);

            controller.enemyWaves[i] =
                new EnemyWaves
                {
                    timeToStart = 0f,
                    wave = wave
                };
        }

        controller.planets =
            new GameObject[planetCount];

        for (int i = 0; i < planetCount; i++)
        {
            GameObject planet =
                new GameObject($"StressPlanet_{i}");

            planet.AddComponent<DirectMoving>();

            controller.planets[i] = planet;
            spawnedObjects.Add(planet);
        }

        yield return new WaitForSeconds(0.5f);

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });

        Assert.AreEqual(
            waveCount,
            controller.enemyWaves.Length);

        Assert.AreEqual(
            planetCount,
            controller.planets.Length);
    }

    [UnityTest]
    public IEnumerator RepeatedObjectCreationAndDestruction_RemainsStable()
    {
        const int iterations = 500;

        for (int i = 0; i < iterations; i++)
        {
            GameObject temporary =
                new GameObject($"StressTemporary_{i}");

            spawnedObjects.Add(temporary);

            Object.Destroy(temporary);

            if (i % 50 == 0)
                yield return null;
        }

        yield return null;

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });
    }

    [UnityTest]
    public IEnumerator PoolingController_ExtremeRetrieval_RemainsStable()
    {
        const int initialPoolSize = 25;
        const int retrievalCount = 5000;

        GameObject prefab =
            new GameObject("StressPoolObject");

        GameObject controllerObject =
            new GameObject("StressPoolingController");

        PoolingController controller =
            controllerObject.AddComponent<PoolingController>();

        controller.poolingObjectsClass =
            new[]
            {
                new PoolingObjects
                {
                    pooledPrefab = prefab,
                    count = initialPoolSize
                }
            };

        spawnedObjects.Add(controllerObject);
        spawnedObjects.Add(prefab);

        yield return null;

        for (int i = 0; i < retrievalCount; i++)
        {
            GameObject obj =
                controller.GetPoolingObject(prefab);

            Assert.IsNotNull(
                obj,
                $"Pool returned null at retrieval {i}.");

            obj.SetActive(false);

            if (i % 500 == 0)
                yield return null;
        }

        Assert.GreaterOrEqual(
            controllerObject.transform.childCount,
            initialPoolSize,
            "Pool should retain at least its configured initial capacity.");
    }

    [UnityTest]
    public IEnumerator L2_SRT_001_HighDifficultyLevel2_RemainsStable()
    {
        GameObject controllerObject =
            new GameObject("Level2StressController");

        Level2Controller controller =
            controllerObject.AddComponent<Level2Controller>();

        GameObject shieldedEnemyPrefab =
            new GameObject("Level2StressShieldedEnemy");

        Enemy enemy =
            shieldedEnemyPrefab.AddComponent<Enemy>();

        enemy.health = 50;
        enemy.shotChance = 0;
        enemy.shotTimeMin = 999f;
        enemy.shotTimeMax = 999f;

        shieldedEnemyPrefab.AddComponent<FollowThePath>();

        EnemyShield shield =
            shieldedEnemyPrefab.AddComponent<EnemyShield>();

        shield.shieldHealth = 100;

        GameObject[] waves =
            new GameObject[6];

        for (int i = 0; i < waves.Length; i++)
        {
            GameObject waveObject =
                new GameObject($"Level2StressWave_{i}");

            Wave wave =
                waveObject.AddComponent<Wave>();

            wave.enemy =
                shieldedEnemyPrefab;

            wave.count =
                20;

            wave.speed =
                20f;

            wave.timeBetween =
                0.01f;

            wave.Loop = false;
            wave.testMode = false;

            wave.shooting =
                new Shooting
                {
                    shotChance = 0,
                    shotTimeMin = 999f,
                    shotTimeMax = 999f
                };

            GameObject[] points =
                new GameObject[4];

            for (int p = 0; p < points.Length; p++)
            {
                points[p] =
                    new GameObject(
                        $"Level2StressWave_{i}_Point_{p}");
            }

            wave.pathPoints =
                new Transform[]
                {
                    points[0].transform,
                    points[1].transform,
                    points[2].transform,
                    points[3].transform
                };

            waves[i] = waveObject;
        }

        controller.wavePool = waves;
        controller.numberOfWaves = 6;
        controller.difficultyMultiplier = 3f;
        controller.additionalShotChance = 30;
        controller.shieldedEnemyPrefab =
            shieldedEnemyPrefab;

        yield return null;

        controller.StartLevel();

        yield return new WaitForSeconds(2f);

        Assert.DoesNotThrow(
            () =>
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None));

        Assert.IsTrue(
            controller.IsRunning ||
            controller.IsCompleted,
            "High-difficulty Level 2 should remain stable under stress.");

        Object.DestroyImmediate(controllerObject);

        foreach (GameObject wave in waves)
            Object.DestroyImmediate(wave);

        Object.DestroyImmediate(shieldedEnemyPrefab);
    }

    [UnityTest]
    public IEnumerator BOS_SRT_001_BossRemainsStableUnderSustainedAttacks()
    {
        GameObject bossObject =
            new GameObject("BossStressTest");

        Boss boss =
            bossObject.AddComponent<Boss>();

        boss.maxHealth = 100000;

        // Boss.Awake() already ran when the component was added,
        // therefore explicitly synchronize health with the new max HP.
        boss.ResetHealth();

        boss.movementSpeed = 2f;
        boss.movementDirection = Vector2.right;

        boss.minX = -10f;
        boss.maxX = 10f;
        boss.minY = -5f;
        boss.maxY = 5f;

        yield return null;

        const int attackCount = 10000;
        const int damagePerAttack = 1;

        for (int i = 0;
             i < attackCount;
             i++)
        {
            Assert.IsNotNull(
                boss,
                $"Boss should still exist at attack {i}.");

            boss.GetDamage(
                damagePerAttack);

            Assert.GreaterOrEqual(
                boss.health,
                0,
                $"Boss health became negative at attack {i}.");

            Assert.LessOrEqual(
                boss.health,
                boss.maxHealth,
                $"Boss health exceeded maximum at attack {i}.");

            if (i % 500 == 0)
            {
                Vector3 positionBeforeYield =
                    boss.transform.position;

                yield return null;

                Assert.IsNotNull(
                    boss,
                    "Boss should remain alive during the sustained attack workload.");

                Assert.AreNotEqual(
                    positionBeforeYield,
                    boss.transform.position,
                    "Boss should continue moving during sustained attacks.");
            }
        }

        Assert.IsNotNull(
            boss,
            "Boss should remain alive throughout the stress workload.");

        Assert.Greater(
            boss.health,
            0,
            "Boss should survive the sustained attack workload.");

        Object.DestroyImmediate(
            bossObject);
    }
    private void CreateEnemyPrefab()
    {
        enemyPrefab =
            new GameObject("StressEnemy");

        enemyPrefab.tag = "Enemy";

        Enemy enemy =
            enemyPrefab.AddComponent<Enemy>();

        enemy.health = 10;

        enemy.destructionVFX =
            new GameObject("StressEnemyVFX");

        enemy.hitEffect =
            new GameObject("StressHitEffect");

        enemy.Projectile =
            new GameObject("StressEnemyProjectile");

        enemyPrefab.AddComponent<FollowThePath>();
    }

    private void CreateStressWave(
        string name,
        int enemyCount)
    {
        GameObject waveObject =
            new GameObject(name);

        spawnedObjects.Add(waveObject);

        Wave wave =
            waveObject.AddComponent<Wave>();

        wave.enemy = enemyPrefab;
        wave.count = enemyCount;
        wave.speed = 20f;
        wave.timeBetween = 0f;
        wave.rotationByPath = false;
        wave.Loop = true;
        wave.testMode = false;

        wave.shooting =
            new Shooting
            {
                shotChance = 0,
                shotTimeMin = 1f,
                shotTimeMax = 1f
            };

        wave.pathPoints =
            new Transform[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject point =
                new GameObject($"{name}_Point_{i}");

            point.transform.position =
                new Vector3(i * 2f, i, 0f);

            spawnedObjects.Add(point);

            wave.pathPoints[i] = point.transform;
        }
    }

    private static int CountObjectsWithPrefix(string prefix)
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        int count = 0;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith(prefix))
                count++;
        }

        return count;
    }
}