using System.Text;
using IniParser.Model;
using IniParser.Parser;
using Lightning.Diagnostics;
using Lightning.Diagnostics.Logging;

namespace WardenControl;

public partial class Container : IDisposable {

    
    public Container(String RootPath, String UID) {
        BaseContainerPath = $"{RootPath}{Path.DirectorySeparatorChar}{BaseUID}";
        BaseStoragePath = $"{RootPath}{Path.DirectorySeparatorChar}{UID}{Path.DirectorySeparatorChar}storage"; 
        BaseControlPath = $"{RootPath}{Path.DirectorySeparatorChar}{UID}{Path.DirectorySeparatorChar}control";
        BaseLogPath     = $"{RootPath}{Path.DirectorySeparatorChar}{UID}{Path.DirectorySeparatorChar}log";
        BaseMountPath   = $"{RootPath}{Path.DirectorySeparatorChar}{UID}{Path.DirectorySeparatorChar}mount";
        BaseConfigPath  = $"{RootPath}{Path.DirectorySeparatorChar}{UID}{Path.DirectorySeparatorChar}container.conf";
        
        BaseDevice = new NetworkDevice(0, 0);
        BaseHostname  = "server";
        BaseDisplayName = "Server";
        BaseDescription = "Server";
        BaseUID       = UID;
        
        BaseAssignedLogicalCPUs = [0, 1];
        BaseAssignedPhysicalCPUs = [];
        BaseAssigned = false;
        BaseAssignedNetworkMaximum = (1875000, 1875000);
        BaseAssignedStorageMaximum = 107374182400;
        BaseAssignedStorageCurrent = 0;
        BaseAssignedMemoryMaximum = 8589934592;
        BaseInterface = "lo";
        BaseEnabled = false;
        BaseAutoboot = false;
        
        BaseV4Addresses = [
            DefaultV4Address
        ];
        BaseV4Gateway = DefaultV4Gateway;
        BaseV6Addresses = [
            DefaultV6Address
        ];
        BaseV6Gateway = DefaultV6Gateway;

        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == false) {
            if (Directory.Exists(RootPath) == false) {
                Directory.CreateDirectory(RootPath);
            }
        
            if (Directory.Exists(BaseStoragePath) == false) {
                Directory.CreateDirectory(BaseStoragePath);
            }
        
            if (Directory.Exists(BaseControlPath) == false) {
                Directory.CreateDirectory(BaseControlPath);
            }
        
            if (Directory.Exists(BaseLogPath) == false) {
                Directory.CreateDirectory(BaseLogPath);
            }
        
            if (Directory.Exists(BaseMountPath) == false) {
                Directory.CreateDirectory(BaseMountPath);
            }
        }
        
        if (File.Exists(BaseConfigPath) == false) {
            Save();
        }
        else {
            Load();
        }
        
