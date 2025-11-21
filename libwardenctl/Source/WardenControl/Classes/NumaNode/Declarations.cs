namespace WardenControl;

public partial class NumaNode {
    private readonly UInt64 BaseNodeID;
    private readonly UInt64 BaseMemoryMaximum;
    private readonly UInt64 BaseMemoryReserved;
    private UInt64 BaseMemoryCurrent;
    
    private readonly UInt64 BaseCoresMaximum;
    private readonly UInt64 BaseCoresReserved;
    private UInt64 BaseCoresCurrent;
    
    private readonly Dictionary<UInt64, NumaCore> BaseCoreMap;
}