using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BossTests
{
    private GameObject bossObject;
    private Boss boss;

    [SetUp]
    public void SetUp()
    {
        bossObject =
            new GameObject("Test_Boss");

        boss =
            bossObject.AddComponent<Boss>();

        boss.maxHealth = 1000;

        boss.movementSpeed = 5f;

        boss.movementDirection =
            Vector2.zero;

        boss.minX = -10f;
        boss.maxX = 10f;
        boss.minY = -5f;
        boss.maxY = 5f;
    }

    [TearDown]
    public void TearDown()
    {
        if (bossObject != null)
            Object.DestroyImmediate(bossObject);
    }

    // ================================================================
    // BOS-UT-001
    // ================================================================

    [Test]
    public void BOS_UT_001_BossInitializesWithConfiguredHP()
    {
        Object.DestroyImmediate(bossObject);

        bossObject = new GameObject("Test_Boss");

        boss = bossObject.AddComponent<Boss>();

        boss.maxHealth = 5000;

        // Re-enable lifecycle after configuration.
        bossObject.SetActive(false);
        bossObject.SetActive(true);

        Assert.AreEqual(
            5000,
            boss.health,
            "Boss should initialize with its configured maximum HP.");
    }

    // ================================================================
    // BOS-UT-002
    // ================================================================

    [Test]
    public void BOS_UT_002_BossReceivesDamage()
    {
        int startingHealth = boss.health;

        boss.GetDamage(100);

        Assert.Less(
            boss.health,
            startingHealth,
            "Boss health should decrease when the Boss receives damage.");
    }

    // ================================================================
    // BOS-UT-003
    // ================================================================

    [Test]
    public void BOS_UT_003_BossSurvivesWhileHealthAboveZero()
    {
        boss.maxHealth = 1000;

        bossObject.SetActive(false);
        bossObject.SetActive(true);

        boss.GetDamage(100);

        Assert.Greater(
            boss.health,
            0,
            "Boss should remain alive while health is above zero.");

        Assert.IsTrue(
            boss.IsAlive,
            "Boss should report itself as alive while health is above zero.");
    }

    // ================================================================
    // BOS-UT-004
    // ================================================================

    [Test]
    public void BOS_UT_004_BossDiesAtZeroHealth()
    {
        boss.maxHealth = 100;

        bossObject.SetActive(false);
        bossObject.SetActive(true);

        boss.GetDamage(100);

        Assert.AreEqual(
            0,
            boss.health,
            "Boss health should reach zero after lethal damage.");

        Assert.IsFalse(
            boss.IsAlive,
            "Boss should no longer be alive at zero health.");
    }

    // ================================================================
    // BOS-UT-005
    // ================================================================

    [UnityTest]
    public IEnumerator BOS_UT_005_BossMovesHorizontally()
    {
        boss.movementDirection =
            Vector2.right;

        Vector3 before =
            bossObject.transform.position;

        yield return new WaitForSeconds(0.2f);

        Vector3 after =
            bossObject.transform.position;

        Assert.Greater(
            after.x,
            before.x,
            "Boss should move horizontally when horizontal movement is configured.");
    }

    // ================================================================
    // BOS-UT-006
    // ================================================================

    [UnityTest]
    public IEnumerator BOS_UT_006_BossMovesVertically()
    {
        boss.movementDirection =
            Vector2.up;

        Vector3 before =
            bossObject.transform.position;

        yield return new WaitForSeconds(0.2f);

        Vector3 after =
            bossObject.transform.position;

        Assert.Greater(
            after.y,
            before.y,
            "Boss should move vertically when vertical movement is configured.");
    }

    // ================================================================
    // BOS-UT-007
    // ================================================================

    [UnityTest]
    public IEnumerator BOS_UT_007_BossMovesDiagonally()
    {
        boss.movementDirection =
            new Vector2(1f, 1f);

        Vector3 before =
            bossObject.transform.position;

        yield return new WaitForSeconds(0.2f);

        Vector3 after =
            bossObject.transform.position;

        Assert.Greater(
            after.x,
            before.x,
            "Boss should move horizontally during diagonal movement.");

        Assert.Greater(
            after.y,
            before.y,
            "Boss should move vertically during diagonal movement.");
    }

    // ================================================================
    // BOS-UT-008
    // ================================================================
    [UnityTest]
    public IEnumerator BOS_UT_008_BossStaysWithinAllowedGameplayArea()
    {
        boss.minX = -2f;
        boss.maxX = 2f;
        boss.minY = -2f;
        boss.maxY = 2f;

        boss.movementSpeed = 10f;

        boss.movementDirection = Vector2.right;

        bossObject.transform.position =
            new Vector3(1.9f, 0f, 0f);

        yield return new WaitForSeconds(0.5f);

        Assert.LessOrEqual(
            bossObject.transform.position.x,
            boss.maxX + 0.001f);

        Assert.GreaterOrEqual(
            bossObject.transform.position.x,
            boss.minX - 0.001f);

        Assert.LessOrEqual(
            bossObject.transform.position.y,
            boss.maxY + 0.001f);

        Assert.GreaterOrEqual(
            bossObject.transform.position.y,
            boss.minY - 0.001f);
    }
}