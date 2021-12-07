using UnityEngine;

public class FlyLeapController : MonoBehaviour
{

    [Header("Hands")]
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    [SerializeField] private Transform _RotationGestoureAnchor;
    [SerializeField] private Transform _MoveGestoureAnchor;


    [Header("Player")]
    public float rotationSpeed = 5f;       // Rotation speed.
    public static bool HandStateRight = false;
    public static bool HandStateLeft = false;

    public float mainSpeed = 70f;
    public float maxSpeed = 10f;
    public float forceMagnitude = 2f;


    public void HandRightClosed()
    {
        HandStateRight = true;

    }
    public void HandRightOpened()
    {
        HandStateRight = false;
    }

    public void HandLeftClosed()
    {
        HandStateLeft = true;

    }
    public void HandLeftOpened()
    {
        HandStateLeft = false;
    }

    private void Update()
    {
        if (HandStateRight) PlayerMovement();
        if (HandStateLeft) GestureRotation();

    }
    private void GestureRotation()
    {
        float distance = Vector3.Distance(_leftHand.position, _RotationGestoureAnchor.position);
        float rotateInfluence;

        if (!Utils.IsFrontAtObject(_RotationGestoureAnchor, _leftHand))
        {
            rotateInfluence = -distance * Time.deltaTime * rotationSpeed;
        }
        else
        {
            rotateInfluence = +distance * Time.deltaTime * rotationSpeed;
        }

        GetComponent<Rigidbody>().AddTorque(new Vector3(0, rotateInfluence, 0));
    }

    private void PlayerMovement()
    {

        float distance = Vector3.Distance(_rightHand.position, _MoveGestoureAnchor.position);
        float moveInfluence;

        if (!Utils.IsFrontAtObject(_MoveGestoureAnchor, _rightHand))
        {
            moveInfluence = distance * Time.deltaTime * rotationSpeed;
        }
        else
        {
            moveInfluence = -distance * Time.deltaTime * rotationSpeed;
        }

        float CurrentSpeed = Mathf.Min(mainSpeed * moveInfluence * (forceMagnitude + Time.deltaTime), maxSpeed);

        Vector3 movement = Camera.main.transform.forward * CurrentSpeed;
        GetComponent<Rigidbody>().AddForce(movement);



    }
}