        BaseConfigFile = new Surface<String>(BaseConfigPath, String.Empty);
        BaseConfigFile.Update += Refresh;
    }
    
    private void Apply() {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            foreach (String Address in BaseV6Addresses) {
                ContainerControlHelper.BindRoutableV6Address(BaseUID, Address, BaseInterface);
            }
            ContainerControlHelper.BindRoutableV6Gateway(BaseUID, BaseV6Gateway, BaseInterface);
            
            foreach (String Address in BaseV4Addresses) {
                ContainerControlHelper.BindRoutableV4Address(BaseUID, Address, BaseInterface);
            }
            ContainerControlHelper.BindRoutableV4Gateway(BaseUID, BaseV4Gateway, BaseInterface);
            
            ContainerControlHelper.SetMemoryMaximum(BaseUID, BaseAssignedMemoryMaximum);

            Int32[] AssignedCores = CPU.Assign(BaseAssignedLogicalCPUs);
            BaseAssigned = true;
            BaseAssignedPhysicalCPUs = AssignedCores;
            ContainerControlHelper.SetLogicalCores(BaseUID, AssignedCores);
            
            ContainerControlHelper.SetStorageMaximum(BaseControlPath, BaseAssignedStorageMaximum);
            ContainerControlHelper.SetNetworkSpeed(BaseUID, BaseInterface, BaseAssignedNetworkMaximum.Item1, BaseAssignedNetworkMaximum.Item2);
        }
    }

    private void Load() {
        IniDataParser Parser = new IniDataParser();
        IniData Config = Parser.Parse(File.ReadAllText(BaseConfigPath).Trim());

        BaseHostname = Config["Container Configuration"]["Hostname"];
        BaseDisplayName = Config["Container Configuration"]["DisplayName"];
        BaseDescription = Config["Container Configuration"]["Description"];
        
        BaseInterface = Config["Container Configuration"]["Interface"];
        BaseV6Addresses.Clear();
        BaseV6Addresses.AddRange(Config["Container Configuration"]["V6Addresses"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        BaseV6Gateway = Config["Container Configuration"]["V6Gateway"];
        BaseV4Addresses.Clear();
        BaseV4Addresses.AddRange(Config["Container Configuration"]["V4Addresses"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        BaseV4Gateway = Config["Container Configuration"]["V4Gateway"];
        
        BaseEnabled = Boolean.Parse(Config["Container Configuration"]["Enabled"]);
        BaseAutoboot = Boolean.Parse(Config["Container Configuration"]["Autoboot"]);
        BaseAssigned = Boolean.Parse(Config["Container Configuration"]["Assigned"]);
        BaseAssignedMemoryMaximum = UInt64.Parse(Config["Container Configuration"]["MaximumMemoryCapacity"]);
        BaseAssignedStorageMaximum = UInt64.Parse(Config["Container Configuration"]["MaximumStorageCapacity"]);
        BaseAssignedStorageCurrent = UInt64.Parse(Config["Container Configuration"]["CurrentStorageCapacity"]);
        BaseAssignedNetworkMaximum = (UInt64.Parse(Config["Container Configuration"]["MaximumNetworkDownloadSpeed"]), UInt64.Parse(Config["Container Configuration"]["MaximumNetworkUploadSpeed"]));

        List<Int32> Cores = [];
        String[] Parseables = Config["Container Configuration"]["AssignedLogicalCores"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (String Parseable in Parseables) {
            if (Int32.TryParse(Parseable.Trim(), out Int32 Value) == true) {
                Cores.Add(Value);
            }
        }
        
        BaseAssignedLogicalCPUs = Cores.ToArray();
        
        Cores = [];
        Parseables = Config["Container Configuration"]["AssignedPhysicalCores"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (String Parseable in Parseables) {
            if (Int32.TryParse(Parseable.Trim(), out Int32 Value) == true) {
                Cores.Add(Value);
            }
        }
        
        BaseAssignedPhysicalCPUs = Cores.ToArray();
    }   
    private void Refresh() {
        IniDataParser Parser = new IniDataParser();
        IniData Config = Parser.Parse(BaseConfigFile.Value);

        BaseHostname = Config["Container Configuration"]["Hostname"];
        BaseDisplayName = Config["Container Configuration"]["DisplayName"];
        BaseDescription = Config["Container Configuration"]["Description"];
        
        BaseInterface = Config["Container Configuration"]["Interface"];
        BaseV6Addresses.Clear();
        BaseV6Addresses.AddRange(Config["Container Configuration"]["V6Addresses"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        BaseV6Gateway = Config["Container Configuration"]["V6Gateway"];
        BaseV4Addresses.Clear();
        BaseV4Addresses.AddRange(Config["Container Configuration"]["V4Addresses"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        BaseV4Gateway = Config["Container Configuration"]["V4Gateway"];
        
        BaseEnabled = Boolean.Parse(Config["Container Configuration"]["Enabled"]);
        BaseAutoboot = Boolean.Parse(Config["Container Configuration"]["Autoboot"]);
        BaseAssigned = Boolean.Parse(Config["Container Configuration"]["Assigned"]);
        BaseAssignedMemoryMaximum = UInt64.Parse(Config["Container Configuration"]["MaximumMemoryCapacity"]);
        BaseAssignedStorageMaximum = UInt64.Parse(Config["Container Configuration"]["MaximumStorageCapacity"]);
        BaseAssignedStorageCurrent = UInt64.Parse(Config["Container Configuration"]["CurrentStorageCapacity"]);
        BaseAssignedNetworkMaximum = (UInt64.Parse(Config["Container Configuration"]["MaximumNetworkDownloadSpeed"]), UInt64.Parse(Config["Container Configuration"]["MaximumNetworkUploadSpeed"]));

        List<Int32> Cores = [];
        String[] Parseables = Config["Container Configuration"]["AssignedLogicalCores"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (String Parseable in Parseables) {
            if (Int32.TryParse(Parseable.Trim(), out Int32 Value) == true) {
                Cores.Add(Value);
            }
        }
        
        BaseAssignedLogicalCPUs = Cores.ToArray();
        
        Cores = [];
        Parseables = Config["Container Configuration"]["AssignedPhysicalCores"].Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (String Parseable in Parseables) {
            if (Int32.TryParse(Parseable.Trim(), out Int32 Value) == true) {
                Cores.Add(Value);
            }
        }
        
        BaseAssignedPhysicalCPUs = Cores.ToArray();
    }

    private String SerializeAddresses(List<String> Addresses) {
        if (Addresses.Count == 0) {
            return String.Empty;
        }
        
        if (Addresses.Count == 1) {
            return Addresses[0];
        }
        
        StringBuilder ResultBuilder = new StringBuilder();
        ResultBuilder.Append(Addresses[0]);

        for (Int32 Index = 1; Index < Addresses.Count; Index++) {
            ResultBuilder.Append(',').Append(Addresses[Index]);
        }

        return ResultBuilder.ToString();
    }

    private void Save() {
        IniData Config = new IniData();
        Config["Container Configuration"]["Hostname"] = BaseHostname;
        Config["Container Configuration"]["DisplayName"] = BaseDisplayName;
        Config["Container Configuration"]["Description"] = BaseDescription;
        Config["Container Configuration"]["V6Addresses"] = SerializeAddresses(BaseV6Addresses);
        Config["Container Configuration"]["V6Gateway"] = BaseV6Gateway;
        Config["Container Configuration"]["V4Addresses"] = SerializeAddresses(BaseV4Addresses);
        Config["Container Configuration"]["V4Gateway"] = BaseV4Gateway;
        Config["Container Configuration"]["Interface"] = BaseInterface;
        Config["Container Configuration"]["Enabled"] = BaseEnabled.ToString();
        Config["Container Configuration"]["Autoboot"] = BaseAutoboot.ToString();
        Config["Container Configuration"]["Assigned"] = BaseAssigned.ToString();
        Config["Container Configuration"]["MaximumMemoryCapacity"] = BaseAssignedMemoryMaximum.ToString();
        Config["Container Configuration"]["MaximumStorageCapacity"] = BaseAssignedStorageMaximum.ToString();
        Config["Container Configuration"]["CurrentStorageCapacity"] = BaseAssignedStorageCurrent.ToString();
        Config["Container Configuration"]["MaximumNetworkDownloadSpeed"] = BaseAssignedNetworkMaximum.Item1.ToString();
        Config["Container Configuration"]["MaximumNetworkUploadSpeed"] = BaseAssignedNetworkMaximum.Item2.ToString();
        Config["Container Configuration"]["AssignedLogicalCores"] = Parsing.GetString(BaseAssignedLogicalCPUs);
        Config["Container Configuration"]["AssignedPhysicalCores"] = Parsing.GetString(BaseAssignedPhysicalCPUs);

        File.WriteAllText(BaseConfigPath, Config.ToString());
    }

    public void Remove() {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == false && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == false) {
            if (Directory.Exists(BaseContainerPath) == true) {
                Directory.Delete(BaseContainerPath, true);
            }
        }
    }
    
    public (Boolean, Boolean, Boolean, Boolean) Status() {
        return (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath), ContainerControlHelper.Running(BaseUID, BaseContainerPath), BaseEnabled, BaseAutoboot);
    }
    
    public void InitializeAndInstall(String Packages) {
        Log.PrintAsync<Container>($"Attempting to initialize and install packages for container '{BaseUID}'.");
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == false && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == false) {
            Log.PrintAsync<Container>("Mounting container filesystem for package install.");
            ContainerControlHelper.Mount(BaseStoragePath, BaseLogPath, BaseControlPath, BaseMountPath, BaseAssignedStorageMaximum);
            Log.PrintAsync<Container>("Installing packages into container.");
            ContainerControlHelper.Populate(BaseMountPath, Packages);
            Log.PrintAsync<Container>("Unmounting container filesystem.");
            ContainerControlHelper.Unmount(BaseControlPath, BaseMountPath);
        }
        Log.PrintAsync<Container>($"Initialize and install complete for container '{BaseUID}'.");
    }
    
    public void Shell(String Username) {
        if (BaseEnabled) {
            ContainerControlHelper.Shell(BaseUID, Username);
        }
    }
    public Boolean Startup() {
        if (BaseEnabled) {
            Log.PrintAsync<Container>($"Booting container '{BaseUID}'.");
            if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == false && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == false) {
                Log.PrintAsync<Container>("Mounting container filesystem for boot.");
                ContainerControlHelper.Mount(BaseStoragePath, BaseLogPath, BaseControlPath, BaseMountPath, BaseAssignedStorageMaximum);
                Log.PrintAsync<Container>("Booting container to network target.");
                ContainerControlHelper.Start(BaseUID, BaseHostname, BaseInterface, BaseMountPath);
                ContainerControlHelper.WaitForOnline(BaseUID, BaseContainerPath);
                Log.PrintAsync<Container>("Enabling externally controlled mvlan interface within container.");
                ContainerControlHelper.EnableInterface(BaseUID, BaseInterface);
                Log.PrintAsync<Container>("Applying container settings and configuration.");
                Apply();
                Log.PrintAsync<Container>("Resuming container boot to multi-user target.");
                ContainerControlHelper.ContinueStart(BaseUID);
                Log.PrintAsync<Container>("Saving runtime state to disk.");
                Save();
                Log.PrintAsync<Container>($"Container '{BaseUID}' has successfully boot.");
                return true;
            }
        }

        return false;
    }
    public void Shutdown() {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            (Int32, (UInt64, UInt64)) StorageStats = ContainerControlHelper.GetStorageUsage(BaseControlPath);
            BaseAssignedStorageCurrent = StorageStats.Item2.Item2;
            ContainerControlHelper.DisableInterface(BaseUID, BaseInterface);
            ContainerControlHelper.Stop(BaseUID);
            ContainerControlHelper.WaitForNotRunning(BaseUID, BaseContainerPath);
            ContainerControlHelper.Unmount(BaseControlPath, BaseMountPath);
            
            CPU.Unassign(BaseAssignedPhysicalCPUs);
            BaseAssigned = false;
            BaseAssignedPhysicalCPUs = [];
        }
        Save();
    }
    public void Restart() {
        if (BaseEnabled) {
            if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
                ContainerControlHelper.DisableInterface(BaseUID, BaseInterface);
                ContainerControlHelper.Stop(BaseUID);
                ContainerControlHelper.WaitForNotRunning(BaseUID, BaseContainerPath);
                ContainerControlHelper.Start(BaseUID, BaseHostname, BaseInterface, BaseMountPath);
                ContainerControlHelper.WaitForOnline(BaseUID, BaseContainerPath);
                ContainerControlHelper.EnableInterface(BaseUID, BaseInterface);
                Apply();
                ContainerControlHelper.ContinueStart(BaseUID);
            }
        }
        Save();
    }
    public void AutomaticStartup() {
        if (BaseAutoboot) {
             Startup();
        }
    }

    public void GetCPUs(out Int32[] CPUs) {
        CPUs = BaseAssignedLogicalCPUs;
    }
    
    public void SetCPUs(Int32[] CPUs) {
        BaseAssignedLogicalCPUs = CPUs;

        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            Int32[] AssignedCores = CPU.Reassign(BaseAssignedPhysicalCPUs, BaseAssignedLogicalCPUs);
            BaseAssigned = true;
            BaseAssignedPhysicalCPUs = AssignedCores;
            ContainerControlHelper.SetLogicalCores(BaseUID, AssignedCores);
        }
        Save();
    }
    
    public void AssignV6Address(String Address, String Gateway) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            foreach (String V6Address in BaseV6Addresses.ToArray()) {
                ContainerControlHelper.RemoveRoutableV6Address(BaseUID, V6Address, BaseInterface);
            }
        }
        
        foreach (String V6Address in BaseV6Addresses.ToArray()) {
            BaseV6Addresses.Remove(V6Address);
        }
        
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV6Address(BaseUID, Address, BaseInterface);
            ContainerControlHelper.BindRoutableV6Gateway(BaseUID, Gateway, BaseInterface);
        }

        BaseV6Addresses.Add(Address);
        BaseV6Gateway = Gateway;
        Save();
    }
    
    public void AssignV4Address(String Address, String Gateway) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            foreach (String V4Address in BaseV4Addresses.ToArray()) {
                ContainerControlHelper.RemoveRoutableV4Address(BaseUID, V4Address, BaseInterface);
            }
        }
        
        foreach (String V4Address in BaseV4Addresses.ToArray()) {
            BaseV4Addresses.Remove(V4Address);
        }
        
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV4Address(BaseUID, Address, BaseInterface);
            ContainerControlHelper.BindRoutableV4Gateway(BaseUID, Gateway, BaseInterface);
        }

        BaseV4Addresses.Add(Address);
        BaseV4Gateway = Gateway;
        Save();
    }
    
    public void AddV6Address(String Address) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV6Address(BaseUID, Address, BaseInterface);
        }
        BaseV6Addresses.Add(Address);
        Save();
    }
    public void AddV4Address(String Address) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV4Address(BaseUID, Address, BaseInterface);
        }
        BaseV4Addresses.Add(Address);
        Save();
    }
    public void RemoveV6Address(String Address) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.RemoveRoutableV6Address(BaseUID, Address, BaseInterface);
        }
        BaseV6Addresses.Remove(Address);
        Save();
    }
    public void RemoveV4Address(String Address) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.RemoveRoutableV4Address(BaseUID, Address, BaseInterface);
        }
        BaseV4Addresses.Remove(Address);
        Save();
    }
    public void RemoveDefaultV6Address() {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.RemoveRoutableV6Address(BaseUID, DefaultV6Address, BaseInterface);
        }
        BaseV6Addresses.Remove(DefaultV6Address);
        Save();
    }
    public void RemoveDefaultV4Address() {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.RemoveRoutableV4Address(BaseUID, DefaultV4Address, BaseInterface);
        }
        BaseV4Addresses.Remove(DefaultV6Address);
        Save();
    }
    
    public Boolean ContainsV6Address(String Address) {
        return BaseV6Addresses.Contains(Address);
    }
    public Boolean ContainsV4Address(String Address) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.RemoveRoutableV4Address(BaseUID, Address, BaseV4Gateway, BaseInterface);
        }
        return BaseV4Addresses.Contains(Address);
    }
    public void GetV6Addresses(out String[] Addresses) {
        Addresses = BaseV6Addresses.ToArray();
    }
    public void GetV4Addresses(out String[] Addresses) {
        Addresses = BaseV4Addresses.ToArray();
    }
    public void ResetV6Addresses() {
        foreach (String Address in BaseV6Addresses.ToArray()) {
            RemoveV6Address(Address);
        }
        
        AddV6Address(DefaultV6Address);
        
        Save();
    }
    public void ResetV4Addresses() {
        foreach (String Address in BaseV4Addresses.ToArray()) {
            RemoveV4Address(Address);
        }
        
        AddV4Address(DefaultV4Address);
        
        Save();
    }
    
    public void SetV6Gateway(String Gateway) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV6Gateway(BaseUID, Gateway, BaseInterface);
        }
        BaseV6Gateway = Gateway;
        Save();
    }
    public void SetV4Gateway(String Gateway) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            ContainerControlHelper.BindRoutableV4Gateway(BaseUID, Gateway, BaseInterface);
        }
        BaseV4Gateway = Gateway;
        Save();
    }
    public void GetV6Gateway(out String Gateway) {
        Gateway = BaseV6Gateway;
    }
    public void GetV4Gateway(out String Gateway) {
        Gateway = BaseV4Gateway;
    }
    public void ResetV6Gateway() {
        SetV6Gateway(DefaultV6Gateway);
        
        Save();
    }
    public void ResetV4Gateway() {
        SetV4Gateway(DefaultV4Gateway);
        
        Save();
    }

    public void GetUsage(out Double NetworkInterfaceDownloadUsage, out Double NetworkInterfaceDownloadMaximum, out Double NetworkInterfaceUploadUsage, out Double NetworkInterfaceUploadMaximum, out Double StorageCapacityUsage, out Double StorageCapacityMaximum, out Double MemoryCapacityUsage, out Double MemoryCapacityMaximum, out Double[] CoreUsages) {
        if (ContainerControlHelper.Mounted(BaseControlPath, BaseMountPath) == true && ContainerControlHelper.Running(BaseUID, BaseContainerPath) == true) {
            (Int32, (UInt64, UInt64)) NetworkInterfaceResults = ContainerControlHelper.GetNetworkDownloadUpload(BaseUID, BaseInterface);
            (Int32, (UInt64, UInt64)) StorageUsageResults = ContainerControlHelper.GetStorageUsage(BaseControlPath);
            (Int32, (UInt64, UInt64)) MemoryUsageResults = ContainerControlHelper.GetMemoryUsage(BaseUID);
            (Int32, Double[]) CPUUsageResults = ContainerControlHelper.GetCPUCoreUsages(BaseUID, BaseAssignedPhysicalCPUs);

            BaseDevice.Update((Double)NetworkInterfaceResults.Item2.Item1, (Double)NetworkInterfaceResults.Item2.Item2);

            CoreUsages = CPUUsageResults.Item2;
            NetworkInterfaceDownloadUsage = BaseDevice.Download / BaseAssignedNetworkMaximum.Item1;
            NetworkInterfaceDownloadMaximum = BaseAssignedNetworkMaximum.Item1;
            NetworkInterfaceUploadUsage = BaseDevice.Upload / BaseAssignedNetworkMaximum.Item2;
            NetworkInterfaceUploadMaximum = BaseAssignedNetworkMaximum.Item2;
            StorageCapacityUsage = (Double)StorageUsageResults.Item2.Item2 / BaseAssignedStorageMaximum;
            StorageCapacityMaximum = BaseAssignedStorageMaximum;
            MemoryCapacityUsage = (Double)MemoryUsageResults.Item2.Item2 / (Double)BaseAssignedMemoryMaximum;
            MemoryCapacityMaximum = BaseAssignedMemoryMaximum;

            return;
        }

        NetworkInterfaceDownloadUsage = 0.0;
        NetworkInterfaceDownloadMaximum = BaseAssignedNetworkMaximum.Item1;
        NetworkInterfaceUploadUsage = 0.0;
        NetworkInterfaceUploadMaximum = BaseAssignedNetworkMaximum.Item2;
        StorageCapacityUsage = (Double)BaseAssignedStorageCurrent / (Double)BaseAssignedStorageMaximum;
        StorageCapacityMaximum = BaseAssignedStorageMaximum;
        MemoryCapacityUsage = 0.0;
        MemoryCapacityMaximum = BaseAssignedMemoryMaximum;

        Double[] Cores = new Double[BaseAssignedLogicalCPUs.Length];
        for (Int32 Index = 0; Index < BaseAssignedLogicalCPUs.Length; Index++) {
            Cores[Index] = 0.0;
        }
        
        CoreUsages = Cores;
    }

    public void Dispose() {
        BaseConfigFile.Dispose();
    }
}