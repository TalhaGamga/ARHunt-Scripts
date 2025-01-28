using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private AnimatorManager animatorManager;
    [SerializeField] private IFirearm firearm;

    public void InitFirearm(Camera cam)
    {
        firearm = GetComponentInChildren<IFirearm>();
        firearm.Init(cam);
    }

    public void InitAnimatorManager()
    {
        animatorManager = GetComponentInChildren<AnimatorManager>();

        animatorManager.Init();
    }

    public void EnableFirearm()
    {
        if (firearm is MonoBehaviour firearmComponent)
        {
            firearmComponent.enabled = true;
        }
    }

    public void EnableAnimatorManager()
    {
        if (animatorManager is MonoBehaviour animatorManagerComponent)
        {
            animatorManagerComponent.enabled = true;
        }
    }
}