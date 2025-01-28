using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class EnemyBehaviour : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public float moveSpeed = 1.5f;
    private Vector3 targetPosition;

    [SerializeField] private ARPlaneManager planeManager;

    void Start()
    {
        currentHealth = maxHealth;

#if UNITY_EDITOR
        planeManager = FindObjectOfType<ARPlaneManager>();
        //Debug.Log("Running in Unity Editor. Using hardcoded bounds for movement.");
        SetRandomTargetEditor(); // Use Unity Editor-specific movement logic
#else
        planeManager = FindObjectOfType<ARPlaneManager>();
        SetRandomTarget(); // Use AR plane logic
#endif
    }

    void Update()
    {
        MoveTowardsTarget();

        // Rotate to face the target while moving
        Vector3 direction = targetPosition - transform.position;
        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
    }

    void MoveTowardsTarget()
    {
        // Move the boss towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If the boss reaches the target, set a new random target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
#if UNITY_EDITOR
            SetRandomTargetEditor();
#else
            SetRandomTarget();
#endif
        }
    }

    void SetRandomTarget()
    {
        if (planeManager.trackables.count > 0)
        {
            foreach (var plane in planeManager.trackables)
            {
                Vector3 center = plane.transform.position;
                Vector2 size = plane.size;

                float randomX = Random.Range(-size.x / 2, size.x / 2);
                float randomZ = Random.Range(-size.y / 2, size.y / 2);

                targetPosition = center + new Vector3(randomX, 0, randomZ);
                return; // Pick the first plane for simplicity
            }
        }
    }

    void SetRandomTargetEditor()
    {
        if (planeManager.trackables.count > 0)
        {
            foreach (var plane in planeManager.trackables)
            {
                Vector3 center = plane.transform.position;
                Vector2 size = plane.size;

                float randomX = Random.Range(-size.x / 2, size.x / 2);
                float randomZ = Random.Range(-size.y / 2, size.y / 2);

                targetPosition = center + new Vector3(randomX, 0, randomZ);
                return; // Pick the first plane for simplicity
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Boss defeated!");
        Destroy(gameObject);
    }
}
