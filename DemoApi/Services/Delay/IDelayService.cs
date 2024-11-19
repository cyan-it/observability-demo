namespace DemoApi.Services.Delay;

public interface IDelayService
{
    Task Delay(int milliseconds);
}