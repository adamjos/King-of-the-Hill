using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    public Rigidbody rb;

    private float moveForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float _horizontal = Input.GetAxis("Horizontal");
        float _vertical = Input.GetAxis("Vertical");

        Vector3 _moveDir = new Vector3(_horizontal, 0f, _vertical).normalized;

        rb.AddForce(_moveDir * moveForce * Time.deltaTime, ForceMode.VelocityChange);
    }
}
