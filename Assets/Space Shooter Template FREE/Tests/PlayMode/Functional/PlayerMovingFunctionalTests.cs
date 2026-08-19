using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovingFunctionalTests
{
    private GameObject playerObject;
    private GameObject cameraObject;

    [SetUp]
    public void SetUp()
    {
        cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.aspect = 1.6f;

        playerObject = new GameObject("Test_Player");

        PlayerMoving movement =
            playerObject.AddComponent<PlayerMoving>();

        movement.borders = new Borders
        {
            minXOffset = 1f,
            maxXOffset = 1f,
            minYOffset = 1f,
            maxYOffset = 1f
        };
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(playerObject);
        DestroyIfExists(cameraObject);

        PlayerMoving.instance = null;
    }

    [UnityTest]
    public IEnumerator PlayerMoving_CalculatesMovementBorders()
    {
        yield return null;

        PlayerMoving movement =
            playerObject.GetComponent<PlayerMoving>();

        Assert.Less(
            movement.borders.minX,
            movement.borders.maxX);

        Assert.Less(
            movement.borders.minY,
            movement.borders.maxY);
    }

    [UnityTest]
    public IEnumerator PlayerMoving_ClampsMinimumX()
    {
        yield return null;

        PlayerMoving movement =
            playerObject.GetComponent<PlayerMoving>();

        playerObject.transform.position =
            new Vector3(
                movement.borders.minX - 10f,
                0f,
                0f);

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(
            movement.borders.minX,
            playerObject.transform.position.x,
            0.001f);
    }

    [UnityTest]
    public IEnumerator PlayerMoving_ClampsMaximumX()
    {
        yield return null;

        PlayerMoving movement =
            playerObject.GetComponent<PlayerMoving>();

        playerObject.transform.position =
            new Vector3(
                movement.borders.maxX + 10f,
                0f,
                0f);

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(
            movement.borders.maxX,
            playerObject.transform.position.x,
            0.001f);
    }

    [UnityTest]
    public IEnumerator PlayerMoving_ClampsMinimumY()
    {
        yield return null;

        PlayerMoving movement =
            playerObject.GetComponent<PlayerMoving>();

        playerObject.transform.position =
            new Vector3(
                0f,
                movement.borders.minY - 10f,
                0f);

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(
            movement.borders.minY,
            playerObject.transform.position.y,
            0.001f);
    }

    [UnityTest]
    public IEnumerator PlayerMoving_ClampsMaximumY()
    {
        yield return null;

        PlayerMoving movement =
            playerObject.GetComponent<PlayerMoving>();

        playerObject.transform.position =
            new Vector3(
                0f,
                movement.borders.maxY + 10f,
                0f);

        movement.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(
            movement.borders.maxY,
            playerObject.transform.position.y,
            0.001f);
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}