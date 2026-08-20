using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RepeatingBackgroundFunctionalTests
{
    private GameObject backgroundObject;

    [SetUp]
    public void SetUp()
    {
        backgroundObject = new GameObject("Test_RepeatingBackground");

        RepeatingBackground background =
            backgroundObject.AddComponent<RepeatingBackground>();

        background.verticalSize = 5f;
    }

    [TearDown]
    public void TearDown()
    {
        if (backgroundObject != null)
            Object.DestroyImmediate(backgroundObject);
    }

    [UnityTest]
    public IEnumerator Background_DoesNotReposition_AboveThreshold()
    {
        RepeatingBackground background =
            backgroundObject.GetComponent<RepeatingBackground>();

        backgroundObject.transform.position =
            new Vector3(0f, 0f, 0f);

        Vector3 before =
            backgroundObject.transform.position;

        background.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        Vector3 after =
            backgroundObject.transform.position;

        Assert.AreEqual(
            before,
            after,
            "Background should not reposition while it is above the threshold.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Background_Repositions_WhenBelowThreshold()
    {
        RepeatingBackground background =
            backgroundObject.GetComponent<RepeatingBackground>();

        backgroundObject.transform.position =
            new Vector3(0f, -6f, 0f);

        background.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.AreEqual(
            4f,
            backgroundObject.transform.position.y,
            0.001f,
            "Background should move upward by verticalSize * 2.");
    }

    [UnityTest]
    public IEnumerator Background_RepositionDistance_EqualsTwiceVerticalSize()
    {
        RepeatingBackground background =
            backgroundObject.GetComponent<RepeatingBackground>();

        float verticalSize = 7f;
        background.verticalSize = verticalSize;

        float startingY = -8f;

        backgroundObject.transform.position =
            new Vector3(0f, startingY, 0f);

        background.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        yield return null;

        float expectedY =
            startingY + verticalSize * 2f;

        Assert.AreEqual(
            expectedY,
            backgroundObject.transform.position.y,
            0.001f,
            "Background reposition distance should be exactly verticalSize * 2.");
    }

    [UnityTest]
    public IEnumerator Background_RemainsFunctional_AfterLevelControllerInitialization()
    {
        GameObject levelControllerObject =
            new GameObject("Regression_LevelController");

        LevelController levelController =
            levelControllerObject.AddComponent<LevelController>();

        GameObject backgroundRegressionObject =
            new GameObject("Regression_Background");

        RepeatingBackground background =
            backgroundRegressionObject.AddComponent<RepeatingBackground>();

        background.verticalSize = 5f;

        yield return null;

        backgroundRegressionObject.transform.position =
            new Vector3(0f, -6f, 0f);

        background.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.AreEqual(
            4f,
            backgroundRegressionObject.transform.position.y,
            0.001f,
            "Background should retain its endless repositioning behavior after LevelController initialization.");

        Object.DestroyImmediate(backgroundRegressionObject);
        Object.DestroyImmediate(levelControllerObject);
    }
}