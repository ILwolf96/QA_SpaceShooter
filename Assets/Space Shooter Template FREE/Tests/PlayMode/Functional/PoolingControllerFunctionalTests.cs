using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PoolingControllerFunctionalTests
{
    private GameObject controllerObject;
    private GameObject pooledPrefab;

    [SetUp]
    public void SetUp()
    {
        pooledPrefab =
            SpaceShooterTestBuilder.CreatePoolingPrefab(
                "TestProjectile");

        controllerObject =
            new GameObject("Test_PoolingController");

        PoolingController controller =
            controllerObject.AddComponent<PoolingController>();

        controller.poolingObjectsClass =
            new[]
            {
                new PoolingObjects
                {
                    pooledPrefab = pooledPrefab,
                    count = 2
                }
            };
    }

    [TearDown]
    public void TearDown()
    {
        if (controllerObject != null)
            Object.DestroyImmediate(controllerObject);

        if (pooledPrefab != null)
            Object.DestroyImmediate(pooledPrefab);

        PoolingController.instance = null;
    }

    [UnityTest]
    public IEnumerator PoolingController_CreatesConfiguredInitialPool()
    {
        yield return null;

        int childCount = controllerObject.transform.childCount;

        Assert.AreEqual(
            2,
            childCount,
            "Initial pool should contain the configured number of objects.");

        for (int i = 0; i < controllerObject.transform.childCount; i++)
        {
            Assert.IsFalse(
                controllerObject.transform.GetChild(i).gameObject.activeSelf);
        }
    }

    [UnityTest]
    public IEnumerator GetPoolingObject_ReturnsInactivePooledObject()
    {
        yield return null;

        PoolingController controller =
            controllerObject.GetComponent<PoolingController>();

        GameObject result =
            controller.GetPoolingObject(pooledPrefab);

        Assert.IsNotNull(result);
        Assert.IsFalse(result.activeSelf);
    }

    [UnityTest]
    public IEnumerator GetPoolingObject_AddsObject_WhenPoolIsExhausted()
    {
        yield return null;

        PoolingController controller =
            controllerObject.GetComponent<PoolingController>();

        GameObject first =
            controller.GetPoolingObject(pooledPrefab);

        first.SetActive(true);

        GameObject second =
            controller.GetPoolingObject(pooledPrefab);

        second.SetActive(true);

        GameObject third =
            controller.GetPoolingObject(pooledPrefab);

        Assert.IsNotNull(third);
        Assert.AreEqual(
            3,
            controllerObject.transform.childCount,
            "A new object should be created after the pool is exhausted.");
    }

    [UnityTest]
    public IEnumerator ReturnedInactiveObject_CanBeRetrievedAgain()
    {
        yield return null;

        PoolingController controller =
            controllerObject.GetComponent<PoolingController>();

        GameObject first =
            controller.GetPoolingObject(pooledPrefab);

        Assert.IsNotNull(first);
        Assert.IsFalse(first.activeSelf);

        first.SetActive(true);
        first.SetActive(false);

        GameObject second =
            controller.GetPoolingObject(pooledPrefab);

        Assert.AreSame(
            first,
            second,
            "An inactive pooled object should be reused rather than replaced.");

        Assert.IsFalse(
            second.activeSelf);
    }
}