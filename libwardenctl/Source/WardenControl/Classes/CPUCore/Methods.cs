namespace WardenControl;

public partial class CPUCore : IDisposable {
    public CPUCore(Double Total, Double Working, String AssignedStatusFile, Boolean Reset) {
        BaseAssigned = new Surface<Boolean>(AssignedStatusFile, false);

        if (Reset == true) {
            BaseAssigned.Reset();
        }
        
        BasePreviousTotal = Total;
        BasePreviousWorking = Working;
        BasePresent = DateTime.UtcNow;
        BasePast = BasePresent;

        BaseTotalDelta = 0;
        BaseWorkingDelta = 0;
        BaseUsage = 0;
    }

    public void Update(Double Total, Double Working) {
        BasePast = BasePresent;
        BasePresent = DateTime.UtcNow;
        TimeSpan Interval = BasePresent.Subtract(BasePast);

        BaseTotalDelta = (Total - BasePreviousTotal) * (1000.0 / Interval.TotalMilliseconds);
        BaseWorkingDelta = (Working - BasePreviousWorking) * (1000.0 / Interval.TotalMilliseconds);
        BaseUsage = BaseWorkingDelta / BaseTotalDelta;
        
        BasePreviousTotal = Total;
        BasePreviousWorking = Working;
    }

    public void Dispose() {
        BaseAssigned.Dispose();
    }
}