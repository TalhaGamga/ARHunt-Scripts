using System;

public class Idle : IState
{
    public string type { get { return "Idle"; } set { } }

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
    }

    public void Update()
    {
    }
}