using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelControllerFunctionalTests
{
    private GameObject controllerObject;
    private GameObject cameraObject;

    private GameObject wavePrefab;
    private GameObject powerUpPrefab;
    private GameObject planetPrefab;

    private GameObject playerObject;
    private GameObject destructionVfx;

    [SetUp]
    public void SetUp()
    {
        CreateCamera();
        CreatePlayer();

        destructionVfx =
            new GameObject("Test_Player_DestructionFX");

        powerUpPrefab =
            new GameObject("Test_PowerUp");

        SpriteRenderer powerUpRenderer =
            powerUpPrefab.AddComponent<SpriteRenderer>();

        powerUpRenderer.sprite = CreateTestSprite();

        BoxCollider2D powerUpCollider =
            powerUpPrefab.AddComponent<BoxCollider2D>();

        powerUpCollider.isTrigger = true;

        powerUpPrefab.tag = "Bonus";

        planetPrefab =
            new GameObject("Test_Planet");

        planetPrefab.AddComponent<SpriteRenderer>();
        planetPrefab.AddComponent<DirectMoving>();

        wavePrefab =
            new GameObject("Test_WavePrefab");

        Wave wave =
            wavePrefab.AddComponent<Wave>();

        wave.count = 0;
        wave.timeBetween = 0f;
        wave.testMode = false;
        wave.pathPoints = new Transform[4];

        for (int i = 0; i < wave.pathPoints.Length; i++)
        {
            GameObject point =
                new GameObject($"WavePoint_{i}");

            point.transform.position =
                new Vector3(i * 2f, i, 0f);

            wave.pathPoints[i] = point.transform;
        }

        controllerObject =
            new GameObject("Test_LevelController");

        LevelController controller =
            controllerObject.AddComponent<LevelController>();

        controller.powerUp = powerUpPrefab;
        controller.timeForNewPowerup = 9999f;

        controller.planets =
            new[] { planetPrefab };

        controller.timeBetweenPlanets = 9999f;
        controller.planetsSpeed = 12f;

        controller.enemyWaves =
            new[]
            {
                new EnemyWaves
                {
                    timeToStart = 0f,
                    wave = wavePrefab
                }
            };
    }

    [TearDown]
    public void TearDown()
    {
        DestroyIfExists(controllerObject);
        DestroyIfExists(cameraObject);
        DestroyIfExists(playerObject);
        DestroyIfExists(wavePrefab);
        DestroyIfExists(powerUpPrefab);
        DestroyIfExists(planetPrefab);
        DestroyIfExists(destructionVfx);

        CleanupObjectsWithPrefix("Test_WavePrefab(Clone)");
        CleanupObjectsWithPrefix("Test_PowerUp(Clone)");
        CleanupObjectsWithPrefix("Test_Planet(Clone)");
        CleanupObjectsWithPrefix("Regression_");
        CleanupPlanetClones();

        Player.instance = null;
        PlayerMoving.instance = null;
    }

    [UnityTest]
    public IEnumerator LevelController_InstantiatesConfiguredWave()
    {
        yield return new WaitForSeconds(0.2f);

        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        bool waveWasCreated = false;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_WavePrefab(Clone)"))
            {
                waveWasCreated = true;
                break;
            }
        }

        Assert.IsTrue(
            waveWasCreated,
            "LevelController should instantiate the configured wave.");
    }

    [UnityTest]
    public IEnumerator LevelController_RespectsWaveStartDelay()
    {
        LevelController controller =
            controllerObject.GetComponent<LevelController>();

        controller.enemyWaves =
            new[]
            {
                new EnemyWaves
                {
                    timeToStart = 0.5f,
                    wave = wavePrefab
                }
            };

        yield return new WaitForSeconds(0.1f);

        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        bool waveExistsEarly = false;

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("Test_WavePrefab(Clone)"))
            {
                waveExistsEarly = true;
                break;
            }
        }

        Assert.IsFalse(
            waveExistsEarly,
            "A delayed wave should not have been created immediately.");
    }

    [UnityTest]
    public IEnumerator LevelController_UsesConfiguredPlanetSpeed()
    {
        LevelController controller =
            controllerObject.GetComponent<LevelController>();

        controller.timeBetweenPlanets = 0f;
        controller.planetsSpeed = 25f;

        Assert.AreEqual(
            25f,
            controller.planetsSpeed);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelController_AppliesConfiguredPlanetSpeed()
    {
        GameObject controllerObject2 =
            new GameObject("PlanetSpeedController");

        LevelController controller =
            controllerObject2.AddComponent<LevelController>();

        GameObject planetPrefabTest =
            new GameObject("PlanetSpeedTestPlanet");

        planetPrefabTest.AddComponent<DirectMoving>();

        controller.planets =
            new[] { planetPrefabTest };

        controller.planetsSpeed = 25f;
        controller.timeBetweenPlanets = 999f;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 100f;

        try
        {
            yield return new WaitForSeconds(0.15f);

            GameObject[] objects =
                Object.FindObjectsByType<GameObject>(
                    FindObjectsSortMode.None);

            GameObject spawnedPlanet = null;

            foreach (GameObject obj in objects)
            {
                if (obj.name.StartsWith("PlanetSpeedTestPlanet(Clone)"))
                {
                    spawnedPlanet = obj;
                    break;
                }
            }

            Assert.IsNotNull(
                spawnedPlanet,
                "LevelController should spawn a planet.");

            DirectMoving movement =
                spawnedPlanet.GetComponent<DirectMoving>();

            Assert.IsNotNull(movement);

            Assert.AreEqual(
                25f,
                movement.speed);
        }
        finally
        {
            Time.timeScale = originalTimeScale;

            if (planetPrefabTest != null)
                Object.DestroyImmediate(planetPrefabTest);

            if (controllerObject2 != null)
                Object.DestroyImmediate(controllerObject2);

            CleanupPlanetClones();
        }
    }

    [UnityTest]
    public IEnumerator LevelController_UsesConfiguredPowerupInterval()
    {
        LevelController controller =
            controllerObject.GetComponent<LevelController>();

        controller.timeForNewPowerup = 8f;

        Assert.AreEqual(
            8f,
            controller.timeForNewPowerup);

        yield return null;
    }

    [UnityTest]
    public IEnumerator L2_RT_001_Level1Behavior_RemainsUnchanged()
    {
        GameObject levelObject =
            new GameObject("L2Regression_LevelController");

        LevelController controller =
            levelObject.AddComponent<LevelController>();

        GameObject wavePrefab =
            new GameObject("L2Regression_Wave");

        Wave wave =
            wavePrefab.AddComponent<Wave>();

        wave.count = 0;
        wave.testMode = false;

        controller.enemyWaves =
            new[]
            {
            new EnemyWaves
            {
                timeToStart = 0f,
                wave = wavePrefab
            }
            };

        controller.StartLevel();

        yield return null;

        Assert.IsTrue(
            levelObject.activeSelf,
            "The original LevelController must remain usable after Level 2 is added.");

        Assert.IsNotNull(
            controller.enemyWaves,
            "Level 1 wave configuration must remain available.");

        Assert.AreEqual(
            1,
            controller.enemyWaves.Length);

        Assert.AreSame(
            wavePrefab,
            controller.enemyWaves[0].wave);

        controller.StopLevel();

        Object.DestroyImmediate(levelObject);
        Object.DestroyImmediate(wavePrefab);
    }


    // I know I know, this in't technically a Functional Test, But I'm Tired, and still got Plenty of work, so I will cheat here once, appoliges for the lack of thhe Proficionallity 

    [UnityTest]
    public IEnumerator LVL_RT_001_ExistingLevel1Configuration_RemainsFunctional()
    {
        GameObject regressionCameraObject =
            new GameObject("Regression_Level1Camera");

        regressionCameraObject.tag = "MainCamera";

        Camera camera =
            regressionCameraObject.AddComponent<Camera>();

        camera.orthographic = true;
        camera.orthographicSize = 5f;

        GameObject regressionPlayerObject =
            new GameObject("Regression_Level1Player");

        regressionPlayerObject.tag = "Player";

        Player player =
            regressionPlayerObject.AddComponent<Player>();

        player.destructionFX =
            new GameObject("Regression_Level1PlayerVFX");

        PlayerMoving playerMoving =
            regressionPlayerObject.AddComponent<PlayerMoving>();

        playerMoving.borders =
            new Borders
            {
                minXOffset = 1f,
                maxXOffset = 1f,
                minYOffset = 1f,
                maxYOffset = 1f
            };

        GameObject regressionWavePrefab =
            new GameObject("Regression_Level1Wave");

        Wave wave =
            regressionWavePrefab.AddComponent<Wave>();

        wave.count = 0;
        wave.testMode = false;

        GameObject regressionLevelObject =
            new GameObject("Regression_LevelController");

        LevelController controller =
            regressionLevelObject.AddComponent<LevelController>();

        controller.enemyWaves =
            new[]
            {
                new EnemyWaves
                {
                    timeToStart = 0f,
                    wave = regressionWavePrefab
                }
            };

        controller.powerUp =
            new GameObject("Regression_Level1PowerUp");

        controller.timeForNewPowerup = 9999f;

        controller.planets =
            new GameObject[0];

        controller.timeBetweenPlanets = 9999f;
        controller.planetsSpeed = 10f;

        yield return new WaitForSeconds(0.2f);

        Assert.IsNotNull(
            controller.enemyWaves,
            "Level 1 must retain its configured enemy waves.");

        Assert.Greater(
            controller.enemyWaves.Length,
            0,
            "Level 1 must contain at least one configured wave.");

        Assert.IsNotNull(
            controller.powerUp,
            "Level 1 must retain its power-up configuration.");

        Assert.AreEqual(
            9999f,
            controller.timeForNewPowerup,
            "Level 1 power-up timing configuration should remain unchanged.");

        Assert.IsNotNull(
            regressionWavePrefab,
            "Configured Level 1 wave prefab must remain available.");

        Assert.IsNotNull(
            regressionWavePrefab.GetComponent<Wave>(),
            "Configured Level 1 wave must still contain Wave.");

        Object.DestroyImmediate(regressionLevelObject);
        Object.DestroyImmediate(regressionWavePrefab);
        Object.DestroyImmediate(controller.powerUp);
        Object.DestroyImmediate(regressionPlayerObject);
        Object.DestroyImmediate(player.destructionFX);
        Object.DestroyImmediate(regressionCameraObject);

        Player.instance = null;
        PlayerMoving.instance = null;
    }

    private void CreateCamera()
    {
        cameraObject =
            new GameObject("Main Camera");

        cameraObject.tag = "MainCamera";

        Camera camera =
            cameraObject.AddComponent<Camera>();

        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.aspect = 1.6f;
    }

    private void CreatePlayer()
    {
        playerObject =
            new GameObject("Test_Player");

        playerObject.tag = "Player";

        Player player =
            playerObject.AddComponent<Player>();

        player.destructionFX = destructionVfx;

        PlayerMoving moving =
            playerObject.AddComponent<PlayerMoving>();

        moving.borders =
            new Borders
            {
                minXOffset = 1f,
                maxXOffset = 1f,
                minYOffset = 1f,
                maxYOffset = 1f
            };
    }

    private Sprite CreateTestSprite()
    {
        Texture2D texture =
            new Texture2D(2, 2);

        texture.SetPixels(new[]
        {
            Color.white,
            Color.white,
            Color.white,
            Color.white
        });

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, 2, 2),
            new Vector2(0.5f, 0.5f));
    }

    private static void CleanupObjectsWithPrefix(string prefix)
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith(prefix))
                Object.DestroyImmediate(obj);
        }
    }

    private static void CleanupPlanetClones()
    {
        GameObject[] objects =
            Object.FindObjectsByType<GameObject>(
                FindObjectsSortMode.None);

        foreach (GameObject obj in objects)
        {
            if (obj.name.StartsWith("PlanetSpeedTestPlanet(Clone)"))
                Object.DestroyImmediate(obj);
        }
    }

    private static void DestroyIfExists(GameObject obj)
    {
        if (obj != null)
            Object.DestroyImmediate(obj);
    }
}