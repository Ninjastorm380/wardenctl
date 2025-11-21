namespace WardenControl;

public partial class HostConfiguration {
    public UInt64 MaximumMemoryCapacity {
        get {
            return BaseMaximumMemoryCapacity;
        }
        set {
            BaseMaximumMemoryCapacity = value;
        }
    }
    
    public UInt64 LogicalCoreCount {
        get {
            return BaseLogicalCoreCount;
        }
        set {
            BaseLogicalCoreCount = value;
        }
    }

    public UInt64 MaximumNetworkReadSpeed {
        get {
            return BaseMaximumNetworkReadSpeed;
        }
        set {
            BaseMaximumNetworkReadSpeed = value;
        }
    }

    public UInt64 MaximumNetworkWriteSpeed {
        get {
            return BaseMaximumNetworkWriteSpeed;
        }
        set {
            BaseMaximumNetworkWriteSpeed = value;
        }
    }

    public UInt64 MaximumStorageCapacity {
        get {
            return BaseMaximumStorageCapacity;
        }
        set {
            BaseMaximumStorageCapacity = value;
        }
    }

    public UInt64 MaximumStorageReadIOPS {
        get {
            return BaseMaximumStorageReadIOPS;
        }
        set {
            BaseMaximumStorageReadIOPS = value;
        }
    }

    public UInt64 MaximumStorageReadSpeed {
        get {
            return BaseMaximumStorageReadSpeed;
        }
        set {
            BaseMaximumStorageReadSpeed = value;
        }
    }

    public UInt64 MaximumStorageWriteIOPS {
        get {
            return BaseMaximumStorageWriteIOPS;
        }
        set {
            BaseMaximumStorageWriteIOPS = value;
        }
    }

    public UInt64 MaximumStorageWriteSpeed {
        get {
            return BaseMaximumStorageWriteSpeed;
        }
        set {
            BaseMaximumStorageWriteSpeed = value;
        }
    }
    
    public String UID {
        get {
            return BaseUID;
        }
        set {
            BaseUID = value;
        }
    }
}