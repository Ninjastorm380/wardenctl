namespace WardenControl;

public static partial class Manager {
    static Manager() {
        BaseContainers = new Dictionary<String, Container>();
        BaseContainerRootPath = String.Empty;
        BaseStatusRootPath = String.Empty;
    }

    public static void Init(String ContainerRootPath, String StatusRootPath) {
        BaseContainerRootPath = Path.GetFullPath(ContainerRootPath, "/");
        BaseStatusRootPath = Path.GetFullPath(StatusRootPath, "/");
        
        if (Directory.Exists(BaseContainerRootPath) == false) {
            Directory.CreateDirectory(BaseContainerRootPath);
        }
        
        if (Directory.Exists(BaseStatusRootPath) == false) {
            Directory.CreateDirectory(BaseStatusRootPath);
        }
        
        Boolean Reset = true;
        foreach (String UID in ContainerControlHelper.ListContainers(BaseContainerRootPath)) {
            Container Container = new Container(BaseContainerRootPath, UID);
            BaseContainers.Add(UID, Container);

            (Boolean Mounted, Boolean Running, Boolean Enabled, Boolean Autoboot) = Container.Status();

            if (Mounted == true | Running == true) {
                Reset = false;
            }
        }
        
        CPU.Init(BaseStatusRootPath, Reset);
        
        BaseRunning = true;

        Thread MonitorThread = new Thread(MonitorMethod);
        MonitorThread.IsBackground = true;
        MonitorThread.Start();
    }
    
    public static void Close() {
        BaseRunning = false;
        Parallel.ForEach(BaseContainers, DisposeMethod);
        CPU.Close();
    }

    private static void DisposeMethod(KeyValuePair<String, Container> Reference) {
        Reference.Value.Dispose();
    }

    private static void MonitorMethod() {
        while (BaseRunning == true) {
            Thread.Sleep(100);
            CPU.Update();
        }
    }

    public static void GetUsage(String UID, out Double NetworkInterfaceDownloadUsage, out Double NetworkInterfaceDownloadMaximum, out Double NetworkInterfaceUploadUsage, out Double NetworkInterfaceUploadMaximum, out Double StorageCapacityUsage, out Double StorageCapacityMaximum, out Double MemoryCapacityUsage, out Double MemoryCapacityMaximum, out Double[] CoreUsages) {
        BaseContainers[UID].GetUsage(out NetworkInterfaceDownloadUsage, out NetworkInterfaceDownloadMaximum, out NetworkInterfaceUploadUsage, out NetworkInterfaceUploadMaximum, out StorageCapacityUsage, out StorageCapacityMaximum, out MemoryCapacityUsage, out MemoryCapacityMaximum, out CoreUsages);
    }

    public static (Boolean, Boolean, Boolean, Boolean) Status(String UID) {
        return BaseContainers[UID].Status();
    }
    public static void Shell(String UID, String Username) {
        BaseContainers[UID].Shell(Username);
    }
    public static void Create(String UID, String Packages) {
        BaseContainers.Add(UID, new Container(BaseContainerRootPath, UID));
        BaseContainers[UID].InitializeAndInstall(Packages);
    }
    public static void Remove(String UID) {
        BaseContainers[UID].Remove();
        BaseContainers.Remove(UID);
    }
    public static void Boot(String UID) {
        BaseContainers[UID].Startup();
    }
    public static void Autoboot() {
        //foreach (KeyValuePair<String, Container> Entry in BaseContainers) {
        //    AutoBootEntryMethod(Entry);
        //}
        
        Parallel.ForEach(BaseContainers, AutoBootEntryMethod);
    }

    private static void AutoBootEntryMethod(KeyValuePair<String, Container> Reference) {
        Reference.Value.AutomaticStartup();
    }

    public static void Shutdown(String UID) {
        BaseContainers[UID].Shutdown();
    }
    
    public static void ShutdownAll() {
        //foreach (KeyValuePair<String, Container> Entry in BaseContainers) {
        //    ShutdownEntryMethod(Entry);
        //}
        Parallel.ForEach(BaseContainers, ShutdownEntryMethod);
    }

