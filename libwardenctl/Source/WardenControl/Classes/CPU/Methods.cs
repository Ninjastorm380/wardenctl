namespace WardenControl;

public static partial class CPU {
    static CPU() {
        BasePerCoreUsage = new List<CPUCore>();
        BaseAverageUsage = null!;
    }

    public static void Init(String CPUStatusFolder, Boolean Reset) {
        Span<ValueTuple<Double, Double>> Parsed = stackalloc ValueTuple<Double, Double>[Environment.ProcessorCount + 1];
        ParseString(File.ReadAllText(CoreListPath).Trim(), Parsed);
        BaseAverageUsage = new CPUCore(Parsed[0].Item1, Parsed[0].Item2, $"{CPUStatusFolder}/A", Reset); for (Int32 Index = 0; Index < Environment.ProcessorCount; Index++) {
            BasePerCoreUsage.Add(new CPUCore(Parsed[1 + Index].Item1, Parsed[1 + Index].Item2,$"{CPUStatusFolder}/{Index}", Reset));
        }
    }

    public static void Close() {
        BaseAverageUsage.Dispose();
        foreach (CPUCore Core in BasePerCoreUsage) {
            Core.Dispose();
        }
    }

    public static Int32[] Assign(Int32[] LogicalCores) {
        lock (BasePerCoreUsage) {
            Int32[] PhysicalCores = new Int32[LogicalCores.Length];
            do {
                for (Int32 Offset = 0; Offset < Environment.ProcessorCount - LogicalCores.Length; Offset++) {
                    Boolean Assignable = true;
                    for (Int32 Index = 0; Index < LogicalCores.Length; Index++) {
                        if (BasePerCoreUsage[Offset + LogicalCores[Index]].Assigned == true) {
                            Assignable = false;
                        }
                    }

                    if (Assignable == true) {
                        for (Int32 Index = 0; Index < LogicalCores.Length; Index++) {
                            BasePerCoreUsage[Offset + LogicalCores[Index]].Assigned = true;
                            PhysicalCores[Index] = Offset + LogicalCores[Index];
                        }
                        return PhysicalCores;
                    }
                }
            } while (true);
        }
    }
    
    public static Boolean CanAssign(Int32[] CurrentPhysicalCores, Int32[] LogicalCores) {
        lock (BasePerCoreUsage) {
            Boolean Assignable = false;

            for (Int32 Offset = 0; Offset < Environment.ProcessorCount - LogicalCores.Length; Offset++) {
                Assignable = true;
                for (Int32 Index = 0; Index < LogicalCores.Length; Index++) {
                    if (BasePerCoreUsage[Offset + LogicalCores[Index]].Assigned == true) {
                        if (CurrentPhysicalCores.Contains(Offset + LogicalCores[Index]) == false) {
                            Assignable = false;
                        }
                    }
                }
            }
            
            return Assignable;
        }
    }
    
    
    public static Int32[] Reassign(Int32[] OldPhysicalCores, Int32[] NewLogicalCores) {
        lock (BasePerCoreUsage) {
            for (Int32 Index = 0; Index < OldPhysicalCores.Length; Index++) {
                BasePerCoreUsage[OldPhysicalCores[Index]].Assigned = false;
            }
        
            Int32[] PhysicalCores = new Int32[NewLogicalCores.Length];
            do {
                for (Int32 Offset = 0; Offset < Environment.ProcessorCount - NewLogicalCores.Length; Offset++) {
                    Boolean Assignable = true;
                    for (Int32 Index = 0; Index < NewLogicalCores.Length; Index++) {
                        if (BasePerCoreUsage[Offset + NewLogicalCores[Index]].Assigned == true) {
                            Assignable = false;
                        }
                    }

                    if (Assignable == true) {
                        for (Int32 Index = 0; Index < NewLogicalCores.Length; Index++) {
                            BasePerCoreUsage[Offset + NewLogicalCores[Index]].Assigned = true;
                            PhysicalCores[Index] = Offset + NewLogicalCores[Index];
                        }
                        return PhysicalCores;
                    }
                }
            } while (true);
        }

        
        

    }
    
    public static void Unassign(Int32[] PhysicalCores) {
        lock (BasePerCoreUsage) {
            for (Int32 Index = 0; Index < PhysicalCores.Length; Index++) {
                BasePerCoreUsage[PhysicalCores[Index]].Assigned = false;
            }
        }
    }

    public static void Update() {
        Span<ValueTuple<Double, Double>> Parsed = stackalloc ValueTuple<Double, Double>[Environment.ProcessorCount + 1];
        ParseString(File.ReadAllText(CoreListPath).Trim(), Parsed);

        BaseAverageUsage.Update(Parsed[0].Item1, Parsed[0].Item2);
        for (Int32 Index = 0; Index < Environment.ProcessorCount; Index++) {
            BasePerCoreUsage[Index].Update(Parsed[1 + Index].Item1, Parsed[1 + Index].Item2);
        }
    }

