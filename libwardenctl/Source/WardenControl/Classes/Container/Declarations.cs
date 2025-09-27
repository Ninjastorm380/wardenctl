using IniParser.Model;

namespace WardenControl;

public partial class Container {
    private readonly Surface<String> BaseConfigFile;

    private readonly NetworkDevice BaseDevice;
    private readonly String BaseUID;
    private readonly String BaseContainerPath;
    private readonly String BaseMountPath;
    private readonly String BaseControlPath;
    private readonly String BaseLogPath;
    private readonly String BaseStoragePath;
    private readonly String BaseConfigPath;

    private Boolean BaseAssigned;
    private Int32[] BaseAssignedPhysicalCPUs;
    private String BaseHostname;
    private String BaseDisplayName;
    private String BaseDescription;
    private Int32[] BaseAssignedLogicalCPUs;
    private UInt64 BaseAssignedMemoryMaximum;
    private UInt64 BaseAssignedStorageMaximum;
    private UInt64 BaseAssignedStorageCurrent;
    private (UInt64, UInt64) BaseAssignedNetworkMaximum;

    private Boolean BaseEnabled;
    private Boolean BaseAutoboot;
    
    private readonly List<String> BaseV4Addresses;
    private String BaseV4Gateway;
    
    private readonly List<String> BaseV6Addresses;
    private String BaseV6Gateway;

    private String BaseInterface;
    
    private const String DefaultV4Address = "169.254.0.2/32";
    private const String DefaultV4Gateway = "169.254.0.1";
    
    private const String DefaultV6Address = "fe80::2/128";
    private const String DefaultV6Gateway = "fe80::1";
}