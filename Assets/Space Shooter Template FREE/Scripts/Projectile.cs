using UnityEngine;

/// <summary>
/// Defines projectile damage and whether the projectile belongs
/// to the Enemy or Player.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Tooltip("Damage which a projectile deals to another object.")]
    public int damage;

    [Tooltip("Whether the projectile belongs to the Enemy or Player.")]
    public bool enemyBullet;

    [Tooltip("Whether the projectile is destroyed on collision.")]
    public bool destroyedByCollision;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyBullet)
        {
            if (collision.CompareTag("Player"))
            {
                if (Player.instance != null)
                {
                    Player.instance.GetDamage(damage);
                }

                if (destroyedByCollision)
                    Destruction();
            }

            return;
        }

        // Player projectile → normal Enemy
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy =
                collision.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.GetDamage(damage);

                if (destroyedByCollision)
                    Destruction();
            }

            return;
        }

        // Player projectile → Boss
        if (collision.CompareTag("Boss"))
        {
            Boss boss =
                collision.GetComponent<Boss>();

            if (boss != null)
            {
                boss.GetDamage(damage);

                if (destroyedByCollision)
                    Destruction();
            }
        }
    }

    private void Destruction()
    {
        Destroy(gameObject);
    }
}