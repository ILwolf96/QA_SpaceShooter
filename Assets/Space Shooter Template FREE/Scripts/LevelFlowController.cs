using UnityEngine;

/// <summary>
/// Controls progression between the game's levels.
///
/// RED-phase TDD shell:
/// the public API exists so the integration tests can compile,
/// but the actual level-transition behavior is intentionally incomplete.
/// </summary>
public class LevelFlowController : MonoBehaviour
{
    [Header("Levels")]
    public int currentLevel = 1;

    public bool Level2Started { get; private set; }

    public bool Level2Completed { get; private set; }

    public bool GameCompleted { get; private set; }

    /// <summary>
    /// Marks Level 1 as completed.
    /// RED-phase implementation intentionally does not start Level 2.
    /// </summary>
    public void CompleteLevel1()
    {
        // Intentionally incomplete for TDD RED phase.
    }

    /// <summary>
    /// Marks Level 2 as completed.
    /// RED-phase implementation intentionally does not complete the game.
    /// </summary>
    public void CompleteLevel2()
    {
        // Intentionally incomplete for TDD RED phase.
    }
}