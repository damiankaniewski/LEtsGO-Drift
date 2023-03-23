using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSwapManager : MonoBehaviour
{
    public CarDetails[] carDetails;

    private CarController carController;
    private CarDynamics carDynamics;
    private Rigidbody rb;
    void Start()
    {
        carController = FindObjectOfType<CarController>();
        carDynamics = FindObjectOfType<CarDynamics>();
        rb = FindObjectOfType<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwapCars(0);
            Debug.Log("Car Swapped!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwapCars(1);
            Debug.Log("Car Swapped!");
        }
        
    }

    public void SwapCars(int index)
    {
        GameObject.FindWithTag("CarWheels").SetActive(false);
        carDetails[index].wheels.SetActive(true);

        carController.wheelMeshes.FRWheel = GameObject.Find("FR").GetComponent<MeshRenderer>();
        carController.wheelMeshes.FLWheel = GameObject.Find("FL").GetComponent<MeshRenderer>();
        carController.wheelMeshes.RRWheel = GameObject.Find("RR").GetComponent<MeshRenderer>();
        carController.wheelMeshes.RLWheel = GameObject.Find("RL").GetComponent<MeshRenderer>();

        carController.colliders.FRWheel = GameObject.Find("FRCollider").GetComponent<WheelCollider>();
        carController.colliders.FLWheel = GameObject.Find("FLCollider").GetComponent<WheelCollider>();
        carController.colliders.RRWheel = GameObject.Find("RRCollider").GetComponent<WheelCollider>();
        carController.colliders.RLWheel = GameObject.Find("RLCollider").GetComponent<WheelCollider>();
        
        carDynamics.colliders.FRWheel = GameObject.Find("FRCollider").GetComponent<WheelCollider>();
        carDynamics.colliders.FLWheel = GameObject.Find("FLCollider").GetComponent<WheelCollider>();
        carDynamics.colliders.RRWheel = GameObject.Find("RRCollider").GetComponent<WheelCollider>();
        carDynamics.colliders.RLWheel = GameObject.Find("RLCollider").GetComponent<WheelCollider>();
        GameObject.FindWithTag("CarModel").SetActive(false);



        carDetails[index].model.SetActive(true);
        
        rb.centerOfMass = carDetails[index].centerOfMass.transform.localPosition;
        
        carController.maxSpeed = carDetails[index].maxSpeed;
        carController.motorPower = carDetails[index].motorPower;
        carController.awdMultiplier = carDetails[index].awdMultiplier;
        carDynamics.stiffnessMultiplierRear = carDetails[index].stiffnessMultiplierRear;
        carDynamics.stiffnessMultiplierFront = carDetails[index].stiffnessMultiplierFront;
        carDynamics.extremumValueMultiplier = carDetails[index].extremumValueMultiplier;
        carDynamics.aeroMultiplier = carDetails[index].aeroMultiplier;
    }
}

[Serializable]
public class CarDetails
{

    [Header("Models and COM")] public GameObject wheels;
    public GameObject model;
    public GameObject centerOfMass;
    [Space][Header("Technical details")] public float maxSpeed;
    public float motorPower;
    public float awdMultiplier;
    public float stiffnessMultiplierRear;
    public float stiffnessMultiplierFront;
    public float extremumValueMultiplier;
    public float aeroMultiplier;
}
