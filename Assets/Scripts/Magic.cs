
/**
 * Script to create and shoot magic
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public static Magic instance;

    public GameObject LeftHand;
    public GameObject RightHand;

    public GameObject LeftHandAnchor;
    public GameObject RightHandAnchor;
    [SerializeField] private Transform _kames;

    [Header("Hand")]
    private Vector3 leftHandValid;
    private Vector3 rightHandValid;
    private Vector3 lastPosition;

    private Vector3 LastLeftHand;
    private Vector3 LastRightHand;

    [Tooltip("Hand distance to init Kame Hame Ha.")]
    [Range(0f, 5f)]
    public float HandDistance = 0.1f;

    [Header("Kame Hame Ha")]
    [Tooltip("Destroy distance from camera")]
    [Range(0f, 10000f)]
    public float _destroyDistance = 1000f;

    [Tooltip("Max size of Kame Hame Ha.")]
    [Range(0f, 8f)]
    public float _kameHameMaxSize = 2f;

    [Tooltip("Vertical position of Kame Hame Ha.")]
    [Range(-3f, 3f)]
    public float _kameHameHaPosition = 0.5f;
    [Header("Shoot")]
    [Tooltip("Hands intensity to launch kame.")]
    [Range(0f, 100f)]
    public float handIntensityToShoot = 1f;
    [Tooltip("Hands max intensity.")]
    [Range(0f, 100f)]
    public float handMaxIntensity = 10f;
    [Tooltip("Velocity max kame.")]
    [Range(0, 1000000)]
    public int shootMaxIntensity = 10000;


    [Header("Effect")]
    [SerializeField] private Transform[] _magicArray;
    private Transform currentKame;

    [Header("Score")]
    [SerializeField] private TMPro.TextMeshPro _aimPercentText;
    [SerializeField] private TMPro.TextMeshPro _hitsText;
    [SerializeField] private TMPro.TextMeshPro _DistanceText;
    private float distance;
    private int index;

    private List<ParticleSystem> _magicParticleList;
    [Header("Sound")]
    public AudioSource AudioSourceKame;
    public AudioClip Create;
    public AudioClip Launch;


    private float _shotCount;
    public static float _hitCount;

    private static bool kameLeftHand = false;
    private static bool kameRightHand = false;

    public static void KameLeftOpened()
    {
        kameLeftHand = true;
    }
    public static void KameRightOpened()
    {
        kameRightHand = true;
    }
    public static void KameLeftClosed()
    {
        kameLeftHand = false;
    }
    public static void KameRightClosed()
    {
        kameRightHand = false;
    }




    void Awake()
    {
        instance = this;
        //OVRCameraRig 
        SceneConfig.MainCamera = GameObject.Find("OVRCameraRig").transform;

    }

    void Start()
    {
        _magicParticleList = new List<ParticleSystem>();
        _shotCount = 0;
        _hitCount = 0;

    }

    private void FixedUpdate()
    {

        // Measure the distance between both palms
        distance = Vector3.Distance(leftHandValid, rightHandValid);
        _DistanceText.text = "Distance: " + String.Format("{0:0.00}", distance);
        // limit kame hame size
        if (distance > _kameHameMaxSize)
        {
            distance = _kameHameMaxSize;
        }
        bool validPosition = false;
        if (isValidController(OVRInput.Controller.LTouch))
        {
            leftHandValid = this.transform.TransformPoint(OVRInput.GetLocalControllerPosition((OVRInput.Controller.LTouch)));
            validPosition = true;
        }
        if (isValidController(OVRInput.Controller.RTouch))
        {
            rightHandValid = this.transform.TransformPoint(OVRInput.GetLocalControllerPosition((OVRInput.Controller.RTouch)));
            validPosition = true;
        }
        if (validPosition)
        {
            Vector3 middlePosition = CenterOfVectors(new Vector3[] { LeftHand.transform.position, RightHand.transform.position });
            middlePosition = new Vector3(middlePosition.x, middlePosition.y + _kameHameHaPosition, middlePosition.z);


            //in futures oculus sdk this must works
            // Vector3 middlePositionVelocity = CenterOfVectors(new Vector3[] { OVRInput.GetLocalControllerVelocity((OVRInput.Controller.LTouch)), OVRInput.GetLocalControllerVelocity((OVRInput.Controller.RTouch)) });
            Vector3 middlePositionVelocity = CenterOfVectors(new Vector3[] { LeftHandAnchor.transform.localPosition, RightHandAnchor.transform.localPosition });

            float speed = Vector3.Distance(lastPosition, middlePositionVelocity) / Time.deltaTime;
            Vector3 midway = CenterOfVectors(new Vector3[] { LeftHand.transform.forward, -RightHand.transform.forward, LastLeftHand, LastRightHand });

            //only shoot if hands move to forward
            if (Utils.IsFrontAtObject(LeftHandAnchor.transform, lastPosition) || Utils.IsFrontAtObject(RightHandAnchor.transform, lastPosition))
            {
                _aimPercentText.text = "isfront: " + speed;
                
            }else{
                speed= 0;
                _aimPercentText.text = "notfront: " + speed;
            }
            //update previous positions
            lastPosition = middlePositionVelocity;
            LastLeftHand = LeftHand.transform.forward;
            LastRightHand = -RightHand.transform.forward;

           // _aimPercentText.text = "Speed: " + String.Format("{0:0.00}", speed);


            if (distance > HandDistance * 0.5f && distance < HandDistance && currentKame == null)
            {
                if (!kameLeftHand || !kameRightHand)
                {
                    DestroyKame();
                    return;
                }
                CreateEffect();
            }
            if (currentKame)
            {
                SizeMagic(middlePosition);
                ShootKame(midway, speed);
            }

        }
        else
        {
            DestroyKame();
        }
    }

    private bool isValidController(OVRInput.Controller controller)
    {
        if (OVRInput.GetControllerOrientationTracked(controller))
        {
            return true;
        }
        else
        {
            if (currentKame)
            {
                AudioSourceKame.Stop();
                Destroy(currentKame.gameObject);
                currentKame = null;
            }
            return false;
        }
    }

    private void CreateEffect()
    {

        // Generated after determining the effect at random
        index = UnityEngine.Random.Range(0, _magicArray.Length);
        currentKame = Instantiate(_magicArray[index], _kames.transform);
        currentKame.name = "kamehameha";
        currentKame.transform.parent = _kames;
        currentKame.GetComponent<KameHameHa>().Distance = _destroyDistance;


        AudioSourceKame.clip = Create;
        AudioSourceKame.loop = true;
        AudioSourceKame.Play();

        _magicParticleList.Clear();

        // Included in a list consisting of several particles
        for (int i = 0; i < currentKame.childCount; i++)
            _magicParticleList.Add(currentKame.GetChild(i).GetComponent<ParticleSystem>());
    }
    public Vector3 CenterOfVectors(Vector3[] vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Length == 0)
        {
            return sum;
        }

        foreach (Vector3 vec in vectors)
        {
            sum += vec;
        }
        return sum / vectors.Length;
    }
    private void SizeMagic(Vector3 middlePosition)
    {

        AudioSourceKame.pitch = distance;
        currentKame.position = middlePosition;
        // Pull out multiple particles from the list and scale them
        for (int i = 0; i < _magicParticleList.Count; i++)
            _magicParticleList[i].transform.localScale = new Vector3(distance, distance, distance) * 0.1f;

    }

    private void DestroyKame()
    {
        AudioSourceKame.Stop();
        if (currentKame)
        {
            Destroy(currentKame.gameObject);
            currentKame = null;
        }
    }

    private void ShootKame(Vector3 middlePosition, float speed)
    {

        if (speed > handIntensityToShoot && speed <= handMaxIntensity)
        {
            float launchSpeed = Mathf.InverseLerp(handIntensityToShoot, handMaxIntensity, speed);
            // _DistanceText.text = "launchSpeed: " + String.Format("{0:0.00}", launchSpeed);
            AudioSourceKame.Stop();
            //AudioSourceKame.clip = Launch;
            // AudioSourceKame.loop = false;
            // float pitch = Mathf.Lerp(0.5f, 2.5f, launchSpeed);
            float SpeedKame = Mathf.Lerp(0, shootMaxIntensity, launchSpeed);
            _hitsText.text = "launchSpeed: " + String.Format("{0:0.00}", launchSpeed);

            // AudioSourceKame.pitch = pitch;
            currentKame.GetComponent<KameHameHa>().Velocity = launchSpeed;
            // AudioSourceKame.PlayOneShot(Launch);
            currentKame.GetComponent<KameHameHa>().Size = distance * 100 / _kameHameMaxSize;
            currentKame.GetComponent<Rigidbody>().AddForce(middlePosition * SpeedKame);
            currentKame = null;
            _shotCount++;
        }
    }



}
