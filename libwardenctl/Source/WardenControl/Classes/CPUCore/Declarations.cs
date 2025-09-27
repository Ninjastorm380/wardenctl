namespace WardenControl;

public partial class CPUCore {
    private Surface<Boolean> BaseAssigned;
    private Double BasePreviousTotal;
    private Double BasePreviousWorking;

    private Double BaseTotalDelta;
    private Double BaseWorkingDelta;
    private Double BaseUsage;

    private DateTime BasePresent;
    private DateTime BasePast;
}