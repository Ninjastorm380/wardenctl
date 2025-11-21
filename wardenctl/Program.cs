using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using Lightning.Diagnostics.Logging;
using WardenControl;

namespace wardenctl;

internal class Program {
    private static Boolean ControlC;
    
    [UnconditionalSuppressMessage("Trimming", "IL2059", Justification = "we enumerate all remaining types to preload static constructors with this call")]
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "we enumerate all remaining types to preload static constructors with this call")]
    private static void FrontLoadAssemblies() {
        Console.Write(' '); Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (Type Type in Assembly.GetTypes()) {
                try {
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(Type.TypeHandle);
                }
                catch {
                    
                }
            }
        }
    }
    
    static void Main(String[] Args) {
        LinuxStandardLibrary.GetEffectiveUserID(out UInt32 UID); if (UID != 0) {
            String? ExecutablePath = Environment.ProcessPath;
            (Int32 Result, String _, String _) = ExecutableLaunchHelper.Run("sudo", ["--version"]);
            if (ExecutablePath == null || Result != 0) {
                Console.WriteLine($"You must be superuser to run this program");
                Environment.Exit(1);
                return;
            }

            String[] ElevateArgs = new String[Args.Length + 1];
            ElevateArgs[0] = ExecutablePath;
            Array.Copy(Args, 0, ElevateArgs, 1, Args.Length);
            Log.Close();
            Environment.Exit(ExecutableLaunchHelper.Fork("sudo", ElevateArgs));
        }
        else {
            FrontLoadAssemblies();
            ElevatedMain(Args);
        }
    }

    static void ElevatedMain(String[] Args) {
        ControlC = false;
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;
        Log.AddLogTarget(new ConsoleLogTarget());
        
        String StorageRoot = Path.GetFullPath(Args[0]);
        String ConfigRoot  = Path.GetFullPath(Args[1]);
        IPAddress Address = IPAddress.Parse(Args[2]);
        UInt16 Port = UInt16.Parse(Args[3]);
        
        if (Args.Length > 4) {
            Log.Level = Log.ToLogLevel(Args[4]);
        }
        
        Boolean PathsOK = true;
        if (Directory.Exists(StorageRoot) == false) {
            PathsOK = false;
            Log.PrintAsync<Program>($"Missing remote storage directory root: '{StorageRoot}'", LogLevel.Critical);
        }
        if (Directory.Exists(ConfigRoot) == false) {
            PathsOK = false;
            Log.PrintAsync<Program>($"Missing remote config directory root: '{ConfigRoot}'", LogLevel.Critical);
        }

        if (PathsOK == false) {
            Log.Close();
            Environment.Exit(2);
        }

        Boolean AccessOK = true;
        if (LinuxStandardLibrary.OpenDescriptor(StorageRoot, LinuxStandardLibrary.FileFlags.Directory, out Int32 StorageRootDescriptor) != 0) {
            Log.PrintAsync<Program>($"Could not access remote storage directory root: '{StorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        if (LinuxStandardLibrary.OpenDescriptor(ConfigRoot, LinuxStandardLibrary.FileFlags.Directory, out Int32 ConfigRootDescriptor) != 0) {
            Log.PrintAsync<Program>($"Could not access remote config directory root: '{ConfigRoot}'", LogLevel.Critical);
            AccessOK = false;
        }

        if (AccessOK == false) {
            if (StorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(StorageRootDescriptor);
            }
            if (ConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(ConfigRootDescriptor);
            }
            Log.Close();
            Environment.Exit(13);
        }
        
        if (LinuxStandardLibrary.DescriptorStat(StorageRootDescriptor, out LinuxStandardLibrary.Stat StorageRootStat) != 0) {
            Log.PrintAsync<Program>($"Could not stat remote storage directory root: '{StorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        if (LinuxStandardLibrary.DescriptorStat(ConfigRootDescriptor, out LinuxStandardLibrary.Stat ConfigRootStat) != 0) {
            Log.PrintAsync<Program>($"Could not stat remote config directory root: '{ConfigRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (AccessOK == false) {
            if (StorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(StorageRootDescriptor);
            }
            if (ConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(ConfigRootDescriptor);
            }
            Log.Close();
            Environment.Exit(13);
        }
        
        Boolean PermsOK = true;
        if (StorageRootStat.st_uid != 0) {
            Log.PrintAsync<Program>($"Root does not own: '{StorageRoot}'", LogLevel.Critical);
            PermsOK = false;
        }
        if (ConfigRootStat.st_uid != 0) {
            Log.PrintAsync<Program>($"Root does not own: '{ConfigRoot}'", LogLevel.Critical);
            PermsOK = false;
        }
        
        if (PermsOK == false) {
            if (StorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(StorageRootDescriptor);
            }
            if (ConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(ConfigRootDescriptor);
            }
            Log.Close();
            Environment.Exit(1);
        }
        
        LinuxStandardLibrary.CloseDescriptor(StorageRootDescriptor);
        LinuxStandardLibrary.CloseDescriptor(ConfigRootDescriptor);
        
        Boolean ExpectedOK = true;
        if (File.Exists($"{StorageRoot}{Path.DirectorySeparatorChar}.warden.tag") == false) {
            Log.PrintAsync<Program>($"Missing warden tag file: '{StorageRoot}{Path.DirectorySeparatorChar}.warden.tag'", LogLevel.Critical);
            ExpectedOK = false;
        }
        if (File.Exists($"{ConfigRoot}{Path.DirectorySeparatorChar}.warden.tag") == false) {
            Log.PrintAsync<Program>($"Missing warden tag file: '{ConfigRoot}{Path.DirectorySeparatorChar}.warden.tag'", LogLevel.Critical);
            ExpectedOK = false;
        }
        
        if (ExpectedOK == false) {
            Log.Close();
            Environment.Exit(22);
        }

        if (ControlC == true) {
            Log.Close();
            Environment.Exit(0);
        }
        
        // TODO: Launch networked client.
        // WardenControl.Connect(Address, Port);
        // while (WardenControl.Connected == false & ControlC == false) {
        //     Thread.Sleep(1000);
        //     WardenControl.Connect(Address, Port);
        // }
        // 
        // while (WardenControl.Connected == true & ControlC == false) {
        //     Thread.Sleep(1000);
        // }
        // 
        // if(ControlC == true & WardenControl.Connected == true) {
        //     WardenControl.Disconnect();
        // }
        //
        // WardenControl.Shutdown();
        
        Log.Close();
        Environment.Exit(0);
    }

    private static void ConsoleOnCancelKeyPress(Object? Sender, ConsoleCancelEventArgs Args) {
        ControlC = true;
        Args.Cancel = true;
    }
}