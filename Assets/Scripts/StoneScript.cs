using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


public class StoneScript : MonoBehaviour
{
    private Vector3[] objectTrajectory;
    private float quantityOfTracking;
    private GameController gameController;
    [SerializeField] private float distanceToNotCount; 
    private Quaternion defaultQuaternion;
    [SerializeField] private Quaternion finalQuaternion;
    private GameObject child;
    private SplineFollower coreCreatedFollower;
    private SplineComputer itsSplineComputer;
    
    private List<Quaternion> stoneRotationValue = new List<Quaternion>();
    private void Awake()
    {
        gameController = GameObject.FindObjectOfType<GameController>();
        gameController.trackingCall += SetTrajectory;
        gameController.onSliderControlGoing += GetTrajectory;
        gameController.onMovementEnded += OnCollision;
        gameController.onMovementEnded += SetRotationFinal;

        gameObject.AddComponent<SplineComputer>();
        gameObject.AddComponent<SplineRenderer>();
        itsSplineComputer = GetComponent<SplineComputer>();
        print("SetTrajectory");
        child = transform.GetChild(0).gameObject;
        
        defaultQuaternion = child.transform.rotation;
    }

    private void OnCollision()
    { 
        child.AddComponent<SplineFollower>().spline = itsSplineComputer;
        child.GetComponent<SplineFollower>().follow = false;
        child.GetComponent<Collider>().isTrigger = true;
        
        child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
        child.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        child.GetComponent<Rigidbody>().isKinematic = true;
        
        coreCreatedFollower = child.GetComponent<SplineFollower>();
        coreCreatedFollower.follow = false;
        coreCreatedFollower.motion.applyRotation = false;
        coreCreatedFollower.physicsMode = SplineTracer.PhysicsMode.Transform;
    }

    private void SetTrajectory(int valueInArray)
    {
        int pointCount = itsSplineComputer.pointCount;
        if (pointCount == 0) 
        {
            itsSplineComputer.SetPointPosition(pointCount, child.transform.position);
            stoneRotationValue.Add(child.transform.rotation);
            return;
        }

        if (Vector3.Distance(itsSplineComputer.GetPointPosition(pointCount - 1),
            child.transform.position) > distanceToNotCount)
        {
            itsSplineComputer.SetPointPosition(pointCount, child.transform.position);
            stoneRotationValue[valueInArray] = child.transform.rotation;
        }
    }

    private void SetRotationFinal()
    {
        finalQuaternion = child.transform.rotation;
    }

    private void GetTransitionRotation(Quaternion firstRotation,Quaternion secondRotation, float fill)
    {
        Quaternion neededQuaternionByFill = new Quaternion((firstRotation.x * (1-fill)) + (secondRotation.x * fill),
            (firstRotation.y * (1-fill)) + (secondRotation.y* fill),
            (firstRotation.z * (1-fill)) + (secondRotation.z* fill),0);
        transform.rotation = neededQuaternionByFill;
    }

    private void GetTrajectory(float fill)
    {
        child.GetComponent<SplineFollower>().startPosition = fill;
        //GetTransitionRotation();
    }
    
    private void GetRotationNeeded(float fill)
    {
        if (finalQuaternion == new Quaternion(0,0,0,0))
        {
            return;
        }
        Quaternion neededQuaternionByFill = new Quaternion((defaultQuaternion.x * (1-fill)) + (finalQuaternion.x * fill),
            (defaultQuaternion.y * (1-fill)) + (finalQuaternion.y* fill),
            (defaultQuaternion.z * (1-fill)) + (finalQuaternion.z* fill),0);
        print(neededQuaternionByFill);
        
        child.transform.rotation = Quaternion.EulerAngles(neededQuaternionByFill.x,neededQuaternionByFill.y,neededQuaternionByFill.z);
    }

}
