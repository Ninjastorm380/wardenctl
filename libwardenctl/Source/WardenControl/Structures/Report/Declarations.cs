namespace WardenControl;

public readonly partial struct Report {
    // "systemd-nspawn", $"-qbPD {MountPath} -M {UID} --hostname {Hostname} --network-macvlan={Interface} --setenv=SYSTEMD_JOURNAL_STORAGE=volatile --bind=/dev/null:/etc/systemd/system/systemd-journal-flush.service"
    private readonly Double[] BaseLogicalCoreUsages;
    
    private readonly Trait BaseMemoryCapacityUsage;
    
    private readonly Trait BaseNetworkReadSpeedUsage;
    private readonly Trait BaseNetworkWriteSpeedUsage;
    
    private readonly Trait BaseStorageCapacityUsage;
    private readonly Trait BaseStorageReadSpeedUsage;
    private readonly Trait BaseStorageWriteSpeedUsage;
    private readonly Trait BaseStorageReadIOPSUsage;
    private readonly Trait BaseStorageWriteIOPSUsage;

    private readonly Boolean BaseEnabled;
    private readonly Boolean BaseLocked;
    private readonly Boolean BaseRunning;
}