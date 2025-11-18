using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace WardenControl;

public readonly partial struct Identifier : IEquatable<Identifier>, IParsable<Identifier> {
    public Identifier(UInt128 A, UInt128 B, UInt128 C, UInt128 D) {
        BaseA = A;
        BaseB = B;
        BaseC = C;
        BaseD = D;

        StringBuilder Builder = new StringBuilder(256); Builder
            .Append(Util.ToHex(BaseA00))
            .Append(Util.ToHex(BaseA01))
            .Append(Util.ToHex(BaseA02))
            .Append(Util.ToHex(BaseA03))
            .Append(Util.ToHex(BaseA04))
            .Append(Util.ToHex(BaseA05))
            .Append(Util.ToHex(BaseA06))
            .Append(Util.ToHex(BaseA07))
            .Append(Util.ToHex(BaseA08))
            .Append(Util.ToHex(BaseA09))
            .Append(Util.ToHex(BaseA10))
            .Append(Util.ToHex(BaseA11))
            .Append(Util.ToHex(BaseA12))
            .Append(Util.ToHex(BaseA13))
            .Append(Util.ToHex(BaseA14))
            .Append(Util.ToHex(BaseA15))
            .Append(Util.ToHex(BaseB00))
            .Append(Util.ToHex(BaseB01))
            .Append(Util.ToHex(BaseB02))
            .Append(Util.ToHex(BaseB03))
            .Append(Util.ToHex(BaseB04))
            .Append(Util.ToHex(BaseB05))
            .Append(Util.ToHex(BaseB06))
            .Append(Util.ToHex(BaseB07))
            .Append(Util.ToHex(BaseB08))
            .Append(Util.ToHex(BaseB09))
            .Append(Util.ToHex(BaseB10))
            .Append(Util.ToHex(BaseB11))
            .Append(Util.ToHex(BaseB12))
            .Append(Util.ToHex(BaseB13))
            .Append(Util.ToHex(BaseB14))
            .Append(Util.ToHex(BaseB15))
            .Append(Util.ToHex(BaseC00))
            .Append(Util.ToHex(BaseC01))
            .Append(Util.ToHex(BaseC02))
            .Append(Util.ToHex(BaseC03))
            .Append(Util.ToHex(BaseC04))
            .Append(Util.ToHex(BaseC05))
            .Append(Util.ToHex(BaseC06))
            .Append(Util.ToHex(BaseC07))
            .Append(Util.ToHex(BaseC08))
            .Append(Util.ToHex(BaseC09))
            .Append(Util.ToHex(BaseC10))
            .Append(Util.ToHex(BaseC11))
            .Append(Util.ToHex(BaseC12))
            .Append(Util.ToHex(BaseC13))
            .Append(Util.ToHex(BaseC14))
            .Append(Util.ToHex(BaseC15))
            .Append(Util.ToHex(BaseD00))
            .Append(Util.ToHex(BaseD01))
            .Append(Util.ToHex(BaseD02))
            .Append(Util.ToHex(BaseD03))
            .Append(Util.ToHex(BaseD04))
            .Append(Util.ToHex(BaseD05))
            .Append(Util.ToHex(BaseD06))
            .Append(Util.ToHex(BaseD07))
            .Append(Util.ToHex(BaseD08))
            .Append(Util.ToHex(BaseD09))
            .Append(Util.ToHex(BaseD10))
            .Append(Util.ToHex(BaseD11))
            .Append(Util.ToHex(BaseD12))
            .Append(Util.ToHex(BaseD13))
            .Append(Util.ToHex(BaseD14))
            .Append(Util.ToHex(BaseD15));
        BaseString = Builder.ToString();
    }
    // TODO: test equality
    public Identifier(String HexString) {
        BaseA00 = Util.FromHex(HexString, 000);
        BaseA01 = Util.FromHex(HexString, 002);
        BaseA02 = Util.FromHex(HexString, 004);
        BaseA03 = Util.FromHex(HexString, 006);
        BaseA04 = Util.FromHex(HexString, 008);
        BaseA05 = Util.FromHex(HexString, 010);
        BaseA06 = Util.FromHex(HexString, 012);
        BaseA07 = Util.FromHex(HexString, 014);
        BaseA08 = Util.FromHex(HexString, 016);
        BaseA09 = Util.FromHex(HexString, 018);
        BaseA10 = Util.FromHex(HexString, 020);
        BaseA11 = Util.FromHex(HexString, 022);
        BaseA12 = Util.FromHex(HexString, 024);
        BaseA13 = Util.FromHex(HexString, 026);
        BaseA14 = Util.FromHex(HexString, 028);
        BaseA15 = Util.FromHex(HexString, 030);
        
        BaseB00 = Util.FromHex(HexString, 032);
        BaseB01 = Util.FromHex(HexString, 034);
        BaseB02 = Util.FromHex(HexString, 036);
        BaseB03 = Util.FromHex(HexString, 038);
        BaseB04 = Util.FromHex(HexString, 040);
        BaseB05 = Util.FromHex(HexString, 042);
        BaseB06 = Util.FromHex(HexString, 044);
        BaseB07 = Util.FromHex(HexString, 046);
        BaseB08 = Util.FromHex(HexString, 048);
        BaseB09 = Util.FromHex(HexString, 050);
        BaseB10 = Util.FromHex(HexString, 052);
        BaseB11 = Util.FromHex(HexString, 054);
        BaseB12 = Util.FromHex(HexString, 056);
        BaseB13 = Util.FromHex(HexString, 058);
        BaseB14 = Util.FromHex(HexString, 060);
        BaseB15 = Util.FromHex(HexString, 062);
        
        BaseC00 = Util.FromHex(HexString, 064);
        BaseC01 = Util.FromHex(HexString, 066);
        BaseC02 = Util.FromHex(HexString, 068);
        BaseC03 = Util.FromHex(HexString, 070);
        BaseC04 = Util.FromHex(HexString, 072);
        BaseC05 = Util.FromHex(HexString, 074);
        BaseC06 = Util.FromHex(HexString, 076);
        BaseC07 = Util.FromHex(HexString, 078);
        BaseC08 = Util.FromHex(HexString, 080);
        BaseC09 = Util.FromHex(HexString, 082);
        BaseC10 = Util.FromHex(HexString, 084);
        BaseC11 = Util.FromHex(HexString, 086);
        BaseC12 = Util.FromHex(HexString, 088);
        BaseC13 = Util.FromHex(HexString, 090);
        BaseC14 = Util.FromHex(HexString, 092);
        BaseC15 = Util.FromHex(HexString, 094);
        
        BaseD00 = Util.FromHex(HexString, 096);
        BaseD01 = Util.FromHex(HexString, 098);
        BaseD02 = Util.FromHex(HexString, 100);
        BaseD03 = Util.FromHex(HexString, 102);
        BaseD04 = Util.FromHex(HexString, 104);
        BaseD05 = Util.FromHex(HexString, 106);
        BaseD06 = Util.FromHex(HexString, 108);
        BaseD07 = Util.FromHex(HexString, 110);
        BaseD08 = Util.FromHex(HexString, 112);
        BaseD09 = Util.FromHex(HexString, 114);
        BaseD10 = Util.FromHex(HexString, 116);
        BaseD11 = Util.FromHex(HexString, 118);
        BaseD12 = Util.FromHex(HexString, 120);
        BaseD13 = Util.FromHex(HexString, 122);
        BaseD14 = Util.FromHex(HexString, 124);
        BaseD15 = Util.FromHex(HexString, 126);

        BaseString = HexString;
    }
    
    public Boolean Equals(Identifier Other) {
        return BaseA == Other.BaseA && BaseB == Other.BaseB && BaseC == Other.BaseC && BaseD == Other.BaseD;
    }
    
    public override Boolean Equals(Object? Reference) {
        return Reference is Identifier Other && Equals(Other);
    }

    public override Int32 GetHashCode() {
        return HashCode.Combine(BaseA, BaseB, BaseC, BaseD);
    }

    public static Boolean operator ==(Identifier Left, Identifier Right) {
        return Left.Equals(Right);
    }

    public static Boolean operator !=(Identifier Left, Identifier Right) {
        return !Left.Equals(Right);
    }
    
    public override String ToString() {
        return BaseString;
    }

    public static Identifier Parse(String Input, IFormatProvider? Provider) {
        if (Input.Length % 2 != 0) {
            throw new ArgumentException($"Input must be evenly divisible by two!");
        }

        if (Input.Length < 128) {
            throw new ArgumentException($"Input must be 128 chaacters or more in length!");
        }

        return new Identifier(Input);
    }

    public static Boolean TryParse([NotNullWhen(true)] String? Input, IFormatProvider? Provider, out Identifier Result) {
        if (Input == null) {
            Result = default;
            return false;
        }
        if (Input.Length % 2 != 0) {
            Result = default;
            return false;
        }

        if (Input.Length < 128) {
            Result = default;
            return false;
        }

        Result = new Identifier(Input);
        return true;
    }
}