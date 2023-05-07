using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCheckPoints : MonoBehaviour
{
    public GameObject objectPrefab;
    public float spacing;

    [HideInInspector] public bool AreCheckpointsReady;

    private int numObjects;
    private List<Vector3> pathPoints;
    private float pathLength;
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
        pathLength = CalculatePathLength();
        numObjects = Mathf.FloorToInt(pathLength / spacing);
        GenerateObjects();
    }

    private float CalculatePathLength()
    {
        float length = 0;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            length += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        return length;
    }

    private void GenerateObjects()
    {
        float spacingInterval = pathLength / (numObjects - 1);
        float distanceAlongPath = spacingInterval;
        GameObject last = null;

        for (int i = 1; i < numObjects; i++)
        {
            Vector3 position = GetPositionOnPath(distanceAlongPath);
            Quaternion rotation = GetRotationOnPath(distanceAlongPath);
            last = Instantiate(objectPrefab, position + objectPrefab.transform.position, rotation, parentTransform);
            distanceAlongPath += spacingInterval;
        }

        last.tag = "End CheckPoint";
        AreCheckpointsReady = true;
    }

    private Vector3 GetPositionOnPath(float distanceAlongPath)
    {
        return GetLinearlyInterpolatedPoint(distanceAlongPath);
    }

    private Vector3 GetLinearlyInterpolatedPoint(float distanceAlongPath)
    {
        float distance = 0;
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            Vector3 pointA = pathPoints[i];
            Vector3 pointB = pathPoints[i + 1];
            float segmentLength = Vector3.Distance(pointA, pointB);
            if (distance + segmentLength >= distanceAlongPath)
            {
                float t = (distanceAlongPath - distance) / segmentLength;
                return Vector3.Lerp(pointA, pointB, t);
            }
            distance += segmentLength;
        }
        return pathPoints[^1];
    }

    private Quaternion GetRotationOnPath(float distanceAlongPath)
    {
        Vector3 forwardVector;
        forwardVector = GetCatmullRomInterpolatedForwardVector(distanceAlongPath);
        
        return Quaternion.LookRotation(forwardVector);
    }

    private Vector3 GetCatmullRomInterpolatedForwardVector(float distanceAlongPath)
    {
        float totalDistance = 0;
        List<float> distances = new()
        {
            0
        };
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            float segmentLength = Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
            totalDistance += segmentLength;
            distances.Add(totalDistance);
        }
        distances.Add(totalDistance);

        int currentIndex = 0;
        for (int i = 0; i < distances.Count - 1; i++)
        {
            if (distanceAlongPath >= distances[i] && distanceAlongPath < distances[i + 1])
            {
                currentIndex = i;
                break;
            }
        }

        float t = (distanceAlongPath - distances[currentIndex]) / (distances[currentIndex + 1] - distances[currentIndex]);
        Vector3[] points = new Vector3[4];
        int indexOffset = -1;
        for (int i = 0; i < 4; i++)
        {
            int index = currentIndex + indexOffset;
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= pathPoints.Count)
            {
                index = pathPoints.Count - 1;
            }
            points[i] = pathPoints[index];
            indexOffset++;
        }

        Vector3 forwardVector = GetCatmullRomInterpolatedTangentVector(points[0], points[1], points[2], points[3], t);
        return forwardVector;
    }

    private Vector3 GetCatmullRomInterpolatedTangentVector(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;

        Vector3 tangentVector = 0.5f * ((-p0 + p2) +
                                        (2f * p0 - 5f * p1 + 4f * p2 - p3) * t +
                                        (-p0 + 3f * p1 - 3f * p2 + p3) * t2);
        return tangentVector.normalized;
    }

}
