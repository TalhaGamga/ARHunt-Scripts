using System;
using UnityEngine;

public class Fire : IState
{
    FirearmSharedContext _context;
    public Fire(FirearmSharedContext context)
    {
        _context = context;
    }

    public string type { get { return "Fire"; } set { } }

    public Action<object> OnStateEnter { get; set; }
    public Action<object> OnStateExit { get; set; }
    public Action<object> OnStateUpdate { get; set; }

    public void Enter()
    {
        OnStateEnter?.Invoke(null);

        shoot();
    }

    public void Exit()
    {
        OnStateExit?.Invoke(null);
    }

    public void Tick()
    {
    }

    public void Update()
    {
    }

    private void shoot()
    {
        RaycastHit hit;

        Physics.Raycast(_context.raycastPoint.position, _context.raycastPoint.forward, out hit, float.MaxValue);
        if (hit.transform != null && hit.transform.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(1);
        }
    }
}
