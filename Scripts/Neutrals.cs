using UnityEngine;

public interface IInitializable
{
    void Init(params object[] parameters);
}

public interface IFirearm : IInitializable
{
    public TriggerableWrapper<object?> OnFiring { get; set; }
    public TriggerableWrapper<object?> OnFired { get; set; }
    public TriggerableWrapper<object?> OnReloading { get; set; }
}


public interface ITriggerableWrapper // Non generic representator for generic triggerables
{
}

public interface IInputHandler
{
    public TriggerableWrapper<object?> FireTrigger { get; set; }
}

public class FirearmSharedContext
{
    public Transform firePoint;
    public Transform raycastPoint;
    public int chargeCount;
    public int ammoPerCharge;
    public float damage;
}

public interface IDamagable
{
    public void TakeDamage(float damage);
}

[CreateAssetMenu(fileName = "FirearmData", menuName = "ScriptableObjects/FirearmData")]
public class FirearmSO : ScriptableObject
{
    public int chargeCount;
    public int ammoPerCharge;
    public float damage;
}

[CreateAssetMenu(fileName = "NewVFX", menuName = "ScriptableObjects/VFX System/VFX Settings")]
public class VFXScriptableObject : ScriptableObject
{
    [Header("Visual Settings")]
    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private Vector3 effectScale = Vector3.one;
    [SerializeField] private float expireDuration = 2.0f;
    [SerializeField] private bool looping = false;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip audioClip;
    [Range(0f, 1f)][SerializeField] private float audioVolume = 1.0f;

    public GameObject EffectPrefab => effectPrefab;
    public Vector3 EffectScale => effectScale;
    public float ExpireDuration => expireDuration;
    public bool Looping => looping;
    public AudioClip AudioClip => audioClip;
    public float AudioVolume => audioVolume;

    public void Play(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (VFXManager.Instance != null)
        {
            VFXManager.Instance.PlayVFX(this, position, rotation, parent);
        }
    }
}

