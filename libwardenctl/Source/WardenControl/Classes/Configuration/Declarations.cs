using System.Net;

namespace WardenControl;

public partial class Configuration {
    private static readonly IPAddress[] BaseDefaultIPv4Addresses = [IPAddress.Parse("169.254.0.2")];
    private static readonly IPAddress   BaseDefaultIPv4Gateway   = IPAddress.Parse("169.254.0.1");
    private static readonly IPAddress[] BaseDefaultIPv6Addresses = [IPAddress.Parse("fe80::2")];
    private static readonly IPAddress   BaseDefaultIPv6Gateway   = IPAddress.Parse("fe80::1");

    private const UInt64  BaseDefaultMaximumStorageCapacity = 107374182400UL;
    private const UInt64  BaseDefaultMaximumStorageReadSpeed = 104857600UL;
    private const UInt64  BaseDefaultMaximumStorageWriteSpeed = 104857600UL;
    private const UInt64  BaseDefaultMaximumStorageReadIOPS = 1000UL;
    private const UInt64  BaseDefaultMaximumStorageWriteIOPS = 1000UL;
    private const UInt64  BaseDefaultMaximumNetworkReadSpeed = 2097152UL;
    private const UInt64  BaseDefaultMaximumNetworkWriteSpeed = 2097152UL;
    private const UInt64  BaseDefaultMaximumMemoryCapacity = 8589934592UL;
    private const UInt64  BaseDefaultLogicalCoreCount = 2UL;
    private const Boolean BaseDefaultEnabled = false;
    private const Boolean BaseDefaultLocked = true;

    private IPAddress[] BaseIPv4Addresses;
    private IPAddress   BaseIPv4Gateway;
    private IPAddress[] BaseIPv6Addresses;
    private IPAddress   BaseIPv6Gateway;

    private UInt64  BaseMaximumStorageCapacity;
    private UInt64  BaseMaximumStorageReadSpeed;
    private UInt64  BaseMaximumStorageWriteSpeed;
    private UInt64  BaseMaximumStorageReadIOPS;
    private UInt64  BaseMaximumStorageWriteIOPS;
    private UInt64  BaseMaximumNetworkReadSpeed;
    private UInt64  BaseMaximumNetworkWriteSpeed;
    private UInt64  BaseMaximumMemoryCapacity;
    private UInt64  BaseLogicalCoreCount;
    private Boolean BaseEnabled;
    private Boolean BaseLocked;
}