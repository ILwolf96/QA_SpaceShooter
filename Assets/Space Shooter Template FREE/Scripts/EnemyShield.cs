using UnityEngine;

/// <summary>
/// Handles optional shield defense for an Enemy.
///
/// The shield absorbs incoming damage before the Enemy's normal
/// health is affected. A visual shield prefab can be instantiated
/// while the shield is active and removed when the shield breaks.
/// </summary>
public class EnemyShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Starting shield hit points.")]
    [Min(0)]
    public int shieldHealth = 10;

    [Tooltip("Prefab used as the visual representation of the shield.")]
    public GameObject shieldVisualPrefab;

    private GameObject shieldVisualInstance;

    /// <summary>
    /// Returns true while the shield has remaining health.
    /// </summary>
    public bool IsActive => shieldHealth > 0;

    private void Awake()
    {
        CreateShieldVisual();
    }

    /// <summary>
    /// Initializes or resets the shield to the supplied amount.
    /// </summary>
    public void Initialize(int health)
    {
        shieldHealth = Mathf.Max(0, health);

        UpdateShieldVisual();
    }

    /// <summary>
    /// Applies damage to the shield and returns any damage
    /// that the shield could not absorb.
    /// </summary>
    public int AbsorbDamage(int damage)
    {
        if (damage <= 0)
            return 0;

        if (!IsActive)
            return damage;

        int absorbedDamage =
            Mathf.Min(shieldHealth, damage);

        shieldHealth -= absorbedDamage;

        UpdateShieldVisual();

        return damage - absorbedDamage;
    }

    private void CreateShieldVisual()
    {
        if (!IsActive)
            return;

        if (shieldVisualPrefab == null)
            return;

        if (shieldVisualInstance != null)
            return;

        shieldVisualInstance =
            Instantiate(
                shieldVisualPrefab,
                transform.position,
                Quaternion.identity,
                transform);

        shieldVisualInstance.transform.localPosition = Vector3.zero;
        shieldVisualInstance.transform.localRotation = Quaternion.identity;
    }

    private void UpdateShieldVisual()
    {
        if (IsActive)
        {
            if (shieldVisualInstance == null)
                CreateShieldVisual();
        }
        else
        {
            DestroyShieldVisual();
        }
    }

    private void DestroyShieldVisual()
    {
        if (shieldVisualInstance != null)
        {
            Destroy(shieldVisualInstance);
            shieldVisualInstance = null;
        }
    }

    private void OnDestroy()
    {
        if (shieldVisualInstance != null)
            Destroy(shieldVisualInstance);
    }
}