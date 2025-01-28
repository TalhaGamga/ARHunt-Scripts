using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    public LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private Transform _obstaclesParent;
    [SerializeField] private Camera mainCamera;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public void CreatePhysicsScene()
    {
        if (_simulationScene.IsValid())
        {
            return;
        }

        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _obstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);

            if (ghostObj.TryGetComponent<Renderer>(out Renderer renderer))
            {
                renderer.enabled = false;
            }

            foreach (var collider in ghostObj.GetComponentsInChildren<Collider>())
            {
                collider.isTrigger = false;
            }

            SetLayerRecursively(ghostObj, LayerMask.NameToLayer("Simulation"));

            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);

            if (!ghostObj.isStatic)
            {
                _spawnedObjects.Add(obj, ghostObj.transform);
            }
        }

    }

    private void Update()
    {
        foreach (var item in _spawnedObjects)
        {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory(Ball ballPrefab, Vector3 pos, Vector3 velocity)
    {
        if (_simulationScene == null || !_simulationScene.IsValid())
        {
            return;
        }

        var ghostObj = Instantiate(ballPrefab, pos, Quaternion.identity);

        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        ghostObj.Init(velocity, true);

        _line.positionCount = _maxPhysicsFrameIterations;

        float maxDistance = 0f;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            Vector3 position = ghostObj.transform.position;
            _line.SetPosition(i, position);

            float distance = Vector3.Distance(pos, position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
            }
        }

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Clamp(60f + maxDistance / 10f, 60f, 120f); 
        }

        Destroy(ghostObj.gameObject);
    }
}
