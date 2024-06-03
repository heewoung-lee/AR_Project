using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneManage : MonoBehaviour
{
    ARPlaneManager planeManager;

    public void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        //planeManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.None;

        
    }

    private void Update()
    {
        foreach (var trackable in planeManager.trackables)
        {
            trackable.GetComponent<Renderer>().enabled = false;
            trackable.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
        }
    }
}
