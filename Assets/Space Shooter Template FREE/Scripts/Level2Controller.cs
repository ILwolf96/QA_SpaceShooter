using UnityEngine;

/// <summary>
/// Level 2-specific configuration and wave selection.
///
/// RED-phase TDD shell.
/// </summary>
public class Level2Controller : MonoBehaviour
{
    [Header("Level 2 Wave Pool")]
    [Tooltip("Wave prefabs that Level 2 may select from.")]
    public GameObject[] wavePool;

    [Header("Difficulty")]
    [Min(1f)]
    public float difficultyMultiplier = 1.5f;

    [Header("Shielded Enemy")]
    public GameObject shieldedEnemyPrefab;

    [Header("Boss")]
    public GameObject bossPrefab;

    public GameObject SelectRandomWave()
    {
        // Intentionally incomplete for RED phase.
        return null;
    }

    public bool IsHarderThanNormal(float baseValue)
    {
        // Intentionally incomplete for RED phase.
        return false;
    }

    public GameObject GetShieldedEnemyPrefab()
    {
        // Intentionally incomplete for RED phase.
        return null;
    }

    public GameObject GetBossPrefab()
    {
        // Intentionally incomplete for RED phase.
        return null;
    }
}