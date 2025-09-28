using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WardenControl;

public static class LinuxStandardLibrary {
    [Flags] public enum NamespaceFlags : Int32 {
        USR = 0x10000000,
        MNT = 0x00020000,
        UTS = 0x04000000,
        IPC = 0x08000000,
        PID = 0x20000000,
        NET = 0x40000000,
        CGP = 0x02000000
    }
    
    [Flags] public enum FileFlags : Int32 {
        ReadOnly                      = 0x00000000,
        WriteOnly                     = 0x00000001,
        ReadWrite                     = 0x00000002,
        Create                        = 0x00000040,
        AtomicCreate                  = 0x00000080,
        DoNotAddAsControllingTerminal = 0x00000100,
        Truncate                      = 0x00000200,
        Append                        = 0x00000400,
        Nonblocking                   = 0x00000800,
        SyncData                      = 0x00001000,
        DirectIO                      = 0x00020000,
        LargeFile                     = 0x00040000,
        Directory                     = 0x00080000,
        DoNotFollowSymlinks           = 0x00100000,
        DoNotModifyAccessTime         = 0x00400000,
        CloseOnExecute                = 0x00800000,
        SyncMetadata                  = 0x01000000,
        SyncAll                       = 0x01001000,
        IsPath                        = 0x02000000,
        TemporaryFile                 = 0x04080000,
    }
    
    [DllImport("libc", EntryPoint = "setns", SetLastError = true)]
    private static extern Int32 InternalSetNamespace(Int32 FileDescriptor, Int32 NamespaceTypeFlags);

    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern Int32 InternalCloseFileDescriptor(Int32 FileDescriptor);

    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern Int32 InternalOpenFileDescriptor(String FilePath, Int32 FileFlags);

    public static Int32 SetNamespace(in Int32 FileDescriptor, in NamespaceFlags NamespaceFlags) {
        if (InternalSetNamespace(FileDescriptor, (Int32)NamespaceFlags) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static Int32 CloseFileDescriptor(in Int32 FileDescriptor) {
        if (InternalCloseFileDescriptor(FileDescriptor) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static Int32 OpenFileDescriptor(in String AbsoluteFilePath, in FileFlags OpenFlags, out Int32 FileDescriptor) {
        Int32 Descriptor = InternalOpenFileDescriptor(AbsoluteFilePath, (Int32)OpenFlags);
        
        if (Descriptor != -1) {
            FileDescriptor = Descriptor;
            return 0;
        }

        FileDescriptor = -1;
        return Marshal.GetLastWin32Error();
    }
}