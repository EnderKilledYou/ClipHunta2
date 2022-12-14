using Tesseract;

namespace ClipHunta2;

public sealed class ImageScannerTaskManager : LongTaskManager<ImageScannerTask>
{ 
    private static ImageScannerTaskManager? _instance; 

    public ImageScannerTaskManager()
    {
        _longTasks = Array.Empty<ImageScannerTask>();
    }

    public override ImageScannerTask createOne()
    {
        return new ImageScannerTask(_cancellationToken);
    }

    public static ImageScannerTaskManager GetInstance()
    {
        if (_instance != null) return _instance;
        _instance = new ImageScannerTaskManager();
        _cancellationToken = new CancellationTokenSource();

        return _instance;
    }

  
}