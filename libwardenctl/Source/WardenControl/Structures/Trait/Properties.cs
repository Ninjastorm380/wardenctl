namespace WardenControl;

public readonly partial struct Trait {
    public UInt64 Maximum {
        get {
            return BaseMaximum;
        }
    }

    public UInt64 Current {
        get {
            return BaseCurrent;
        }
    }

    public Double Relative {
        get {
            return BaseRelative;
        }
    }
}