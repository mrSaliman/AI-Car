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
    /*private int Batch = 0;
    private Vector2 nextBatchVector = Vector2.zero, curentBatchVector = Vector2.zero;*/

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
            //Debug.Log("wrong");
        }
    }

    private void TrackCheckpoints_OnCarCorrectCheckpoint(object sender, TrackCheckpoints.CarCheckpointEventArgs e)
    {
        if (e.carTransform == transform)
        {
            AddReward(+2f);
            //Debug.Log("correct");
        }
    }

    public override void OnEpisodeBegin()
    {
        ResetCar();
    }

    private void ResetCar()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        transform.SetLocalPositionAndRotation(carController.startPosition, carController.startRotation);
        trackCheckpoints.ResetCheckpoints(transform);
        stopTime = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        CheckPointSingle nextCheckpoint = trackCheckpoints.GetNextCheckpoint(transform);
        if (nextCheckpoint != null)
        {
            Vector3 checkpointForward = nextCheckpoint.transform.forward;
            float directionDot = Vector3.SignedAngle(transform.forward, checkpointForward, Vector3.up);
            sensor.AddObservation(directionDot);
        }
        else
        {
            sensor.AddObservation(0f);
        }
        sensor.AddObservation(rigidbody.velocity.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float verticalInput = actions.ContinuousActions[0];
        float horizontalInput = actions.ContinuousActions[1];
        bool isBreaking = actions.DiscreteActions[0] != 0;

        /*if (Batch < 5)
        {
            nextBatchVector += new Vector2(verticalInput, horizontalInput);
            Batch++;
        }
        else 
        {
            Batch = 0;
            curentBatchVector = nextBatchVector / 5;
            nextBatchVector = Vector2.zero;
        }*/
        carController.SetInput(horizontalInput, verticalInput, isBreaking); 
        //Debug.Log(curentBatchVector[0] + " : " + curentBatchVector[1]);

        if (rigidbody.velocity.magnitude * 3.6f < speedEpsilon)
        {
            stopTime++;
        } else
        {
            stopTime = 0;
        }

        //Debug.Log(verticalInput);
        if (verticalInput <= 0 || isBreaking)
        {
            AddReward(-0.00001f * stopTime * stopTime * Time.fixedDeltaTime);
        }

        //Debug.Log(-0.00001f * stopTime * stopTime * Time.fixedDeltaTime);

        //Debug.Log(GetCumulativeReward());
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
            AddReward(-2f);
            //Debug.Log("wall touched");
        }
        else if (other.CompareTag("End CheckPoint"))
        {
            AddReward(+5f);
            //Debug.Log("End");
            ResetCar();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Car"))
        {
            AddReward(-2f);
            //Debug.Log("car touched");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            AddReward(-0.2f * Time.deltaTime);
            AddReward(-0.00001f * stopTime * stopTime * Time.fixedDeltaTime);
            //Debug.Log("wall Stay");
        }
        if (collision.transform.CompareTag("Car"))
        {
            AddReward(-0.2f * Time.deltaTime);
            AddReward(-0.00001f * stopTime * stopTime * Time.fixedDeltaTime);
            //Debug.Log("Car Stay");
        }
    }
}
