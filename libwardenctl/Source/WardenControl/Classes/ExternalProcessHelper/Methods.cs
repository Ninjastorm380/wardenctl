using Lightning.Diagnostics.Logging;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace WardenControl;

public class ExternalProcessHelper {
    public static void Run(String Command, String Arguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(Command, Arguments) {
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };
        
        System.Diagnostics.Process? Process = System.Diagnostics.Process.Start(StartInfo);
        Process?.Dispose();
    }
    
    public static void Fork(String Command, String Arguments) {
        Log.PrintAsync<ExternalProcessHelper>($"attempting to fork new executable process. Command: '{Command}', Args: {Arguments}", LogLevel.Debug);
        ProcessStartInfo StartInfo = new ProcessStartInfo(Command, Arguments) {
            UseShellExecute = true
        };
        
        System.Diagnostics.Process? Process = System.Diagnostics.Process.Start(StartInfo);
        //Process?.Dispose();
    }
    
    public static void Block(String Command, String Arguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(Command, Arguments) {
            WorkingDirectory = "/usr/bin"
        };
        
        System.Diagnostics.Process? Process = System.Diagnostics.Process.Start(StartInfo);
        Process?.WaitForExit();
        Process?.Dispose();
    }
    
    public static void Silent(String Command, String Arguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(Command, Arguments) {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            RedirectStandardInput = true
        };
        
        System.Diagnostics.Process? Process = System.Diagnostics.Process.Start(StartInfo);
        Process?.WaitForExit();
        Process?.Dispose();
    }
    
    
    public static (String, String) Check(String Command, String Arguments) {
        ProcessStartInfo StartInfo = new ProcessStartInfo(Command, Arguments) {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true
        };
        
        System.Diagnostics.Process? Process = System.Diagnostics.Process.Start(StartInfo);
        Process!.WaitForExit();
        
        String ValueOut = String.Empty;
        String ValueError = String.Empty;
        
        if (Process.StandardError.EndOfStream == false) {
            ValueError = Process.StandardError.ReadToEnd().Trim();
        }
        if (Process.StandardOutput.EndOfStream == false) {
            ValueOut = Process.StandardOutput.ReadToEnd().Trim();
        }

        Process.Dispose();

        return (ValueOut, ValueError);
    }
}