    private static void ShutdownEntryMethod(KeyValuePair<String, Container> Reference) {
        Reference.Value.Shutdown();
    }

    public static void Restart(String UID) {
        BaseContainers[UID].Restart();
    }
    
    public static void AssignV4Address(String UID, String Address, String Gateway) {
        BaseContainers[UID].AssignV4Address(Address, Gateway);
    }
    public static void AssignV6Address(String UID, String Address, String Gateway) {
        BaseContainers[UID].AssignV6Address(Address, Gateway);
    }
    
    public static void ResetV4Address(String UID) {
        BaseContainers[UID].ResetV4Addresses();
    }
    public static void ResetV6Address(String UID) {
        BaseContainers[UID].ResetV6Addresses();
    }
    
    public static String GetNetworkInterface(String UID) {
        return BaseContainers[UID].AssignedInterface;
    }
    public static void SetNetworkInterface(String UID, String Interface) {
        BaseContainers[UID].AssignedInterface = Interface;
    }
    
    public static String GetDisplayName(String UID) {
        return BaseContainers[UID].DisplayName;
    }
    public static void SetDisplayName(String UID, String DisplayName) {
        BaseContainers[UID].DisplayName = DisplayName;
    }
    
    public static String GetDescription(String UID) {
        return BaseContainers[UID].Description;
    }
    public static void SetDescription(String UID, String Description) {
        BaseContainers[UID].Description = Description;
    }
    
    public static Int32[] GetCores(String UID) {
        BaseContainers[UID].GetCPUs(out Int32[] CPUs);
        
        return CPUs;
    }
    public static void SetCores(String UID, Int32[] Cores) {
        BaseContainers[UID].SetCPUs(Cores);
    }
    
    public static Boolean GetEnabled(String UID) {
        return BaseContainers[UID].Enabled;
    }
    public static void SetEnabled(String UID, Boolean Value) {
        BaseContainers[UID].Enabled = Value;
    }
    
    public static UInt64 GetMemoryCapacity(String UID) {
        return BaseContainers[UID].MemoryCapacity;
    }
    public static void SetMemoryCapacity(String UID, UInt64 Value) {
        BaseContainers[UID].MemoryCapacity = Value;
    }
    
    public static UInt64 GetStorageCapacity(String UID) {
        return BaseContainers[UID].StorageCapacity;
    }
    public static void SetStorageCapacity(String UID, UInt64 Value) {
        BaseContainers[UID].StorageCapacity = Value;
    }
    
    public static UInt64 GetNetworkInterfaceUploadSpeed(String UID) {
        return BaseContainers[UID].NetworkInterfaceUploadSpeed;
    }
    public static void SetNetworkInterfaceUploadSpeed(String UID, UInt64 Value) {
        BaseContainers[UID].NetworkInterfaceUploadSpeed = Value;
    }
    
    public static UInt64 GetNetworkInterfaceDownloadSpeed(String UID) {
        return BaseContainers[UID].NetworkInterfaceDownloadSpeed;
    }
    public static void SetNetworkInterfaceDownloadSpeed(String UID, UInt64 Value) {
        BaseContainers[UID].NetworkInterfaceDownloadSpeed = Value;
    }
    
    public static Boolean GetAutoboot(String UID) {
        return BaseContainers[UID].Autoboot;
    }
    public static void SetAutoboot(String UID, Boolean Value) {
        BaseContainers[UID].Autoboot = Value;
    }
    
    public static void GetV4Address(String UID, out String Address, out String Gateway) {
        BaseContainers[UID].GetV4Addresses(out String[] Addresses);
        Address = Addresses[0];
        BaseContainers[UID].GetV4Gateway(out Gateway);
    }
    public static void GetV6Address(String UID, out String Address, out String Gateway) {
        BaseContainers[UID].GetV6Addresses(out String[] Addresses);
        Address = Addresses[0];
        BaseContainers[UID].GetV6Gateway(out Gateway);
    }
    public static String[] List() {
        return BaseContainers.Keys.ToArray();
    }
}