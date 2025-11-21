using System.Text;

namespace WardenControl;

public partial class NumaSlice {
    public NumaSlice(NumaCore[] LogicalCores, UInt64 LogicalMemory, UInt64 NumaNode) {
        this.BaseAssignedCores  = LogicalCores;
        this.BaseAssignedMemory = LogicalMemory;
        this.BaseNumaNode       = NumaNode;
        
        StringBuilder Builder = new StringBuilder();
        Builder.Append("{ NumaNode: ").Append(NumaNode.ToString("0000")).Append(", AssignedMemory: ").Append(AssignedMemory).Append("B, AssignedCores: { Core 0: ").Append(AssignedCores[0]);
        
        for (Int32 Index = 1; Index < AssignedCores.Length; Index++) {
            Builder.Append(", Core ").Append(Index).Append(": ").Append(AssignedCores[Index]);
        }

        Builder.Append(" } }");

        BaseTextRepresentation = Builder.ToString();
    }
    
    public override String ToString() {
        return BaseTextRepresentation;
    }
}