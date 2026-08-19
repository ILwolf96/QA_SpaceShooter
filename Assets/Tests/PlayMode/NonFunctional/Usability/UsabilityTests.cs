using NUnit.Framework;
using UnityEngine;

public class UsabilityTests
{
    [Test]
    public void Player_HasMovementComponent()
    {
        GameObject player =
            new GameObject("Usability_Player");

        try
        {
            PlayerMoving movement =
                player.AddComponent<PlayerMoving>();

            Assert.IsNotNull(
                movement,
                "Player must have movement functionality.");
        }
        finally
        {
            Object.DestroyImmediate(player);
            PlayerMoving.instance = null;
        }
    }

    [Test]
    public void Player_HasShootingComponent()
    {
        GameObject player =
            new GameObject("Usability_Player");

        try
        {
            PlayerShooting shooting =
                player.AddComponent<PlayerShooting>();

            Assert.IsNotNull(
                shooting,
                "Player must have shooting functionality.");
        }
        finally
        {
            Object.DestroyImmediate(player);
            PlayerShooting.instance = null;
        }
    }

    [Test]
    public void PlayerShooting_HasValidWeaponPowerRange()
    {
        GameObject player =
            new GameObject("Usability_Player");

        try
        {
            PlayerShooting shooting =
                player.AddComponent<PlayerShooting>();

            Assert.GreaterOrEqual(
                shooting.weaponPower,
                1);

            Assert.LessOrEqual(
                shooting.weaponPower,
                shooting.maxweaponPower);
        }
        finally
        {
            Object.DestroyImmediate(player);
            PlayerShooting.instance = null;
        }
    }

    [Test]
    public void PlayerMoving_HasUsableScreenOffsets()
    {
        GameObject player =
            new GameObject("Usability_Player");

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

            Assert.Greater(
                movement.borders.minXOffset,
                0f);

            Assert.Greater(
                movement.borders.maxXOffset,
                0f);

            Assert.Greater(
                movement.borders.minYOffset,
                0f);

            Assert.Greater(
                movement.borders.maxYOffset,
                0f);
        }
        finally
        {
            Object.DestroyImmediate(player);
            PlayerMoving.instance = null;
        }
    }

    [Test]
    public void Wave_HasValidConfigurationFields()
    {
        GameObject waveObject =
            new GameObject("Usability_Wave");

        try
        {
            Wave wave =
                waveObject.AddComponent<Wave>();

            Assert.GreaterOrEqual(
                wave.count,
                0);

            Assert.GreaterOrEqual(
                wave.speed,
                0f);

            Assert.GreaterOrEqual(
                wave.timeBetween,
                0f);

            Assert.GreaterOrEqual(
                wave.shooting.shotChance,
                0);

            Assert.LessOrEqual(
                wave.shooting.shotChance,
                100);
        }
        finally
        {
            Object.DestroyImmediate(waveObject);
        }
    }
}