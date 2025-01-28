using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("Pooling Settings")]
    [SerializeField] private int poolSize = 10;
    private Dictionary<VFXScriptableObject, Queue<GameObject>> vfxPool = new Dictionary<VFXScriptableObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayVFX(VFXScriptableObject vfx, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (vfx == null || vfx.EffectPrefab == null)
        {
            Debug.LogWarning("VFX or its prefab is not assigned.");
            return;
        }

        if (!vfxPool.ContainsKey(vfx))
            vfxPool[vfx] = new Queue<GameObject>();

        GameObject effectInstance;

        if (vfxPool[vfx].Count > 0)
        {
            effectInstance = vfxPool[vfx].Dequeue();
            effectInstance.transform.position = position;
            effectInstance.transform.rotation = rotation;
            effectInstance.transform.localScale = vfx.EffectScale;
            effectInstance.SetActive(true);
        }
        else
        {
            effectInstance = Instantiate(vfx.EffectPrefab, position, rotation, parent);
            effectInstance.transform.localScale = vfx.EffectScale;
        }

        ParticleSystem[] allEffects = effectInstance.GetComponentsInChildren<ParticleSystem>();
        foreach (var effect in allEffects)
        {
            effect.Play(true);
        }

        if (vfx.AudioClip != null)
        {
            AudioSource.PlayClipAtPoint(vfx.AudioClip, position, vfx.AudioVolume);
        }

        if (!vfx.Looping)
        {
            StartCoroutine(DisableAfterDelay(effectInstance, vfx.ExpireDuration, vfx));
        }
    }

    private System.Collections.IEnumerator DisableAfterDelay(GameObject instance, float delay, VFXScriptableObject vfx)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
        vfxPool[vfx].Enqueue(instance);
    }
}
