namespace WardenControl;

public partial class NumaNode {
    public UInt64 NodeID {
        get {
            return BaseNodeID;
        }
    }

    public UInt64 MemoryMaximum  { 
        get {
            return BaseMemoryMaximum;
        } 
    }
    public UInt64 MemoryCurrent  { 
        get {
            return BaseMemoryCurrent;
        } 
    }
    public UInt64 MemoryReserved { 
        get {
            return BaseMemoryReserved;
        } 
    }
    
    public UInt64 CoresMaximum  { 
        get {
            return BaseCoresMaximum;
        } 
    }
    public UInt64 CoresCurrent  { 
        get {
            return BaseCoresCurrent;
        } 
    }
    public UInt64 CoresReserved { 
        get {
            return BaseCoresReserved;
        } 
    }
}