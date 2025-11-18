namespace WardenControl;

public readonly partial struct Report {
    public Report(Configuration Configuration, Boolean Running, Double[] LogicalCoreUsages, UInt64 CurrentMemoryCapacity, UInt64 CurrentNetworkReadSpeed, UInt64 CurrentNetworkWriteSpeed, UInt64 CurrentStorageCapacity, UInt64 CurrentStorageReadSpeed, UInt64 CurrentStorageWriteSpeed, UInt64 CurrentStorageReadIOPS, UInt64 CurrentStorageWriteIOPS) {
        BaseLogicalCoreUsages = LogicalCoreUsages;
        
        BaseMemoryCapacityUsage    = new Trait(CurrentMemoryCapacity,    Configuration.MaximumMemoryCapacity);
        BaseNetworkReadSpeedUsage  = new Trait(CurrentNetworkReadSpeed,  Configuration.MaximumNetworkReadSpeed);
        BaseNetworkWriteSpeedUsage = new Trait(CurrentNetworkWriteSpeed, Configuration.MaximumNetworkWriteSpeed);
        BaseStorageCapacityUsage   = new Trait(CurrentStorageCapacity,   Configuration.MaximumStorageCapacity);
        BaseStorageReadSpeedUsage  = new Trait(CurrentStorageReadSpeed,  Configuration.MaximumStorageReadSpeed);
        BaseStorageWriteSpeedUsage = new Trait(CurrentStorageWriteSpeed, Configuration.MaximumStorageWriteSpeed);
        BaseStorageReadIOPSUsage   = new Trait(CurrentStorageReadIOPS,   Configuration.MaximumStorageReadIOPS);
        BaseStorageWriteIOPSUsage  = new Trait(CurrentStorageWriteIOPS,  Configuration.MaximumStorageWriteIOPS);

        BaseLocked  = Configuration.Locked;
        BaseEnabled = Configuration.Enabled;
        BaseRunning = Running;
    }
}