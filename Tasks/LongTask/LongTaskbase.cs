namespace ClipHunta2;

public abstract class LongTask
{
    private readonly CancellationTokenSource _cts;
    private readonly AutoResetEvent _are = new AutoResetEvent(false);

    protected static readonly TimeSpan DefaultSleep = TimeSpan.FromSeconds(1);
    private Task? _task;

    public LongTask(CancellationTokenSource cts)
    {
        _cts = cts;
    }

    protected async Task _sleep(TimeSpan amount)
    {
        await Task.Delay(amount);
    }

    public abstract int Count();
    public abstract int Finished();
    public abstract int AverageMilliSeconds();
    public abstract int MaxMilliSeconds();
    public abstract int MaxBackPressure();
    public abstract int FastestMilliSecond();

    public override string ToString()
    {
        return $"Total: {Count()}";
    }

    public virtual void StartTask()
    {
        if (_task != null)
        {
            throw new Exception("Task is still running");
        }
    
        _task = Task.Run(() =>
        {
       
            while (!_cts.IsCancellationRequested)
            {
          
                _iteration().Wait();
           }

            _task = null;
        });
    }

    protected virtual async Task _iteration()
    {
    }
}