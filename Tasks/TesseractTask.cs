using Serilog;
using Tesseract;

namespace ClipHunta2;

public class TesseractTask : LongTaskWithReturn<Pix, string>, IDisposable
{
    public TesseractTask(CancellationTokenSource _ctr, string tesseractDataPath = @"c:\tmp\tessdata_fast",
        string tesseractLanguage = "eng",
        EngineMode mode = EngineMode.LstmOnly) : base(_ctr)
    {
        _engine = new TesseractEngine(tesseractDataPath, tesseractLanguage, mode);
        _engine.SetVariable("debug_file", "/dev/null");
        _engine.SetVariable("load_system_dawg", false);
        _engine.SetVariable("load_freq_dawg", false);
        _engine.SetVariable("tessedit_char_whitelist", "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321+ ");
        _count = new ThreadSafeInt(0);
    }

    public override void StartTask()
    {
    }

    public override int Count()
    {
        return _count.Value;
    }

    protected object? GetTop()
    {
        return TesseractLongTaskManager.GetInstance().GetTopTasker();
    }

    private TesseractEngine _engine;
    private readonly ThreadSafeInt _count;


 

    public void Dispose()
    {
        _engine.Dispose();
    }

    public async Task<string?> GetText(Pix pix)
    {
        _count.Increment();
        Monitor.Enter(_engine);
        try
        {
            using var page = _engine.Process(pix, PageSegMode.SingleColumn);
            return page.GetText();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error in Get Text");
            Console.WriteLine(ex);
        }
        finally
        {
            Monitor.Exit(_engine);
            _count.Decrement();
        }

        return null;
    }
}