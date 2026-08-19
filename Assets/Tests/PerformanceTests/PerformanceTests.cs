using NUnit.Framework;
using Unity.PerformanceTesting;
using UnityEngine;

public class PerformanceTests
{
    private GameObject projectilePrefab;
    private GameObject poolControllerObject;
    private GameObject poolPrefab;

    [SetUp]
    public void SetUp()
    {
        projectilePrefab =
            new GameObject("PerformanceProjectile");

        Projectile projectile =
            projectilePrefab.AddComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        poolPrefab =
            new GameObject("PerformancePoolObject");

        poolControllerObject =
            new GameObject("PerformancePoolingController");

        PoolingController controller =
            poolControllerObject.AddComponent<PoolingController>();

        controller.poolingObjectsClass =
            new[]
            {
                new PoolingObjects
                {
                    pooledPrefab = poolPrefab,
                    count = 100
                }
            };
    }

    [TearDown]
    public void TearDown()
    {
        if (projectilePrefab != null)
            Object.DestroyImmediate(projectilePrefab);

        if (poolControllerObject != null)
            Object.DestroyImmediate(poolControllerObject);

        if (poolPrefab != null)
            Object.DestroyImmediate(poolPrefab);

        PoolingController.instance = null;
    }

    [Test, Performance]
    public void ProjectileInstantiation_Performance()
    {
        Measure.Method(
            () =>
            {
                GameObject instance =
                    Object.Instantiate(
                        projectilePrefab,
                        Vector3.zero,
                        Quaternion.identity);

                Object.DestroyImmediate(instance);
            })
            .WarmupCount(10)
            .MeasurementCount(100)
            .IterationsPerMeasurement(10)
            .Run();
    }

    [Test, Performance]
    public void PoolRetrieval_Performance()
    {
        PoolingController controller =
            poolControllerObject.GetComponent<PoolingController>();

        Measure.Method(
            () =>
            {
                GameObject obj =
                    controller.GetPoolingObject(poolPrefab);

                obj.SetActive(false);
            })
            .WarmupCount(10)
            .MeasurementCount(100)
            .IterationsPerMeasurement(50)
            .Run();
    }

    [Test, Performance]
    public void ObjectLookup_Performance()
    {
        GameObject[] objects =
            new GameObject[100];

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i] =
                new GameObject($"PerformanceObject_{i}");
        }

        try
        {
            Measure.Method(
                () =>
                {
                    Object.FindObjectsByType<GameObject>(
                        FindObjectsSortMode.None);
                })
                .WarmupCount(5)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10)
                .Run();
        }
        finally
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                    Object.DestroyImmediate(obj);
            }
        }
    }
}