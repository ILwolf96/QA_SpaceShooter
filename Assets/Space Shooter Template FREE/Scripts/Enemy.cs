using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script defines 'Enemy's' health and behavior.
/// </summary>
public class Enemy : MonoBehaviour
{
    #region FIELDS

    [Tooltip("Health points in integer")]
    public int health;

    [Tooltip("Enemy's projectile prefab")]
    public GameObject Projectile;

    [Tooltip("VFX prefab generating after destruction")]
    public GameObject destructionVFX;

    public GameObject hitEffect;

    [HideInInspector]
    public int shotChance;

    [HideInInspector]
    public float shotTimeMin, shotTimeMax;

    #endregion

    private EnemyShield enemyShield;

    private void Awake()
    {
        enemyShield = GetComponent<EnemyShield>();
    }

    private void Start()
    {
        Invoke(
            nameof(ActivateShooting),
            Random.Range(shotTimeMin, shotTimeMax));
    }

    // Coroutine making a shot
    void ActivateShooting()
    {
        if (Random.value < (float)shotChance / 100)
        {
            Instantiate(
                Projectile,
                gameObject.transform.position,
                Quaternion.identity);
        }
    }

    /// <summary>
    /// Processes incoming damage.
    ///
    /// If the Enemy has an active shield, the shield absorbs the damage
    /// first. Any remaining damage is then applied to Enemy health.
    ///
    /// Enemies without an EnemyShield retain the original behavior.
    /// </summary>
    public void GetDamage(int damage)
    {
        if (damage <= 0)
            return;

        if (enemyShield == null)
            enemyShield = GetComponent<EnemyShield>();

        int remainingDamage = damage;

        if (enemyShield != null && enemyShield.IsActive)
        {
            remainingDamage = enemyShield.AbsorbDamage(damage);
        }

        // Shield absorbed all incoming damage.
        if (remainingDamage <= 0)
            return;

        health -= remainingDamage;

        if (health <= 0)
            Destruction();
        else
            Instantiate(
                hitEffect,
                transform.position,
                Quaternion.identity,
                transform);
    }

    // If Enemy collides with Player, Player gets damage equal
    // to Projectile's damage value.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (Projectile != null &&
                Projectile.GetComponent<Projectile>() != null)
            {
                Player.instance.GetDamage(
                    Projectile.GetComponent<Projectile>().damage);
            }
            else
            {
                Player.instance.GetDamage(1);
            }
        }
    }

    // Method of destroying the Enemy
    void Destruction()
    {
        Instantiate(
            destructionVFX,
            transform.position,
            Quaternion.identity);

        Destroy(gameObject);
    }
}