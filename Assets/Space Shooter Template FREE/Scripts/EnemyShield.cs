using UnityEngine;

/// <summary>
/// TDD placeholder for Enemy Shield Defense.
///
/// This class intentionally contains only the API required by the
/// first Shield tests.
/// The actual Shield behavior will be implemented
/// after the tests are written and verified to fail.
/// </summary>
public class EnemyShield : MonoBehaviour
{
    [Tooltip("Shield hit points.")]
    public int shieldHealth = 10;

    public bool IsActive => shieldHealth > 0;

    /// <summary>
    /// Receives shield damage.
    ///
    /// This allows the tests to compile and fail for behavioral reasons.
    /// </summary>
    public int AbsorbDamage(int damage)
    {
        return damage;
    }

    /// <summary>
    /// Returns the amount of shield damage that can currently be absorbed.
    /// </summary>
    public void Initialize(int health)
    {
        shieldHealth = health;
    }
}