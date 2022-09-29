/* Options:
Date: 2022-09-28 12:28:20
Version: 6.21
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

//GlobalNamespace: 
//MakePartial: True
//MakeVirtual: True
//MakeInternal: False
//MakeDataContractsExtensible: False
//AddReturnMarker: True
//AddDescriptionAsComments: True
//AddDataContractAttributes: False
//AddIndexesToDataMembers: False
//AddGeneratedCodeAttributes: False
//AddResponseStatus: False
//AddImplicitVersion: 
//InitializeCollections: True
//ExportValueTypes: False
//IncludeTypes: 
//ExcludeTypes: 
//AddNamespaces: 
//AddDefaultXmlNamespace: http://schemas.servicestack.net/types
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ClipHost.ServiceModel;
using ClipHost;
using BlazorQueue;
using ClipHost.ServiceModel.ListDtoProgramInstanceModels;
using ClipHost.ServiceModel.ListStreamFrameEventModels;
using ClipHost.ServiceModel.CreateStreamFrameEventModels;
using ClipHost.ServiceModel.ListStreamerModels;
using ClipHost.ServiceModel.CreateStreamerModels;
using ClipHost.ServiceModel.ListStreamerCommandCenterModels;
using ClipHost.ServiceModel.CreateStreamerCommandCenterModels;
using ClipHost.ServiceModel.ListQueueReportModels;
using ClipHost.ServiceModel.ListProcessReportModels;
using ClipHost.ServiceModel.CreateProcessReportModels;
using ClipHost.ServiceModel.ListFrameEventModels;
using ClipHost.ServiceModel.CreateFrameEventModels;
using ClipHost.ServiceModel.ListCommandCenterModels;
using ClipHost.ServiceModel.CreateCommandCenterModels;
using ClipHost.ServiceModel.ListCommandCenterReportModels;
using ClipHost.ServiceModel.CreateCommandCenterReportModels;
using ClipHost.ServiceModel.ListClipFrameEventModels;
using ClipHost.ServiceModel.CreateClipFrameEventModels;

namespace BlazorQueue
{
    public partial interface IHaveBlazorConnection
    {
    }

    public partial interface IProgramInstance
    {
    }

    public partial interface IQueueReport
    {
        int AverageMilliSeconds { get; set; }
        int HighMilliSeconds { get; set; }
        int Id { get; set; }
        int LowMilliSeconds { get; set; }
        int MaxSize { get; set; }
        string Name { get; set; }
        int ProcessId { get; set; }
        int Size { get; set; }
    }

    public partial interface IReportInstance
    {
        QueueReport[] ReportsArray { get; set; }
    }

    public partial class ProgramInstance
        : IHaveBlazorConnection, IProgramInstance, IReportInstance
    {
        public ProgramInstance()
        {
            ReportsArray = new QueueReport[]{};
        }

        public virtual QueueReport[] ReportsArray { get; set; }
    }

    public partial class QueueReport
        : IQueueReport
    {
        public virtual int Id { get; set; }
        public virtual int Size { get; set; }
        public virtual int MaxSize { get; set; }
        public virtual int AverageMilliSeconds { get; set; }
        public virtual int HighMilliSeconds { get; set; }
        public virtual int LowMilliSeconds { get; set; }
        public virtual string Name { get; set; }
        public virtual int ProcessId { get; set; }
    }

}

namespace ClipHost
{
    public partial class DtoProgramInstance
        : ProgramInstance, IDtoProgramInstance
    {
        public DtoProgramInstance()
        {
            ReportsArray = new QueueReport[]{};
        }

        public virtual int? DtoId { get; set; }
        public virtual QueueReport[] ReportsArray { get; set; }
        public virtual int DatabaseId { get; set; }
    }

    public partial interface IDtoProgramInstance
    {
        int? DtoId { get; set; }
    }

}

namespace ClipHost.ServiceModel
{
    public partial class ClipFrameEvent
        : TablesUp
    {
        [References(typeof(ClipHost.ServiceModel.TwitchClip))]
        public virtual int TwitchClipId { get; set; }

        public virtual int FrameNumber { get; set; }
        public virtual int FPS { get; set; }
        public virtual int Second { get; set; }
        public virtual string EventName { get; set; }
    }

    public partial class CommandCenter
        : TablesUp
    {
        public virtual string Name { get; set; }
        public virtual int StreamerCount { get; set; }
        public virtual int MaxStreamers { get; set; }
    }

    public partial class CommandCenterReport
        : TablesUp, IQueueReport
    {
        public virtual string Name { get; set; }
        public virtual int TotalProcessed { get; set; }
        public virtual int AverageMilliSeconds { get; set; }
        public virtual int HighMilliSeconds { get; set; }
        public virtual int LowMilliSeconds { get; set; }
        public virtual int MaxSize { get; set; }
        public virtual int _processId { get; set; }
        public virtual int ProcessId { get; set; }
        public virtual int Size { get; set; }
        [Required]
        [References(typeof(ClipHost.ServiceModel.StreamerCommandCenter))]
        public virtual int StreamerCommandCenterId { get; set; }
    }

    [Route("/hello")]
    [Route("/hello/{Name}")]
    public partial class Hello
        : IReturn<HelloResponse>
    {
        public virtual string Name { get; set; }
    }

    public partial class HelloResponse
    {
        public virtual string Result { get; set; }
    }

    [Route("/test")]
    [Route("/test/{Name}")]
    public partial class HelloTest
        : IReturn<HelloTestResponse>
    {
        public virtual string Name { get; set; }
    }

    public partial class HelloTestResponse
    {
        public virtual string Result { get; set; }
    }

    public partial interface ITableUp
    {
    }

    public partial class ProcessReport
        : TablesUp
    {
        public virtual bool IsRunning { get; set; }
        public virtual int ExitCode { get; set; }
        public virtual string ReportText { get; set; }
        public virtual int ProcessId { get; set; }
        [Required]
        [References(typeof(ClipHost.ServiceModel.StreamerCommandCenter))]
        public virtual int StreamerCommandCenterId { get; set; }
    }

    public partial class Streamer
        : TablesUp
    {
        [Required]
        public virtual string Name { get; set; }

        public virtual bool Enabled { get; set; }
    }

    public partial class StreamerCommandCenter
        : TablesUp
    {
        [Required]
        [References(typeof(ClipHost.ServiceModel.Streamer))]
        public virtual int StreamerId { get; set; }

        [Required]
        [References(typeof(ClipHost.ServiceModel.CommandCenter))]
        public virtual int CommandCenterId { get; set; }
    }

    public partial class StreamFrameEvent
        : TablesUp
    {
        [References(typeof(ClipHost.ServiceModel.TwitchStream))]
        public virtual int TwitchStreamId { get; set; }

        public virtual int FrameNumber { get; set; }
        public virtual int FPS { get; set; }
        public virtual int Second { get; set; }
        public virtual string EventName { get; set; }
    }

    public partial class TablesUp
        : ITableUp
    {
        public virtual int Id { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateClipFrameEventModels
{
    public partial class CreateClipFrameEventRequest
        : IReturn<CreateClipFrameEventResponse>
    {
        public CreateClipFrameEventRequest()
        {
            ClipFrameEvent = new ClipFrameEvent[]{};
        }

        public virtual ClipFrameEvent[] ClipFrameEvent { get; set; }
    }

    public partial class CreateClipFrameEventResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual ResponseStatus ResponseStatus { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateCommandCenterModels
{
    public partial class CreateCommandCenterRequest
        : IReturn<CreateCommandCenterResponse>
    {
        public virtual CommandCenter CommandCenter { get; set; }
    }

    public partial class CreateCommandCenterResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateCommandCenterReportModels
{
    public partial class CreateCommandCenterReportRequest
        : IReturn<CreateCommandCenterReportResponse>
    {
        public virtual CommandCenterReport CommandCenterReport { get; set; }
    }

    public partial class CreateCommandCenterReportResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual ResponseStatus ResponseStatus { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateFrameEventModels
{
    public partial class CreateFrameEventRequest
        : IReturn<CreateFrameEventResponse>
    {
        public CreateFrameEventRequest()
        {
            FrameEvent = new StreamFrameEvent[]{};
        }

        public virtual StreamFrameEvent[] FrameEvent { get; set; }
    }

    public partial class CreateFrameEventResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual ResponseStatus ResponseStatus { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateProcessReportModels
{
    public partial class CreateProcessReportRequest
        : IReturn<CreateProcessReportResponse>
    {
        public virtual ProcessReport ProcessReport { get; set; }
    }

    public partial class CreateProcessReportResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual ResponseStatus ResponseStatus { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateStreamerCommandCenterModels
{
    public partial class CreateStreamerCommandCenterRequest
        : IReturn<CreateStreamerCommandCenterResponse>
    {
        public virtual StreamerCommandCenter StreamerCommandCenter { get; set; }
    }

    public partial class CreateStreamerCommandCenterResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateStreamerModels
{
    public partial class CreateStreamerRequest
        : IReturn<CreateStreamerResponse>
    {
        public virtual Streamer Streamer { get; set; }
    }

    public partial class CreateStreamerResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
    }

}

namespace ClipHost.ServiceModel.CreateStreamFrameEventModels
{
    public partial class CreateStreamFrameEventRequest
        : IReturn<CreateStreamFrameEventResponse>
    {
        public CreateStreamFrameEventRequest()
        {
            StreamFrameEvent = new StreamFrameEvent[]{};
        }

        public virtual StreamFrameEvent[] StreamFrameEvent { get; set; }
    }

    public partial class CreateStreamFrameEventResponse
    {
        public virtual long Id { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual ResponseStatus ResponseStatus { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListClipFrameEventModels
{
    public partial class ListClipFrameEventRequest
        : IReturn<ListClipFrameEventResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListClipFrameEventResponse
    {
        public ListClipFrameEventResponse()
        {
            ClipFrameEvents = new List<ClipFrameEvent>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<ClipFrameEvent> ClipFrameEvents { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListCommandCenterModels
{
    public partial class ListCommandCenterRequest
        : IReturn<ListCommandCenterResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListCommandCenterResponse
    {
        public ListCommandCenterResponse()
        {
            CommandCenters = new List<CommandCenter>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<CommandCenter> CommandCenters { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListCommandCenterReportModels
{
    public partial class ListCommandCenterReportRequest
        : IReturn<ListCommandCenterReportResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListCommandCenterReportResponse
    {
        public ListCommandCenterReportResponse()
        {
            CommandCenterReports = new List<CommandCenterReport>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<CommandCenterReport> CommandCenterReports { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListDtoProgramInstanceModels
{
    public partial class ListDtoProgramInstanceRequest
        : IReturn<ListDtoProgramInstanceResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListDtoProgramInstanceResponse
    {
        public ListDtoProgramInstanceResponse()
        {
            DtoProgramInstances = new DtoProgramInstance[]{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual DtoProgramInstance[] DtoProgramInstances { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListFrameEventModels
{
    public partial class ListFrameEventRequest
        : IReturn<ListFrameEventResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListFrameEventResponse
    {
        public ListFrameEventResponse()
        {
            FrameEvents = new List<StreamFrameEvent>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<StreamFrameEvent> FrameEvents { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListProcessReportModels
{
    public partial class ListProcessReportRequest
        : IReturn<ListProcessReportResponse>
    {
        public virtual int After { get; set; }
        public virtual bool? IsRunning { get; set; }
        public virtual int ProcessId { get; set; }
    }

    public partial class ListProcessReportResponse
    {
        public ListProcessReportResponse()
        {
            ProcessReports = new List<Tuple<StreamerCommandCenter,ProcessReport,Streamer,CommandCenter>>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<Tuple<StreamerCommandCenter,ProcessReport,Streamer,CommandCenter>> ProcessReports { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListQueueReportModels
{
    public partial class ListQueueReportRequest
        : IReturn<ListQueueReportResponse>
    {
        public virtual int After { get; set; }
        public virtual string Name { get; set; }
    }

    public partial class ListQueueReportResponse
    {
        public ListQueueReportResponse()
        {
            QueueReports = new List<QueueReport>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<QueueReport> QueueReports { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListStreamerCommandCenterModels
{
    public partial class ListStreamerCommandCenterRequest
        : IReturn<ListStreamerCommandCenterResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListStreamerCommandCenterResponse
    {
        public ListStreamerCommandCenterResponse()
        {
            StreamerCommandCenters = new List<Tuple<StreamerCommandCenter,Streamer,CommandCenter>>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<Tuple<StreamerCommandCenter,Streamer,CommandCenter>> StreamerCommandCenters { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListStreamerModels
{
    public partial class DeleteStreamerRequest
        : IReturn<DeleteStreamerResponse>
    {
        public virtual int Id { get; set; }
    }

    public partial class DeleteStreamerResponse
    {
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual Streamer DeletedStreamer { get; set; }
    }

    public partial class ListStreamerRequest
        : IReturn<ListStreamerResponse>
    {
        public virtual int After { get; set; }
        public virtual string Name { get; set; }
    }

    public partial class ListStreamerResponse
    {
        public ListStreamerResponse()
        {
            Streamers = new List<Streamer>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<Streamer> Streamers { get; set; }
    }

}

namespace ClipHost.ServiceModel.ListStreamFrameEventModels
{
    public partial class ListStreamFrameEventRequest
        : IReturn<ListStreamFrameEventResponse>
    {
        public virtual int After { get; set; }
    }

    public partial class ListStreamFrameEventResponse
    {
        public ListStreamFrameEventResponse()
        {
            StreamFrameEvents = new List<StreamFrameEvent>{};
        }

        public virtual long Count { get; set; }
        public virtual string Message { get; set; }
        public virtual bool Success { get; set; }
        public virtual List<StreamFrameEvent> StreamFrameEvents { get; set; }
    }

}

