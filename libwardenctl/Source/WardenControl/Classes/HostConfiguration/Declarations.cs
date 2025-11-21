using System.Net;

namespace WardenControl;

public partial class HostConfiguration {
    private const UInt64  BaseDefaultMaximumStorageCapacity = 107374182400UL;
    private const UInt64  BaseDefaultMaximumStorageReadSpeed = 104857600UL;
    private const UInt64  BaseDefaultMaximumStorageWriteSpeed = 104857600UL;
    private const UInt64  BaseDefaultMaximumStorageReadIOPS = 1000UL;
    private const UInt64  BaseDefaultMaximumStorageWriteIOPS = 1000UL;
    private const UInt64  BaseDefaultMaximumNetworkReadSpeed = 2097152UL;
    private const UInt64  BaseDefaultMaximumNetworkWriteSpeed = 2097152UL;
    private const UInt64  BaseDefaultMaximumMemoryCapacity = 8589934592UL;
    private const UInt64  BaseDefaultLogicalCoreCount = 2UL;

    private String BaseUID;

    private UInt64  BaseMaximumStorageCapacity;
    private UInt64  BaseMaximumStorageReadSpeed;
    private UInt64  BaseMaximumStorageWriteSpeed;
    private UInt64  BaseMaximumStorageReadIOPS;
    private UInt64  BaseMaximumStorageWriteIOPS;
    private UInt64  BaseMaximumNetworkReadSpeed;
    private UInt64  BaseMaximumNetworkWriteSpeed;
    private UInt64  BaseMaximumMemoryCapacity;
    private UInt64  BaseLogicalCoreCount;
}