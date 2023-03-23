using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarDynamics : MonoBehaviour
{
    private CarController carController;
    private Rigidbody rb;
    public GameObject centerOfMass;
    
    private float forceaeroR;
    private float forceaeroF;
    private float forceDown;

    //multipliers
    public float stiffnessMultiplierFront;
    public float stiffnessMultiplierRear;
    public float extremumValueMultiplier;
    public float aeroMultiplier;

    //colliders and meshes
    public WheelColliders colliders;

    private void Start()
    {
        carController = FindObjectOfType<CarController>();
        rb = FindObjectOfType<Rigidbody>();
        rb.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void Update()
    {
        AeroDynamicsSideways();
        ExtremumValueChanges();
    }
    
    void AeroDynamicsSideways()
    {
        forceDown = 10 * aeroMultiplier * carController.speed;

        forceaeroR = carController.speed * 0.02f * aeroMultiplier + 0.5f;
        forceaeroF = carController.speed * 0.023f * aeroMultiplier + 0.8f;

        WheelFrictionCurve myWfcR;
        myWfcR = colliders.RRWheel.sidewaysFriction;
        myWfcR.stiffness = forceaeroR * stiffnessMultiplierRear;
        colliders.RRWheel.sidewaysFriction = myWfcR;
        colliders.RLWheel.sidewaysFriction = myWfcR;

        WheelFrictionCurve myWfcF;
        myWfcF = colliders.FLWheel.sidewaysFriction;
        myWfcF.stiffness = forceaeroF * stiffnessMultiplierFront;
        colliders.FRWheel.sidewaysFriction = myWfcF;
        colliders.FLWheel.sidewaysFriction = myWfcF;


        if (carController.brakeInput != 0)
        {
            myWfcR = colliders.RRWheel.sidewaysFriction;
            myWfcR.stiffness = forceaeroR * 1.6f * stiffnessMultiplierRear;
            colliders.RRWheel.sidewaysFriction = myWfcR;
            colliders.RLWheel.sidewaysFriction = myWfcR;
        }

        if (carController.movingDirection < -0.01f)
        {
            myWfcR = colliders.RRWheel.sidewaysFriction;
            myWfcR.stiffness = forceaeroR * 1.6f * stiffnessMultiplierRear;
            colliders.RRWheel.sidewaysFriction = myWfcR;
            colliders.RLWheel.sidewaysFriction = myWfcR;
        }

    }

    void ExtremumValueChanges()
    {
        WheelFrictionCurve myWfc;
        myWfc = colliders.RRWheel.sidewaysFriction;
        if (carController.speed <= 10f && carController.gasInput > 0.5f)

        {
            if (carController.handbrakeInput == 1)
            {
                myWfc.extremumValue = 0.3f;
                colliders.RRWheel.sidewaysFriction = myWfc;
                colliders.RLWheel.sidewaysFriction = myWfc;
            }
            else
            {
                myWfc.extremumValue = (carController.speed * 0.039f + 0.3f) * extremumValueMultiplier + (1-extremumValueMultiplier);
                colliders.RRWheel.sidewaysFriction = myWfc;
                colliders.RLWheel.sidewaysFriction = myWfc;
            }
        }
        else
        {
            if (carController.handbrakeInput == 1)
            {
                myWfc.extremumValue = 0.3f;
                colliders.RRWheel.sidewaysFriction = myWfc;
                colliders.RLWheel.sidewaysFriction = myWfc;
            }
            else
            {
                myWfc.extremumValue = 1f;
                colliders.RRWheel.sidewaysFriction = myWfc;
                colliders.RLWheel.sidewaysFriction = myWfc;
            }
        }
    }
}

