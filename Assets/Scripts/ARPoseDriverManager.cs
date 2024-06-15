using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPoseDriverManager : MonoBehaviour
{
    private void Awake()
    {
        // 씬 내의 모든 ARPoseDriver를 찾습니다.
        var poseDrivers = FindObjectsOfType<ARPoseDriver>();

        // ARPoseDriver가 하나 이상 있는지 확인합니다.
        if (poseDrivers.Length > 1)
        {
            // 첫 번째 ARPoseDriver를 제외한 나머지를 비활성화합니다.
            for (int i = 1; i < poseDrivers.Length; i++)
            {
                poseDrivers[i].enabled = false;
            }
        }
    }
}