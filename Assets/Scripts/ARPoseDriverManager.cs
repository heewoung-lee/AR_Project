using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPoseDriverManager : MonoBehaviour
{
    private void Awake()
    {
        // �� ���� ��� ARPoseDriver�� ã���ϴ�.
        var poseDrivers = FindObjectsOfType<ARPoseDriver>();

        // ARPoseDriver�� �ϳ� �̻� �ִ��� Ȯ���մϴ�.
        if (poseDrivers.Length > 1)
        {
            // ù ��° ARPoseDriver�� ������ �������� ��Ȱ��ȭ�մϴ�.
            for (int i = 1; i < poseDrivers.Length; i++)
            {
                poseDrivers[i].enabled = false;
            }
        }
    }
}