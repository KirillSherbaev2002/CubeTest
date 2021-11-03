using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManager : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Slider timeSlider;

    [Header("Objects")]
    [SerializeField] private GameObject core;
    private GameObject coreCreated;
    [SerializeField] private GameObject coreInstantiator;
    [SerializeField] private GameObject movingPartOfCannon;
    [SerializeField] private GameObject explosion;
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject aim;
    private Vector3 aimToWorld;
    
    [Header("TimeAndPhysics")]
    [SerializeField] private float forceToAdd;
    public float timeToWait;
    public float timeScore;

    [Header("Actions")]
    public Action<float> onSliderChanged;
    public Action onSliderEnded;
    public Action onMovingEnded;


    private void Awake()
    {
        timeSlider.interactable = false;
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
        Time.timeScale = 1f;
        timeScore = 0;
        onSliderEnded();
        
        var spawnPosition = coreInstantiator.transform.position;
        var spawnRotation = coreInstantiator.transform.rotation;
        
        coreCreated = Instantiate(core, spawnPosition, spawnRotation);
        Instantiate(explosion, spawnPosition, spawnRotation);
        
        coreCreated.GetComponent<Rigidbody>().AddForce(aimToWorld * forceToAdd);
        timeSlider.value = 1f;
        StartCoroutine(ControllerLaunched(0));
    }

    IEnumerator ControllerLaunched(int i)
    {
        yield return new WaitForSeconds(0.03f);
        bool isBallPhysical = true;

        while (isBallPhysical)
        {
            i++;
            yield return new WaitForSeconds(0.3f / coreCreated.GetComponent<Rigidbody>().velocity.magnitude);
            timeScore += 3 * Time.deltaTime;
            if (timeScore >= timeToWait)
            {
                isBallPhysical = false;
            }
        } 
        timeScore = timeToWait;
        onMovingEnded();
        timeSlider.interactable = true;
        Time.timeScale = 0f;
    }

    public void OnSliderMoved()
    {
        float fill = timeSlider.value;
        onSliderChanged(fill);
    }
}
