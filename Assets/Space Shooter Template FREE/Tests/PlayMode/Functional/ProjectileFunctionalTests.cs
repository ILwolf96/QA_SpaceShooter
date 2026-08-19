using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ProjectileFunctionalTests
{
    private GameObject projectileObject;
    private GameObject playerObject;
    private GameObject enemyObject;
    private GameObject destructionFx;

    [SetUp]
    public void SetUp()
    {
        destructionFx = new GameObject("Test_Player_DestructionFX");

        playerObject = new GameObject("Test_Player");
        playerObject.tag = "Player";

        Player player = playerObject.AddComponent<Player>();
        player.destructionFX = destructionFx;

        enemyObject = SpaceShooterTestBuilder.CreateEnemy(
            health: 10);

        projectileObject = new GameObject("Test_Projectile");
        projectileObject.tag = "Projectile";

        projectileObject.AddComponent<Projectile>();
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(projectileObject);
        DestroyIfExists(playerObject);
        DestroyIfExists(enemyObject);
        DestroyIfExists(destructionFx);

        Player.instance = null;
    }

    [UnityTest]
    public IEnumerator PlayerProjectile_DamagesEnemy()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 3;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        Collider2D enemyCollider =
            enemyObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            enemyCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Enemy enemy = enemyObject.GetComponent<Enemy>();

        Assert.AreEqual(
            7,
            enemy.health,
            "Player projectile should reduce Enemy health by its configured damage.");
    }

    [UnityTest]
    public IEnumerator EnemyProjectile_DamagesPlayer()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 1;
        projectile.enemyBullet = true;
        projectile.destroyedByCollision = false;

        Collider2D playerCollider =
            playerObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            playerCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsTrue(
            playerObject == null,
            "The current Player implementation destroys the Player when it receives damage.");
    }

    [UnityTest]
    public IEnumerator PlayerProjectile_DoesNotDamagePlayer()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 5;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        Collider2D playerCollider =
            playerObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            playerCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsNotNull(
            playerObject,
            "A Player projectile should not damage a Player.");
    }

    [UnityTest]
    public IEnumerator EnemyProjectile_DoesNotDamageEnemy()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 5;
        projectile.enemyBullet = true;
        projectile.destroyedByCollision = false;

        Collider2D enemyCollider =
            enemyObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            enemyCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.AreEqual(
            10,
            enemyObject.GetComponent<Enemy>().health,
            "Enemy projectile should not damage an Enemy.");
    }

    [UnityTest]
    public IEnumerator Projectile_IsDestroyed_WhenDestroyedByCollisionIsTrue()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 3;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = true;

        Collider2D enemyCollider =
            enemyObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            enemyCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsTrue(
            projectileObject == null,
            "Projectile should be destroyed after collision when configured to do so.");
    }

    [UnityTest]
    public IEnumerator Projectile_Remains_WhenDestroyedByCollisionIsFalse()
    {
        Projectile projectile =
            projectileObject.GetComponent<Projectile>();

        projectile.damage = 3;
        projectile.enemyBullet = false;
        projectile.destroyedByCollision = false;

        Collider2D enemyCollider =
            enemyObject.AddComponent<BoxCollider2D>();

        projectileObject.SendMessage(
            "OnTriggerEnter2D",
            enemyCollider,
            SendMessageOptions.RequireReceiver);

        yield return null;

        Assert.IsNotNull(
            projectileObject,
            "Projectile should remain when destroyedByCollision is false.");
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}