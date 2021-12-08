using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public struct Gesture
{
    public string name;
    public List<Vector3> fingerDatas;
    public UnityEvent onRecognized;


}
public class OculusGestoureController : MonoBehaviour
{
    public float threshold = 0.1f;
    public bool debugMode = true;
    public OVRCustomSkeleton skeleton;
    public List<Gesture> gestures = new List<Gesture>();
    private List<OVRBone> fingerBones;

    private Gesture previousGesture;

    [Header("Score")]
    [SerializeField] private TMPro.TextMeshPro _GestureScoreText;

    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        previousGesture = new Gesture();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode && Input.GetKeyDown(KeyCode.Space))
        {
            save();
        }
        Gesture currentGesture = Reconized();
        bool hasRecognized = !currentGesture.Equals(new Gesture());
        //check if is new gesture
        if (!hasRecognized && !currentGesture.Equals(previousGesture))
        {
            Debug.Log("New gesture found: " + currentGesture.name);
         //   _GestureScoreText.text = "Gesture: " + currentGesture.name;
            previousGesture = currentGesture;
            currentGesture.onRecognized.Invoke();
        }
    }

    void save()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        Gesture g = new Gesture();
        g.name = "new gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (OVRBone bone in fingerBones)
        {
          //  Debug.Log("bone: " + bone.Id);
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }
        g.fingerDatas = data;
        gestures.Add(g);

    }
    Gesture Reconized()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        Gesture currentGesture = new Gesture();
        float currentMin = float.MaxValue;
        foreach (Gesture gesture in gestures)
        {
            float sumDistance = 0;
            bool isdiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currentData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currentData, gesture.fingerDatas[i]);
                if (distance > threshold)
                {
                   // Debug.Log("bone: " + fingerBones[i].Id + " distance: " + distance);
                    isdiscarded = true;
                    break;
                }
                sumDistance += distance;


            }
            if (!isdiscarded && sumDistance < currentMin)
            {
                //currentMin = sumDistance;
                currentGesture = gesture;
                 Debug.Log("New gesture found2: " + currentGesture.name);
                _GestureScoreText.text = "Gesture: " + currentGesture.name;
                break;
            }
            else{
                _GestureScoreText.text = "";
            }

        }
        return currentGesture;
    }
}
