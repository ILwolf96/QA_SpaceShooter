using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DirectMovingTests
{
    private GameObject movingObject;

    [SetUp]
    public void SetUp()
    {
        movingObject = new GameObject("Test_DirectMoving");

        DirectMoving movement =
            movingObject.AddComponent<DirectMoving>();

        movement.enabled = false;
    }

    [TearDown]
    public void TearDown()
    {
        if (movingObject != null)
            Object.DestroyImmediate(movingObject);
    }

    [UnityTest]
    public IEnumerator DirectMoving_PositiveSpeed_MovesUpward()
    {
        DirectMoving movement =
            movingObject.GetComponent<DirectMoving>();

        movement.speed = 5f;

        Vector3 before =
            movingObject.transform.position;

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Vector3 after =
            movingObject.transform.position;

        Assert.Greater(
            after.y,
            before.y,
            "Positive speed should move the object upward.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator DirectMoving_ZeroSpeed_DoesNotMove()
    {
        DirectMoving movement =
            movingObject.GetComponent<DirectMoving>();

        movement.speed = 0f;

        Vector3 before =
            movingObject.transform.position;

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Vector3 after =
            movingObject.transform.position;

        Assert.AreEqual(
            before,
            after,
            "Zero speed should not move the object.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator DirectMoving_NegativeSpeed_MovesDownward()
    {
        DirectMoving movement =
            movingObject.GetComponent<DirectMoving>();

        movement.speed = -5f;

        Vector3 before =
            movingObject.transform.position;

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Vector3 after =
            movingObject.transform.position;

        Assert.Less(
            after.y,
            before.y,
            "Negative speed should move the object downward.");

        yield return null;
    }
}