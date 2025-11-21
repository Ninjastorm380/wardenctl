namespace WardenControl;

public partial class NumaSlice {
    public NumaCore[] AssignedCores {
        get {
            return BaseAssignedCores;
        }
    }

    public UInt64 AssignedMemory {
        get {
            return BaseAssignedMemory;
        }
    }

    public UInt64 NumaNode {
        get {
            return BaseNumaNode;
        }
    }
}