using OpenTelemetry.Trace;
using System.Diagnostics;

namespace DemoApi.Services.Random
{
    public class RandomService : IRandomService
    {
        private readonly System.Random _random = new();
        private static readonly ActivitySource ActivitySource = new ActivitySource("DemoApi.Services.Random");

        public int Next(int min, int max)
        {
            using Activity? activity = ActivitySource.StartActivity("Next");
            activity?.SetTag("Min", min);
            activity?.SetTag("Max", max);

            // Simulate an error in 10% of the cases
            if (_random.NextDouble() < 0.1)
            {
                activity?.SetTag("Error", "Internal Server Error");
                activity?.SetStatus(Status.Error);
                throw new Exception("Internal Server Error");
            }

            int result = _random.Next(min, max);
            activity?.SetTag("Result", result);
            return result;
        }


        public double NextDouble()
        {
            using Activity? activity = ActivitySource.StartActivity("NextDouble");
            double result = _random.NextDouble();
            activity?.SetTag("Result", result);
            return result;
        }
    }
}