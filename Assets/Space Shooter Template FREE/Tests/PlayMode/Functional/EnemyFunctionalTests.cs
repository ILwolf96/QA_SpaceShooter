using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class EnemyFunctionalTests
{
    private GameObject enemyObject;
    private GameObject hitEffect;
    private GameObject destructionVfx;

    [SetUp]
    public void SetUp()
    {
        hitEffect = SpaceShooterTestBuilder.CreateSimplePrefab(
            "Test_HitEffectPrefab");

        destructionVfx = SpaceShooterTestBuilder.CreateSimplePrefab(
            "Test_DestructionVFXPrefab");

        enemyObject = SpaceShooterTestBuilder.CreateEnemy(
            health: 10,
            hitEffect: hitEffect,
            destructionVfx: destructionVfx);
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(enemyObject);
        DestroyIfExists(hitEffect);
        DestroyIfExists(destructionVfx);

        Player.instance = null;
    }

    [UnityTest]
    public IEnumerator Enemy_StartsWithConfiguredHealth()
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        Assert.AreEqual(10, enemy.health);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Enemy_NonLethalDamage_ReducesHealth()
    {
        Enemy enemy = enemyObject.GetComponent<Enemy>();

        enemy.GetDamage(3);

        Assert.AreEqual(7, enemy.health);
        Assert.IsNotNull(enemyObject);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Enemy_LethalDamage_DestroysEnemy()
    {
        enemyObject.GetComponent<Enemy>().GetDamage(10);

        yield return null;

        Assert.IsTrue(enemyObject == null,
            "Enemy should be destroyed when health reaches zero.");
    }

    [UnityTest]
    public IEnumerator Enemy_NonLethalDamage_CreatesHitEffect()
    {
        enemyObject.GetComponent<Enemy>().GetDamage(3);

        yield return null;

        GameObject[] objects = Object.FindObjectsByType<GameObject>(
            FindObjectsSortMode.None);

        bool hitEffectCreated = false;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_HitEffectPrefab"))
            {
                hitEffectCreated = true;
                break;
            }
        }

        Assert.IsTrue(
            hitEffectCreated,
            "A hit effect should be created after non-lethal damage.");
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}