using System;
public class EventManager
{
    private static EventManager _instance;
    public static EventManager Instance => _instance ?? (_instance = new EventManager());

    public Action OnBirdDied;

    private EventManager()
    {
        // Initialize any additional data if needed
    }
}
