using Newtonsoft.Json.Linq;
using Serilog;
using ServiceStack;

namespace ClipHunta2;

public class EventRouterTask : LongTask<(StreamDefinition streamDefinition, InternalFrameEvent[] frameEvents,
    StreamCaptureStatus streamCaptureStatus)>
{
    public static List<(StreamDefinition streamDefinition, InternalFrameEvent frameEvent)> eventsrecv { get; set; } = new();

    public EventRouterTask(CancellationTokenSource cts) : base(cts)
    {

    }

    protected bool IsSameEvent(InternalFrameEvent a, InternalFrameEvent b)
    {
        return IsSameSecond(a, b);
    }
    public static (StreamDefinition streamDefinition, InternalFrameEvent frameEvent)[]? GetEvents()
    {
        Monitor.Enter(eventsrecv);
        try
        {
            var tmp = eventsrecv.ToArray();
            eventsrecv.Clear();
            return tmp;

        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error readingd frame event intoeventsrecv");
            return null;
        }
        finally
        {
            Monitor.Exit(eventsrecv);
        }
    }
    protected void AddEvent((StreamDefinition streamDefinition, InternalFrameEvent[] frameEvents) value)
    {
        Monitor.Enter(eventsrecv);
        try
        {
            foreach (var frame in value.frameEvents)
            {
                
                eventsrecv.Add((value.streamDefinition, frame));
            }

        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, "Error recording frame event intoeventsrecv");
        }
        finally
        {
            Monitor.Exit(eventsrecv);
        }
    }

    protected bool IsSameSecond(InternalFrameEvent a, InternalFrameEvent b)
    {
        return b.EventName == a.EventName && b.Second == a.Second;
    }
    protected string[] blockedByElim = new[] { "elim" };
    protected override async Task<string?> _action(
        (StreamDefinition streamDefinition, InternalFrameEvent[] frameEvents, StreamCaptureStatus streamCaptureStatus) value)
    {


        if (value.frameEvents.Length > 0)
        {
    
            AddEvent((value.streamDefinition, value.frameEvents ));

        }



        switch (value.streamDefinition.StreamCaptureType)
        {
            case StreamCaptureType.Clip:

                break;
            case StreamCaptureType.Stream:

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        value.streamCaptureStatus.IncrementEventsRouted();
        value.streamCaptureStatus.IncrementFinishedCount();
        return null;
    }
}




//
//
// var items = EventRouterTask.eventsrecv.OrderBy(A => A.frameEvent.Second).GroupBy(a => a.frameEvent.EventName).ToDictionary(a => a.Key);
//
//
// foreach ( string eventName in items.Keys)
// {
//     List<int> removeIndex = new();
//     var evented = items[eventName].GroupBy(a => a.frameEvent.Second).Select(a => a.First()).ToList();
//     var removing = false;
//
//     var removeEnd = 0;
//     for (int i = 0; i < evented.Count; i++)
//     {
//         (StreamDefinition streamDefinition, FrameEvent frameEvent) value = evented[i];
//         if (removing)
//         {
//             if(removeEnd < value.frameEvent.Second)
//             {
//                 removing = false;
//             }
//             else
//             {
//                 removeIndex.Add(i);
//             }
//
//         }
//
//         if(value.frameEvent.EventName == "elimed")
//         {
//             removing = true;
//
//             removeEnd = value.frameEvent.Second + 8;
//         }
//     }
//     foreach(var index in removeIndex.OrderByDescending(a => a))
//     {
//         evented.RemoveAt(index);
//     }
//     
//     foreach (var value in evented) {
//
//         Console.WriteLine($"{value.frameEvent.EventName} {value.frameEvent.Second} {value.streamDefinition.StreamerName}");
//     }
//     //Console.WriteLine(valueTuple.frameEvents.Select(a => a.ToString()).ToArray());
// }