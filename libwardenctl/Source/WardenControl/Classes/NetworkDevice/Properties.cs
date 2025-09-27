namespace WardenControl;

public partial class NetworkDevice {
    public Double Download {
        get {
            return BaseDownloadedDelta;
        }
    }
    
    public Double Upload {
        get {
            return BaseUploadedDelta;
        }
    }
}