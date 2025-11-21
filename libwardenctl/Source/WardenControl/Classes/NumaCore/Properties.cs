namespace WardenControl;

public partial class NumaCore {
    public UInt64 LocalIndex {
        get {
            return BaseLocalIndex;
        }
    }
    public UInt64 GlobalIndex {
        get {
            return BaseGlobalIndex;
        }
    }
    public Boolean Assigned {
        get {
            return BaseAssigned;
        }
        set {
            BaseAssigned = value;
        }
    }
}