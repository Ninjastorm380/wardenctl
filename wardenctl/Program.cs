using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Lightning.Diagnostics.Logging;
using WardenControl;

namespace wardenctl;

internal class Program {
    static void Main(String[] Args) {
        if (Args.Length > 3) {
            Log.Level = Log.ToLogLevel(Args[3]);
        }
        
        LinuxStandardLibrary.GetEffectiveUserID(out UInt32 UID); if (UID != 0) {
            String? ExecutablePath = Environment.ProcessPath;
            if (ExecutablePath == null || File.Exists("/usr/bin/sudo") == false) {
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
            ElevatedMain(Args);
        }
    }

    static void ElevatedMain(String[] Args) {
        Log.AddLogTarget(new ConsoleLogTarget());
        String RemoteStorageRoot = Path.GetFullPath(Args[0]);
        String RemoteConfigRoot  = Path.GetFullPath(Args[1]);
        String LocalStorageRoot  = Path.GetFullPath(Args[2]);
        
        Boolean PathsOK = true;
        if (Directory.Exists(RemoteStorageRoot) == false) {
            PathsOK = false;
            Log.PrintAsync<Program>($"Missing remote storage directory root: '{RemoteStorageRoot}'", LogLevel.Critical);
        }
        if (Directory.Exists(RemoteConfigRoot) == false) {
            PathsOK = false;
            Log.PrintAsync<Program>($"Missing remote config directory root: '{RemoteConfigRoot}'", LogLevel.Critical);
        }
        if (Directory.Exists(LocalStorageRoot) == false) {
            PathsOK = false;
            Log.PrintAsync<Program>($"Missing local storage directory root: '{LocalStorageRoot}'", LogLevel.Critical);
        }

        if (PathsOK == false) {
            Log.Close();
            Environment.Exit(2);
        }

        Boolean AccessOK = true;
        if (LinuxStandardLibrary.OpenDescriptor(RemoteStorageRoot, LinuxStandardLibrary.FileFlags.Directory, out Int32 RemoteStorageRootDescriptor) != 0) {
            Log.PrintAsync<Program>($"Could not access remote storage directory root: '{RemoteStorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (LinuxStandardLibrary.OpenDescriptor(RemoteConfigRoot, LinuxStandardLibrary.FileFlags.Directory, out Int32 RemoteConfigRootDescriptor) != 0) {
            Log.PrintAsync<Program>($"Could not access remote config directory root: '{RemoteConfigRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (LinuxStandardLibrary.OpenDescriptor(LocalStorageRoot, LinuxStandardLibrary.FileFlags.Directory, out Int32 LocalStorageRootDescriptor) != 0) {
            Log.PrintAsync<Program>($"Could not access local storage directory root: '{LocalStorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }

        if (AccessOK == false) {
            if (RemoteStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteStorageRootDescriptor);
            }
            if (RemoteConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteConfigRootDescriptor);
            }
            if (LocalStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(LocalStorageRootDescriptor);
            }
            Log.Close();
            Environment.Exit(13);
        }
        
        if (LinuxStandardLibrary.DescriptorStat(RemoteStorageRootDescriptor, out LinuxStandardLibrary.Stat RemoteStorageRootStat) != 0) {
            Log.PrintAsync<Program>($"Could not stat remote storage directory root: '{RemoteStorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (LinuxStandardLibrary.DescriptorStat(RemoteConfigRootDescriptor, out LinuxStandardLibrary.Stat RemoteConfigRootStat) != 0) {
            Log.PrintAsync<Program>($"Could not stat remote config directory root: '{RemoteConfigRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (LinuxStandardLibrary.DescriptorStat(LocalStorageRootDescriptor, out LinuxStandardLibrary.Stat LocalStorageRootStat) != 0) {
            Log.PrintAsync<Program>($"Could not stat local storage directory root: '{LocalStorageRoot}'", LogLevel.Critical);
            AccessOK = false;
        }
        
        if (AccessOK == false) {
            if (RemoteStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteStorageRootDescriptor);
            }
            if (RemoteConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteConfigRootDescriptor);
            }
            if (LocalStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(LocalStorageRootDescriptor);
            }
            Log.Close();
            Environment.Exit(13);
        }
        
        Boolean PermsOK = true;
        if (RemoteStorageRootStat.st_uid != 0) {
            Log.PrintAsync<Program>($"Root does not own: '{RemoteStorageRoot}'", LogLevel.Critical);
            PermsOK = false;
        }
        if (RemoteConfigRootStat.st_uid != 0) {
            Log.PrintAsync<Program>($"Root does not own: '{RemoteConfigRoot}'", LogLevel.Critical);
            PermsOK = false;
        }
        if (LocalStorageRootStat.st_uid != 0) {
            Log.PrintAsync<Program>($"Root does not own: '{LocalStorageRoot}'", LogLevel.Critical);
            PermsOK = false;
        }
        
        if (PermsOK == false) {
            if (RemoteStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteStorageRootDescriptor);
            }
            if (RemoteConfigRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(RemoteConfigRootDescriptor);
            }
            if (LocalStorageRootDescriptor != -1) {
                LinuxStandardLibrary.CloseDescriptor(LocalStorageRootDescriptor);
            }
            Log.Close();
            Environment.Exit(1);
        }
        
        LinuxStandardLibrary.CloseDescriptor(RemoteStorageRootDescriptor);
        LinuxStandardLibrary.CloseDescriptor(RemoteConfigRootDescriptor);
        LinuxStandardLibrary.CloseDescriptor(LocalStorageRootDescriptor);
        
        Boolean ExpectedOK = true;
        if (File.Exists($"{RemoteStorageRoot}{Path.DirectorySeparatorChar}.warden.tag") == false) {
            Log.PrintAsync<Program>($"Missing warden tag file: '{RemoteStorageRoot}{Path.DirectorySeparatorChar}.warden.tag'", LogLevel.Critical);
            ExpectedOK = false;
        }
        if (File.Exists($"{RemoteConfigRoot}{Path.DirectorySeparatorChar}.warden.tag") == false) {
            Log.PrintAsync<Program>($"Missing warden tag file: '{RemoteConfigRoot}{Path.DirectorySeparatorChar}.warden.tag'", LogLevel.Critical);
            ExpectedOK = false;
        }
        if (File.Exists($"{LocalStorageRoot}{Path.DirectorySeparatorChar}.warden.tag") == false) {
            Log.PrintAsync<Program>($"Missing warden tag file: '{LocalStorageRoot}{Path.DirectorySeparatorChar}.warden.tag'", LogLevel.Critical);
            ExpectedOK = false;
        }
        
        if (ExpectedOK == false) {
            Log.Close();
            Environment.Exit(22);
        }
        
        Log.Close();
        Environment.Exit(0);
    }
}