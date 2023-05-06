using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveThroughCheckpointsAgent : Agent
{
    [SerializeField] private new Rigidbody rigidbody;

    public override void OnEpisodeBegin()
    {
        gameObject.GetComponent<DeleteCheckPoints>().Regenrate();

        GameObject car = gameObject;
        car.transform.SetPositionAndRotation(car.GetComponent<CarController>().startPosition, car.GetComponent<CarController>().startRotation);
        rigidbody.velocity = Vector3.zero;
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
        gameObject.GetComponent<CarController>().SetInput(horizontalInput, verticalInput, isBreaking);

        AddReward(-0.016f);
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
        int layer = other.gameObject.layer;
        switch (layer)
        {
            case 7:
                AddReward(-5f);
                EndEpisode();
                break;
            case 6:
                AddReward(+1f);
                if (other.CompareTag("End CheckPoint"))
                {
                    AddReward(+5f);
                    EndEpisode();
                }
                break;
            default:
                break;
        }
    }
}
