using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] private GameObject birdPrefab;
    [SerializeField] private int birdCount = 10;
    [SerializeField] private float spawnRadius = 25f;
    private Transform _target;

    private List<Bird> birds = new List<Bird>();

    public void SetTaregt(Transform target)
    {
        _target = target;
    }

    public void Init(int count)
    {
        SpawnFlock(count);
    }

    private void SpawnFlock(int count)
    {
        if (birdPrefab == null)
        {
            Debug.LogError("Bird prefab is not assigned.");
            return;
        }

        if (_target == null)
        {
            Debug.LogError("Target is not assigned.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            spawnRadius = Random.Range(25f, 50f);
            Vector3 spawnPosition = _target.position + Random.insideUnitSphere * spawnRadius;
            spawnPosition.y = transform.position.y;

            GameObject birdObject = Instantiate(birdPrefab, spawnPosition, Quaternion.identity);

            Bird bird = birdObject.GetComponent<Bird>();
            if (bird != null)
            {
                bird.Init(_target);
                birds.Add(bird);
            }
        }
    }

    public void ClearFlock()
    {
        foreach (Bird bird in birds)
        {
            if (bird != null)
            {
                Destroy(bird.gameObject);
            }
        }

        birds.Clear();
    }

    public void RespawnFlock(int count)
    {
        ClearFlock();
        SpawnFlock(count);
    }
}
