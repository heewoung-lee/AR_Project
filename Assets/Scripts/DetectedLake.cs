using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DetectedLake : MonoBehaviour
{
    public ARPlaneManager arPlaneManager;
    public GameObject planePrefab;
    public float minimumPlaneArea = 1.0f; // 최소 면적 (m^2)

    void OnEnable()
    {
        arPlaneManager.planesChanged += OnPlanesChanged;
    }

    void OnDisable()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            if (PlaneArea(plane) >= minimumPlaneArea)
            {
                Instantiate(planePrefab, plane.transform.position, Quaternion.identity);
            }
        }
    }

    float PlaneArea(ARPlane plane)
    {
        // 평면의 경계를 이용하여 면적 계산
        float area = 0;
        List<Vector2> boundaryPoints = new List<Vector2>(plane.boundary);
        for (int i = 0; i < boundaryPoints.Count; i++)
        {
            Vector2 current = boundaryPoints[i];
            Vector2 next = boundaryPoints[(i + 1) % boundaryPoints.Count];
            area += current.x * next.y - next.x * current.y;
        }
        return Mathf.Abs(area) * 0.5f;
    }
}
