using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaneManage : MonoBehaviour
{
    ARPlaneManager planeManager;

    public void Start()
    {
        planeManager = GetComponent<ARPlaneManager>();
        //planeManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.None; // �̰� �ƹ��͵� �ν� ���ϴ°��� 

        
    }

    private void Update()
    {
        // �ż��� ��� Ȯ���� �Ⱥ��̵��� �ϴ°� 
        foreach (var trackable in planeManager.trackables)
        {
            trackable.GetComponent<Renderer>().enabled = false;
            trackable.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
        }
    }
}
