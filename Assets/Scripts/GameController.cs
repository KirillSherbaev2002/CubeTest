using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Dreamteck.Splines;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Slider coreForceSlider;
    [SerializeField] private GameObject sliderHandle;
    
    [Header("Objects")]
    [SerializeField] private GameObject core;
    [SerializeField] private GameObject movingPartOfCannon;
    [SerializeField] private GameObject cannon;
    [SerializeField] private GameObject aim;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject coreInstantiator;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject splineCore;

    [Header("Tracking")]
    private Vector3[] trajectory;
    [SerializeField] private float trackingFrequency;
    [SerializeField] private float valueToStop;
    [SerializeField] private float forceToAdd;
    [SerializeField] private float distanceToSkipShot;
    public float timeToWait;

    private Vector3 aimToWorld;
    private GameObject coreCreated;
    private SplineFollower coreCreatedFollower;
    private float sliderValue;
    private bool isControlledBySlider;

    public Action< int > trackingCall;
    public Action< float> onSliderControlGoing;
    public Action onMovementEnded;
   
    
    private void Awake()
    {
        SetToDefault();
        trackingCall += SetTrajectoryOfSphere;
        onSliderControlGoing += OnShotCompleted;
    }
    
    private void SetToDefault()
    {
        splineCore.AddComponent<SplineComputer>();
        splineCore.AddComponent<SplineRenderer>();
        coreForceSlider.interactable = false; 
    }

    private void OnShotCompleted(float value2)
    {
        sliderHandle.SetActive(true);
    }
    private void OnMouseDrag()
    {
        if (Input.mousePosition.x <= Screen.height / 15f )
            return;

        aim.SetActive(true);
        //Aim activation
            
        Vector3 localTouch = Input.mousePosition;
        localTouch.z = 23f;
        //Needed because Z means distance for Quaternion.LookRotation
            
        aim.GetComponent<RectTransform>().position = new Vector3(localTouch.x, localTouch.y);
        aimToWorld = cam.ScreenToWorldPoint(localTouch);
        movingPartOfCannon.transform.rotation = Quaternion.LookRotation(aimToWorld);
        //Set needed values to aim and movingPartOfCannon
    }

    private void OnMouseUp()
    {
        DestoryPreviousBall();
        Shot();
    }

    private void DestoryPreviousBall()
    {
        GameObject previousBall = GameObject.FindGameObjectWithTag("Core");
        Destroy(previousBall);
    }

    private void Shot()
    {
        var spawnPosition = coreInstantiator.transform.position;
        var spawnRotation = coreInstantiator.transform.rotation;
        
        coreCreated = Instantiate(core, spawnPosition, spawnRotation);
        Instantiate(explosion, spawnPosition, spawnRotation);
        
        coreCreated.GetComponent<Rigidbody>().AddForce(aimToWorld * forceToAdd);
        
        StartCoroutine(ControllerLaunched(0));
    }

    private void FixedUpdate()
    {
        if (isControlledBySlider)
        {
            coreCreatedFollower.startPosition = coreForceSlider.value;
            onSliderControlGoing( coreForceSlider.value);
        }
    }

    IEnumerator ControllerLaunched(int i)
    {
        yield return new WaitForSeconds(0.03f);
        bool isBallPhysical = true;
        float timeScore = 0;

        while (isBallPhysical)
        {
            i++;
            trackingCall(i);
            yield return new WaitForSeconds(0.3f / coreCreated.GetComponent<Rigidbody>().velocity.magnitude);
            print(0.1f / coreCreated.GetComponent<Rigidbody>().velocity.magnitude);

            timeScore += 3 * Time.deltaTime;

            print(timeScore);
            if (timeScore >= timeToWait)
            {
                isBallPhysical = false;
            }
        }

        onMovementEnded();
        coreCreated.GetComponent<Collider>().isTrigger = true;
        coreCreated.GetComponent<Rigidbody>().isKinematic = true;
        coreCreated.AddComponent<SplineFollower>().spline = splineCore.GetComponent<SplineComputer>();
        coreCreatedFollower = coreCreated.GetComponent<SplineFollower>();
        coreCreatedFollower.follow = false;
        coreCreatedFollower.physicsMode = SplineTracer.PhysicsMode.Transform;
        coreForceSlider.interactable = true;
        isControlledBySlider = true;
    }

    // private void GetTrajectoryOfSphere(int valueInArray, float fillSlider)
    // {
    //     if (sliderValue != coreForceSlider.value)
    //     {
    //         sliderValue = coreForceSlider.value;
    //         coreCreated.GetComponent<SplineFollower>().startPosition = fillSlider;
    //     }
    // }

    private void SetTrajectoryOfSphere(int valueInArray)
    {
        splineCore.GetComponent<SplineComputer>().SetPointPosition(valueInArray, coreCreated.transform.position);
    }
}
