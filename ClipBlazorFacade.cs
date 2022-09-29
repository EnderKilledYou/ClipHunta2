using BlazorQueue;
using ClipHost.ServiceModel;
using ClipHost.ServiceModel.CreateClipFrameEventModels;
using ClipHost.ServiceModel.CreateFrameEventModels;
using ClipHost.ServiceModel.CreateStreamFrameEventModels;
using ClipHunta2;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using ServiceStack;
using System.ComponentModel;
using System.Diagnostics;
public class ClipBlazorFacade : BlazorInstanceTransmitter //, IProcessClipFacade
{
    private readonly CancellationTokenSource _tokenSource;
    private readonly JsonApiClient apiClient;
    private readonly BackgroundWorker _uploader;

    public ClipBlazorFacade(HubConnectionInfo parentConnectionInfo, JsonApiClient ApiClient, bool isRoot = false) : base(parentConnectionInfo,
        isRoot)
    {
        _tokenSource = new CancellationTokenSource();

        if (Connection == null) return;

        Connection!.On<string, string, int>("Clip", Clip);
        Connection!.On<string, int>("Watch", Watch);
        Connection!.Closed += ConnectionOnClosed;
        apiClient = ApiClient;
        _uploader = new BackgroundWorker();
        _uploader.DoWork += _uploader_DoWork;
        _uploader.RunWorkerAsync();
    }

    private async void _uploader_DoWork(object? sender, DoWorkEventArgs e)
    {

        while (!_tokenSource.IsCancellationRequested)
        {
            await Task.Delay(500);
            var events = EventRouterTask.GetEvents();
            if (events == null) continue;
            if (events.Length == 0) continue;
            var streamEvents = events.Where(a => a.streamDefinition.StreamCaptureType == StreamCaptureType.Stream).ToArray();
            var clipEvents = events.Where(a => a.streamDefinition.StreamCaptureType == StreamCaptureType.Clip).ToArray();
            if (streamEvents.Length > 0)
                while (!uploadEvents(streamEvents))
                {
                    await Task.Delay(2700); // retry every 2.7 second until server comes back -- 
                }
            if (clipEvents.Length > 0)
                while (!uploadEventsclip(clipEvents))
                {
                    await Task.Delay(2700); // retry every 2.7 second until server comes back -- 
                }
        }



    }
    private bool uploadEventsclip((StreamDefinition streamDefinition, InternalFrameEvent frameEvent)[] events)
    {
        try
        {
            apiClient.Post(new CreateClipFrameEventRequest()
            {

                ClipFrameEvent = events.Select(BlazorDtoHelper.ToTransferClipDto).ToArray()
            });
            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Big ole error"); //todo: write better error messages

        }
        return false;
    }
    private bool uploadEvents((StreamDefinition streamDefinition, InternalFrameEvent frameEvent)[] events)
    {
        try
        {
            apiClient.Post(new CreateStreamFrameEventRequest()
            {
                StreamFrameEvent = events.Select(BlazorDtoHelper.ToTransferStreamDto).ToArray()
            });
            return true;
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Big ole error"); //todo: write better error messages

        }
        return false;
    }


    private async Task ConnectionOnClosed(Exception? arg)
    {
        _tokenSource.Cancel();
    }

    public async Task Watch(string streamer, int twitchStreamId)
    {
        try
        {
            await ClipBlazorFacadeHelper.Watch(streamer, twitchStreamId, apiClient);
            await Connection!.SendAsync("StreamFinished", twitchStreamId);
        }//todo: catch connection error and write to log
        catch (Exception ex)
        {
            await Connection!.SendAsync("StreamError", twitchStreamId, ex.Message, ex.StackTrace);

        }

    }

    public async Task Clip(string streamer, string clipId, int twitchClipId)
    {
        try
        {
            await ClipBlazorFacadeHelper.Clip(streamer, twitchClipId, clipId, apiClient);
            await Connection!.SendAsync("ClipFinished", twitchClipId);
        }//todo: catch connection error and write to log
        catch (Exception ex)
        {
            await Connection!.SendAsync("ClipError", twitchClipId, ex.Message, ex.StackTrace);

        }

    }

    public async Task Running()
    {
        try
        {
            await Task.Delay(-1, _tokenSource.Token);
        }
        catch
        {
            // ignored
        }
    }
};