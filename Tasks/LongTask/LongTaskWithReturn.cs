using System.Collections.Concurrent;
using System.Diagnostics;
using Serilog;

namespace ClipHunta2;

public partial class LongTaskWithReturn<T, TR> : LongTask
{
    private readonly ConcurrentQueue<LongTaskQueueItemWithReturn<T, TR>> _queue = new();
    private Stopwatch sw = new Stopwatch();
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

            var tmp = await _action(value.Item);

            value.ReturnQueue.SetValue(tmp);
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

    public override int Count()
    {
        return _queue.Count;
    }


    protected async Task<LongTaskQueueItemWithReturn<T, TR>?> _take()
    {
        if (_queue.TryDequeue(out var tmp))
            return tmp;
        return default;
    }

    public LongTaskQueueItemWithReturn<T, TR>? Take()
    {
        var take = _take();
        take.Wait();
        return take.Result;
    }

    public async Task Put(LongTaskQueueItemWithReturn<T, TR> work)
    {
        Task.Run(() => { _queue.Enqueue(work); });
    }

    public async Task<TR?> PutAndGet(T item)
    {
        var returnQueue = new ReturnQueue();
        var work = new LongTaskQueueItemWithReturn<T, TR>(item, returnQueue);
        _queue.Enqueue(work);
        var retval = returnQueue.GetReturn();
        work = null;
        returnQueue = null;
        return retval;
    }

    protected virtual async Task<TR?> _action(T value)
    {
        return default;
    }


    public LongTaskWithReturn(CancellationTokenSource cts) : base(cts)
    {
    }
}