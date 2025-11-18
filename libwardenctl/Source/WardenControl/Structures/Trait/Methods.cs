namespace WardenControl;

public readonly partial struct Trait {
    public Trait(UInt64 Current, UInt64 Maximum) {
        BaseCurrent  = Current;
        BaseMaximum  = Maximum;
        BaseRelative = Current / Maximum;
    }
}