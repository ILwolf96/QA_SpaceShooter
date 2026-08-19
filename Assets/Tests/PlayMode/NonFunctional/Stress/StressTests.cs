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