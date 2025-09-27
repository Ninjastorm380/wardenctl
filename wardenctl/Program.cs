using System.Runtime.InteropServices;
using System.Text;
using WardenControl;

namespace wardenctl;

internal static class Program {
    [DllImport("libc", EntryPoint = "geteuid")]
    private static extern UInt32 GetEUID();
     
    static void Main(String[] Args) {
        
        if (GetEUID() != 0) {
            Console.WriteLine("You must be superuser to run this program.");

            return;
        }

        String WardenRoot = Path.GetFullPath("/srv/warden");
        String WardenContainerRoot = Path.GetFullPath("/srv/warden/containers");
        String WardenStatusRoot = Path.GetFullPath("/srv/warden/status");

        if (Directory.Exists(WardenRoot) == false) {
            Directory.CreateDirectory(WardenRoot);
        }

        Manager.Init(WardenContainerRoot, WardenStatusRoot);

        if (Args.Length == 0) {
            Manager.Close();
            
            return;
        }

        switch (Args[0].ToLower()) {
            case "create":
                if (Args.Length > 3) {
                    String Packages = Args[2];
                    for (int Index = 3; Index < Args.Length; Index++) {
                        Packages += $" {Args[Index]}";
                    }
                    
                    Manager.Create(Args[1], Packages);

                    break;
                }
                if (Args.Length > 2) {
                    
                    Manager.Create(Args[1], Args[2]);

                    break;
                }
                if (Args.Length > 1) {
                    Manager.Create(Args[1], "sudo nano btop openssh");
                }

                break;
            case "remove":
                if (Args.Length > 1) {
                    Manager.Remove(Args[1]);
                }

                break;
            case "list":
                foreach (String UID in Manager.List()) {
                    (Boolean, Boolean, Boolean, Boolean) EntryStatus = Manager.Status(UID);
                    String DisplayName = Manager.GetDisplayName(UID);
                    String Description = Manager.GetDescription(UID);

                    String EntryPrintable = $"{UID}:{Environment.NewLine} Flags:{Environment.NewLine}  ";

                    if (EntryStatus.Item3) {
                        EntryPrintable += "Enabled";
                    }
                    else {
                        EntryPrintable += "Disabled";
                    }

                    if (EntryStatus.Item4) {
                        EntryPrintable += ", Boot";
                    }

                    if (EntryStatus.Item2) {
                        EntryPrintable += ", Running";
                    }

                    BaseBuilder.Append(" Display Name:").AppendLine().Append("  ").Append(DisplayName).AppendLine();
                    BaseBuilder.Append(" Description:").AppendLine().Append("  ").Append(Description).AppendLine();
                    
                    Console.WriteLine($"{EntryPrintable}{Environment.NewLine} Display Name:{Environment.NewLine}  {DisplayName}{Environment.NewLine} Description:{Environment.NewLine}  {Description}{Environment.NewLine}");
                }
                break;
            case "status":
                if (Args.Length > 1) {
                    PrintStatus(Args[1]);
                    PrintStatus(Args[1]);
                }

                break;

            case "monitor":
                if (Args.Length > 1) {
                    while (true) {
                        Thread.Sleep(100);
                        Console.Clear();
                        PrintStatus(Args[1]);
                    }
                }

                break;
            case "shell":
                if (Args.Length > 2) {
                    Manager.Shell(Args[1], Args[2]);
                }

                break;
            case "network-interface":
                if (Args.Length > 2) {
                    Manager.SetNetworkInterface(Args[1], Args[2]);

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Selected Network Interface: '{Manager.GetNetworkInterface(Args[1])}'");
                }

                break;
            case "upload-speed":
                if (Args.Length > 2) {
                    Manager.SetNetworkInterfaceUploadSpeed(Args[1], UInt64.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Maximum Network Interface Upload Speed: '{Manager.GetNetworkInterfaceUploadSpeed(Args[1])}'");
                }

                break;
            case "download-speed":
                if (Args.Length > 2) {
                    Manager.SetNetworkInterfaceDownloadSpeed(Args[1], UInt64.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Maximum Network Interface Download Speed: '{Manager.GetNetworkInterfaceDownloadSpeed(Args[1])}'");
                }

                break;
            case "cores":
                if (Args.Length > 2) {
                    Manager.SetCores(Args[1], Parsing.GetInt32Array(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Logical CPU Cores: '{Parsing.GetString(Manager.GetCores(Args[1]))}'");
                }

                break;
            case "boot":
                if (Args.Length > 2) {
                    Manager.SetAutoboot(Args[1], Boolean.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Boot On Physical Server Startup: '{Manager.GetAutoboot(Args[1])}'");
                }

                break;
            case "enabled":
                if (Args.Length > 2) {
                    Manager.SetEnabled(Args[1], Boolean.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Enabled: '{Manager.GetEnabled(Args[1])}'");
                }

                break;
            case "memory-capacity":
                if (Args.Length > 2) {
                    Manager.SetMemoryCapacity(Args[1], UInt64.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Logical Memory Capacity: '{Manager.GetMemoryCapacity(Args[1])}'");
                }

                break;
            case "disk-capacity":
                if (Args.Length > 2) {
                    Manager.SetStorageCapacity(Args[1], UInt64.Parse(Args[2]));

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Logical Disk Capacity: '{Manager.GetStorageCapacity(Args[1])}'");
                }

                break;
            case "display-name":
                if (Args.Length > 2) {
                    Manager.SetDisplayName(Args[1], Args[2]);

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Display Name: '{Manager.GetDisplayName(Args[1])}'");
                }

                break;
            case "description":
                if (Args.Length > 2) {
                    Manager.SetDescription(Args[1], Args[2]);

                    break;
                }

                if (Args.Length > 1) {
                    Console.WriteLine($"Description: '{Manager.GetDescription(Args[1])}'");
                }

                break;
            case "ipv4":
                if (Args.Length > 3) {
                    Manager.AssignV4Address(Args[1], Args[2], Args[3]);

                    break;
                }

                if (Args.Length > 2) {
                    if (Args[2].ToLower() == "reset") {
                        Manager.ResetV4Address(Args[1]);
                    }

                    break;
                }

                if (Args.Length > 1) {
                    Manager.GetV4Address(Args[1], out String Address, out String Gateway);
                    Console.WriteLine($"IPv4 Information: {{ Address: '{Address}', Gateway: '{Gateway}' }}");
                }

                break;
            case "ipv6":
                if (Args.Length > 3) {
                    Manager.AssignV6Address(Args[1], Args[2], Args[3]);

                    break;
                }

                if (Args.Length > 2) {
                    if (Args[2].ToLower() == "reset") {
                        Manager.ResetV6Address(Args[1]);
                    }

                    break;
                }

                if (Args.Length > 1) {
                    Manager.GetV6Address(Args[1], out String Address, out String Gateway);
                    Console.WriteLine($"IPv6 Information: {{ Address: '{Address}', Gateway: '{Gateway}' }}");
                }

                break;
            case "startup":
                if (Args.Length > 2) {
                    Manager.Boot(Args[1]);
                    Manager.Shell(Args[1], Args[2]);

                    break;
                }

                if (Args.Length > 1) {
                    Manager.Boot(Args[1]);

                    break;
                }

                if (Args.Length > 0) {
                    Manager.Autoboot();
                }

                break;
            case "shutdown":
                if (Args.Length > 1) {
                    Manager.Shutdown(Args[1]);

                    break;
                }
                
                if (Args.Length > 0) {
                    Manager.ShutdownAll();
                }

                break;
            case "restart":
                if (Args.Length > 2) {
                    Manager.Restart(Args[1]);
                    Manager.Shell(Args[1], Args[2]);
                    break;
                }
                if (Args.Length > 1) {
                    Manager.Restart(Args[1]);
                }

                break;
            case "init":
                break;
        }

        Manager.Close();
    }

    private static readonly StringBuilder BaseBuilder = new StringBuilder(200000);
    private static String BaseNextPrint = String.Empty;
    
    private static void PrintStatus(String UID) {
        Console.Write(BaseNextPrint);
        
        (Boolean, Boolean, Boolean, Boolean) Status = Manager.Status(UID);
        Manager.GetUsage(UID, out Double DownloadUsage, out Double DownloadMaximum, out Double UploadUsage, out Double UploadMaximum, out Double StorageUsage, out Double StorageMaximum, out Double MemoryUsage, out Double MemoryMaximum, out Double[] CoreUsages);
        String DisplayName = Manager.GetDisplayName(UID);
        String Description = Manager.GetDescription(UID);
        Int32[] Cores = Manager.GetCores(UID);
        
        
        Double DownloadCurrent = DownloadMaximum * DownloadUsage;
        Double UploadCurrent = UploadMaximum * UploadUsage;
        Double StorageCurrent = StorageMaximum * StorageUsage;
        Double MemoryCurrent = MemoryMaximum * MemoryUsage;
        
        BaseBuilder.Clear();
            
        BaseBuilder.Append("Server '").Append(UID).Append("':").AppendLine().Append(" Flags:").AppendLine().Append("  ").Append(Status.Item3 ? "Enabled" : "Disabled");

        if (Status.Item4) {
            BaseBuilder.Append(", Boot");
        }

        if (Status.Item2) {
            BaseBuilder.Append(", Running");
        }

        BaseBuilder.AppendLine();

        BaseBuilder.Append(" Display Name:").AppendLine().Append("  ").Append(DisplayName).AppendLine();
        BaseBuilder.Append(" Description:").AppendLine().Append("  ").Append(Description).AppendLine();
        
        BaseBuilder.Append(" Download Speed:").AppendLine().Append("  ").Append(DownloadCurrent.ToString("000000000000000000")).Append("B / ").Append(DownloadMaximum.ToString("000000000000000000")).Append("B (").Append(DownloadUsage.ToString("000%")).Append(')').AppendLine();
        BaseBuilder.Append(" Upload Speed:").AppendLine().Append("  ").Append(UploadCurrent.ToString("000000000000000000")).Append("B / ").Append(UploadMaximum.ToString("000000000000000000")).Append("B (").Append(UploadUsage.ToString("000%")).Append(')').AppendLine();
        BaseBuilder.Append(" Storage Capacity:").AppendLine().Append("  ").Append(StorageCurrent.ToString("000000000000000000")).Append("B / ").Append(StorageMaximum.ToString("000000000000000000")).Append("B (").Append(StorageUsage.ToString("000%")).Append(')').AppendLine();
        BaseBuilder.Append(" Memory Capacity:").AppendLine().Append("  ").Append(MemoryCurrent.ToString("000000000000000000")).Append("B / ").Append(MemoryMaximum.ToString("000000000000000000")).Append("B (").Append(MemoryUsage.ToString("000%")).Append(')').AppendLine();
        
        if (CoreUsages.Length > 0) {
            BaseBuilder.AppendLine(" CPU Usage:").Append("  ").Append(CoreUsages[0].ToString("000.0%"));

            for (int Index = 1; Index < CoreUsages.Length; Index++) {
                BaseBuilder.Append(", ").Append(CoreUsages[Index].ToString("000.0%"));
            }

            BaseBuilder.AppendLine();
        }
        
        if (Cores.Length > 0) {
            BaseBuilder.AppendLine(" CPU Cores:").Append("  ").Append(Cores[0].ToString("000000"));

            for (int Index = 1; Index < Cores.Length; Index++) {
                BaseBuilder.Append(", ").Append(Cores[Index].ToString("000000"));
            }

            BaseBuilder.AppendLine();
        }
        
        BaseNextPrint = BaseBuilder.ToString();
    }
}