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
            AddReward(-0.1f);
        }
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(+0.1f);
        }
    }

    public override void OnEpisodeBegin()
    {
        rigidbody.velocity = Vector3.zero;
        transform.SetPositionAndRotation(carController.startPosition, carController.startRotation);
        trackCheckpoints.ResetCheckpoints(transform);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(rigidbody.velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float verticalInput = actions.ContinuousActions[0];
        float horizontalInput = actions.ContinuousActions[1];
        bool isBreaking = actions.DiscreteActions[0] != 0;
        carController.SetInput(horizontalInput, verticalInput, isBreaking);

        AddReward(-0.02f);
        if (verticalInput >  0 && !isBreaking) 
        {
            AddReward(0.02f *  verticalInput);
        }
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
        if (other.gameObject.layer == 7) AddReward(-0.5f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            AddReward(-0.1f);
        }
    }
}
