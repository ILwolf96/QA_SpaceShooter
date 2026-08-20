using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Serializable classes

[System.Serializable]
public class EnemyWaves
{
    [Tooltip("time for wave generation from the moment the game started")]
    public float timeToStart;

    [Tooltip("Enemy wave's prefab")]
    public GameObject wave;
}

#endregion

public class LevelController : MonoBehaviour
{
    public EnemyWaves[] enemyWaves;

    public GameObject powerUp;
    public float timeForNewPowerup;

    public GameObject[] planets;
    public float timeBetweenPlanets;
    public float planetsSpeed;

    private readonly List<GameObject> planetsList =
        new List<GameObject>();

    private Camera mainCamera;

    private bool levelRunning;

    private void Start()
    {
        StartLevel();
    }

    /// <summary>
    /// Starts the configured level.
    /// </summary>
    public void StartLevel()
    {
        StopLevel();

        mainCamera = Camera.main;
        levelRunning = true;

        if (enemyWaves != null)
        {
            for (int i = 0; i < enemyWaves.Length; i++)
            {
                if (enemyWaves[i] == null ||
                    enemyWaves[i].wave == null)
                    continue;

                StartCoroutine(
                    CreateEnemyWave(
                        enemyWaves[i].timeToStart,
                        enemyWaves[i].wave));
            }
        }

        if (powerUp != null)
            StartCoroutine(PowerupBonusCreation());

        if (planets != null &&
            planets.Length > 0)
        {
            StartCoroutine(PlanetsCreation());
        }
    }

    /// <summary>
    /// Stops all level-controller spawning activity.
    /// </summary>
    public void StopLevel()
    {
        levelRunning = false;

        StopAllCoroutines();

        planetsList.Clear();
    }

    IEnumerator CreateEnemyWave(
        float delay,
        GameObject Wave)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);

        if (!levelRunning)
            yield break;

        if (Player.instance != null &&
            Wave != null)
        {
            Instantiate(Wave);
        }
    }

    IEnumerator PowerupBonusCreation()
    {
        while (levelRunning)
        {
            yield return new WaitForSeconds(
                timeForNewPowerup);

            if (!levelRunning)
                yield break;

            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null ||
                PlayerMoving.instance == null ||
                powerUp == null)
                continue;

            Renderer powerUpRenderer =
                powerUp.GetComponent<Renderer>();

            if (powerUpRenderer == null)
                continue;

            Instantiate(
                powerUp,
                new Vector2(
                    Random.Range(
                        PlayerMoving.instance.borders.minX,
                        PlayerMoving.instance.borders.maxX),

                    mainCamera.ViewportToWorldPoint(
                        Vector2.up).y
                    +
                    powerUpRenderer.bounds.size.y / 2),

                Quaternion.identity);
        }
    }

    IEnumerator PlanetsCreation()
    {
        planetsList.Clear();

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != null)
                planetsList.Add(planets[i]);
        }

        yield return new WaitForSeconds(10);

        while (levelRunning)
        {
            if (planetsList.Count == 0)
            {
                RebuildPlanetList();

                if (planetsList.Count == 0)
                    yield break;
            }

            int randomIndex =
                Random.Range(
                    0,
                    planetsList.Count);

            GameObject newPlanet =
                Instantiate(
                    planetsList[randomIndex]);

            planetsList.RemoveAt(randomIndex);

            DirectMoving moving =
                newPlanet.GetComponent<DirectMoving>();

            if (moving != null)
                moving.speed = planetsSpeed;

            yield return new WaitForSeconds(
                timeBetweenPlanets);
        }
    }

    private void RebuildPlanetList()
    {
        planetsList.Clear();

        if (planets == null)
            return;

        for (int i = 0; i < planets.Length; i++)
        {
            if (planets[i] != null)
                planetsList.Add(planets[i]);
        }
    }
}