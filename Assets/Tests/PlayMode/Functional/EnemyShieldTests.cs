using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyShieldTests
{
    private GameObject enemyObject;
    private GameObject destructionVfx;
    private GameObject hitEffect;

    [SetUp]
    public void SetUp()
    {
        destructionVfx =
            new GameObject("ShieldTest_DestructionVFX");

        hitEffect =
            new GameObject("ShieldTest_HitEffect");

        enemyObject =
            new GameObject("ShieldTest_Enemy");

        enemyObject.tag = "Enemy";

        Enemy enemy =
            enemyObject.AddComponent<Enemy>();

        enemy.health = 20;
        enemy.destructionVFX = destructionVfx;
        enemy.hitEffect = hitEffect;

        enemyObject.AddComponent<EnemyShield>();
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(enemyObject);
        DestroyIfExists(destructionVfx);
        DestroyIfExists(hitEffect);
    }

    // ------------------------------------------------------------------
    // SHD-UT-001
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_001_ShieldInitializesWithConfiguredHealth()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(30);

        Assert.AreEqual(
            30,
            shield.shieldHealth,
            "Shield should initialize with the configured shield HP.");
    }

    // ------------------------------------------------------------------
    // SHD-UT-002
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_002_ShieldAbsorbsIncomingDamage()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(20);

        int remainingDamage =
            shield.AbsorbDamage(5);

        Assert.AreEqual(
            5,
            remainingDamage,
            "Damage-processing contract should return remaining damage.");

        Assert.Less(
            shield.shieldHealth,
            20,
            "Shield HP should decrease when damage is absorbed.");
    }

    // ------------------------------------------------------------------
    // SHD-UT-003
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_003_EnemyHealthRemainsUnchangedWhileShieldIsActive()
    {
        Enemy enemy =
            enemyObject.GetComponent<Enemy>();

        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(20);

        enemy.GetDamage(5);

        Assert.AreEqual(
            20,
            enemy.health,
            "Enemy HP should remain unchanged while the shield absorbs damage.");
    }

    // ------------------------------------------------------------------
    // SHD-UT-004
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_004_ShieldDecreasesByIncomingDamage()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(20);

        shield.AbsorbDamage(7);

        Assert.AreEqual(
            13,
            shield.shieldHealth,
            "Shield HP should decrease by absorbed damage.");
    }

    // ------------------------------------------------------------------
    // SHD-UT-005
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_005_ShieldBreaksAtZero()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(10);

        shield.AbsorbDamage(10);

        Assert.AreEqual(
            0,
            shield.shieldHealth);

        Assert.IsFalse(
            shield.IsActive,
            "Shield should become inactive when its HP reaches zero.");
    }

    // ------------------------------------------------------------------
    // SHD-UT-006
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_006_BrokenShieldNoLongerAbsorbsDamage()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(10);

        shield.AbsorbDamage(10);

        int remainingDamage =
            shield.AbsorbDamage(5);

        Assert.AreEqual(
            5,
            remainingDamage,
            "A broken shield should not absorb additional damage.");

        Assert.AreEqual(
            0,
            shield.shieldHealth);
    }

    // ------------------------------------------------------------------
    // SHD-UT-007
    // ------------------------------------------------------------------

    [Test]
    public void SHD_UT_007_ExcessDamageIsReturnedToEnemy()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(10);

        int remainingDamage =
            shield.AbsorbDamage(15);

        Assert.AreEqual(
            5,
            remainingDamage,
            "Damage exceeding remaining shield HP should pass through.");
    }

    // ------------------------------------------------------------------
    // SHD-FT-001
    // ------------------------------------------------------------------

    [UnityTest]
    public IEnumerator SHD_FT_001_PlayerProjectileDamagesShieldedEnemy()
    {
        GameObject projectileObject =
            new GameObject("ShieldTest_PlayerProjectile");

        Projectile projectile =
            projectileObject.AddComponent<Projectile>();

        projectile.damage = 5;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        Collider2D enemyCollider =
            enemyObject.AddComponent<BoxCollider2D>();

        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(20);

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            enemyCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.Less(
            shield.shieldHealth,
            20,
            "Player projectile should damage the enemy shield.");

        Assert.AreEqual(
            20,
            enemyObject.GetComponent<Enemy>().health,
            "Enemy HP should remain unchanged while shield absorbs the attack.");

        Object.DestroyImmediate(projectileObject);
    }

    // ------------------------------------------------------------------
    // SHD-RT-001
    // ------------------------------------------------------------------

    [Test]
    public void SHD_RT_001_UnshieldedEnemyRetainsExistingDamageBehavior()
    {
        GameObject unshieldedEnemyObject =
            new GameObject("ShieldTest_UnshieldedEnemy");

        unshieldedEnemyObject.tag = "Enemy";

        Enemy enemy =
            unshieldedEnemyObject.AddComponent<Enemy>();

        enemy.health = 20;
        enemy.destructionVFX = destructionVfx;
        enemy.hitEffect = hitEffect;

        enemy.GetDamage(5);

        Assert.AreEqual(
            15,
            enemy.health,
            "Enemies without a shield must retain the original damage behavior.");

        Object.DestroyImmediate(unshieldedEnemyObject);
    }

    // ------------------------------------------------------------------
    // Additional edge-case tests
    // ------------------------------------------------------------------

    [Test]
    public void Shield_CannotBecomeNegative()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(10);

        shield.AbsorbDamage(50);

        Assert.GreaterOrEqual(
            shield.shieldHealth,
            0,
            "Shield HP must never become negative.");
    }

    [Test]
    public void Shield_ZeroDamageDoesNotChangeShieldHealth()
    {
        EnemyShield shield =
            enemyObject.GetComponent<EnemyShield>();

        shield.Initialize(10);

        shield.AbsorbDamage(0);

        Assert.AreEqual(
            10,
            shield.shieldHealth,
            "Zero damage should not change shield HP.");
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}