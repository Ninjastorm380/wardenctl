using System.Diagnostics.CodeAnalysis;

namespace WardenControl;

public partial class NumaNode {
        public NumaNode(String NodePath, UInt64 CoresReserved, UInt64 MemoryReserved) {
        String Name = Path.GetFileName(NodePath);
        BaseNodeID = UInt64.Parse(Name.AsSpan(4));
        
        Span<Range> Ranges = stackalloc Range[5];
        const Char Seperator = ' ';
        
        
        BaseMemoryReserved = MemoryReserved;
        foreach (String Line in File.ReadLines(Path.Combine(NodePath, "meminfo"))) {
            if (Line.Contains("MemTotal:", StringComparison.Ordinal)) {
                ReadOnlySpan<Char> Span = Line.Trim().AsSpan();
                Span.Split(Ranges, Seperator, StringSplitOptions.RemoveEmptyEntries);
                ReadOnlySpan<Char> Sliced = Span.Slice(Ranges[3].Start.Value, Ranges[3].End.Value - Ranges[3].Start.Value);
                BaseMemoryMaximum = UInt64.Parse(Sliced) * 1024UL;
                break;
            }
        }
        
        this.BaseCoresReserved = CoresReserved;
        BaseCoreMap = new Dictionary<UInt64, NumaCore>(1024);
        String CoreListRaw = File.ReadAllText(Path.Combine(NodePath, "cpulist")).Trim();
        UInt64[] Cores = GetUInt64Array(CoreListRaw);
        for (UInt64 Index = 0; Index < (UInt64)Cores.LongLength; Index++) {
            NumaCore Core = new NumaCore(Index, Cores[Index]);
            BaseCoreMap.Add(Index, Core);
        }

        BaseCoresMaximum = (UInt64)BaseCoreMap.Count;

        for (UInt64 Counter = 0; Counter < CoresReserved; Counter++) {
            BaseCoreMap.Remove(BaseCoreMap.First().Key);
        }

        BaseMemoryCurrent = 0;
        BaseCoresCurrent = 0;
    }
    
    public NumaNode(UInt64 NodeID, NumaCore[] Cores, UInt64 CoresReserved, UInt64 MemoryMaximum, UInt64 MemoryReserved) {
        this.BaseNodeID = NodeID;
        
        BaseMemoryReserved = MemoryReserved;
        BaseMemoryMaximum = MemoryMaximum;

        
        BaseCoresReserved = CoresReserved;
        BaseCoreMap = new Dictionary<UInt64, NumaCore>(1024);
        for (UInt64 Index = 0; Index < (UInt64)Cores.LongLength; Index++) {
            NumaCore Core = Cores[Index];
            BaseCoreMap.Add(Index, Core);
        }

        this.BaseCoresMaximum = (UInt64)BaseCoreMap.Count;

        for (UInt64 Counter = 0; Counter < CoresReserved; Counter++) {
            BaseCoreMap.Remove(BaseCoreMap.First().Key);
        }
    }
    
    private static UInt64[] GetUInt64Array(String Value) {
        String[] Parseables = Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (Parseables.Length == 0) {
            return [];
        }

        List<UInt64> ArrayValues = [];
        foreach (String Parseable in Parseables) {
            if (UInt64.TryParse(Parseable.Trim(), out UInt64 Selected) == true) {
                ArrayValues.Add(Selected);
            }
            else {
                String[] SubParseables = Parseable.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (SubParseables.Length != 2) {
                    continue;
                }

                if (UInt64.TryParse(SubParseables[0].Trim(), out UInt64 Start) == false || UInt64.TryParse(SubParseables[1].Trim(), out UInt64 End) == false) {
                    continue;
                }

                for (UInt64 Core = Start; Core <= End; Core++) {
                    ArrayValues.Add(Core);
                }
            }
        }

        return ArrayValues.ToArray();
    }

    public Boolean Allocate(UInt64 RequestedCoreCount, UInt64 RequestedMemory, [NotNullWhen(true)] out NumaSlice? Slice) {
        if (RequestedMemory <= BaseMemoryMaximum - BaseMemoryCurrent - BaseMemoryReserved && RequestedCoreCount <= BaseCoresMaximum - BaseCoresCurrent - BaseCoresReserved) {
            NumaCore[] Cores = BaseCoreMap.Where(Pair => {
                return Pair.Value.Assigned == false;
            }).Take((Int32)RequestedCoreCount).Select(Pair => {
                return Pair.Value;
            }).ToArray();

            foreach (NumaCore Core in Cores) {
                Core.Assigned = true;
            }

            BaseCoresCurrent += RequestedCoreCount;
            BaseMemoryCurrent += RequestedMemory;

            Slice = new NumaSlice(Cores, RequestedMemory, NodeID);

            return true;
        }

        Slice = null;
        return false;
    }
    
    public Boolean Free(ref NumaSlice? Slice) {
        if (Slice == null) {
            return false;
        }
        
        if (NodeID != Slice.NumaNode) {
            return false;
        }
        
        foreach (NumaCore Core in Slice.AssignedCores) {
            Core.Assigned = false;
        }

        BaseCoresCurrent -= (UInt64)Slice.AssignedCores.LongLength;
        BaseMemoryCurrent -= Slice.AssignedMemory;

        Slice = null;
        return true;
    }
}