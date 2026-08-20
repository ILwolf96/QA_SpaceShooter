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
    [Tooltip("Probability of firing when a shooting attempt occurs.")]
    public int shotChance = 50;

    [Tooltip("Minimum time before a shooting attempt.")]
    public float shotTimeMin = 2f;

    [Tooltip("Maximum time before a shooting attempt.")]
    public float shotTimeMax = 4f;

    [Header("Movement")]
    [Tooltip("Boss movement speed.")]
    [Min(0f)]
    public float movementSpeed = 2.5f;

    [Tooltip("Initial movement direction. Use X/Y combinations for horizontal, vertical or diagonal movement.")]
    public Vector2 movementDirection = Vector2.right;

    [Tooltip("Automatically reverse direction when the Boss reaches an arena boundary.")]
    public bool bounceAtBounds = true;

    [Header("Arena Bounds")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -4f;
    public float maxY = 4f;

    public bool IsAlive => health > 0;

    private void Awake()
    {
        ResetHealth();

        if (movementDirection.sqrMagnitude <= 0.0001f)
        {
            movementDirection = Vector2.right;
        }
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
        ClampAndBounce();
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

    private void Move()
    {
        Vector2 direction =
            movementDirection.normalized;

        transform.position +=
            new Vector3(
                direction.x,
                direction.y,
                0f)
            * movementSpeed
            * Time.deltaTime;
    }

    private void ClampAndBounce()
    {
        Vector3 position = transform.position;

        bool hitHorizontalBoundary = false;
        bool hitVerticalBoundary = false;

        if (position.x <= minX)
        {
            position.x = minX;
            hitHorizontalBoundary = true;
        }
        else if (position.x >= maxX)
        {
            position.x = maxX;
            hitHorizontalBoundary = true;
        }

        if (position.y <= minY)
        {
            position.y = minY;
            hitVerticalBoundary = true;
        }
        else if (position.y >= maxY)
        {
            position.y = maxY;
            hitVerticalBoundary = true;
        }

        transform.position = position;

        if (!bounceAtBounds)
            return;

        Vector2 direction =
            movementDirection.normalized;

        if (hitHorizontalBoundary)
            direction.x *= -1f;

        if (hitVerticalBoundary)
            direction.y *= -1f;

        if (direction.sqrMagnitude > 0.0001f)
            movementDirection = direction;
    }

    private void AttemptToShoot()
    {
        if (!IsAlive)
            return;

        if (Projectile != null &&
            Random.value < shotChance / 100f)
        {
            Instantiate(
                Projectile,
                transform.position,
                Quaternion.identity);
        }

        float nextAttempt =
            Random.Range(
                shotTimeMin,
                shotTimeMax);

        Invoke(
            nameof(AttemptToShoot),
            nextAttempt);
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