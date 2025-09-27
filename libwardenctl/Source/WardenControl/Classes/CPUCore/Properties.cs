namespace WardenControl;

public partial class CPUCore {
    public Double TotalTelta {
        get {
            return BaseTotalDelta;
        }
    }
    
    public Double WorkingDelta {
        get {
            return BaseWorkingDelta;
        }
    }
    
    public Double Usage {
        get {
            return BaseUsage;
        }
    }
    
    public Boolean Assigned {
        get {
            return BaseAssigned.Value;
        }
        set {
            BaseAssigned.Value = value;
        }
    }
}