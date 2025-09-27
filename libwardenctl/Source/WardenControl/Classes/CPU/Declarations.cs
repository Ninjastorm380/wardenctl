namespace WardenControl;

public static partial class CPU {
    private const String CoreListPath = "/proc/stat";
    
    private static CPUCore BaseAverageUsage;
    private static readonly List<CPUCore> BasePerCoreUsage;

    

}