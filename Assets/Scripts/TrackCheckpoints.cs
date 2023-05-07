using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler<CarCheckpointEventArgs> OnCarCorrectCheckpoint;
    public event EventHandler<CarCheckpointEventArgs> OnCarWrongCheckpoint;

    [HideInInspector] public bool CheckpointsAreSet;

    private GenerateCheckPoints generator;
    private RoomTemplates roomTemplates;
    private List<CheckPointSingle> checkPointSingleList;
    private List<Transform> carTransformList;
    private List<int> nextCheckPointSingleIndexList;

    private IEnumerator Start()
    {
        CheckpointsAreSet = false;
        checkPointSingleList = new List<CheckPointSingle>();
        carTransformList = new List<Transform>();
        nextCheckPointSingleIndexList = new List<int>();
        Transform checkpointsTransform = transform.Find("CheckPoints");
        Transform carTransform = transform.Find("Cars");
        generator = GetComponent<GenerateCheckPoints>();
        roomTemplates = GetComponent<RoomTemplates>();

        while (!generator.AreCheckpointsReady)
            yield return null;
        Debug.Log("Setting checkpoints...");

        foreach (Transform checkpoint in checkpointsTransform)
        {
            CheckPointSingle checkPointSingle = checkpoint.GetComponent<CheckPointSingle>();
            checkPointSingle.SetTrackCheckpoints(this);
            checkPointSingleList.Add(checkPointSingle);
        }

        CheckpointsAreSet = true;

        while (!roomTemplates.CarsAreSet)
            yield return null;
        Debug.Log("Attaching cars to checkpoint system...");

        foreach (Transform car in carTransform)
        {
            carTransformList.Add(car);
            nextCheckPointSingleIndexList.Add(0);
        }
    }

    public void CarThroughCheckpoint(CheckPointSingle checkpoint, Transform carTransform)
    {
        int nextCheckPointSingleIndex = nextCheckPointSingleIndexList[carTransformList.IndexOf(carTransform)];
        if (checkPointSingleList.IndexOf(checkpoint) == nextCheckPointSingleIndex)
        {
            nextCheckPointSingleIndexList[carTransformList.IndexOf(carTransform)] = (nextCheckPointSingleIndex + 1) % checkPointSingleList.Count;
            
            OnCarCorrectCheckpoint?.Invoke(this, new CarCheckpointEventArgs(carTransform));
        }
        else
        {
            OnCarWrongCheckpoint?.Invoke(this, new CarCheckpointEventArgs(carTransform));
        }
    }

    public void ResetCheckpoints(Transform car)
    {
        nextCheckPointSingleIndexList[carTransformList.IndexOf(car)] = 0;
    }

    public class CarCheckpointEventArgs : EventArgs
    {
        public Transform carTransform;

        public CarCheckpointEventArgs(Transform carTransform)
        {
            this.carTransform = carTransform;
        }
    }
}