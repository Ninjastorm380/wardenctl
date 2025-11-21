using System.Diagnostics.CodeAnalysis;

namespace WardenControl;

public static class NamespaceHelper {
    private static Boolean CheckEnterNamespace(Int32 Self, Int32 Target, LinuxStandardLibrary.NamespaceFlags NamespaceFlags) {
        if (LinuxStandardLibrary.DescriptorStat(Self, out LinuxStandardLibrary.Stat SelfStats) == 0) {
            if (LinuxStandardLibrary.DescriptorStat(Target, out LinuxStandardLibrary.Stat TargetStats) == 0) {
                if (SelfStats.Device == TargetStats.Device & SelfStats.INode == TargetStats.INode) {
                    return true;
                }

                if (LinuxStandardLibrary.SetNamespace(Target, NamespaceFlags) == 0) {
                    return true;
                }
            }
        }

        return false;
    }
    
    private static void CheckExitNamespace(Int32 Self, Int32 Target, LinuxStandardLibrary.NamespaceFlags NamespaceFlags) {
        if (LinuxStandardLibrary.DescriptorStat(Self, out LinuxStandardLibrary.Stat SelfStats) == 0) {
            if (LinuxStandardLibrary.DescriptorStat(Target, out LinuxStandardLibrary.Stat TargetStats) == 0) {
                if (SelfStats.Device == TargetStats.Device & SelfStats.INode == TargetStats.INode) {
                    return;
                }
                
                if (LinuxStandardLibrary.SetNamespace(Self, NamespaceFlags) == 0) {
                    
                }
            }
        }
    }
    private static Boolean CheckEnterMountNamespace(Int32 Self, Int32 Target) {
        if (LinuxStandardLibrary.DescriptorStat(Self, out LinuxStandardLibrary.Stat SelfStats) == 0) {
            if (LinuxStandardLibrary.DescriptorStat(Target, out LinuxStandardLibrary.Stat TargetStats) == 0) {
                if (SelfStats.Device == TargetStats.Device & SelfStats.INode == TargetStats.INode) {
                    return true;
                }

                LinuxStandardLibrary.UnshareNamespace(LinuxStandardLibrary.NamespaceFlags.MNT);
                if (LinuxStandardLibrary.SetNamespace(Target, LinuxStandardLibrary.NamespaceFlags.MNT) == 0) {
                    return true;
                }
            }
        }

        return false;
    }
    private static void CheckExitMountNamespace(Int32 Self, Int32 Target) {
        if (LinuxStandardLibrary.DescriptorStat(Self, out LinuxStandardLibrary.Stat SelfStats) == 0) {
            if (LinuxStandardLibrary.DescriptorStat(Target, out LinuxStandardLibrary.Stat TargetStats) == 0) {
                if (SelfStats.Device == TargetStats.Device & SelfStats.INode == TargetStats.INode) {
                    return;
                }

                LinuxStandardLibrary.UnshareNamespace(LinuxStandardLibrary.NamespaceFlags.MNT);
                if (LinuxStandardLibrary.SetNamespace(Self, LinuxStandardLibrary.NamespaceFlags.MNT) == 0) {
                }
            }
        }
    }
    private static Boolean AcquireSelfMounts(out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor) {
        SelfUSRDescriptor = -1;
        SelfMNTDescriptor = -1;
        SelfUTSDescriptor = -1;
        SelfIPCDescriptor = -1;
        SelfPIDDescriptor = -1;
        SelfNETDescriptor = -1;
        SelfCGPDescriptor = -1;
        
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/user", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfUSRDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/mnt", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfMNTDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/uts", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfUTSDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/ipc", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfIPCDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/pid", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfPIDDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/net", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfNETDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor("/proc/self/ns/cgroup", LinuxStandardLibrary.FileFlags.ReadOnly, out SelfCGPDescriptor) != 0) {
            return false;
        }

