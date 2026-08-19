using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerRegressionTests
{
    private GameObject playerObject;
    private GameObject destructionFxPrefab;

    [SetUp]
    public void SetUp()
    {
        destructionFxPrefab =
            SpaceShooterTestBuilder.CreateSimplePrefab(
                "Test_Player_DestructionFX");

        playerObject = new GameObject("Test_Player");
        playerObject.tag = "Player";

        Player player = playerObject.AddComponent<Player>();
        player.destructionFX = destructionFxPrefab;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObject != null)
            Object.DestroyImmediate(playerObject);

        if (destructionFxPrefab != null)
            Object.DestroyImmediate(destructionFxPrefab);

        Player.instance = null;
    }

    [UnityTest]
    public IEnumerator Player_Instance_IsAssigned()
    {
        Assert.AreEqual(
            playerObject,
            Player.instance.gameObject);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Player_GetDamage_DestroysPlayer()
    {
        Player player = playerObject.GetComponent<Player>();

        player.GetDamage(1);

        yield return null;

        Assert.IsTrue(
            playerObject == null,
            "Current game behavior destroys the Player when damage is received.");
    }
}