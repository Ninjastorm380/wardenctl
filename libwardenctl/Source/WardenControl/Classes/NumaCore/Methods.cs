using System.Text;

namespace WardenControl;

public partial class NumaCore {
    public NumaCore(UInt64 LocalIndex, UInt64 GlobalIndex) {
        BaseLocalIndex = LocalIndex;
        BaseGlobalIndex = GlobalIndex;
        BaseAssigned = false;
        StringBuilder Builder = new StringBuilder();
        Builder.Append("{ Local: ").Append(LocalIndex.ToString("0000")).Append(", Logical: ").Append(GlobalIndex.ToString("0000")).Append(" }");
        BaseTextRepresentation = Builder.ToString();
    }
    
    public override String ToString() {
        return BaseTextRepresentation;
    }
}