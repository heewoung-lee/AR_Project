using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Findlake : MonoBehaviour
{
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject fishingSpotPrefab; // ������ ������
    [SerializeField] private Vector2 targetAreaCenter; // Ư�� ������ �߽�
    [SerializeField] private float targetAreaRadius; // Ư�� ������ �ݰ�

    private List<ARPlane> detectedPlanes = new List<ARPlane>();

    void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (ARPlane plane in eventArgs.added)
        {
            Vector3 planePosition = plane.transform.position;
            if (IsWithinTargetArea(planePosition))
            {
                detectedPlanes.Add(plane);
                CreateFishingSpot(plane);
            }
        }
    }

    bool IsWithinTargetArea(Vector3 position)
    {
        Vector2 planePosition2D = new Vector2(position.x, position.z);
        return Vector2.Distance(planePosition2D, targetAreaCenter) <= targetAreaRadius;
    }

    void CreateFishingSpot(ARPlane plane)
    {
        Vector3 fishingSpotPosition = plane.transform.position;
        fishingSpotPosition.y = plane.center.y; // ���� ����� �ٴ����� ����
        GameObject fishingSpot = Instantiate(fishingSpotPrefab, fishingSpotPosition, Quaternion.identity);
        fishingSpot.transform.SetParent(plane.transform); // �����͸� ����� �ڽ����� �����Ͽ� ���� �Բ� �̵�
    }
}
