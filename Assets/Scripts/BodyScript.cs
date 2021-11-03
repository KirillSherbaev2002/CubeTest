using UnityEngine;

public class BodyScript : MonoBehaviour
{
    [SerializeField] public float[] timeOfMoveValues;
    [SerializeField] public Vector3[] positionOfMoveValues;
    private Vector3 defaultGameObjectPosition;
    [SerializeField] public Quaternion[] rotationOfMoveValues;
    private Registrator _registrator;
    private Rigidbody _localRigidBody;
    private ProjectManager _projManager;
    [SerializeField] private float minCorrectionToRegister;
    [SerializeField] private bool isLagNeeded;
    [SerializeField] private int lagLength;

    private void Awake()
    {
        _localRigidBody = GetComponent<Rigidbody>();
        _registrator = FindObjectOfType<Registrator>();
        _projManager = FindObjectOfType<ProjectManager>();
        _projManager.onSliderChanged += GetCurrentTrajectory;
        _projManager.onMovingEnded += OnMovingEnded;
        _projManager.onSliderEnded += SetArraysToZero;
    }

    private void Start()
    {
        _registrator.SetTrajectory(gameObject, transform.position, transform.rotation);
        defaultGameObjectPosition = transform.position;
    }

    private void OnMovingEnded()
    {
        _registrator.SetTrajectory(gameObject, transform.position, transform.rotation, true);
    }

    private void FixedUpdate()
    {
        CheckOnMove();
    }

    private void CheckOnMove()
    {
        if (_localRigidBody.velocity.magnitude >= minCorrectionToRegister)
            _registrator.SetTrajectory(gameObject, transform.position, transform.rotation);
    }
    
    private void GetCurrentTrajectory(float fill)
    {
        float delta = 11;
        int indexToCloseState = 0;
        for (int i = 0; i < timeOfMoveValues.Length; i++)
        {
            if (timeOfMoveValues.Length < 3) return;
            //Not calculate too small arrays
            var localDelta = timeOfMoveValues[i] - (fill * _projManager.timeToWait);
            if (localDelta < delta && localDelta >= 0)
            {
                delta = localDelta;
                indexToCloseState = i;
                //print("localDelta" + localDelta);
            }
        }
        transform.position = positionOfMoveValues[indexToCloseState];
        transform.rotation = rotationOfMoveValues[indexToCloseState];
    }
    
    private void SetArraysToZero()
    {
        _registrator.SetArraysToZero(gameObject);
        if (isLagNeeded)
        {
            _registrator.SetDelay(gameObject, defaultGameObjectPosition, lagLength);
        }
    }

}
