using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SphereAgent : Agent
{
    public GameObject sphere;
    public LayerMask finishMask;
    public float winTime = 3f;
    public Transform winPosition;

    private Rigidbody rb;
    private float maxPosValue = 50f;
    private float moveForce = 20f;
    private float kingTime = 0f;

    public override void Initialize()
    {
        base.Initialize();
        rb = sphere.GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
        // Store observations
        sensor.AddObservation(transform.position.z - winPosition.position.z);
        sensor.AddObservation(transform.position.x - winPosition.position.x);
        sensor.AddObservation(rb.velocity);

        // Could be based on raycast, postion etc
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        base.OnActionReceived(actions);
        // Move sphere agent
        var actionZ = 2f * Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var actionX = 2f * Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        Vector3 _moveDir = new Vector3(actionX, 0f, actionZ).normalized;

        rb.AddForce(_moveDir * moveForce * Time.deltaTime, ForceMode.VelocityChange);

        if (Vector3.Dot(rb.velocity, -(transform.position - new Vector3(winPosition.position.x, 0f, winPosition.position.z))) > 0.25f)
        {
            AddReward(0.005f);
        }

        if (transform.position.y > 2f) 
        {
            SetReward(0.25f);
        }

        if (transform.position.y > 5f)
        {
            SetReward(0.5f);
        }

        if (transform.position.z > (winPosition.position.z + maxPosValue) ||
            transform.position.z < (winPosition.position.z - maxPosValue) ||
            transform.position.x > (winPosition.position.x + maxPosValue) ||
            transform.position.x < (winPosition.position.x - maxPosValue))
        {
            SetReward(-1f);
            EndEpisode();
        }

        /*if (transform.position.sqrMagnitude > maxPosValue * maxPosValue)
        {
            SetReward(-1f);
            EndEpisode();
        }*/

        Collider[] hitObjects = Physics.OverlapSphere(transform.position, 1f, finishMask);

        if (hitObjects.Length > 0)
        {
            Debug.Log("King of the hill!");
            Debug.Log(hitObjects[0].name);
            SetReward(1f);

            kingTime += Time.deltaTime;
            if (kingTime >= winTime)
            {
                kingTime = 0;
                EndEpisode();
            }
        }

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        // Reset postion of sphere agent 
        transform.position = new Vector3(winPosition.position.x + Random.Range(-49f, 49f), 1f, winPosition.position.z + Random.Range(-49f, 49f));
        rb.velocity = new Vector3(0f, 0f, 0f);

    }

}
