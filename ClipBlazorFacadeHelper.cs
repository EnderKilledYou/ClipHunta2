using ClipHost.ServiceModel;
using ClipHost.ServiceModel.CreateCommandCenterReportModels;
using ClipHunta2;
using Serilog;
using ServiceStack;

public class ClipBlazorFacadeHelper
{
    public static async Task Clip(string streamer, int twitchClipId, string twitchClip, JsonApiClient apiClient)
    {



        var cancellationTokenSource = new CancellationTokenSource();
        StreamCaptureTaskStarterTask streamCaptureTaskStarterTask =
            new(cancellationTokenSource, streamer, StreamCaptureType.Clip);

        var streamStatus = streamCaptureTaskStarterTask.Start(twitchClip, twitchClipId);

        while (streamStatus.FinishedCount != streamStatus.FinalFrameCount)
        {

            await LongTaskerReports(apiClient, ImagePrepperTaskManager.GetInstance());
            await LongTaskerReports(apiClient, ImageScannerTaskManager.GetInstance());
            await LongTaskerReports(apiClient, EventRouterTaskManager.GetInstance());
            await LongTaskerReports(apiClient, TesseractLongTaskManager.GetInstance());
            await Task.Delay(500, cancellationTokenSource.Token);
        }

        cancellationTokenSource.Cancel(false);

    }

    public static async Task Watch(string streamer, int twitchStreamId, JsonApiClient apiClient)
    {




        var cancellationTokenSource = new CancellationTokenSource();
        StreamCaptureTaskStarterTask streamCaptureTaskStarterTask =
            new(cancellationTokenSource, streamer, StreamCaptureType.Stream);

        var streamStatus = streamCaptureTaskStarterTask.Start(streamer, twitchStreamId);

        while (streamStatus.FinishedCount != streamStatus.FinalFrameCount)
        {

            await LongTaskerReports(apiClient, ImagePrepperTaskManager.GetInstance());
            await LongTaskerReports(apiClient, ImageScannerTaskManager.GetInstance());
            await LongTaskerReports(apiClient, EventRouterTaskManager.GetInstance());
            await LongTaskerReports(apiClient, TesseractLongTaskManager.GetInstance());
            await Task.Delay(500, cancellationTokenSource.Token);
        }

        cancellationTokenSource.Cancel(false);

    }

    private static async Task LongTaskerReports(JsonApiClient apiClient, ILongTaskManagerReports imagePrepperTaskManager)
    {
        var reports = imagePrepperTaskManager.GetReports();

        foreach (var report in reports)
        {
            var reportResponse = await apiClient.PostAsync(new CreateCommandCenterReportRequest()
            {
                CommandCenterReport = report
            });
            imagePrepperTaskManager.UpdateReportWithId(report.Name, (int)reportResponse.Id);
        }
    }

}

public static class BlazorDtoHelper
{

    public static ClipFrameEvent ToTransferClipDto((StreamDefinition streamDefinition, InternalFrameEvent frameEvent) a)
    {
        return new ClipFrameEvent()
        {
            EventName = a.frameEvent.EventName,
            FPS = a.frameEvent.FPS,
            FrameNumber = a.frameEvent.FrameNumber,
            Second = a.frameEvent.Second,
            TwitchClipId = a.streamDefinition.DtoId
        };

    }
    public static StreamFrameEvent ToTransferStreamDto((StreamDefinition streamDefinition, InternalFrameEvent frameEvent) a)
    {
        return new StreamFrameEvent()
        {
            EventName = a.frameEvent.EventName,
            FPS = a.frameEvent.FPS,
            FrameNumber = a.frameEvent.FrameNumber,
            Second = a.frameEvent.Second,
            TwitchStreamId = a.streamDefinition.DtoId
        };
    }
}

