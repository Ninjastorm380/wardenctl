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
    
    [StructLayout(LayoutKind.Explicit, Size = 144)] public struct Stat {
        [FieldOffset(000)] public UInt64 Device;
        [FieldOffset(008)] public UInt64 INode;
        [FieldOffset(016)] public UInt64 st_nlink;
        [FieldOffset(024)] public UInt32 st_mode;
        [FieldOffset(028)] public UInt32 st_uid;
        [FieldOffset(032)] public UInt32 st_gid;
        [FieldOffset(036)] public UInt64 st_rdev;
        [FieldOffset(052)] public Int64 st_size;
        [FieldOffset(060)] public Int32 st_blksize;
        [FieldOffset(072)] public Int64 st_blocks;
        [FieldOffset(080)] public Int64 st_atime;
        [FieldOffset(088)] public Int64 st_atimensec;
        [FieldOffset(096)] public Int64 st_mtime;
        [FieldOffset(104)] public Int64 st_mtimensec;
        [FieldOffset(112)] public Int64 st_ctime;
        [FieldOffset(120)] public Int64 st_ctimensec;
    }
    
    [DllImport("libc", EntryPoint = "geteuid", SetLastError = true)]
    private static extern UInt32 InternalGetEUID();
    
    [DllImport("libc", EntryPoint = "setns", SetLastError = true)]
    private static extern Int32 InternalSetNamespace(Int32 FileDescriptor, Int32 NamespaceTypeFlags);

    [DllImport("libc", EntryPoint = "unshare", SetLastError = true)]
    private static extern Int32 InternalUnshareNamespace(Int32 NamespaceTypeFlags);
    
    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern Int32 InternalCloseFileDescriptor(Int32 FileDescriptor);

    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern Int32 InternalOpenFileDescriptor(String FilePath, Int32 FileFlags);
    
    [DllImport("libc", EntryPoint = "fstat", SetLastError = true)]
    private static extern Int32 InternalDescriptorStat(Int32 Descriptor, out Stat Stats);
    
    public static Int32 DescriptorStat(Int32 Descriptor, out Stat Stats) {
        if (InternalDescriptorStat(Descriptor, out Stats) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static void GetEffectiveUserID(out UInt32 UserID) {
        UserID = InternalGetEUID();
    }
    
    public static Int32 SetNamespace(in Int32 Descriptor, in NamespaceFlags NamespaceFlags) {
        if (InternalSetNamespace(Descriptor, (Int32)NamespaceFlags) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static Int32 UnshareNamespace(in NamespaceFlags NamespaceFlags) {
        if (InternalUnshareNamespace((Int32)NamespaceFlags) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static Int32 CloseDescriptor(in Int32 Descriptor) {
        if (InternalCloseFileDescriptor(Descriptor) != -1) {
            return 0;
        }

        return Marshal.GetLastWin32Error();
    }
    
    public static Int32 OpenDescriptor(in String AbsoluteFilePath, in FileFlags OpenFlags, out Int32 Descriptor) {
        Int32 InternalDescriptor = InternalOpenFileDescriptor(AbsoluteFilePath, (Int32)OpenFlags);
        
        if (InternalDescriptor != -1) {
            Descriptor = InternalDescriptor;
            return 0;
        }

        Descriptor = -1;
        return Marshal.GetLastWin32Error();
    }
}