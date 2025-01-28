using System;
public class TriggerableWrapper<T> : ITriggerableWrapper
{
    public Action<T> OnTrigger;
}