    using System.Runtime.InteropServices;
    using System.Text;
    using Lightning.Diagnostics.Logging;

    namespace WardenControl;

    public class ContainerControlHelper {
        private const Int32 ReadOnly = 0;

        [DllImport("libc", EntryPoint = "setns", SetLastError = true)]
        private static extern Int32 SetNamespace(Int32 FileDescriptor, Int32 NamespaceTypeFlags);

        [DllImport("libc", EntryPoint = "close", SetLastError = true)]
        private static extern Int32 CloseFileDescriptor(Int32 FileDescriptor);

        [DllImport("libc", EntryPoint = "open", SetLastError = true)]
        private static extern Int32 OpenFileDescriptor(String FilePath, Int32 FileFlags);
        
        private const Int32 CloneNewuser   = 0x10000000;
        private const Int32 CloneNewpid    = 0x20000000;
        private const Int32 CloneNewns     = 0x00020000;
        private const Int32 CloneNewnet    = 0x40000000;
        private const Int32 CloneNewipc    = 0x08000000;
        private const Int32 CloneNewuts    = 0x04000000;
        private const Int32 CloneNewcgroup = 0x02000000;
        
        private static Int32 SetNamespaceEx(Int32 Descriptor, String Filename) {
            Int32 Flag = Filename switch {
                "user" => CloneNewuser,
                "pid"  => CloneNewpid,
                "mnt"  => CloneNewns,
                "net"  => CloneNewnet,
                "ipc"  => CloneNewipc,
                "uts"  => CloneNewuts,
                "cgroup" => CloneNewcgroup,
                _ => 0
            };
            
            if (SetNamespace(Descriptor, Flag) == -1) {
                Int32 Err = Marshal.GetLastWin32Error();
                Console.WriteLine($"setns({Filename}) failed: fd={Descriptor} flag=0x{Flag:x} errno={Err}");

                return -1;
            }

            return 0;
        }
        private static (String, String) RunInNetworkNamespaceOnContainer(String UID, String Command, String Arguments) {
            String RawLeaderProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            if (Int32.TryParse(RawLeaderProcessID, out Int32 TargetProcessIdentifier) == false) {
                return default;
            }
            
            Int32 OriginalNamespace = OpenFileDescriptor("/proc/self/ns/net", ReadOnly);
            if (OriginalNamespace == -1) {
                return (String.Empty, String.Empty);
            }
            
            String TargetNamespaceFilePath = $"/proc/{TargetProcessIdentifier}/ns/net";
            Int32 TargetNamespace = OpenFileDescriptor(TargetNamespaceFilePath, ReadOnly);
            if (TargetNamespace == -1) {
                CloseFileDescriptor(OriginalNamespace);
                return (String.Empty, String.Empty);
            }
                
            if (SetNamespaceEx(TargetNamespace, "net") == -1) {
                CloseFileDescriptor(OriginalNamespace);
                CloseFileDescriptor(TargetNamespace);
                return (String.Empty, String.Empty);
            }
            
            

            (String, String) Results = ExternalProcessHelper.Check(Command, Arguments);

            if (SetNamespaceEx(OriginalNamespace, "net") == -1) {
                CloseFileDescriptor(OriginalNamespace);
                CloseFileDescriptor(TargetNamespace);
                return (String.Empty, String.Empty);
            }
            
            CloseFileDescriptor(OriginalNamespace);
            CloseFileDescriptor(TargetNamespace);

            return Results;
        }
        
        public static Boolean Mounted(String ControlPath, String MountPath) {
            String MountsFile = "/proc/mounts";
            String MountsData = File.ReadAllText(MountsFile).Trim();
            
            if (MountsData.Contains(MountPath) == false & MountsData.Contains(ControlPath) == false) {
                return false;
            }
            
            String Flag = $"{ControlPath}{Path.DirectorySeparatorChar}mounted";
            
            if (File.Exists(Flag) == false) {
                return false;
            }
            
            if (File.ReadAllText(Flag).Trim() == "0") {
                return false;
            }
            
            return true;
        }
        public static Boolean Running(String UID, String ContainerPath) {
            String MachinePath = $"/sys/fs/cgroup/machine.slice/machine-{Escape(UID)}.scope";
            
            (String?, String?) Result = RunInNetworkNamespaceOnContainer(UID, "/usr/bin/systemctl", "is-system-running"); //ExternalProcessHelper.Check("machinectl", $"-q shell root@{UID} /usr/bin/systemctl is-system-running");
            // (String, String) Result = ExternalProcessHelper.Check("machinectl", $"-q shell root@{UID} /usr/bin/systemctl is-system-running");

            if (Result.Item1 == null || Result.Item2 == null) {
                return false;
            }

            return Directory.Exists(MachinePath) & (Result.Item1.ToLower() == "running" | Result.Item1.ToLower() == "degraded" |  Result.Item2.ToLower() == "failed to get shell pty: connection refused") & Result.Item2.ToLower() != $"failed to get shell pty: no machine '{UID.ToLower()}' known" & Result.Item2.ToLower() != $"failed to get shell pty: There is no system bus in container {UID.ToLower()}.";
        }
        private static Boolean Online(String UID, String ContainerPath) {
            Log.PrintAsync<ContainerControlHelper>($"checking if container '{UID}' is online", LogLevel.Debug);
            
            String MachinePath = $"/sys/fs/cgroup/machine.slice/machine-{Escape(UID)}.scope";
            
            
            (String, String) Result = ExternalProcessHelper.Check("machinectl", $"list");

            Boolean A = Directory.Exists(MachinePath);
            Boolean C = Result.Item1.Contains(UID);

            if (Result.Item2 != String.Empty) {
                Log.PrintAsync<ContainerControlHelper>($"container '{UID}' reports {{ Control Folder Exists: '{A}', Container Is Listed: '{C}', Error Report: '{Result.Item2}' }}", LogLevel.Critical);
            }
            else {
                Log.PrintAsync<ContainerControlHelper>($"container '{UID}' reports {{ Control Folder Exists: '{A}', Container Is Listed: '{C}' }}", LogLevel.Debug);
            }
            
            return A & C;
        }
        private static Boolean NotRunning(String UID, String ContainerPath) {
            String MachinePath = $"/sys/fs/cgroup/machine.slice/machine-{Escape(UID)}.scope";

            //(String?, String?) Result = RunInNetworkNamespaceOnContainer(UID, "/usr/bin/systemctl", "is-system-running"); //ExternalProcessHelper.Check("machinectl", $"-q shell root@{UID} /usr/bin/systemctl is-system-running");
            (String, String) Result = ExternalProcessHelper.Check("machinectl", $"-q shell root@{UID} /usr/bin/systemctl is-system-running");
            
            return Directory.Exists(MachinePath) == false & Result.Item1.ToLower() != "running" & Result.Item1.ToLower() != "degraded" & Result.Item2.ToLower() == $"failed to get shell pty: no machine '{UID.ToLower()}' known";
        }
        
        public static Int32 WaitForRunning(String UID, String ContainerPath) {
            do {
                Thread.Sleep(100);
            } while (Running(UID, ContainerPath) == false);
            
            return 0;
        }
        public static Int32 WaitForOnline(String UID, String ContainerPath) {
            Log.PrintAsync<ContainerControlHelper>($"waiting for container '{UID}' to be online", LogLevel.Debug);
            do {
                Thread.Sleep(100);
            } while (Online(UID, ContainerPath) == false);
            Log.PrintAsync<ContainerControlHelper>($"container '{UID}' is now online", LogLevel.Debug);
            return 0;
        }
        public static Int32 WaitForNotRunning(String UID, String ContainerPath) {
            do {
                Thread.Sleep(100);
            } while (NotRunning(UID, ContainerPath) == false);
            
            return 0;
        }
        
        public static Int32 Mount(String StoragePath, String LogPath, String ControlPath, String MountPath, UInt64 Capacity) {
            String MountsFile = "/proc/mounts";
            Log.PrintAsync<ContainerControlHelper>($"Mounting tmpfs control filesystem at '{ControlPath}'", LogLevel.Debug);
            ExternalProcessHelper.Silent("mount", $"-t tmpfs tmpfs -o size=50K,noswap {ControlPath}");
            Log.PrintAsync<ContainerControlHelper>($"Waiting for tmpfs control filesystem to exist at '{ControlPath}'", LogLevel.Debug);
            while (File.ReadAllText(MountsFile).Trim().Contains(ControlPath) == false) {
                Thread.Sleep(100);
            }
            
            Log.PrintAsync<ContainerControlHelper>($"Mounting rampartfs container filesystem at '{MountPath}' with initial capacity '{Capacity}'", LogLevel.Debug);
            ExternalProcessHelper.Silent("rampartfs", $"{StoragePath} {ControlPath} {LogPath} {MountPath} {Capacity}");
            Log.PrintAsync<ContainerControlHelper>($"Waiting for rampartfs container filesystem to exist at '{MountPath}'", LogLevel.Debug);
            while (File.ReadAllText(MountsFile).Trim().Contains(MountPath) == false) {
                Thread.Sleep(100);
            }
            
            String Flag = $"{ControlPath}{Path.DirectorySeparatorChar}mounted";
            
            Log.PrintAsync<ContainerControlHelper>($"Waiting for rampartfs container filesystem to be ready at '{MountPath}'", LogLevel.Debug);
            while (File.Exists(Flag) == false) {
                Thread.Sleep(100);
            }
            
            while (File.ReadAllText(Flag).Trim() == "0") {
                Thread.Sleep(100);
            }
            
            return 0;
        }
        public static Int32 Unmount(String ControlPath, String MountPath) {
            String MountsFile = "/proc/mounts";
            String MountedStatusFile = $"{ControlPath}{Path.DirectorySeparatorChar}mounted";
            
            Log.PrintAsync<ContainerControlHelper>($"Unmounting rampartfs container filesystem at '{MountPath}'", LogLevel.Debug);
            while (File.ReadAllText(MountsFile).Trim().Contains(MountPath) == true) {
                ExternalProcessHelper.Silent("umount", $" {MountPath}");
            }
            
            if (File.Exists(MountedStatusFile) == true) {
                while (File.ReadAllText(MountedStatusFile).Trim() != "0") {
                    Thread.Sleep(100);
                }
            }
            
            Log.PrintAsync<ContainerControlHelper>($"Unmounting tmpfs control filesystem at '{ControlPath}'", LogLevel.Debug);
            while (File.ReadAllText(MountsFile).Trim().Contains(ControlPath) == true) {
                ExternalProcessHelper.Silent("umount", $" {ControlPath}");
            }
            
            return 0;
        }
        public static Int32 Start(String UID, String Hostname, String Interface, String MountPath) {    
            Log.PrintAsync<ContainerControlHelper>($"beginning container boot for '{UID}'", LogLevel.Debug);
            ExternalProcessHelper.Fork("systemd-nspawn", $"-qbPD {MountPath} -M {UID} --hostname {Hostname} --network-macvlan={Interface} --setenv=SYSTEMD_JOURNAL_STORAGE=volatile --bind=/dev/null:/etc/systemd/system/systemd-journal-flush.service");
            Log.PrintAsync<ContainerControlHelper>($"container now booting for '{UID}'", LogLevel.Debug);
            
            return 0;
        }
        public static Int32 Populate(String MountPath, String Packages) {
            ExternalProcessHelper.Silent("pacstrap", $"-K -c {MountPath} base {Packages}");
            ExternalProcessHelper.Silent("rm", $"{MountPath}/etc/systemd/system/default.target");
            ExternalProcessHelper.Silent("ln", $"-s /usr/lib/systemd/system/network.target {MountPath}/etc/systemd/system/default.target");
            
            return 0;
        }
        public static Int32 ContinueStart(String UID) {
            Log.PrintAsync<ContainerControlHelper>($"countinuing container boot for '{UID}'", LogLevel.Debug);
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            String Error; do {
                Error = ExternalProcessHelper.Check("nsenter", $"-a -t {ProcessID} /usr/bin/systemctl isolate multi-user.target").Item2;
            } while (Error == "Failed to connect to system scope bus via local transport: No such file or directory");
            Log.PrintAsync<ContainerControlHelper>($"continued container boot for '{UID}'", LogLevel.Debug);
            return 0;
        }
        public static Int32 Shell(String UID, String Username) {
            ExternalProcessHelper.Block("machinectl", $"-q shell {Username}@{UID}");

            return 0;
        }
        public static Int32 Stop(String UID) {
            ExternalProcessHelper.Silent("machinectl", $"-q poweroff {UID}");
            
            return 0;
        }
        
        public static Int32 EnableInterface(String UID, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip link set dev mv-{Interface} up");
            
            return 0;
        }
        public static Int32 DisableInterface(String UID, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip link set dev mv-{Interface} down");
            
            return 0;
        }
        
        public static Int32 BindRoutableV4Address(String UID, String V4AssignedAddress, String V4GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 addr add {V4AssignedAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route add {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route add default via {V4GatewayAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 BindRoutableV6Address(String UID, String V6AssignedAddress, String V6GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 addr add {V6AssignedAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route add {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route add default via {V6GatewayAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 RemoveRoutableV4Address(String UID, String V4AssignedAddress, String V4GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip route del default via {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip route del {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip addr del {V4AssignedAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 RemoveRoutableV6Address(String UID, String V6AssignedAddress, String V6GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip route del default via {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip route del {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip addr del {V6AssignedAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 BindRoutableV4Gateway(String UID, String V4GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route add {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route add default via {V4GatewayAddress} dev mv-{Interface}");
                
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route change {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route change default via {V4GatewayAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 BindRoutableV6Gateway(String UID, String V6GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route add {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route add default via {V6GatewayAddress} dev mv-{Interface}");
                
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route change {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route change default via {V6GatewayAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 RemoveRoutableV4Gateway(String UID, String V4GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route del default via {V4GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 route del {V4GatewayAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 RemoveRoutableV6Gateway(String UID, String V6GatewayAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route del default via {V6GatewayAddress} dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 route del {V6GatewayAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 BindRoutableV4Address(String UID, String V4AssignedAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 addr add {V4AssignedAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 BindRoutableV6Address(String UID, String V6AssignedAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;
            
            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 addr add {V6AssignedAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 RemoveRoutableV4Address(String UID, String V4AssignedAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -4 addr del {V4AssignedAddress} dev mv-{Interface}");
                
                return 0;
            }

            return -1;
        }
        public static Int32 RemoveRoutableV6Address(String UID, String V6AssignedAddress, String Interface) {
            String ProcessID = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value").Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/ip -6 addr del {V6AssignedAddress} dev mv-{Interface}");

                return 0;
            }

            return -1;
        }
        
        public static Int32 SetNetworkSpeed(String UID, String Interface, UInt64 Download, UInt64 Upload) {
            (String, String) Result = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value");
            String ProcessID = Result.Item1;

            if (ProcessID != String.Empty) {
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/tc qdisc del dev mv-{Interface}");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/tc qdisc add dev mv-{Interface} root handle 1: htb default 10");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/tc class add dev mv-{Interface} parent 1: classid 1:10 htb rate {Upload}Bps ceil {Upload}Bps");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/tc qdisc add dev mv-{Interface} handle ffff: ingress");
                ExternalProcessHelper.Silent("nsenter", $"-a -t {ProcessID} /usr/bin/tc filter add dev mv-{Interface} parent ffff: protocol all u32 match u32 0 0 police rate {Download}Bps burst {DeriveBurstFrom(Download)}B drop");
            
                return 0;
            }

            return -1;
        }
        public static Int32 SetLogicalCores(String UID, Int32[] Cores) {
            ExternalProcessHelper.Run("systemctl", $"set-property machine-{UID}.scope AllowedCPUs={Parsing.GetString(Cores)}");
            
            return 0;
        }
        public static Int32 SetMemoryMaximum(String UID, UInt64 Value) { 
            ExternalProcessHelper.Run("systemctl", $"set-property machine-{UID}.scope MemoryMax={Value}");
            
            return 0;
        }
        public static Int32 SetStorageMaximum(String ControlPath, UInt64 Value) {
            // File.WriteAllText($"{ControlPath}{Path.DirectorySeparatorChar}storagemaximum", Value.ToString());

            return 0;
        }
        
        public static (Int32, Double[]) GetCPUCoreUsages(String UID, Int32[] CPUs) {
            return (0, CPUs.Select(CPU.UsagePercent).ToArray());
        }
        public static (Int32, (UInt64, UInt64)) GetMemoryUsage(String UID) {
            
            if (UInt64.TryParse(File.ReadAllText($"/sys/fs/cgroup/machine.slice/machine-{UID}.scope/memory.current").Trim(), out UInt64 Current) == false) {
                return (-1, (0, 0));
            }
            
            return (0, (0, Current));
        }
        public static (Int32, (UInt64, UInt64)) GetStorageUsage(String ControlPath) {
            //UInt64.TryParse(File.ReadAllText($"{ControlPath}{Path.DirectorySeparatorChar}storagecurrent").Trim(), out UInt64 Current);
            
            return (0, (0, 0));
        }

        private static String FriendlyReadAllText(String FilePath) {
            FileStream ReadStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Write | FileShare.Delete);
            Span<Byte> Buffer = stackalloc Byte[100];
            Int32 BufferLength = ReadStream.Read(Buffer);
            ReadStream.Close();
            ReadStream.Dispose();

            Span<Char> Raw = stackalloc Char[50];

            Encoding.ASCII.TryGetChars(Buffer.Slice(0, BufferLength), Raw, out Int32 RawLength);

            return Raw.Slice(0, RawLength).ToString();
        }
        
        public static (Int32, (UInt64, UInt64)) GetNetworkDownloadUpload(String UID, String Interface) {
            (String, String) Result = ExternalProcessHelper.Check("machinectl", $"show {UID} --property=Leader --value");
            String ProcessID = Result.Item1;
            (String, String) DownloadReadResult = ExternalProcessHelper.Check("nsenter", $"-m -t {ProcessID} /usr/bin/cat /sys/class/net/mv-{Interface}/statistics/rx_bytes");
            (String, String) UploadReadResult = ExternalProcessHelper.Check("nsenter", $"-m -t {ProcessID} /usr/bin/cat /sys/class/net/mv-{Interface}/statistics/tx_bytes");
            
             return (0, (UInt64.Parse(DownloadReadResult.Item1), UInt64.Parse(UploadReadResult.Item1)));
        }

        public static String[] ListContainers(String RootDirectory) {
            List<String> UIDList = [];

            foreach (String ContainerPath in Directory.GetDirectories(RootDirectory, "*", SearchOption.TopDirectoryOnly)) {
                UIDList.Add(Path.GetFileName(ContainerPath));
            }

            return UIDList.ToArray();
        }
        
        private static UInt64 DeriveBurstFrom(UInt64 Speed) {
            UInt64 Result = (UInt64)Math.Ceiling(Speed * 0.10);

            return Result;
        }
        
        private static String Escape(String Name) {
            StringBuilder Builder = new StringBuilder();
            foreach (Byte Byte in Encoding.UTF8.GetBytes(Name)) {
                if ((Byte >= (Byte)'A' && Byte <= (Byte)'Z') || (Byte >= (Byte)'a' && Byte <= (Byte)'z') || (Byte >= (Byte)'0' && Byte <= (Byte)'9') || Byte == (Byte)'_' || Byte == (Byte)':') {
                    Builder.Append((Char)Byte);
                }
                else {
                    Builder.Append("\\x");
                    Builder.Append(Byte.ToString("x2"));
                }
            }
            return Builder.ToString();
        }
    }