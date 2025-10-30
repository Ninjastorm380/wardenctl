using System.Diagnostics;

namespace WardenControl;

public static class ExecutableLaunchHelper {
    public static void ForkAsync(String ExecutableFilePath, String[] ExecutableArguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(ExecutableFilePath, ExecutableArguments) {
            UseShellExecute = true
        }; Process.Start(StartInfo)?.Dispose();
    }
    
    public static Int32 Fork(String ExecutableFilePath, String[] ExecutableArguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(ExecutableFilePath, ExecutableArguments) {
            UseShellExecute = true
        }; Process? Process = Process.Start(StartInfo);
        
        Process?.WaitForExit();
        Int32 Code = Process?.ExitCode ?? -1;
        Process?.Dispose();

        return Code;
    }
    
    public static (Int32, String, String) Run(String ExecutableFilePath, String[] ExecutableArguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(ExecutableFilePath, ExecutableArguments) {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true
        }; Process? Process = Process.Start(StartInfo);
        
        Int32 Code = -1; String Out = String.Empty; String Error = String.Empty;

        if (Process != null) {
            Process.WaitForExit();
            Code = Process.ExitCode;
            
            if (Process.StandardOutput.EndOfStream == false) {
                Out = Process.StandardOutput.ReadToEnd().Trim();
            }
            
            if (Process.StandardError.EndOfStream == false) {
                Error = Process.StandardError.ReadToEnd().Trim();
            }
        }

        return (Code, Out, Error);
    }
    
    public static void RunAsync(String ExecutableFilePath, String[] ExecutableArguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(ExecutableFilePath, ExecutableArguments) {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true
        }; Process.Start(StartInfo)?.Dispose();
    }
}