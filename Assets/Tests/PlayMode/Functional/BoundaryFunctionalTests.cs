using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoundaryFunctionalTests
{
    private GameObject boundaryObject;
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

        boundaryObject = new GameObject("Test_Boundary");

        BoxCollider2D collider =
            boundaryObject.AddComponent<BoxCollider2D>();

        collider.isTrigger = true;

        boundaryObject.AddComponent<Boundary>();
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(boundaryObject);
        DestroyIfExists(cameraObject);
    }

    [UnityTest]
    public IEnumerator Boundary_ResizesColliderFromCameraViewport()
    {
        yield return null;

        BoxCollider2D collider =
            boundaryObject.GetComponent<BoxCollider2D>();

        Assert.Greater(
            collider.size.x,
            0f);

        Assert.Greater(
            collider.size.y,
            0f);
    }

    [UnityTest]
    public IEnumerator Boundary_DestroysProjectileOutsideBoundary()
    {
        GameObject projectile =
            new GameObject("Test_Projectile");

        projectile.tag = "Projectile";

        Collider2D projectileCollider =
            projectile.AddComponent<BoxCollider2D>();

        boundaryObject.SendMessage(
            "OnTriggerExit2D",
            projectileCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsTrue(
            projectile == null,
            "Boundary should destroy projectiles leaving the boundary.");
    }

    [UnityTest]
    public IEnumerator Boundary_DestroysBonusOutsideBoundary()
    {
        GameObject bonus =
            new GameObject("Test_Bonus");

        bonus.tag = "Bonus";

        Collider2D bonusCollider =
            bonus.AddComponent<BoxCollider2D>();

        boundaryObject.SendMessage(
            "OnTriggerExit2D",
            bonusCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsTrue(
            bonus == null,
            "Boundary should destroy bonuses leaving the boundary.");
    }

    [UnityTest]
    public IEnumerator Boundary_DoesNotDestroyUnrecognizedObject()
    {
        GameObject other =
            new GameObject("Test_Other");

        other.tag = "Untagged";

        Collider2D otherCollider =
            other.AddComponent<BoxCollider2D>();

        boundaryObject.SendMessage(
            "OnTriggerExit2D",
            otherCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsNotNull(
            other,
            "Boundary should not destroy objects with unrelated tags.");

        Object.DestroyImmediate(other);
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}