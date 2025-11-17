using System.Net;

namespace WardenControl.Containers;

public partial class ResourceConfiguration {
    private Int32 BaseLogicalCoreCount;
    private Int32 BaseMemoryCapacityBytes;
    private Int32 BaseStorageCapacityBytes;
    private Int32 BaseStorageReadSpeedBytes;
    private Int32 BaseStorageWriteSpeedBytes;
    private Int32 BaseNetworkUploadSpeedBytes;
    private Int32 BaseNetworkDownloadSpeedBytes;
    
    private IPAddress[] IPv4Addresses;
    private IPAddress   IPv4Gateway;
    private IPAddress[] IPv6Addresses;
    private IPAddress   IPv6Gateway;
}