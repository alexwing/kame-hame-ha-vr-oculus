
using UnityEngine;

public class KameHameHa : MonoBehaviour
{

    [SerializeField] private int FrameRate = 1;
    public float Distance = 20;
    public float Size = 1;
    public float Velocity = 1;
    void Start()
    {
        InvokeRepeating("CheckDistance", 0, 0.5f / FrameRate);
    }

    private void CheckDistance()
    {
        float cameraDistance = Vector3.Distance(SceneConfig.MainCamera.position, transform.position);
        if (cameraDistance > Distance)
        {
            Destroy(gameObject);
        }
    }

    void OnDisable()
    {
        CancelInvoke("CheckDistance");
    }
    private void OnDestroy()
    {
        CancelInvoke("CheckDistance");
    }
}




