using BlazorQueue;
using ClipHunta2;
using Serilog;
using ServiceStack;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/testing.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

Console.WriteLine("Hello, World!");

#if TrainingData
TrainingDataTaskManager.GetInstance().AddLongTasker();
#endif
ImageScannerTaskManager.GetInstance().AddLongTasker();
ImageScannerTaskManager.GetInstance().AddLongTasker();
ImageScannerTaskManager.GetInstance().AddLongTasker();
ImageScannerTaskManager.GetInstance().AddLongTasker();
EventRouterTaskManager.GetInstance().AddLongTasker();
TesseractLongTaskManager.GetInstance().AddLongTasker();
TesseractLongTaskManager.GetInstance().AddLongTasker();
ImagePrepperTaskManager.GetInstance().AddLongTasker();
ImagePrepperTaskManager.GetInstance().AddLongTasker();

Console.WriteLine(args);
string baseUri = "https://localhost:5001/";
string userName = "";
string password = "";

var apiClient = new JsonApiClient(baseUri);


//try
//{
//    var authResult = await apiClient.PostAsync(new Authenticate()
//    {
//        UserName = userName,
//        Password = password
//    });
//}
//catch (Exception ex)
//{
//    //todo: what if the generated login gets misconfigured and doesn't work. Well ur right here looking at this.
//    Log.Logger.Error(ex, "Yeah this nasty error {error}");
//    return;
//}



//await ClipBlazorFacadeHelper.Watch("bestboyfriend4", apiClient);




if (args.Length > 1)
{
    baseUri = args[1];
}


var info = new HubConnectionInfo(baseUri, "ClipHub", null);


ClipBlazorFacade parent = new(info, ApiClient: apiClient);

await parent.Start();

await parent.Running();



