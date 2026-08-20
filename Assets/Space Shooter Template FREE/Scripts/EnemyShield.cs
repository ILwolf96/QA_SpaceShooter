using UnityEngine;

/// <summary>
/// Handles optional shield defense for an Enemy.
///
/// Damage is absorbed by the shield first. Any damage that exceeds
/// the remaining shield health is returned to the caller so that
/// the Enemy can apply it to its normal health.
/// </summary>
public class EnemyShield : MonoBehaviour
{
    [Header("Shield Settings")]
    [Tooltip("Maximum/current shield hit points.")]
    [Min(0)]
    public int shieldHealth = 10;

    /// <summary>
    /// Returns true while the shield has remaining health.
    /// </summary>
    public bool IsActive => shieldHealth > 0;

    /// <summary>
    /// Initializes or resets the shield to the supplied amount.
    /// </summary>
    public void Initialize(int health)
    {
        shieldHealth = Mathf.Max(0, health);
    }

    /// <summary>
    /// Applies incoming damage to the shield.
    ///
    /// Returns any damage that the shield could not absorb.
    /// </summary>
    public int AbsorbDamage(int damage)
    {
        if (damage <= 0)
            return 0;

        if (!IsActive)
            return damage;

        int absorbedDamage = Mathf.Min(shieldHealth, damage);

        shieldHealth -= absorbedDamage;

        return damage - absorbedDamage;
    }
}