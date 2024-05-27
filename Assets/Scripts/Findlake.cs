using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Findlake : MonoBehaviour
{
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private GameObject fishingSpotPrefab; // 낚시터 프리팹
    [SerializeField] private Vector2 targetAreaCenter; // 특정 지역의 중심
    [SerializeField] private float targetAreaRadius; // 특정 지역의 반경

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
        fishingSpotPosition.y = plane.center.y; // 고도를 평면의 바닥으로 설정
        GameObject fishingSpot = Instantiate(fishingSpotPrefab, fishingSpotPosition, Quaternion.identity);
        fishingSpot.transform.SetParent(plane.transform); // 낚시터를 평면의 자식으로 설정하여 평면과 함께 이동
    }
}
