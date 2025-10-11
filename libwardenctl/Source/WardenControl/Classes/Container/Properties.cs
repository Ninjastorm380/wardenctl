namespace WardenControl;

public partial class Container {
    public UInt64 MemoryCapacity {
        get {
            return BaseAssignedMemoryMaximum;
        }
        set {
            BaseAssignedMemoryMaximum = value;
            if (ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
                ContainerControlHelper.SetMemoryMaximum(BaseUID, value);
            }
            Save();
        }
    }
    
    public UInt64 StorageCapacity {
        get {
            return BaseAssignedStorageMaximum;
        }
        set {
            BaseAssignedStorageMaximum = value;
            if (ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
                ContainerControlHelper.SetStorageMaximum(BaseControlPath, value);
            }
            Save();
        }
    }
    
    public UInt64 NetworkInterfaceUploadSpeed {
        get {
            return BaseAssignedNetworkMaximum.Item2;
        }
        set {
            BaseAssignedNetworkMaximum = (BaseAssignedNetworkMaximum.Item1, value);
            if (ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
                ContainerControlHelper.SetNetworkSpeed(BaseControlPath, BaseInterface, BaseAssignedNetworkMaximum.Item1, value);
            }
            Save();
        }
    }
    
    public UInt64 NetworkInterfaceDownloadSpeed {
        get {
            return BaseAssignedNetworkMaximum.Item1;
        }
        set {
            BaseAssignedNetworkMaximum = (value, BaseAssignedNetworkMaximum.Item2);
            if (ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
                ContainerControlHelper.SetNetworkSpeed(BaseControlPath, BaseInterface, value, BaseAssignedNetworkMaximum.Item2);
            }
            Save();
        }
    }
    
    public String AssignedInterface {
        get {
            return BaseInterface;
        }
        set {
            BaseInterface = value;
            Save();
        }
    }
    
    public Boolean Enabled {
        get {
            return BaseEnabled;
        }
        set {
            BaseEnabled = value;
            Save();
        }
    }
    
    public Boolean Autoboot {
        get {
            return BaseAutoboot;
        }
        set {
            BaseAutoboot = value;
            Save();
        }
    }
    
    public String DisplayName {
        get {
            return BaseDisplayName;
        }
        set {
            BaseDisplayName = value;
            Save();
        }
    }
    
    public String Description {
        get {
            return BaseDescription;
        }
        set {
            BaseDescription = value;
            Save();
        }
    }
}