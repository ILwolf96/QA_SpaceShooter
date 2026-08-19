using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BonusFunctionalTests
{
    private GameObject playerObject;
    private GameObject bonusObject;

    [SetUp]
    public void SetUp()
    {
        playerObject =
            SpaceShooterTestBuilder.CreatePlayerWithShooting();

        bonusObject =
            SpaceShooterTestBuilder.CreateBonus();

        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 1;
        shooting.maxweaponPower = 4;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObject != null)
            Object.DestroyImmediate(playerObject);

        if (bonusObject != null)
            Object.DestroyImmediate(bonusObject);

        Player.instance = null;
        PlayerShooting.instance = null;
    }

    [UnityTest]
    public IEnumerator Bonus_Collection_IncreasesWeaponPower()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        Bonus bonus = bonusObject.GetComponent<Bonus>();

        Collider2D playerCollider =
            playerObject.GetComponent<Collider2D>();

        // Create the required collider if the builder has not created it.
        if (playerCollider == null)
        {
            playerCollider = playerObject.AddComponent<CircleCollider2D>();
        }

        bonusObject.GetComponent<Collider2D>()
            .isTrigger = true;

        // Exercise the actual trigger callback.
        bonusObject.SendMessage(
            "OnTriggerEnter2D",
            playerCollider,
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(2, shooting.weaponPower);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Bonus_DoesNotIncreaseWeaponPower_AboveMaximum()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = shooting.maxweaponPower;

        Collider2D playerCollider =
            playerObject.GetComponent<Collider2D>();

        bonusObject.SendMessage(
            "OnTriggerEnter2D",
            playerCollider,
            SendMessageOptions.RequireReceiver);

        Assert.AreEqual(
            shooting.maxweaponPower,
            shooting.weaponPower);

        yield return null;
    }
}