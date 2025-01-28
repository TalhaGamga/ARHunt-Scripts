using System;
using UnityEngine;

public class Bird : MonoBehaviour, IDamagable
{
    [SerializeField] private StateMachine stateMachine;

    private IState dive;
    private IState roam;

    [SerializeField] private BirdSharedContext context;

    [SerializeField] private float diveRange = 10;
    [SerializeField] private float diveRangeMin;
    [SerializeField] private float diveRangeMax;

    [SerializeField] private float roamRange = 50;
    [SerializeField] private float roamRangeMin;
    [SerializeField] private float roamRangeMax;

    [SerializeField] private VFXScriptableObject dieVfx;
    [SerializeField] private VFXScriptableObject diveVfx;
    public void Init(Transform target)
    {
        context = new BirdSharedContext
        {
            bird = gameObject.transform,
            _target = target,
            diveVfx = diveVfx
        };

        roam = new Roam(context);
        dive = new Dive(context);

        stateMachine = new StateMachine();

        StateTransition roamToDive = new StateTransition(roam, dive, () => { return context.distanceToTarget < diveRange; });
        roamToDive.SetOnTransition(() => { setDiveRange(); });

        StateTransition diveToRoam = new StateTransition(dive, roam, () => { return context.distanceToTarget > roamRange; });
        diveToRoam.SetOnTransition(() => { setRoamRange(); });

        stateMachine.AddNormalTransition(roamToDive);
        stateMachine.AddNormalTransition(diveToRoam);

        stateMachine.SetState(roam);
    }

    private void Update()
    {
        checkTargetDistance();

        stateMachine?.Update();
    }

    private void setRoamRange()
    {
        roamRange = UnityEngine.Random.Range(roamRangeMin, roamRangeMax);
    }

    private void setDiveRange()
    {
        diveRange = UnityEngine.Random.Range(diveRangeMin, diveRangeMax);
    }

    private void checkTargetDistance()
    {
        context.distanceToTarget = Vector3.Distance(transform.position, context._target.position);
    }

    public void TakeDamage(float damage)
    {
        dieVfx?.Play(gameObject.transform.position, Quaternion.identity);
        EventManager.Instance.OnBirdDied?.Invoke();
        Destroy(gameObject);
    }

    public class Roam : IState
    {
        private BirdSharedContext _context;
        private float _rotationSpeed = 0.5f;
        private float _moveSpeed = 5.0f;

        public Roam(BirdSharedContext context)
        {
            _context = context;
        }

        public string type { get { return "Roam"; } set { } }
        public Action<object> OnStateEnter { get; set; }
        public Action<object> OnStateExit { get; set; }
        public Action<object> OnStateUpdate { get; set; }

        public void Enter()
        {
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            Update();
        }

        public void Update()
        {
            if (_context._target == null) return;

            Vector3 directionToTarget = (_context._target.position - _context.bird.position).normalized;

            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);

            _context.moveSpeed = Mathf.Lerp(_context.moveSpeed, _moveSpeed, 0.1f);

            _context.bird.rotation = Quaternion.Slerp(
                _context.bird.rotation,
                lookRotation,
                Time.deltaTime * _rotationSpeed
            );

            _context.bird.position += _context.bird.forward * _context.moveSpeed * Time.deltaTime;
        }
    }

    public class Dive : IState
    {
        public Dive(BirdSharedContext context)
        {
            _context = context;
        }

        public string type { get { return "Dive"; } set { } }
        public Action<object> OnStateEnter { get; set; }
        public Action<object> OnStateExit { get; set; }
        public Action<object> OnStateUpdate { get; set; }

        private BirdSharedContext _context;
        private Vector3 diveDirection;
        private float diveSpeed = 20f;       // Speed of the dive
        private float rotationSpeed = 0.8f;    // Speed of the rotation

        public void Enter()
        {
            diveDirection = (_context._target.position - _context.bird.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(diveDirection);
            _context.bird.rotation = targetRotation;
            _context.diveVfx.Play(_context.bird.position, Quaternion.identity);
        }

        public void Exit()
        {
        }

        public void Tick()
        {
            Update();
        }

        public void Update()
        {
            Quaternion targetRotation = Quaternion.LookRotation(diveDirection);
            _context.bird.rotation = Quaternion.Slerp(
                _context.bird.rotation,        // Current rotation
                targetRotation,                // Target rotation
                Time.deltaTime * rotationSpeed // Smoothness
            );

            _context.moveSpeed = Mathf.Lerp(_context.moveSpeed, diveSpeed, 0.1f);
            _context.bird.position += _context.bird.forward * _context.moveSpeed * Time.deltaTime;
        }
    }

    [System.Serializable]
    public class BirdSharedContext
    {
        public Transform bird;
        public Transform _target;
        public float distanceToTarget;
        public float moveSpeed;
        public VFXScriptableObject diveVfx;
    }
}