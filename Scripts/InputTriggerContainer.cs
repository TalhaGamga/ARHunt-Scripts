public class InputTriggerContainer
{
    // Singleton instance
    private static InputTriggerContainer _instance;
    public static InputTriggerContainer Instance => _instance ?? (_instance = new InputTriggerContainer());

    public TriggerableWrapper<object> FireTrigger { get; private set; } = new TriggerableWrapper<object> { };
    public TriggerableWrapper<object> ReloadTrigger { get; private set; } = new TriggerableWrapper<object> { };

    private InputTriggerContainer()
    {
        // Initialize any additional data if needed
    }
}
