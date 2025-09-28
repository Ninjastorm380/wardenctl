using System.Runtime.InteropServices;
using System.Text;

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
    }
}