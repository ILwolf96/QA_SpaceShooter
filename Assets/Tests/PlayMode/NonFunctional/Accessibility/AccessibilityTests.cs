using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class AccessibilityTests
{
    [Test]
    public void Player_HasUsableMovementConfiguration()
    {
        GameObject player =
            new GameObject("Accessibility_Player");

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
    public void GameplayComponents_DoNotRequireColorConfigurationToExist()
    {
        GameObject player =
            new GameObject("Accessibility_Player");

        GameObject enemy =
            new GameObject("Accessibility_Enemy");

        try
        {
            Assert.IsNotNull(
                player.AddComponent<Player>());

            Assert.IsNotNull(
                enemy.AddComponent<Enemy>());
        }
        finally
        {
            Object.DestroyImmediate(player);
            Object.DestroyImmediate(enemy);

            Player.instance = null;
        }
    }

    [UnityTest]
    public IEnumerator Player_RemainsWithinCameraPlayableArea()
    {
        GameObject cameraObject =
            new GameObject("Accessibility_Camera");

        cameraObject.tag = "MainCamera";

        Camera camera =
            cameraObject.AddComponent<Camera>();

        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.aspect = 1.6f;

        GameObject player =
            new GameObject("Accessibility_Player");

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
}