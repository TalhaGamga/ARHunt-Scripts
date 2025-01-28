using UnityEngine;

// Sword will be rendered as AR object.
public class Sword : MonoBehaviour
{
    // Variables
    /*
     float damage
     float length
     */

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(5);
        }
    }
}
