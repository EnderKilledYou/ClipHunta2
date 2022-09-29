namespace ClipHunta2;

public sealed class DatabaseTaskManager : LongTaskManager<DatabaseTask>
{
    private static DatabaseTaskManager? _instance;

    public DatabaseTaskManager()
    {
        _longTasks = Array.Empty<DatabaseTask>();
    }

    public override DatabaseTask createOne()
    {
        return new DatabaseTask(_cancellationToken);
    }

    public static DatabaseTaskManager GetInstance()
    {
        if (_instance != null) return _instance;
        _instance = new DatabaseTaskManager();
        _cancellationToken = new CancellationTokenSource();

        return _instance;
    }
}