        return true;
    }
    private static Boolean AcquireTargetMounts(Int32 PID, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor) {
        TargetUSRDescriptor = -1;
        TargetMNTDescriptor = -1;
        TargetUTSDescriptor = -1;
        TargetIPCDescriptor = -1;
        TargetPIDDescriptor = -1;
        TargetNETDescriptor = -1;
        TargetCGPDescriptor = -1;
        
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/user", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetUSRDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/mnt", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetMNTDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/uts", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetUTSDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/ipc", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetIPCDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/pid", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetPIDDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/net", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetNETDescriptor) != 0) {
            return false;
        }
        if (LinuxStandardLibrary.OpenDescriptor($"/proc/{PID}/ns/cgroup", LinuxStandardLibrary.FileFlags.ReadOnly, out TargetCGPDescriptor) != 0) {
            return false;
        }

        return true;
    }
    private static void ReleaseMounts(Int32 USRDescriptor, Int32 MNTDescriptor, Int32 UTSDescriptor, Int32 IPCDescriptor, Int32 PIDDescriptor, Int32 NETDescriptor, Int32 CGPDescriptor) {
        if (USRDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(USRDescriptor);
        }
        if (MNTDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(MNTDescriptor);
        }
        if (UTSDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(UTSDescriptor);
        }
        if (IPCDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(IPCDescriptor);
        }
        if (PIDDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(PIDDescriptor);
        }
        if (NETDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(NETDescriptor);
        }
        if (CGPDescriptor != -1) {
            LinuxStandardLibrary.CloseDescriptor(CGPDescriptor);
        }
    }
    private static Int32 GetLeaderPID(String UID) {
        String Path = $"/run/systemd/machines/{UID}";

        if (!File.Exists(Path)) {
            return -1;
        }
        
        foreach (String Line in File.ReadLines(Path)) {
            String Selected = Line.Trim();
            if (Selected.StartsWith("LEADER=")) {
                String[] PairSplit = Selected.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                String ValueRaw = PairSplit[1].Trim();
                if (Int32.TryParse(ValueRaw, out Int32 PID)) {
                    return PID;
                }
            }
        }
        
        return -1;
    }
    private static void TransferFromContainerNamespace(Int32 SelfUSRDescriptor, Int32 SelfMNTDescriptor, Int32 SelfUTSDescriptor, Int32 SelfIPCDescriptor, Int32 SelfPIDDescriptor, Int32 SelfNETDescriptor, Int32 SelfCGPDescriptor, Int32 TargetUSRDescriptor, Int32 TargetMNTDescriptor, Int32 TargetUTSDescriptor, Int32 TargetIPCDescriptor, Int32 TargetPIDDescriptor, Int32 TargetNETDescriptor, Int32 TargetCGPDescriptor) {
        CheckExitNamespace(SelfCGPDescriptor, TargetCGPDescriptor, LinuxStandardLibrary.NamespaceFlags.CGP);
        CheckExitNamespace(SelfNETDescriptor, TargetNETDescriptor, LinuxStandardLibrary.NamespaceFlags.NET);
        CheckExitNamespace(SelfPIDDescriptor, TargetPIDDescriptor, LinuxStandardLibrary.NamespaceFlags.PID);
        CheckExitNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC);
        CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
        CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
        CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
        ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
        ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);
    }
    private static Boolean TransferToContainerNamespace(String UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor) {
        Int32 PID = GetLeaderPID(UID);
        SelfUSRDescriptor = -1;
        SelfMNTDescriptor = -1;
        SelfUTSDescriptor = -1;
        SelfIPCDescriptor = -1;
        SelfPIDDescriptor = -1;
        SelfNETDescriptor = -1;
        SelfCGPDescriptor = -1;
        TargetUSRDescriptor = -1;
        TargetMNTDescriptor = -1;
        TargetUTSDescriptor = -1;
        TargetIPCDescriptor = -1;
        TargetPIDDescriptor = -1;
        TargetNETDescriptor = -1;
        TargetCGPDescriptor = -1;
        
        if (PID == -1) {
            return false;
        }
        
        if (!AcquireSelfMounts(out SelfUSRDescriptor, out SelfMNTDescriptor, out SelfUTSDescriptor, out SelfIPCDescriptor, out SelfPIDDescriptor, out SelfNETDescriptor, out SelfCGPDescriptor)) {
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!AcquireTargetMounts(PID, out TargetUSRDescriptor, out TargetMNTDescriptor, out TargetUTSDescriptor, out TargetIPCDescriptor, out TargetPIDDescriptor, out TargetNETDescriptor, out TargetCGPDescriptor)) {
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR)) {
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);
            return false;
        }
        
        if (!CheckEnterMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor)) {
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS)) {
            CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC)) {
            CheckExitNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC);
            CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfPIDDescriptor, TargetPIDDescriptor, LinuxStandardLibrary.NamespaceFlags.PID)) {
            CheckExitNamespace(SelfPIDDescriptor, TargetPIDDescriptor, LinuxStandardLibrary.NamespaceFlags.PID);
            CheckExitNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC);
            CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfNETDescriptor, TargetNETDescriptor, LinuxStandardLibrary.NamespaceFlags.NET)) {
            CheckExitNamespace(SelfNETDescriptor, TargetNETDescriptor, LinuxStandardLibrary.NamespaceFlags.NET);
            CheckExitNamespace(SelfPIDDescriptor, TargetPIDDescriptor, LinuxStandardLibrary.NamespaceFlags.PID);
            CheckExitNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC);
            CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        if (!CheckEnterNamespace(SelfCGPDescriptor, TargetCGPDescriptor, LinuxStandardLibrary.NamespaceFlags.CGP)) {
            CheckExitNamespace(SelfCGPDescriptor, TargetCGPDescriptor, LinuxStandardLibrary.NamespaceFlags.CGP);
            CheckExitNamespace(SelfNETDescriptor, TargetNETDescriptor, LinuxStandardLibrary.NamespaceFlags.NET);
            CheckExitNamespace(SelfPIDDescriptor, TargetPIDDescriptor, LinuxStandardLibrary.NamespaceFlags.PID);
            CheckExitNamespace(SelfIPCDescriptor, TargetIPCDescriptor, LinuxStandardLibrary.NamespaceFlags.IPC);
            CheckExitNamespace(SelfUTSDescriptor, TargetUTSDescriptor, LinuxStandardLibrary.NamespaceFlags.UTS);
            CheckExitMountNamespace(SelfMNTDescriptor, TargetMNTDescriptor);
            CheckExitNamespace(SelfUSRDescriptor, TargetUSRDescriptor, LinuxStandardLibrary.NamespaceFlags.USR);
                
            ReleaseMounts(TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            ReleaseMounts(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor);

            return false;
        }
        
        return true;
    }
    
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6>(Action<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4, TInput5 Input5, TInput6 Input6) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1, Input2, Input3, Input4, Input5, Input6);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4, TInput5>(Action<TInput1, TInput2, TInput3, TInput4, TInput5> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4, TInput5 Input5) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1, Input2, Input3, Input4, Input5);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4>(Action<TInput1, TInput2, TInput3, TInput4> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1, Input2, Input3, Input4);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3>(Action<TInput1, TInput2, TInput3> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1, Input2, Input3);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2>(Action<TInput1, TInput2> Task, String UID, TInput1 Input1, TInput2 Input2) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1, Input2);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced<TInput1>(Action<TInput1> Task, String UID, TInput1 Input1) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task(Input1);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    public static Boolean Namespaced(Action Task, String UID) {
        Boolean Success = false;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Task();
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();

        return Success;
    }
    
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TOutput>(Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TOutput> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4, TInput5 Input5, TInput6 Input6, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1, Input2, Input3, Input4, Input5, Input6);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4, TInput5, TOutput>(Func<TInput1, TInput2, TInput3, TInput4, TInput5, TOutput> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4, TInput5 Input5, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1, Input2, Input3, Input4, Input5);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TInput4, TOutput>(Func<TInput1, TInput2, TInput3, TInput4, TOutput> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, TInput4 Input4, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1, Input2, Input3, Input4);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TInput3, TOutput>(Func<TInput1, TInput2, TInput3, TOutput> Task, String UID, TInput1 Input1, TInput2 Input2, TInput3 Input3, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1, Input2, Input3);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TInput1, TInput2, TOutput>(Func<TInput1, TInput2, TOutput> Task, String UID, TInput1 Input1, TInput2 Input2, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1, Input2);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TInput1, TOutput>(Func<TInput1, TOutput> Task, String UID, TInput1 Input1, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
        
            Result = Task(Input1);
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);

            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
    public static Boolean Namespaced<TOutput>(Func<TOutput> Task, String UID, [NotNullWhen(true)] out TOutput? Output) {
        Boolean Success = false;
        TOutput? Result = default;
        Thread AsyncThread = new Thread(() => {
            if (!TransferToContainerNamespace(UID, out Int32 SelfUSRDescriptor, out Int32 SelfMNTDescriptor, out Int32 SelfUTSDescriptor, out Int32 SelfIPCDescriptor, out Int32 SelfPIDDescriptor, out Int32 SelfNETDescriptor, out Int32 SelfCGPDescriptor, out Int32 TargetUSRDescriptor, out Int32 TargetMNTDescriptor, out Int32 TargetUTSDescriptor, out Int32 TargetIPCDescriptor, out Int32 TargetPIDDescriptor, out Int32 TargetNETDescriptor, out Int32 TargetCGPDescriptor)) {
                Success = false;
                return;
            }
            
            Result = Task();
        
            TransferFromContainerNamespace(SelfUSRDescriptor, SelfMNTDescriptor, SelfUTSDescriptor, SelfIPCDescriptor, SelfPIDDescriptor, SelfNETDescriptor, SelfCGPDescriptor, TargetUSRDescriptor, TargetMNTDescriptor, TargetUTSDescriptor, TargetIPCDescriptor, TargetPIDDescriptor, TargetNETDescriptor, TargetCGPDescriptor);
            
            Success = true;
        }); AsyncThread.Start(); AsyncThread.Join();
        Output = Result;
        return Success;
    }
}