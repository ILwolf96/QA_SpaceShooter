using UnityEngine;

/// <summary>
/// Controls the Boss ship's health and movement.
/// </summary>
public class Boss : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Maximum Boss health.")]
    [Min(1)]
    public int maxHealth = 5000;

    [HideInInspector]
    public int health;

    [Header("Movement")]
    [Tooltip("Boss movement speed.")]
    [Min(0f)]
    public float movementSpeed = 5f;

    [Tooltip("Boss movement direction. Horizontal, vertical, or combined.")]
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
            Destruction();
    }

    private void Move()
    {
        Vector2 normalizedDirection =
            movementDirection.sqrMagnitude > 1f
                ? movementDirection.normalized
                : movementDirection;

        Vector3 movement =
            new Vector3(
                normalizedDirection.x,
                normalizedDirection.y,
                0f)
            * movementSpeed
            * Time.deltaTime;

        transform.position += movement;
    }

    private void ClampToArena()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            minX,
            maxX);

        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY);

        position.z = 0f;

        transform.position = position;
    }

    private void Destruction()
    {
        Destroy(gameObject);
    }
}