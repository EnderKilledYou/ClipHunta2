using ClipHost.ServiceModel;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ClipHunta2;

public class LongTaskManager<T> : ILongTaskManagerReports where T : LongTask
{
    private readonly CancellationTokenSource _cts;
    protected T[] _longTasks;
    private static LongTaskManager<T> _instance;
    protected static CancellationTokenSource _cancellationToken;
    protected ConcurrentDictionary<string, int> _reportIds = new();

    public virtual T createOne()
    {
        return default(T);
    }

    public override string ToString()
    {
        return string.Join("\n", _longTasks.Select(a => a.ToString()));
    }

    public CommandCenterReport[] GetReports()
    {

        return _longTasks.Select(LongTaskToReport).ToArray();
    }
    private CommandCenterReport LongTaskToReport(T a, int i)
    {
        var dtoId = 0;
        var processId = Environment.ProcessId;
        string tubeName = $"{typeof(T).Name} {i}";
        if (_reportIds.ContainsKey(tubeName))
        {
            dtoId = _reportIds[tubeName];
        }
        return new CommandCenterReport()
        {
            Name = tubeName,
            Size = a.Count(),
            TotalProcessed = a.Finished(),
            ProcessId = processId,
            AverageSeconds = a.AverageMilliSeconds(),
            HighSeconds = a.MaxMilliSeconds(),
            Low = a.FastestMilliSecond(),
            MaxSize = a.MaxBackPressure(),
            _processId = processId,
            Id = dtoId
        };
    }
    public T? GetLongTasker()
    {
        var tmp = _longTasks;
        if (tmp.Length == 0)
        {
            return null;
        }

        return tmp.OrderBy(SortTasks).First();
    }

    public T? GetTopTasker()
    {
        var tmp = _longTasks;
        if (tmp.Length == 0)
        {
            return null;
        }

        return tmp.OrderByDescending(SortTasks).First();
    }

    private static int SortTasks(T t)
    {
        return t.Count();
    }

    public void AddLongTasker()
    {
        var _longTask = createOne();
        _longTask.StartTask();
        List<T> tmp = new List<T>(_longTasks);
        tmp.Add(_longTask);
        _longTasks = tmp.ToArray();
        tmp.Clear();
        tmp = null;
    }

    public void UpdateReportWithId(string name, int Id)
    {
        _reportIds.AddOrUpdate(name, (name) => Id, (name, Id) => Id);
    }
}