using UnityEngine;

public class Pistol : MonoBehaviour, IFirearm
{
    [Header("Pistol Related")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private FirearmSO firearmData;
    [SerializeField] private FirearmSharedContext context;
    [SerializeField] private VFXScriptableObject fireVfx;

    private Camera cam;

    [Header("State Machine")]
    [SerializeField] private StateMachine stateMachine;

    private AnimatorManager animatorManager;

    public TriggerableWrapper<object> OnFiring { get; set; } = new TriggerableWrapper<object>();
    public TriggerableWrapper<object> OnReloading { get; set; } = new TriggerableWrapper<object>();
    public TriggerableWrapper<object> OnFired { get; set; } = new TriggerableWrapper<object>();

    private PlayerInputHandler inputHandler;

    private IState idle;
    private IState fire;
    private IState reload;

    private StateTransition idleToFire;
    private StateTransition fireToIdle;


    private void OnEnable()
    {
        addActionCallbacks();
    }

    private void OnDisable()
    {
        removeActionCallbacks();
    }

    public void Init(params object[] parameters)
    {
        stateMachine = new StateMachine();

        cam = (Camera)parameters[0];

        context = new FirearmSharedContext
        {
            ammoPerCharge = firearmData.ammoPerCharge,
            damage = firearmData.damage,
            chargeCount = firearmData.chargeCount,
            firePoint = firePoint,
            raycastPoint = cam.transform
        };

        idle = new Idle();
        fire = new Fire(context);

        stateMachine.SetState(idle);

        idleToFire = new StateTransition(idle, fire, () => { return true; });
        fireToIdle = new StateTransition(fire, idle, () => { return true; });

        inputHandler = GetComponentInParent<PlayerInputHandler>();
        animatorManager = GetComponentInParent<AnimatorManager>();
    }

    private void addActionCallbacks()
    {
        stateMachine.AddNormalTransitionTrigger(InputTriggerContainer.Instance?.FireTrigger, idleToFire);
        stateMachine.AddNormalTransitionTrigger(animatorManager.OnFireCompleted, fireToIdle);

        fire.OnStateEnter += onFireEntered;
    }

    private void removeActionCallbacks()
    {
        stateMachine.RemoveTrigger(InputTriggerContainer.Instance?.FireTrigger);
        stateMachine.RemoveTrigger(OnFired);

        fire.OnStateEnter -= onFireEntered;
    }

    private void onFireEntered(object var)
    {
        OnFiring.OnTrigger?.Invoke(var);
        fireVfx.Play(firePoint.position, Quaternion.identity, firePoint);
    }
}