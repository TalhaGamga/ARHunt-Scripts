using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PointCloudManager : MonoBehaviour
{
    private ARPointCloudManager _arPointCloudManager;
    private List<Vector3> pointCloudPositions = new List<Vector3>();

    void Start()
    {
        _arPointCloudManager = FindObjectOfType<ARPointCloudManager>();
        if (_arPointCloudManager == null)
        {
            Debug.LogError("ARPointCloudManager is not found in the scene.");
        }
    }

    void Update()
    {
        UpdatePointCloudData();
    }

    private void UpdatePointCloudData()
    {
        pointCloudPositions.Clear();

        foreach (var pointCloud in _arPointCloudManager.trackables)
        {
            foreach (var point in pointCloud.positions)
            {
                pointCloudPositions.Add(point);
            }
        }
    }

    public Vector3 GetRandomPoint()
    {
        if (pointCloudPositions.Count == 0) return Vector3.zero;

        int randomIndex = Random.Range(0, pointCloudPositions.Count);
        return pointCloudPositions[randomIndex];
    }
}
