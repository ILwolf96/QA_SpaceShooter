using UnityEngine;

/// <summary>
/// Controls Level 2-specific configuration, wave selection,
/// shielded enemy configuration and Boss spawning.
/// </summary>
public class Level2Controller : MonoBehaviour
{
    [Header("Level 2 Wave Pool")]
    [Tooltip("Wave prefabs available for Level 2.")]
    public GameObject[] wavePool;

    [Header("Difficulty")]
    [Tooltip("Multiplier applied to Level 2 difficulty.")]
    [Min(1f)]
    public float difficultyMultiplier = 1.5f;

    [Header("Shielded Enemy")]
    [Tooltip("Enemy prefab containing EnemyShield.")]
    public GameObject shieldedEnemyPrefab;

    [Header("Boss")]
    [Tooltip("Boss prefab used in Level 2.")]
    public GameObject bossPrefab;

    [Tooltip("Boss spawn position in world space.")]
    public Transform bossSpawnPoint;

    public GameObject SelectRandomWave()
    {
        if (wavePool == null || wavePool.Length == 0)
            return null;

        return wavePool[
            Random.Range(0, wavePool.Length)];
    }

    public bool IsHarderThanNormal(float baseValue)
    {
        return difficultyMultiplier * baseValue > baseValue;
    }

    public GameObject GetShieldedEnemyPrefab()
    {
        return shieldedEnemyPrefab;
    }

    public GameObject GetBossPrefab()
    {
        return bossPrefab;
    }

    /// <summary>
    /// Spawns the configured Boss for Level 2.
    /// </summary>
    public GameObject SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning(
                "Level2Controller: Boss prefab is not assigned.");

            return null;
        }

        Vector3 spawnPosition =
            bossSpawnPoint != null
                ? bossSpawnPoint.position
                : transform.position;

        return Instantiate(
            bossPrefab,
            spawnPosition,
            Quaternion.identity);
    }
}