    public static ValueTuple<Double, Double> Usage(Int32 Index) {
        return new ValueTuple<Double, Double>(BasePerCoreUsage[Index].TotalTelta, BasePerCoreUsage[Index].WorkingDelta);
    }
    public static ValueTuple<Double, Double> Usage() {
        return new ValueTuple<Double, Double>(BaseAverageUsage.TotalTelta, BaseAverageUsage.WorkingDelta);
    }
    
    public static Double UsagePercent(Int32 Index) {
        return BasePerCoreUsage[Index].Usage;
    }
    public static Double UsagePercent() {
        return BaseAverageUsage.Usage;
    }

    
    private static void ParseString(String Value, Span<ValueTuple<Double, Double>> Parsed) {
        Span<Range> Rows = stackalloc Range[Parsed.Length];
        Span<Range> Columns = stackalloc Range[11];
        
        ReadOnlySpan<Char> ValueSpan = Value.AsSpan();
        Int32 Index = 0;

        ValueSpan.Split(Rows, Environment.NewLine.AsSpan(), StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        foreach (Range RowRange in Rows) {
            ReadOnlySpan<Char> Row = ValueSpan.Slice(RowRange.Start.Value, RowRange.End.Value - RowRange.Start.Value);
            Row.Split(Columns, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Range Column00 = Columns[00];
            Range Column01 = Columns[01];
            Range Column02 = Columns[02];
            Range Column03 = Columns[03];
            Range Column04 = Columns[04];
            Range Column05 = Columns[05];
            Range Column06 = Columns[06];
            Range Column07 = Columns[07];
            Range Column08 = Columns[08];
            Range Column09 = Columns[09];
            Range Column10 = Columns[10];

            ReadOnlySpan<Char> RawName = Row.Slice(Column00.Start.Value, Column00.End.Value - Column00.Start.Value).Trim();
            ReadOnlySpan<Char> RawUser = Row.Slice(Column01.Start.Value, Column01.End.Value - Column01.Start.Value).Trim();
            ReadOnlySpan<Char> RawNice = Row.Slice(Column02.Start.Value, Column02.End.Value - Column02.Start.Value).Trim();
            ReadOnlySpan<Char> RawSystem = Row.Slice(Column03.Start.Value, Column03.End.Value - Column03.Start.Value).Trim();
            ReadOnlySpan<Char> RawIdle = Row.Slice(Column04.Start.Value, Column04.End.Value - Column04.Start.Value).Trim();
            ReadOnlySpan<Char> RawIOWait = Row.Slice(Column05.Start.Value, Column05.End.Value - Column05.Start.Value).Trim();
            ReadOnlySpan<Char> RawIRQ = Row.Slice(Column06.Start.Value, Column06.End.Value - Column06.Start.Value).Trim();
            ReadOnlySpan<Char> RawSoftIRQ = Row.Slice(Column07.Start.Value, Column07.End.Value - Column07.Start.Value).Trim();
            ReadOnlySpan<Char> RawSteal = Row.Slice(Column08.Start.Value, Column08.End.Value - Column08.Start.Value).Trim();
            ReadOnlySpan<Char> RawGuest = Row.Slice(Column09.Start.Value, Column09.End.Value - Column09.Start.Value).Trim();
            ReadOnlySpan<Char> RawGuestNice = Row.Slice(Column10.Start.Value, Column10.End.Value - Column10.Start.Value).Trim();

            if (Double.TryParse(RawUser, out Double User) == false) {
                
            }
            if (Double.TryParse(RawNice, out Double Nice) == false) {
                
            }
            if (Double.TryParse(RawSystem, out Double System) == false) {
                
            }
            if (Double.TryParse(RawIdle, out Double Idle) == false) {
                
            }
            if (Double.TryParse(RawIOWait, out Double IOWait) == false) {
                
            }
            if (Double.TryParse(RawIRQ, out Double IRQ) == false) {
                
            }
            if (Double.TryParse(RawSoftIRQ, out Double SoftIRQ) == false) {
                
            }
            if (Double.TryParse(RawSteal, out Double Steal) == false) {
                
            }
            if (Double.TryParse(RawGuest, out Double Guest) == false) {
                
            }
            if (Double.TryParse(RawGuestNice, out Double GuestNice) == false) {
                
            }

            Double Total = User + Nice + System + IOWait + IRQ + SoftIRQ + Steal + Guest + GuestNice + Idle;
            Double Working = User + Nice + System + IOWait + IRQ + SoftIRQ + Steal + Guest + GuestNice;

            Parsed[Index] = new ValueTuple<Double, Double>(Total, Working);
            
            Index += 1;
        }
    }
}