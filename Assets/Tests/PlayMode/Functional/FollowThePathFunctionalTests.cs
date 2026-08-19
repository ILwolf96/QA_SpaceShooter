using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class FollowThePathFunctionalTests
{
    private GameObject enemyObject;
    private GameObject[] pathObjects;

    [SetUp]
    public void SetUp()
    {
        enemyObject = new GameObject("Test_PathEnemy");

        FollowThePath follow =
            enemyObject.AddComponent<FollowThePath>();

        follow.speed = 10f;
        follow.rotationByPath = false;
        follow.loop = false;

        pathObjects = new GameObject[4];

        for (int i = 0; i < pathObjects.Length; i++)
        {
            pathObjects[i] = new GameObject($"Test_PathPoint_{i}");
            pathObjects[i].transform.position =
                new Vector3(i * 2f, i, 0f);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (enemyObject != null)
            Object.DestroyImmediate(enemyObject);

        if (pathObjects != null)
        {
            foreach (GameObject pathPoint in pathObjects)
            {
                if (pathPoint != null)
                    Object.DestroyImmediate(pathPoint);
            }
        }
    }

    [UnityTest]
    public IEnumerator SetPath_InitializesEnemyAtPathStart()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        Transform[] path = GetPathTransforms();

        follow.path = path;
        follow.SetPath();

        Assert.IsTrue(
            follow.movingIsActive,
            "Enemy movement should be active after SetPath().");

        Assert.AreEqual(
            path[0].position.x,
            enemyObject.transform.position.x,
            0.001f);

        Assert.AreEqual(
            path[0].position.y,
            enemyObject.transform.position.y,
            0.001f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SetPath_StoresConfiguredPath()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        Transform[] path = GetPathTransforms();

        follow.path = path;
        follow.SetPath();

        Assert.IsNotNull(follow.path);
        Assert.AreEqual(path.Length, follow.path.Length);

        for (int i = 0; i < path.Length; i++)
        {
            Assert.AreEqual(
                path[i],
                follow.path[i]);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator FollowThePath_MovesWhenMovementIsActive()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        follow.path = GetPathTransforms();
        follow.speed = 50f;
        follow.SetPath();

        Vector3 before = enemyObject.transform.position;

        yield return new WaitForSeconds(0.1f);

        Vector3 after = enemyObject.transform.position;

        Assert.AreNotEqual(
            before,
            after,
            "Enemy should move after SetPath() activates movement.");
    }

    [UnityTest]
    public IEnumerator RotationByPathFalse_ResetsRotationToIdentity()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        enemyObject.transform.rotation =
            Quaternion.Euler(0f, 0f, 45f);

        follow.path = GetPathTransforms();
        follow.rotationByPath = false;

        follow.SetPath();

        Assert.AreEqual(
            Quaternion.identity,
            enemyObject.transform.rotation);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LoopEnabled_LeavesEnemyActiveAfterPathProgress()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        follow.path = GetPathTransforms();
        follow.speed = 100f;
        follow.loop = true;

        follow.SetPath();

        yield return new WaitForSeconds(1f);

        Assert.IsNotNull(
            enemyObject,
            "Looping path should not destroy the enemy.");
    }

    [UnityTest]
    public IEnumerator LoopDisabled_DestroysEnemyAfterPathCompletion()
    {
        FollowThePath follow =
            enemyObject.GetComponent<FollowThePath>();

        follow.path = GetPathTransforms();
        follow.speed = 100f;
        follow.loop = false;

        follow.SetPath();

        yield return new WaitForSeconds(2f);

        Assert.IsTrue(
            enemyObject == null,
            "Non-looping path should destroy the enemy after completion.");
    }

    private Transform[] GetPathTransforms()
    {
        Transform[] result = new Transform[pathObjects.Length];

        for (int i = 0; i < pathObjects.Length; i++)
            result[i] = pathObjects[i].transform;

        return result;
    }
}