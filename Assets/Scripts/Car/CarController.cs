using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarController : MonoBehaviour
{
    private Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;

    [Header("Game Objects")] public AnimationCurve steeringCurve;


    [Header("Debug")] public float gasInput;
    public float brakeInput;
    public float handbrakeInput;
    public float steeringInput;

    public float driftAngle;

    [HideInInspector] public float speed;
    [HideInInspector] public float movingDirection;

    [Space] [Header("Properties")] public float motorPower;
    public float brakePower;
    [Tooltip("km/h")] public float maxSpeed;
    public float awdMultiplier;

    void Start()
    {
        playerRB = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        speed = playerRB.velocity.magnitude;
        movingDirection = Vector3.Dot(transform.forward, playerRB.velocity);

        CheckInput();
        ApplyMotor();
        ApplyWheelPositions();
        ApplySteering();
        ApplyBrake();
        ApplyHandbrake();
    }

    void CheckInput()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        handbrakeInput = Input.GetAxis("Jump");
        driftAngle = Vector3.SignedAngle(transform.forward, playerRB.velocity - transform.forward, transform.up);

        if (driftAngle >= 150 && driftAngle <= -150)
        {
            driftAngle = 0;
        }

        //fixed code to brake even after going on reverse by Andrew Alex 
        if (movingDirection < -0.01f && gasInput > 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else if (movingDirection > 0.01f && gasInput < 0)
        {
            brakeInput = Mathf.Abs(gasInput);
        }
        else
        {
            brakeInput = 0;
        }
    }

    void ApplyHandbrake()
    {
        if (speed < 1 && gasInput == 0 && brakeInput == 0)
        {
            colliders.RRWheel.brakeTorque = brakePower;
            colliders.RLWheel.brakeTorque = brakePower;
        }
        else
        {
            colliders.RRWheel.brakeTorque = handbrakeInput * brakePower;
            colliders.RLWheel.brakeTorque = handbrakeInput * brakePower;
        }
    }

    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower;
    }

    void ApplyMotor() // pseudo dociskanie do ziemi
    {
        float instantMotorPower;

        if (speed >= maxSpeed / 3.6f)
        {
            instantMotorPower = 0;
        }
        else
        {
            instantMotorPower = motorPower;
        }

        var speedInterpolation = Mathf.Lerp(0, maxSpeed / 3.6f, speed / (maxSpeed / 3.6f)) / (maxSpeed / 3.6f);

        colliders.RRWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        (1f - (awdMultiplier * 0.5f));
        colliders.RLWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        (1f - (awdMultiplier * 0.5f));
        colliders.FRWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        awdMultiplier * 0.5f;
        colliders.FLWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        awdMultiplier * 0.5f;

        if (movingDirection < -0.01f)
        {
            colliders.RRWheel.motorTorque = (instantMotorPower * gasInput + speedInterpolation * instantMotorPower) *
                                            (1f - (awdMultiplier * 0.5f));
            colliders.RLWheel.motorTorque = (instantMotorPower * gasInput + speedInterpolation * instantMotorPower) *
                                            (1f - (awdMultiplier * 0.5f));
            colliders.FRWheel.motorTorque = (instantMotorPower * gasInput + speedInterpolation * instantMotorPower) *
                                            awdMultiplier * 0.5f;
            colliders.FLWheel.motorTorque = (instantMotorPower * gasInput + speedInterpolation * instantMotorPower) *
                                            awdMultiplier * 0.5f;
        }

        colliders.RRWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        (1f - (awdMultiplier * 0.5f));
        colliders.RLWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        (1f - (awdMultiplier * 0.5f));
        colliders.FRWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        awdMultiplier * 0.5f;
        colliders.FLWheel.motorTorque = (instantMotorPower * gasInput - speedInterpolation * instantMotorPower) *
                                        awdMultiplier * 0.5f;
    }

    void ApplySteering()
    {
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
        if (driftAngle < 120f && driftAngle > 5f && steeringInput * driftAngle > 0 && speed > 10)
        {
            steeringAngle = Mathf.Abs(steeringInput) *
                            Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }

        if (driftAngle > -120f && driftAngle < -5f && steeringInput * driftAngle > 0 && speed > 10)
        {
            steeringAngle = Mathf.Abs(steeringInput) *
                            Vector3.SignedAngle(transform.forward, playerRB.velocity + transform.forward, Vector3.up);
        }

        steeringAngle = Mathf.Clamp(steeringAngle, -70f, 70f);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}