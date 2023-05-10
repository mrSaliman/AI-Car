using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;

public class MoveThroughCheckpointsAgent : Agent
{
    [SerializeField] private new Rigidbody rigidbody;
    private TrackCheckpoints trackCheckpoints;
    private CarController carController;
    private int stopTime;
    private readonly float speedEpsilon = 20f;

    private void Awake()
    {
        carController = GetComponent<CarController>();
        trackCheckpoints = transform.parent.parent.GetComponent<TrackCheckpoints>();
    }

    private void Start()
    {
        trackCheckpoints.OnCarCorrectCheckpoint += TrackCheckpoints_OnCarCorrectCheckpoint;
        trackCheckpoints.OnCarWrongCheckpoint += TrackCheckpoints_OnCarWrongCheckpoint;
    }

    private void TrackCheckpoints_OnCarWrongCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(-1f);
            Debug.Log("wrong");
        }
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(+1f);
            Debug.Log("correct");
        }
    }

    public override void OnEpisodeBegin()
    {
        rigidbody.velocity = Vector3.zero;
        transform.SetPositionAndRotation(carController.startPosition, carController.startRotation);
        trackCheckpoints.ResetCheckpoints(transform);
        stopTime = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint(transform).transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
        sensor.AddObservation(rigidbody.velocity.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float verticalInput = actions.ContinuousActions[0];
        float horizontalInput = actions.ContinuousActions[1];
        bool isBreaking = actions.DiscreteActions[0] != 0;
        carController.SetInput(horizontalInput, verticalInput, isBreaking);

        if (rigidbody.velocity.magnitude * 3.6f < speedEpsilon)
        {
            stopTime++;
        } else
        {
            stopTime = 0;
        }

        if (verticalInput > 0 && !isBreaking)
        {
            AddReward(0.05f * verticalInput);
        }
        if (verticalInput > 0.5f && stopTime > 1)
        {
            stopTime -= 2;
        }
        AddReward(-0.00001f * stopTime * stopTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Vertical");
        continuousActions[1] = Input.GetAxis("Horizontal");
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetButton("Jump") ? 1 : 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            AddReward(-1f);
            Debug.Log("wall touched");
        }
        else if (other.CompareTag("End CheckPoint"))
        {
            AddReward(+5f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
        {
            AddReward(-1f);
            Debug.Log("car touched");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            AddReward(-0.1f);
        }
        if (collision.transform.CompareTag("Car"))
        {
            AddReward(-0.1f);
        }
    }
}
