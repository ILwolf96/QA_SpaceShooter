using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LoadTests
{
    private readonly List<GameObject> spawnedObjects = new();

    private GameObject projectilePrefab;
    private GameObject enemyPrefab;
    private GameObject destructionVfx;
    private GameObject hitEffect;

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

        if (destructionVfx != null)
            Object.Destroy(destructionVfx);

        if (hitEffect != null)
            Object.Destroy(hitEffect);

        yield return null;
    }

    [UnityTest]
    public IEnumerator ManyProjectiles_CanExistSimultaneously()
    {
        const int projectileCount = 250;

        projectilePrefab = new GameObject("LoadProjectile");
        Projectile projectile =
            projectilePrefab.AddComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject instance = Object.Instantiate(
                projectilePrefab,
                new Vector3(i % 25, i / 25, 0f),
                Quaternion.identity);

            spawnedObjects.Add(instance);
        }

        yield return null;

        int activeCount = CountObjectsWithPrefix(
            "LoadProjectile(Clone)");

        Assert.AreEqual(
            projectileCount,
            activeCount,
            "All load-test projectiles should exist simultaneously.");
    }

    [UnityTest]
    public IEnumerator MultipleSimultaneousWaves_RemainStable()
    {
        const int waveCount = 5;
        const int enemiesPerWave = 10;

        CreateEnemyPrefab();

        for (int i = 0; i < waveCount; i++)
        {
            CreateWave(
                $"LoadWave_{i}",
                enemiesPerWave);
        }

        yield return new WaitForSeconds(1f);

        int enemyCount =
            CountObjectsWithPrefix("LoadEnemy(Clone)");

        Assert.GreaterOrEqual(
            enemyCount,
            waveCount,
            "Multiple simultaneous waves should remain operational.");
    }

    [UnityTest]
    public IEnumerator FullGameplayWorkload_RemainsStable()
    {
        const int enemyCount = 50;
        const int projectileCount = 150;

        CreateEnemyPrefab();

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy =
                Object.Instantiate(
                    enemyPrefab,
                    new Vector3(i % 10, i / 10, 0f),
                    Quaternion.identity);

            spawnedObjects.Add(enemy);
        }

        projectilePrefab =
            new GameObject("LoadGameplayProjectile");

        projectilePrefab.AddComponent<Projectile>();

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject projectile =
                Object.Instantiate(
                    projectilePrefab,
                    new Vector3(i % 15, i / 15, 0f),
                    Quaternion.identity);

            spawnedObjects.Add(projectile);
        }

        yield return new WaitForSeconds(1f);

        Assert.DoesNotThrow(() =>
        {
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);
        });

        Assert.Greater(
            CountObjectsWithPrefix("LoadEnemy(Clone)"),
            0);

        Assert.Greater(
            CountObjectsWithPrefix("LoadGameplayProjectile(Clone)"),
            0);
    }

    [UnityTest]
    public IEnumerator PoolingController_RemainsStableUnderRepeatedRequests()
    {
        GameObject poolPrefab =
            new GameObject("LoadPoolObject");

        spawnedObjects.Add(poolPrefab);

        GameObject controllerObject =
            new GameObject("LoadPoolingController");

        spawnedObjects.Add(controllerObject);

        PoolingController controller =
            controllerObject.AddComponent<PoolingController>();

        controller.poolingObjectsClass =
            new[]
            {
                new PoolingObjects
                {
                    pooledPrefab = poolPrefab,
                    count = 25
                }
            };

        yield return null;

        for (int i = 0; i < 500; i++)
        {
            GameObject obj =
                controller.GetPoolingObject(poolPrefab);

            Assert.IsNotNull(obj);

            obj.SetActive(false);
        }

        Assert.GreaterOrEqual(
            controllerObject.transform.childCount,
            25);
    }

    [UnityTest]
    public IEnumerator SustainedProjectileGeneration_RemainsStable()
    {
        const int iterations = 100;
        const int projectilesPerIteration = 10;

        GameObject prefab =
            new GameObject("SustainedProjectile");

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

        yield return null;

        int count =
            CountObjectsWithPrefix(
                "SustainedProjectile(Clone)");

        Assert.AreEqual(
            iterations * projectilesPerIteration,
            count,
            "Sustained projectile generation should remain stable under load.");
    }

    private void CreateEnemyPrefab()
    {
        enemyPrefab =
            new GameObject("LoadEnemy");

        enemyPrefab.tag = "Enemy";

        Enemy enemy =
            enemyPrefab.AddComponent<Enemy>();

        enemy.health = 10;

        destructionVfx =
            new GameObject("LoadEnemyVFX");

        hitEffect =
            new GameObject("LoadHitEffect");

        enemy.destructionVFX = destructionVfx;
        enemy.hitEffect = hitEffect;

        enemy.Projectile =
            new GameObject("LoadEnemyProjectile");

        enemyPrefab.AddComponent<FollowThePath>();
    }

    private void CreateWave(string name, int count)
    {
        GameObject waveObject =
            new GameObject(name);

        spawnedObjects.Add(waveObject);

        Wave wave =
            waveObject.AddComponent<Wave>();

        wave.enemy = enemyPrefab;
        wave.count = count;
        wave.speed = 10f;
        wave.timeBetween = 0.01f;
        wave.rotationByPath = false;
        wave.Loop = true;
        wave.testMode = false;

        wave.shooting =
            new Shooting
            {
                shotChance = 0,
                shotTimeMin = 1f,
                shotTimeMax = 2f
            };

        GameObject[] points = new GameObject[4];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] =
                new GameObject($"{name}_Point_{i}");

            spawnedObjects.Add(points[i]);
        }

        wave.pathPoints = new Transform[4];

        for (int i = 0; i < points.Length; i++)
            wave.pathPoints[i] = points[i].transform;
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