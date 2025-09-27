namespace WardenControl;

public partial class NetworkDevice {
    public NetworkDevice(Double Downloaded, Double Uploaded) {
        BaseDownloadedPrevious = Downloaded;
        BaseUploadedPrevious = Uploaded;
        BasePresent = DateTime.UtcNow;
        BasePast = BasePresent;

        BaseDownloadedDelta = 0;
        BaseUploadedDelta = 0;
    }
    
    
    public void Update(Double Downloaded, Double Uploaded) {
        BasePast = BasePresent;
        BasePresent = DateTime.UtcNow;
        TimeSpan Interval = BasePresent.Subtract(BasePast);

        BaseDownloadedDelta = (Downloaded - BaseDownloadedPrevious) * (1000.0 / Interval.TotalMilliseconds);
        BaseUploadedDelta = (Uploaded - BaseUploadedPrevious) * (1000.0 / Interval.TotalMilliseconds);

        BaseDownloadedPrevious = Downloaded;
        BaseUploadedPrevious = Uploaded;
    }
}