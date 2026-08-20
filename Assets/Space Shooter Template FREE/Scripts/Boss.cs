using UnityEngine;

/// <summary>
/// Controls the Boss ship's health, movement, shooting and destruction.
/// </summary>
public class Boss : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Maximum Boss health.")]
    [Min(1)]
    public int maxHealth = 5000;

    [HideInInspector]
    public int health;

    [Header("Projectile")]
    [Tooltip("Projectile prefab fired by the Boss.")]
    public GameObject Projectile;

    [Header("Visual Effects")]
    [Tooltip("Effect created when the Boss is destroyed.")]
    public GameObject destructionVFX;

    [Tooltip("Effect created when the Boss receives non-lethal damage.")]
    public GameObject hitEffect;

    [Header("Shooting")]
    [Range(0, 100)]
    [Tooltip("Probability of firing when the shooting attempt occurs.")]
    public int shotChance = 50;

    [Tooltip("Minimum time before a shooting attempt.")]
    public float shotTimeMin = 2f;

    [Tooltip("Maximum time before a shooting attempt.")]
    public float shotTimeMax = 4f;

    [Header("Movement")]
    [Tooltip("Boss movement speed.")]
    [Min(0f)]
    public float movementSpeed = 5f;

    [Tooltip("Boss movement direction.")]
    public Vector2 movementDirection = Vector2.zero;

    [Header("Arena Bounds")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    public bool IsAlive => health > 0;

    private void Awake()
    {
        ResetHealth();
    }

    private void Start()
    {
        if (shotTimeMin <= shotTimeMax)
        {
            Invoke(
                nameof(AttemptToShoot),
                Random.Range(shotTimeMin, shotTimeMax));
        }
    }

    private void Update()
    {
        if (!IsAlive)
            return;

        Move();
        ClampToArena();
    }

    public void ResetHealth()
    {
        health = Mathf.Max(1, maxHealth);
    }

    public void GetDamage(int damage)
    {
        if (damage <= 0 || !IsAlive)
            return;

        health = Mathf.Max(
            0,
            health - damage);

        if (health <= 0)
        {
            Destruction();
        }
        else if (hitEffect != null)
        {
            Instantiate(
                hitEffect,
                transform.position,
                Quaternion.identity,
                transform);
        }
    }

    private void AttemptToShoot()
    {
        if (!IsAlive)
            return;

        if (Projectile != null &&
            Random.value < (float)shotChance / 100f)
        {
            Instantiate(
                Projectile,
                transform.position,
                Quaternion.identity);
        }
    }

    private void Move()
    {
        Vector2 direction =
            movementDirection.sqrMagnitude > 1f
                ? movementDirection.normalized
                : movementDirection;

        transform.position +=
            new Vector3(
                direction.x,
                direction.y,
                0f)
            * movementSpeed
            * Time.deltaTime;
    }

    private void ClampToArena()
    {
        Vector3 position =
            transform.position;

        position.x =
            Mathf.Clamp(
                position.x,
                minX,
                maxX);

        position.y =
            Mathf.Clamp(
                position.y,
                minY,
                maxY);

        position.z = 0f;

        transform.position = position;
    }

    private void Destruction()
    {
        if (destructionVFX != null)
        {
            Instantiate(
                destructionVFX,
                transform.position,
                Quaternion.identity);
        }

        Destroy(gameObject);
    }
}