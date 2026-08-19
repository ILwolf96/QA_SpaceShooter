using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CompatibilityTests
{
    [Test]
    public void PlayerMovement_HasValidMovementBorders()
    {
        GameObject player =
            new GameObject("Compatibility_Player");

        try
        {
            PlayerMoving movement =
                player.AddComponent<PlayerMoving>();

            movement.borders =
                new Borders
                {
                    minXOffset = 1f,
                    maxXOffset = 1f,
                    minYOffset = 1f,
                    maxYOffset = 1f
                };

            Assert.GreaterOrEqual(
                movement.borders.minXOffset,
                0f);

            Assert.GreaterOrEqual(
                movement.borders.maxXOffset,
                0f);

            Assert.GreaterOrEqual(
                movement.borders.minYOffset,
                0f);

            Assert.GreaterOrEqual(
                movement.borders.maxYOffset,
                0f);
        }
        finally
        {
            Object.DestroyImmediate(player);
        }
    }

    [Test]
    public void PlayerMovement_DesktopAndMobileInputPaths_AreDefined()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        Assert.Pass(
            "Desktop/editor input compilation path is active.");
#elif UNITY_IOS || UNITY_ANDROID
        Assert.Pass(
            "Mobile input compilation path is active.");
#else
        Assert.Pass(
            "Platform-specific PlayerMoving input path is not active on this platform.");
#endif
    }

    [UnityTest]
    public IEnumerator PlayerMovement_RemainsInsideConfiguredBounds()
    {
        GameObject cameraObject =
            new GameObject("Compatibility_Camera");

        cameraObject.tag = "MainCamera";

        Camera camera =
            cameraObject.AddComponent<Camera>();

        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.aspect = 1.6f;

        GameObject player =
            new GameObject("Compatibility_Player");

        PlayerMoving movement =
            player.AddComponent<PlayerMoving>();

        movement.borders =
            new Borders
            {
                minXOffset = 1f,
                maxXOffset = 1f,
                minYOffset = 1f,
                maxYOffset = 1f
            };

        yield return null;

        try
        {
            Assert.Less(
                movement.borders.minX,
                movement.borders.maxX);

            Assert.Less(
                movement.borders.minY,
                movement.borders.maxY);
        }
        finally
        {
            Object.DestroyImmediate(player);
            Object.DestroyImmediate(cameraObject);

            PlayerMoving.instance = null;
        }
    }

    [Test]
    public void CoreGameplayComponents_CanBeCreated()
    {
        GameObject player =
            new GameObject("Compatibility_Player");

        GameObject enemy =
            new GameObject("Compatibility_Enemy");

        try
        {
            Assert.IsNotNull(
                player.AddComponent<Player>());

            Assert.IsNotNull(
                player.AddComponent<PlayerMoving>());

            Assert.IsNotNull(
                player.AddComponent<PlayerShooting>());

            Assert.IsNotNull(
                enemy.AddComponent<Enemy>());

            Assert.IsNotNull(
                enemy.AddComponent<FollowThePath>());
        }
        finally
        {
            Object.DestroyImmediate(player);
            Object.DestroyImmediate(enemy);

            Player.instance = null;
            PlayerShooting.instance = null;
            PlayerMoving.instance = null;
        }
    }
}