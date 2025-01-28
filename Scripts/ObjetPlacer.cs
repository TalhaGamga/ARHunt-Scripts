using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private GameObject bossPrefab;
    private ARRaycastManager raycastManager;

    private GameObject spawnedBoss;

    private void Start()
    {
#if UNITY_EDITOR
        Debug.Log("Running in Unity Editor. Using mouse input for placement.");
#else
        raycastManager = GetComponent<ARRaycastManager>();
#endif
    }

    private void Update()
    {
//#if UNITY_EDITOR
//        // Simulate touch input in the Editor using mouse clicks
//        if (Input.GetMouseButtonDown(0))
//        {
//            Vector3 mousePosition = Input.mousePosition;
//            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

//            if (Physics.Raycast(ray, out RaycastHit hit))
//            {
//                if (spawnedBoss == null)
//                {
//                    spawnedBoss = Instantiate(bossPrefab, hit.point, Quaternion.identity);
//                }
//            }
//        }
//#else
         //AR mode: Handle touch inputs for placing the boss
        if (Input.touchCount > 0 && spawnedBoss == null)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;

                     //Instantiate the boss prefab on the detected plane
                    spawnedBoss = Instantiate(bossPrefab, hitPose.position, hitPose.rotation);
                }
            }
        }
//#endif
    }
}
