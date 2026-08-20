using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
    [Header("Controllers")]
    public LevelController levelController;
    public Level2Controller level2Controller;

    [Header("Level State")]
    [SerializeField]
    private int currentLevel = 1;

    public bool Level2Started { get; private set; }

    public bool Level2Completed { get; private set; }

    public bool GameCompleted { get; private set; }

    public int CurrentLevel =>
        currentLevel;

    private void Awake()
    {
        if (levelController == null)
        {
            levelController =
                GetComponentInChildren<LevelController>();
        }

        if (level2Controller == null)
        {
            level2Controller =
                GetComponentInChildren<Level2Controller>();
        }
    }

    private void Start()
    {
        currentLevel = 1;
        Level2Started = false;
        Level2Completed = false;
        GameCompleted = false;

        if (level2Controller != null)
            level2Controller.StopLevel();

        if (level2Controller != null)
            level2Controller.LevelCompleted +=
                HandleLevel2Completed;
    }

    private void OnDestroy()
    {
        if (level2Controller != null)
            level2Controller.LevelCompleted -=
                HandleLevel2Completed;
    }

    public void StartLevel1()
    {
        currentLevel = 1;

        Level2Started = false;
        Level2Completed = false;
        GameCompleted = false;

        if (level2Controller != null)
            level2Controller.StopLevel();

        if (levelController != null)
            levelController.StartLevel();
    }

    public void CompleteLevel1()
    {
        if (currentLevel != 1)
            return;

        StartLevel2();
    }

    public void StartLevel2()
    {
        currentLevel = 2;

        Level2Started = true;
        Level2Completed = false;
        GameCompleted = false;

        if (levelController != null)
            levelController.StopLevel();

        if (level2Controller != null)
            level2Controller.StartLevel();
    }

    public void CompleteLevel2()
    {
        if (currentLevel != 2 ||
            !Level2Started)
            return;

        Level2Completed = true;
        GameCompleted = true;
    }

    private void HandleLevel2Completed()
    {
        CompleteLevel2();
    }
}