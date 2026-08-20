using UnityEngine;

/// <summary>
/// Development/manual-testing shortcuts for quickly switching levels
/// and spawning the Level 2 Boss.
///
/// Hold:
/// 1 for 3 seconds -> Level 1
/// 2 for 3 seconds -> Level 2
/// B for 3 seconds -> Spawn Level 2 Boss
///
/// This component is intended for development/testing convenience,
/// not normal gameplay.
/// </summary>
public class GameLevelCheatController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Controls Level 1/Level 2 progression.")]
    public LevelFlowController levelFlowController;

    [Tooltip("Controls Level 2 configuration and Boss spawning.")]
    public Level2Controller level2Controller;

    [Header("Cheat Settings")]
    [Tooltip("How long a key must be held.")]
    [Min(0.1f)]
    public float holdDuration = 3f;

    private float level1HoldTime;
    private float level2HoldTime;
    private float bossHoldTime;

    private void Awake()
    {
        if (levelFlowController == null)
            levelFlowController =
                GetComponent<LevelFlowController>();

        if (level2Controller == null)
            level2Controller =
                GetComponent<Level2Controller>();
    }

    private void Update()
    {
        HandleLevel1Cheat();
        HandleLevel2Cheat();
        HandleBossCheat();
    }

    private void HandleLevel1Cheat()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            level1HoldTime += Time.unscaledDeltaTime;

            if (level1HoldTime >= holdDuration)
            {
                StartLevel1();

                level1HoldTime = 0f;
            }
        }
        else
        {
            level1HoldTime = 0f;
        }
    }

    private void HandleLevel2Cheat()
    {
        if (Input.GetKey(KeyCode.Alpha2))
        {
            level2HoldTime += Time.unscaledDeltaTime;

            if (level2HoldTime >= holdDuration)
            {
                StartLevel2();

                level2HoldTime = 0f;
            }
        }
        else
        {
            level2HoldTime = 0f;
        }
    }

    private void HandleBossCheat()
    {
        if (levelFlowController == null ||
            levelFlowController.CurrentLevel != 2)
        {
            bossHoldTime = 0f;
            return;
        }

        if (Input.GetKey(KeyCode.B))
        {
            bossHoldTime += Time.unscaledDeltaTime;

            if (bossHoldTime >= holdDuration)
            {
                SpawnBoss();

                bossHoldTime = 0f;
            }
        }
        else
        {
            bossHoldTime = 0f;
        }
    }

    private void StartLevel1()
    {
        if (levelFlowController == null)
        {
            Debug.LogWarning(
                "GameLevelCheatController: LevelFlowController is not assigned.");

            return;
        }

        levelFlowController.StartLevel1();

        Debug.Log(
            "CHEAT: Level 1 started.");
    }

    private void StartLevel2()
    {
        if (levelFlowController == null)
        {
            Debug.LogWarning(
                "GameLevelCheatController: LevelFlowController is not assigned.");

            return;
        }

        levelFlowController.StartLevel2();

        Debug.Log(
            "CHEAT: Level 2 started.");
    }

    private void SpawnBoss()
    {
        if (level2Controller == null)
        {
            Debug.LogWarning(
                "GameLevelCheatController: Level2Controller is not assigned.");

            return;
        }

        GameObject boss =
            level2Controller.SpawnBoss();

        if (boss != null)
        {
            Debug.Log(
                "CHEAT: Level 2 Boss spawned.");
        }
    }
}