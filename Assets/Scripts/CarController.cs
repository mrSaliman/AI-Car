using System;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheeTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private MeshRenderer mr;

    private float CarMass
    {
        get 
        {
            return rb.mass;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //FireRays();
        //GetInput();
        HandleMotor();
        HandleSteering();
        AddDownForce();
        UpdateWheels();
    }

    private void FireRays()
    {
        for (int i = -90; i <= 90; i += 18)
        {
            Vector3 angle = transform.forward;
            var pos = transform.position;

            angle = Quaternion.AngleAxis(i, Vector3.up) * angle;
            pos.y += 0.4f;

            Ray ray = new(pos, angle);

            Debug.DrawRay(pos, angle * 15, Color.red);

            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData, 15, 448))
            {
                print(hitData.transform.gameObject.tag + ' ' + hitData.distance);
            }

        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetButton("Jump");
    }

    public void SetInput(float horizontal, float vertical, bool breaking)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
        isBreaking = breaking;
    }

    private void AddDownForce()
    {
        float downforce = 3.5f * rb.velocity.sqrMagnitude * 1.2754f * mr.bounds.size.x * mr.bounds.size.y / 2;
        rb.AddForce(downforce * -transform.up);
        UpdateWheelsSlips(downforce);
    }

    private void UpdateWheelsSlips(float downforce)
    {
        var temp = frontLeftWheelCollider.sidewaysFriction;
        temp.extremumSlip = rearLeftWheelCollider.sidewaysFriction.extremumSlip * (1 + downforce / CarMass);
        temp.asymptoteSlip = rearLeftWheelCollider.sidewaysFriction.asymptoteSlip * (1 + downforce / CarMass);
        frontRightWheelCollider.sidewaysFriction = temp;
        frontLeftWheelCollider.sidewaysFriction = temp;
    }

    private void UpdateSingleWheelSlips(WheelCollider wheelCollider, float downforce) 
    {
        
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce * 0.63f;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce * 0.63f;
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce * 0.37f;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce * 0.37f;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * Math.Max(Math.Min(10.0f / rb.velocity.magnitude, 1), 0.1f) * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.SetPositionAndRotation(pos, rot);
    }
}