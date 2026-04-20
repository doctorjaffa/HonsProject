/* Event called during the scene that will trigger the crime response function in another script */

using System;

public static class EventManager
{
    public static event Action OnCommitCrime;

    public static void TriggerCrimeResponse()
    {
        OnCommitCrime?.Invoke();
    }
}
