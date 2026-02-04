using JasperFx.Core;
using Wolverine;
using Wolverine.Tracking;

namespace Petrichor.TestUtilities;

public static class WolverineTestExtensions
{
    public static async Task<(HttpResponseMessage, ITrackedSession)> TrackHttpCall(
        this IServiceProvider services,
        Func<Task<HttpResponseMessage>> call,
        TimeSpan? timeout = null)
    {
        timeout ??= 2.Seconds();

        var activity = services.TrackActivity(timeout.Value);

        HttpResponseMessage? response = null;

        var session = await activity.ExecuteAndWaitAsync((Func<IMessageContext, Task>)(async _ =>
        {
            response = await call();
        }));

        return (response!, session);
    }
}