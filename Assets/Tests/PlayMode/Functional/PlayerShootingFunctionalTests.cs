using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerShootingFunctionalTests
{
    private GameObject playerObject;
    private GameObject projectilePrefab;
    private GameObject centralGun;
    private GameObject leftGun;
    private GameObject rightGun;

    [SetUp]
    public void SetUp()
    {
        playerObject = new GameObject("Test_Player");

        Player player = playerObject.AddComponent<Player>();

        GameObject destructionFx =
            new GameObject("Test_DestructionFX");

        player.destructionFX = destructionFx;

        projectilePrefab =
            new GameObject("Test_Projectile");

        projectilePrefab.AddComponent<Projectile>();

        centralGun = CreateGun("CentralGun");
        leftGun = CreateGun("LeftGun");
        rightGun = CreateGun("RightGun");

        PlayerShooting shooting =
            playerObject.AddComponent<PlayerShooting>();

        shooting.projectileObject = projectilePrefab;
        shooting.fireRate = 1000f;
        shooting.weaponPower = 1;

        shooting.guns = new Guns
        {
            centralGun = centralGun,
            leftGun = leftGun,
            rightGun = rightGun
        };
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(playerObject);
        DestroyIfExists(projectilePrefab);
        DestroyIfExists(centralGun);
        DestroyIfExists(leftGun);
        DestroyIfExists(rightGun);

        CleanupSpawnedProjectiles();

        Player.instance = null;
        PlayerShooting.instance = null;
    }

    [UnityTest]
    public IEnumerator WeaponPower1_CreatesOneProjectile()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 1;

        int before = CountSpawnedProjectiles();

        shooting.SendMessage(
            "MakeAShot",
            SendMessageOptions.RequireReceiver);

        yield return null;

        int after = CountSpawnedProjectiles();

        Assert.AreEqual(
            1,
            after - before,
            "Weapon Power 1 should create exactly one projectile.");
    }

    [UnityTest]
    public IEnumerator WeaponPower2_CreatesTwoProjectiles()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 2;

        int before = CountSpawnedProjectiles();

        shooting.SendMessage(
            "MakeAShot",
            SendMessageOptions.RequireReceiver);

        yield return null;

        int after = CountSpawnedProjectiles();

        Assert.AreEqual(
            2,
            after - before,
            "Weapon Power 2 should create exactly two projectiles.");
    }

    [UnityTest]
    public IEnumerator WeaponPower3_CreatesThreeProjectiles()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 3;

        int before = CountSpawnedProjectiles();

        shooting.SendMessage(
            "MakeAShot",
            SendMessageOptions.RequireReceiver);

        yield return null;

        int after = CountSpawnedProjectiles();

        Assert.AreEqual(
            3,
            after - before,
            "Weapon Power 3 should create exactly three projectiles.");
    }

    [UnityTest]
    public IEnumerator WeaponPower4_CreatesSixProjectiles()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 4;

        int before = CountSpawnedProjectiles();

        shooting.SendMessage(
            "MakeAShot",
            SendMessageOptions.RequireReceiver);

        yield return null;

        int after = CountSpawnedProjectiles();

        Assert.AreEqual(
            6,
            after - before,
            "Weapon Power 4 should create exactly six projectiles.");
    }

    [UnityTest]
    public IEnumerator Shooting_ExistingWeaponPowerBehavior_RemainsIntact()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = 1;

        int before = CountSpawnedProjectiles();

        shooting.SendMessage(
            "MakeAShot",
            SendMessageOptions.RequireReceiver);

        yield return null;

        int after = CountSpawnedProjectiles();

        Assert.Greater(
            after - before,
            0,
            "Existing shooting behavior should continue producing projectiles.");
    }

    [UnityTest]
    public IEnumerator WeaponPower_IsLimitedToConfiguredMaximum()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.weaponPower = shooting.maxweaponPower;

        Assert.AreEqual(
            4,
            shooting.maxweaponPower,
            "Current game configuration expects a maximum weapon power of 4.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator FireRate_ControlsNextFireTime()
    {
        PlayerShooting shooting =
            playerObject.GetComponent<PlayerShooting>();

        shooting.fireRate = 2f;

        shooting.nextFire = 100f;

        float before = shooting.nextFire;

        shooting.SendMessage(
            "Update",
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.AreEqual(
            before,
            shooting.nextFire,
            "The current implementation should not fire before nextFire is reached.");
    }

    private GameObject CreateGun(string name)
    {
        GameObject gun = new GameObject(name);
        gun.AddComponent<ParticleSystem>();
        return gun;
    }

    private static int CountSpawnedProjectiles()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        int count = 0;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_Projectile(Clone)"))
                count++;
        }

        return count;
    }

    private static void CleanupSpawnedProjectiles()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_Projectile(Clone)"))
                Object.DestroyImmediate(obj);
        }
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}