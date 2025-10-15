namespace Petrichor.TestUtilities;

public static class Poller
{
    public static async Task WaitAsync(TimeSpan timeout, Func<Task<bool>> condition)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        DateTime endTimeUtc = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTimeUtc && await timer.WaitForNextTickAsync())
        {
            if (await condition()) return;
        }

        throw new TimeoutException("Condition not met within timeout.");
    }
}