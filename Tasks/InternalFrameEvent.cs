using ClipHost.ServiceModel;

namespace ClipHunta2;

public class InternalFrameEvent
{
    public static implicit operator StreamFrameEvent(InternalFrameEvent frameEvent)
    {
        return new StreamFrameEvent()
        {
            EventName = frameEvent.EventName,
            FPS = frameEvent.FPS,
            FrameNumber = frameEvent.FrameNumber,
            Second = frameEvent.Second,
            TwitchStreamId = EventRouterTaskManager.GetInstance().TwitchStreamId()

        };
    }
    public static implicit operator ClipFrameEvent(InternalFrameEvent frameEvent)
    {
        return new ClipFrameEvent()
        {
            EventName = frameEvent.EventName,
            FPS = frameEvent.FPS,
            FrameNumber = frameEvent.FrameNumber,
            Second = frameEvent.Second,
            TwitchClipId = EventRouterTaskManager.GetInstance().TwitchStreamId()

        };
    }
    public InternalFrameEvent(string eventName, int frameNumber, int second, int fps)
    {
        FrameNumber = frameNumber;
        EventName = eventName;
        Second = second;
        FPS = fps;
    }

    public int FrameNumber { get; }
    public int FPS { get; }
    public int Second { get; }
    public string EventName { get; }

    public override string ToString()
    {
        return $"{EventName} {FrameNumber} {Second} {FPS}";
    }
}