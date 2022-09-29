using System.Collections.Concurrent;
using System.Diagnostics;
using Serilog;

namespace ClipHunta2;

public partial class LongTask<T> : LongTask
{
    public LongTask(CancellationTokenSource cts) : base(cts)
    {
    }

    private readonly ConcurrentQueue<LongTaskQueueItem<T>> _queue = new();
    private Stopwatch sw = new Stopwatch();
    public override int Count()
    {
        return _queue.Count;
    }

    protected override async Task _iteration()
    {
        var value = await _take();
        if (value == null)
        {
            await _sleep(DefaultSleep);
            return;
        }

        try
        {

            sw.Start();
            await _action(value.Item);
            sw.Stop();
            var total = sw.Elapsed.Milliseconds;
            if (total < fastestMilliSeconds)
            {
                fastestMilliSeconds = total;
            }
            _finished++;
            averageMilliSeconds = total / _finished;
            if (_queue.Count > maxBackPressure)
            {
                maxBackPressure = _queue.Count;
            }

        }
        catch (Exception e)
        {
            Log.Logger.Error("Error in _iteration {Message} Stack: {Stack}", e.Message, e.StackTrace);
        }
    }

    protected virtual async Task _action(T value)
    {
    }

    protected virtual LongTask<T>? GetTop()
    {
        return null;
    }

    private async Task<LongTaskQueueItem<T>?> _take()
    {
        if (_queue.TryDequeue(out var tmp))
            return tmp;
        var man = GetTop();

        if (man == null) return default;
        if (man._queue.Count < 2) return default;
        if (man._queue.TryDequeue(out var tmp2))
            return tmp2;

        return default;
    }

    public async Task Put(LongTaskQueueItem<T> work)
    {
        Task.Run(() => { _queue.Enqueue(work); });
    }
    int _finished = 0;
    int averageMilliSeconds = 0;
    int maxMilliSeconds = 0;
    int maxBackPressure = 0;
    int fastestMilliSeconds = int.MaxValue;
    public override int Finished()
    {
        return _finished;
    }

    public override int AverageMilliSeconds()
    {
        return averageMilliSeconds;
    }

    public override int MaxMilliSeconds()
    {
        return maxMilliSeconds;
    }

    public override int MaxBackPressure()
    {
        return maxBackPressure;
    }

    public override int FastestMilliSecond()
    {
        return fastestMilliSeconds;
    }
}