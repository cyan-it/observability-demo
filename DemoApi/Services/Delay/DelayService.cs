using System.Diagnostics;

namespace DemoApi.Services.Delay
{
    public class DelayService : IDelayService
    {
        private static readonly ActivitySource ActivitySource = new ActivitySource("DemoApi.Services.Delay");

        public async Task Delay(int milliseconds)
        {
            using Activity? activity = ActivitySource.StartActivity("Delay");
            activity?.SetTag("DelayTime", milliseconds);
            await Task.Delay(milliseconds);
        }
    }
}