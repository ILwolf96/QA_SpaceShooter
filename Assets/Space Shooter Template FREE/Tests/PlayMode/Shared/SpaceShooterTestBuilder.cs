using UnityEngine;

public static class SpaceShooterTestBuilder
{
    public static GameObject CreateSimplePrefab(string name)
    {
        GameObject prefab = new GameObject(name);
        return prefab;
    }

    public static GameObject CreatePlayer()
    {
        GameObject player = new GameObject("Test_Player");
        player.tag = "Player";

        Player playerComponent = player.AddComponent<Player>();

        GameObject destructionFx = CreateSimplePrefab("Test_Player_DestructionFX");
        playerComponent.destructionFX = destructionFx;

        return player;
    }

    public static GameObject CreatePlayerWithShooting()
    {
        GameObject player = CreatePlayer();

        PlayerShooting shooting = player.AddComponent<PlayerShooting>();

        // Awake() has already executed when AddComponent is called,
        // so PlayerShooting.instance is assigned.

        // Prevent Start() from trying to access gun ParticleSystems
        // when this test only needs weapon-power functionality.
        shooting.enabled = false;

        return player;
    }

    public static GameObject CreateEnemy(
        int health = 10,
        GameObject hitEffect = null,
        GameObject destructionVfx = null)
    {
        GameObject enemyObject = new GameObject("Test_Enemy");
        enemyObject.tag = "Enemy";

        Enemy enemy = enemyObject.AddComponent<Enemy>();
        enemy.health = health;

        if (hitEffect == null)
            hitEffect = CreateSimplePrefab("Test_HitEffect");

        if (destructionVfx == null)
            destructionVfx = CreateSimplePrefab("Test_Enemy_DestructionVFX");

        enemy.hitEffect = hitEffect;
        enemy.destructionVFX = destructionVfx;

        return enemyObject;
    }

    public static GameObject CreateProjectile(
        int damage,
        bool enemyBullet,
        bool destroyedByCollision)
    {
        GameObject projectileObject = new GameObject("Test_Projectile");

        Projectile projectile = projectileObject.AddComponent<Projectile>();
        projectile.damage = damage;
        projectile.enemyBullet = enemyBullet;
        projectile.destroyedByCollision = destroyedByCollision;

        return projectileObject;
    }

    public static GameObject CreateBonus()
    {
        GameObject bonus = new GameObject("Test_Bonus");

        CircleCollider2D collider = bonus.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;

        bonus.tag = "Bonus";

        bonus.AddComponent<Bonus>();

        return bonus;
    }

    public static GameObject CreateTriggerPlayer()
    {
        GameObject player = CreatePlayerWithShooting();

        CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
        collider.isTrigger = false;

        Rigidbody2D body = player.AddComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Kinematic;

        return player;
    }

    public static GameObject CreatePoolingPrefab(string name)
    {
        GameObject prefab = new GameObject(name);
        return prefab;
    }
}