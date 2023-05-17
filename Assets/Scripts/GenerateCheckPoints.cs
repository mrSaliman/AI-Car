using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCheckPoints : MonoBehaviour
{
    public GameObject objectPrefab;
    public int checkpointsPerSegment = 5;

    [HideInInspector] public bool AreCheckpointsReady;

    private List<Vector3> pathPoints;
    private RoomTemplates roomTemplates;
    private Transform parentTransform;

    private IEnumerator Start()
    {
        AreCheckpointsReady = false;
        roomTemplates = GetComponent<RoomTemplates>();
        parentTransform = transform.Find("CheckPoints");

        while (!roomTemplates.IsGenerationDone)
            yield return null;
        Debug.Log("Generating checkpoints...");

        pathPoints = new List<Vector3>();
        var rightWay = roomTemplates.GetRightWay();
        foreach (Transform t in rightWay)
        {
            pathPoints.Add(t.position);
        }
        GenerateObjects();
    }

    private void GenerateObjects()
    {
        GameObject last = null;

        //adjust start checkpoints
        Vector3 p0 = pathPoints[0];
        Vector3 p2 = (pathPoints[1] + pathPoints[0]) / 2;
        Vector3 p1 = (p0 + p2) / 2;

        for (int j = 1; j < checkpointsPerSegment / 2; j++)
        {
            float t = (float)j / (checkpointsPerSegment / 2);
            Vector3 checkpointPosition = BezierCurve(p0, p1, p2, t);
            Vector3 tangent = BezierCurveTangent(p0, p1, p2, t);
            Quaternion rotation = Quaternion.LookRotation(tangent);
            Instantiate(objectPrefab, checkpointPosition + objectPrefab.transform.position, rotation, parentTransform);
        }

        for (int i = 0; i < pathPoints.Count - 2; i++)
        {
            p1 = pathPoints[i + 1];
            p0 = (pathPoints[i] + p1) / 2;
            p2 = (pathPoints[i + 2] + p1) / 2;

            for (int j = 0; j < checkpointsPerSegment; j++)
            {
                float t = (float)j / checkpointsPerSegment;
                Vector3 checkpointPosition = BezierCurve(p0, p1, p2, t);
                Vector3 tangent = BezierCurveTangent(p0, p1, p2, t);
                Quaternion rotation = Quaternion.LookRotation(tangent);
                Instantiate(objectPrefab, checkpointPosition + objectPrefab.transform.position, rotation, parentTransform);
            }
        }

        p0 = pathPoints[^2];
        p2 = pathPoints[^1];
        p1 = (p0 + p2) / 2;

        for (int j = checkpointsPerSegment / 2; j <= checkpointsPerSegment; j++)
        {
            float t = (float)j / checkpointsPerSegment;
            Vector3 checkpointPosition = BezierCurve(p0, p1, p2, t);
            Vector3 tangent = BezierCurveTangent(p0, p1, p2, t);
            Quaternion rotation = Quaternion.LookRotation(tangent);
            last = Instantiate(objectPrefab, checkpointPosition + objectPrefab.transform.position, rotation, parentTransform);
        }

        last.tag = "End CheckPoint";
        AreCheckpointsReady = true;
    }

    public static Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = p1;
        Vector3 c = Vector3.Lerp(p1, p2, t);
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(d, e, t);
    }

    public static Vector3 BezierCurveTangent(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }
}
