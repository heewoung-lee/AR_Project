using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneManage : MonoBehaviour
{
    ARPlaneManager planeManager;

    public void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        //planeManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.None; // 이건 아무것도 인식 안하는거임 

        
    }

    private void Update()
    {
        // 매순간 평면 확인을 안보이도록 하는거 
        foreach (var trackable in planeManager.trackables)
        {
            trackable.GetComponent<Renderer>().enabled = false;
            trackable.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
        }
    }
}
