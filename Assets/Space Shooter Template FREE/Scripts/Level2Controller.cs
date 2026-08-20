using UnityEngine;

/// <summary>
/// Controls Level 2-specific configuration and wave selection.
///
/// Level 2 reuses the existing Wave and LevelController systems.
/// This component provides:
/// - the pool of available Level 2 waves
/// - harder difficulty settings
/// - shielded enemy configuration
/// - Boss configuration
/// </summary>
public class Level2Controller : MonoBehaviour
{
    [Header("Level 2 Wave Pool")]
    [Tooltip("Wave prefabs available for random Level 2 selection.")]
    public GameObject[] wavePool;

    [Header("Difficulty")]
    [Tooltip("Multiplier applied to Level 2 difficulty.")]
    [Min(1f)]
    public float difficultyMultiplier = 1.5f;

    [Header("Shielded Enemy")]
    [Tooltip("Enemy prefab containing EnemyShield.")]
    public GameObject shieldedEnemyPrefab;

    [Header("Boss")]
    [Tooltip("Boss prefab used at the end of Level 2.")]
    public GameObject bossPrefab;

    /// <summary>
    /// Selects one wave randomly from the configured Level 2 wave pool.
    /// </summary>
    public GameObject SelectRandomWave()
    {
        if (wavePool == null || wavePool.Length == 0)
            return null;

        return wavePool[Random.Range(0, wavePool.Length)];
    }

    /// <summary>
    /// Determines whether Level 2 difficulty is greater than the
    /// supplied normal baseline.
    /// </summary>
    public bool IsHarderThanNormal(float baseValue)
    {
        return difficultyMultiplier * baseValue > baseValue;
    }

    /// <summary>
    /// Returns the configured shielded enemy prefab.
    /// </summary>
    public GameObject GetShieldedEnemyPrefab()
    {
        return shieldedEnemyPrefab;
    }

    /// <summary>
    /// Returns the configured Boss prefab.
    /// </summary>
    public GameObject GetBossPrefab()
    {
        return bossPrefab;
    }
}