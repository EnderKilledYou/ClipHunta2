using ClipHost.ServiceModel;

namespace ClipHunta2
{
    public interface ILongTaskManagerReports
    {
        CommandCenterReport[] GetReports();
        void UpdateReportWithId(string name, int Id);
    }
}