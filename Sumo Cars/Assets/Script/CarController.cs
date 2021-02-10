using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    private float horizontalInput;
    private float verticalInput;
    private float currentsteerAngle;
    private float currentbreakForce;
    private bool isBreaking=false;


    public float motorForce;
    public float breakForce;
    public float maxSteerAngle;

    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public WheelCollider RearLeftWheel;
    public WheelCollider RearRightWheel;

    public Transform FrontLeftWheelTransform;
    public Transform FrontRightWheelTransform;
    public Transform RearLeftWheelTransform;
    public Transform RearRightWheelTransform;

    public Rigidbody rb;

    PhotonView PV;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }

        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "car")
            rb.AddForce(collision.relativeVelocity, ForceMode.VelocityChange);
 
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FrontLeftWheel, FrontLeftWheelTransform);
        UpdateSingleWheel(FrontRightWheel, FrontRightWheelTransform);
        UpdateSingleWheel(RearLeftWheel, RearLeftWheelTransform);
        UpdateSingleWheel(RearRightWheel, RearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheel, Transform wheeltransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheel.GetWorldPose(out pos, out rot);
        wheeltransform.rotation = rot;
        wheeltransform.position = pos;

    }

    private void HandleSteering()
    {
        currentsteerAngle = maxSteerAngle * horizontalInput;
        FrontLeftWheel.steerAngle = currentsteerAngle;
        FrontRightWheel.steerAngle = currentsteerAngle;

    }

    private void HandleMotor()
    {

        FrontLeftWheel.motorTorque = verticalInput * motorForce;
        FrontRightWheel.motorTorque = verticalInput * motorForce;
        RearLeftWheel.motorTorque = verticalInput * motorForce;
        RearRightWheel.motorTorque = verticalInput * motorForce;

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();       

    }

    private void ApplyBreaking()
    {
        FrontLeftWheel.brakeTorque = currentbreakForce*2;
        FrontRightWheel.brakeTorque = currentbreakForce * 2;
        RearLeftWheel.brakeTorque = currentbreakForce * 2;
        RearRightWheel.brakeTorque = currentbreakForce * 2;
    }

    private void GetInput()
    {
        horizontalInput = SimpleInput.GetAxis("Horizontal");
        verticalInput = SimpleInput.GetAxis("Vertical");
        isBreaking = SimpleInput.GetKey(KeyCode.Space);
        
    }
}
