using System.Runtime.InteropServices;

namespace WardenControl;

[StructLayout(LayoutKind.Explicit)] public readonly partial struct Identifier : IEquatable<Identifier>, IParsable<Identifier> {
    [FieldOffset(0000)] private readonly UInt128 BaseA;
    [FieldOffset(0000)] private readonly Byte BaseA00;
    [FieldOffset(0001)] private readonly Byte BaseA01;
    [FieldOffset(0002)] private readonly Byte BaseA02;
    [FieldOffset(0003)] private readonly Byte BaseA03;
    [FieldOffset(0004)] private readonly Byte BaseA04;
    [FieldOffset(0005)] private readonly Byte BaseA05;
    [FieldOffset(0006)] private readonly Byte BaseA06;
    [FieldOffset(0007)] private readonly Byte BaseA07;
    [FieldOffset(0008)] private readonly Byte BaseA08;
    [FieldOffset(0009)] private readonly Byte BaseA09;
    [FieldOffset(0010)] private readonly Byte BaseA10;
    [FieldOffset(0011)] private readonly Byte BaseA11;
    [FieldOffset(0012)] private readonly Byte BaseA12;
    [FieldOffset(0013)] private readonly Byte BaseA13;
    [FieldOffset(0014)] private readonly Byte BaseA14;
    [FieldOffset(0015)] private readonly Byte BaseA15;
    
    [FieldOffset(0016)] private readonly UInt128 BaseB;
    [FieldOffset(0016)] private readonly Byte BaseB00;
    [FieldOffset(0017)] private readonly Byte BaseB01;
    [FieldOffset(0018)] private readonly Byte BaseB02;
    [FieldOffset(0019)] private readonly Byte BaseB03;
    [FieldOffset(0020)] private readonly Byte BaseB04;
    [FieldOffset(0021)] private readonly Byte BaseB05;
    [FieldOffset(0022)] private readonly Byte BaseB06;
    [FieldOffset(0023)] private readonly Byte BaseB07;
    [FieldOffset(0024)] private readonly Byte BaseB08;
    [FieldOffset(0025)] private readonly Byte BaseB09;
    [FieldOffset(0026)] private readonly Byte BaseB10;
    [FieldOffset(0027)] private readonly Byte BaseB11;
    [FieldOffset(0028)] private readonly Byte BaseB12;
    [FieldOffset(0029)] private readonly Byte BaseB13;
    [FieldOffset(0030)] private readonly Byte BaseB14;
    [FieldOffset(0031)] private readonly Byte BaseB15;
    
    [FieldOffset(0032)] private readonly UInt128 BaseC;
    [FieldOffset(0032)] private readonly Byte BaseC00;
    [FieldOffset(0033)] private readonly Byte BaseC01;
    [FieldOffset(0034)] private readonly Byte BaseC02;
    [FieldOffset(0035)] private readonly Byte BaseC03;
    [FieldOffset(0036)] private readonly Byte BaseC04;
    [FieldOffset(0037)] private readonly Byte BaseC05;
    [FieldOffset(0038)] private readonly Byte BaseC06;
    [FieldOffset(0039)] private readonly Byte BaseC07;
    [FieldOffset(0040)] private readonly Byte BaseC08;
    [FieldOffset(0041)] private readonly Byte BaseC09;
    [FieldOffset(0042)] private readonly Byte BaseC10;
    [FieldOffset(0043)] private readonly Byte BaseC11;
    [FieldOffset(0044)] private readonly Byte BaseC12;
    [FieldOffset(0045)] private readonly Byte BaseC13;
    [FieldOffset(0046)] private readonly Byte BaseC14;
    [FieldOffset(0047)] private readonly Byte BaseC15;
    
    [FieldOffset(0048)] private readonly UInt128 BaseD;
    [FieldOffset(0048)] private readonly Byte BaseD00;
    [FieldOffset(0049)] private readonly Byte BaseD01;
    [FieldOffset(0050)] private readonly Byte BaseD02;
    [FieldOffset(0051)] private readonly Byte BaseD03;
    [FieldOffset(0052)] private readonly Byte BaseD04;
    [FieldOffset(0053)] private readonly Byte BaseD05;
    [FieldOffset(0054)] private readonly Byte BaseD06;
    [FieldOffset(0055)] private readonly Byte BaseD07;
    [FieldOffset(0056)] private readonly Byte BaseD08;
    [FieldOffset(0057)] private readonly Byte BaseD09;
    [FieldOffset(0058)] private readonly Byte BaseD10;
    [FieldOffset(0059)] private readonly Byte BaseD11;
    [FieldOffset(0060)] private readonly Byte BaseD12;
    [FieldOffset(0061)] private readonly Byte BaseD13;
    [FieldOffset(0062)] private readonly Byte BaseD14;
    [FieldOffset(0063)] private readonly Byte BaseD15;
    
    [FieldOffset(0064)] private readonly String BaseString;


    // TODO: implement equality for custom identifier type.
}