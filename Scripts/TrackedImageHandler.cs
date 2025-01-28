using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TrackedImageHandler : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager; 
    [SerializeField] private GameObject cgiModelPrefab; 

    private readonly Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();

    public void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            if (!spawnedObjects.ContainsKey(trackedImage.trackableId.ToString()))
            {
                var cgiModel = Instantiate(cgiModelPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                cgiModel.transform.parent = trackedImage.transform;
                cgiModel.SetActive(true);

                spawnedObjects[trackedImage.trackableId.ToString()] = cgiModel;
            }
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            if (spawnedObjects.TryGetValue(trackedImage.trackableId.ToString(), out var cgiModel))
            {
                if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    cgiModel.transform.position = trackedImage.transform.position;
                    cgiModel.transform.rotation = trackedImage.transform.rotation;
                    cgiModel.SetActive(true);
                }
                else
                {
                    cgiModel.SetActive(false);
                }
            }
        }
    }
}
