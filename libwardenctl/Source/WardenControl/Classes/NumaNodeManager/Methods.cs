using System.Diagnostics.CodeAnalysis;

namespace WardenControl;

public partial class NumaNodeManager {
    public NumaNodeManager(Boolean Autodetect = true, UInt64 AutoLogicalMemoryReserved = 0, UInt64 AutoLogicalCoresReserved = 0) {
        BaseNodes = new Dictionary<UInt64, NumaNode>(128);
        if (Autodetect) {
            foreach (String NodePath in Directory.GetDirectories("/sys/devices/system/node", "node*", SearchOption.TopDirectoryOnly)) {
                NumaNode Node = new NumaNode(NodePath, AutoLogicalCoresReserved, AutoLogicalMemoryReserved);
                BaseNodes.Add(Node.NodeID, Node);
            }
        }
    }
    
    public void AddNode(UInt64 NodeID, NumaCore[] Cores, UInt64 LogicalCoresReserved, UInt64 LogicalMemoryMaximum, UInt64 LogicalMemoryReserved) {
        NumaNode Node = new NumaNode(NodeID, Cores, LogicalCoresReserved, LogicalMemoryMaximum, LogicalMemoryReserved);
        BaseNodes.Add(NodeID, Node);
    }
    
    public void RemoveNode(UInt64 NodeID) {
        BaseNodes.Remove(NodeID);
    }

    public Boolean Allocate(UInt64 RequestedLogicalCoreCount, UInt64 RequestedMemoryMaximum, [NotNullWhen(true)] out NumaSlice? Slice) {
        foreach (KeyValuePair<UInt64, NumaNode> Node in BaseNodes) {
            if (Node.Value.Allocate(RequestedLogicalCoreCount, RequestedMemoryMaximum, out Slice)) {
                return true;
            }
        }

        Slice = null;
        return false;
    }

    public Boolean Free(ref NumaSlice? Slice) {
        if (Slice == null) {
            return false;
        }
        
        if (BaseNodes.TryGetValue(Slice.NumaNode, out NumaNode? Node) == false) {
            return false;
        }

        return Node.Free(ref Slice);
    }
}