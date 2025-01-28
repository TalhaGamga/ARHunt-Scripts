using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private IFirearm firearm;

    public TriggerableWrapper<object?> OnFireCompleted = new TriggerableWrapper<object?>();

    public void Init()
    {
        firearm = GetComponentInChildren<IFirearm>();
    }

    private void OnEnable()
    {
        firearm.OnFiring.OnTrigger += triggerFire;
    }

    private void OnDisable()
    {
        firearm.OnFiring.OnTrigger -= triggerFire;
    }

    private void triggerFire(object? data)
    {
        animator.SetTrigger("Fire");
    }

    public void InvokeOnFireCompleted()
    {
        OnFireCompleted.OnTrigger?.Invoke(null);
    }
}
