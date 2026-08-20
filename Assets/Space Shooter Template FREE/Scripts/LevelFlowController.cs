using UnityEngine;

/// <summary>
/// Controls progression between Level 1, Level 2 and final victory.
/// Also exposes explicit methods used by the development/manual-test cheats.
/// </summary>
public class LevelFlowController : MonoBehaviour
{
    [Header("Level State")]
    [SerializeField]
    private int currentLevel = 1;

    [Header("Level Objects")]
    [Tooltip("Optional Level 1 root/controller object.")]
    public GameObject level1Object;

    [Tooltip("Optional Level 2 root/controller object.")]
    public GameObject level2Object;

    public bool Level2Started { get; private set; }

    public bool Level2Completed { get; private set; }

    public bool GameCompleted { get; private set; }

    public int CurrentLevel => currentLevel;

    /// <summary>
    /// Starts Level 1.
    /// </summary>
    public void StartLevel1()
    {
        currentLevel = 1;

        Level2Started = false;
        Level2Completed = false;
        GameCompleted = false;

        if (level1Object != null)
            level1Object.SetActive(true);

        if (level2Object != null)
            level2Object.SetActive(false);
    }

    /// <summary>
    /// Completes Level 1 and starts Level 2.
    /// </summary>
    public void CompleteLevel1()
    {
        if (currentLevel != 1)
            return;

        StartLevel2();
    }

    /// <summary>
    /// Starts Level 2.
    /// </summary>
    public void StartLevel2()
    {
        currentLevel = 2;

        Level2Started = true;
        Level2Completed = false;
        GameCompleted = false;

        if (level1Object != null)
            level1Object.SetActive(false);

        if (level2Object != null)
            level2Object.SetActive(true);
    }

    /// <summary>
    /// Completes Level 2 and finishes the game.
    /// </summary>
    public void CompleteLevel2()
    {
        if (currentLevel != 2 || !Level2Started)
            return;

        Level2Completed = true;
        GameCompleted = true;
    }
}