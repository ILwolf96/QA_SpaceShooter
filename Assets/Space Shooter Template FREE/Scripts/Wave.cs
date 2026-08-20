using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Shooting
{
    [Range(0, 100)]
    [Tooltip("probability with which the ship of this wave will make a shot")]
    public int shotChance;

    [Tooltip("min and max time from the beginning of the path when the enemy can make a shot")]
    public float shotTimeMin, shotTimeMax;
}

public class Wave : MonoBehaviour
{
    #region FIELDS

    [Tooltip("Enemy's prefab")]
    public GameObject enemy;

    [Tooltip("a number of enemies in the wave")]
    public int count;

    [Tooltip("path passage speed")]
    public float speed;

    [Tooltip("time between emerging of the enemies in the wave")]
    public float timeBetween;

    [Tooltip("points of the path. delete or add elements to the list if you want to change the number of the points")]
    public Transform[] pathPoints;

    [Tooltip("whether 'Enemy' rotates in path passage direction")]
    public bool rotationByPath;

    [Tooltip("if loop is activated, after completing the path 'Enemy' will return to the starting point")]
    public bool Loop;

    [Tooltip("color of the path in the Editor")]
    public Color pathColor = Color.yellow;

    public Shooting shooting;

    [Tooltip("if testMode is marked the wave will be re-generated after 3 sec")]
    public bool testMode;

    #endregion

    public event Action<Wave> WaveCompleted;

    private bool completionReported;

    private void Start()
    {
        StartCoroutine(CreateEnemyWave());
    }

    IEnumerator CreateEnemyWave()
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newEnemy =
                Instantiate(
                    enemy,
                    enemy.transform.position,
                    Quaternion.identity);

            FollowThePath followComponent =
                newEnemy.GetComponent<FollowThePath>();

            followComponent.path = pathPoints;
            followComponent.speed = speed;
            followComponent.rotationByPath = rotationByPath;
            followComponent.loop = Loop;
            followComponent.SetPath();

            Enemy enemyComponent =
                newEnemy.GetComponent<Enemy>();

            enemyComponent.shotChance =
                shooting.shotChance;

            enemyComponent.shotTimeMin =
                shooting.shotTimeMin;

            enemyComponent.shotTimeMax =
                shooting.shotTimeMax;

            newEnemy.SetActive(true);

            yield return new WaitForSeconds(timeBetween);
        }

        if (testMode)
        {
            yield return new WaitForSeconds(3);
            StartCoroutine(CreateEnemyWave());
        }
        else if (!Loop)
        {
            ReportCompletion();
            Destroy(gameObject);
        }
    }

    private void ReportCompletion()
    {
        if (completionReported)
            return;

        completionReported = true;
        WaveCompleted?.Invoke(this);
    }

    private void OnDestroy()
    {
        WaveCompleted = null;
    }

    void OnDrawGizmos()
    {
        DrawPath(pathPoints);
    }

    void DrawPath(Transform[] path)
    {
        if (path == null || path.Length < 4)
            return;

        Vector3[] pathPositions =
            new Vector3[path.Length];

        for (int i = 0; i < path.Length; i++)
            pathPositions[i] = path[i].position;

        Vector3[] newPathPositions =
            CreatePoints(pathPositions);

        Vector3 previousPosition =
            Interpolate(newPathPositions, 0);

        Gizmos.color = pathColor;

        int smoothAmount =
            path.Length * 20;

        for (int i = 1; i <= smoothAmount; i++)
        {
            float t =
                (float)i / smoothAmount;

            Vector3 currentPosition =
                Interpolate(newPathPositions, t);

            Gizmos.DrawLine(
                currentPosition,
                previousPosition);

            previousPosition = currentPosition;
        }
    }

    Vector3 Interpolate(
        Vector3[] path,
        float t)
    {
        int numSections =
            path.Length - 3;

        int currPt =
            Mathf.Min(
                Mathf.FloorToInt(
                    t * numSections),
                numSections - 1);

        float u =
            t * numSections - currPt;

        Vector3 a = path[currPt];
        Vector3 b = path[currPt + 1];
        Vector3 c = path[currPt + 2];
        Vector3 d = path[currPt + 3];

        return 0.5f *
            ((-a + 3f * b - 3f * c + d) *
                (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) *
                (u * u)
            + (-a + c) * u
            + 2f * b);
    }

    Vector3[] CreatePoints(Vector3[] path)
    {
        Vector3[] newPathPos;

        int dist = 2;

        newPathPos =
            new Vector3[path.Length + dist];

        Array.Copy(
            path,
            0,
            newPathPos,
            1,
            path.Length);

        newPathPos[0] =
            newPathPos[1] +
            (newPathPos[1] - newPathPos[2]);

        newPathPos[newPathPos.Length - 1] =
            newPathPos[newPathPos.Length - 2] +
            (newPathPos[newPathPos.Length - 2] -
             newPathPos[newPathPos.Length - 3]);

        if (newPathPos[1] ==
            newPathPos[newPathPos.Length - 2])
        {
            Vector3[] loopSpline =
                new Vector3[newPathPos.Length];

            Array.Copy(
                newPathPos,
                loopSpline,
                newPathPos.Length);

            loopSpline[0] =
                loopSpline[loopSpline.Length - 3];

            loopSpline[loopSpline.Length - 1] =
                loopSpline[2];

            newPathPos =
                new Vector3[loopSpline.Length];

            Array.Copy(
                loopSpline,
                newPathPos,
                loopSpline.Length);
        }

        return newPathPos;
    }
}