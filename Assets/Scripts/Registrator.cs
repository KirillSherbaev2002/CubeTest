using UnityEngine;
using System;

public class Registrator: MonoBehaviour
{
    private ProjectManager projManager;
    
    private void Awake()
    {
        projManager = FindObjectOfType<ProjectManager>();
    }

    public void SetTrajectory(GameObject body, Vector3 inputBodyVector3, Quaternion inputBodyRotation, bool isLast = false)
    {
        BodyScript bodyScript = body.GetComponent<BodyScript>();
        if(projManager.timeScore> projManager.timeToWait && !isLast) return;

        #region Initialization
        
        var timeOfActionValuesLocal = bodyScript.timeOfMoveValues;
        var lengthOfArrays = timeOfActionValuesLocal.Length;
        var positionOfActionValuesLocal = bodyScript.positionOfMoveValues;
        var rotationOfActionValuesLocal = bodyScript.rotationOfMoveValues;

        #endregion

        #region Resize and Reset

        Array.Resize(ref timeOfActionValuesLocal,  lengthOfArrays + 1); 
        Array.Resize(ref positionOfActionValuesLocal,  lengthOfArrays + 1);
        Array.Resize(ref rotationOfActionValuesLocal,  lengthOfArrays + 1);
        
        timeOfActionValuesLocal[lengthOfArrays] = projManager.timeScore;
        positionOfActionValuesLocal[lengthOfArrays] = inputBodyVector3;
        rotationOfActionValuesLocal[lengthOfArrays] = inputBodyRotation;
        
        bodyScript.timeOfMoveValues = timeOfActionValuesLocal;
        bodyScript.positionOfMoveValues = positionOfActionValuesLocal;
        bodyScript.rotationOfMoveValues = rotationOfActionValuesLocal;

        #endregion
    }
    
    public void SetArraysToZero (GameObject body)
    {
        #region Resize and Reset

        BodyScript bodyScript = body.GetComponent<BodyScript>();
        
        var timeOfActionValuesLocal = bodyScript.timeOfMoveValues;
        var positionOfActionValuesLocal = bodyScript.positionOfMoveValues;
        var rotationOfActionValuesLocal = bodyScript.rotationOfMoveValues; 
        
        Array.Resize(ref timeOfActionValuesLocal,  1); 
        Array.Resize(ref positionOfActionValuesLocal,  1);
        Array.Resize(ref rotationOfActionValuesLocal, 1);
        
        bodyScript.timeOfMoveValues = timeOfActionValuesLocal;
        bodyScript.positionOfMoveValues = positionOfActionValuesLocal;
        bodyScript.rotationOfMoveValues = rotationOfActionValuesLocal;

        #endregion
    }

    public void SetDelay(GameObject body, Vector3 defaultPosition, int delay = 12)
    {
        #region Set Lag Without Motion

        BodyScript bodyScript = body.GetComponent<BodyScript>();
        
        var timeOfActionValuesLocal = bodyScript.timeOfMoveValues;
        var positionOfActionValuesLocal = bodyScript.positionOfMoveValues;
        var rotationOfActionValuesLocal = bodyScript.rotationOfMoveValues; 
        
        Array.Resize(ref timeOfActionValuesLocal,  delay); 
        Array.Resize(ref positionOfActionValuesLocal,  delay);
        Array.Resize(ref rotationOfActionValuesLocal,  delay);
        
        for (int i = 0; i < delay; i++)
        {
            timeOfActionValuesLocal[i] = i * 3f * Time.deltaTime;
            positionOfActionValuesLocal[i] = defaultPosition;
            rotationOfActionValuesLocal[i] = new Quaternion(0, 0, 0, 0);
        }
        
        bodyScript.timeOfMoveValues = timeOfActionValuesLocal;
        bodyScript.positionOfMoveValues = positionOfActionValuesLocal;
        bodyScript.rotationOfMoveValues = rotationOfActionValuesLocal;
        
        #endregion
    }
}