namespace WardenControl;

public readonly partial struct Report {
    public Double[] LogicalCoreUsages {
        get {
            return BaseLogicalCoreUsages;
        }
    }

    public Trait MemoryCapacityUsage {
        get {
            return BaseMemoryCapacityUsage;
        }
    }

    public Trait NetworkReadSpeedUsage {
        get {
            return BaseNetworkReadSpeedUsage;
        }
    }

    public Trait NetworkWriteSpeedUsage {
        get {
            return BaseNetworkWriteSpeedUsage;
        }
    }

    public Trait StorageCapacityUsage {
        get {
            return BaseStorageCapacityUsage;
        }
    }

    public Trait StorageReadSpeedUsage {
        get {
            return BaseStorageReadSpeedUsage;
        }
    }

    public Trait StorageWriteSpeedUsage {
        get {
            return BaseStorageWriteSpeedUsage;
        }
    }

    public Trait StorageReadIOPSUsage {
        get {
            return BaseStorageReadIOPSUsage;
        }
    }

    public Trait StorageWriteIOPSUsage {
        get {
            return BaseStorageWriteIOPSUsage;
        }
    }

    public Boolean Locked {
        get {
            return BaseLocked;
        }
    }

    public Boolean Enabled {
        get {
            return BaseEnabled;
        }
    }

    public Boolean Running {
        get {
            return BaseRunning;
        }
    }
}