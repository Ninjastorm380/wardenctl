using System.Text;

namespace WardenControl;

public static class Parsing {
    public static String GetString(Int32[] Value) {
        StringBuilder Builder = new StringBuilder();
        foreach (Int32 Core in Value) {
            if (Core != Value[^1]) {
                Builder.Append(Core).Append(',');
            }
            else {
                Builder.Append(Core);
            }
        }

        return Builder.ToString();
    }

    public static Int32[] GetInt32Array(String Value) {
        String[] Parseables = Value.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        if (Parseables.Length == 0) {
            return [];
        }

        List<Int32> ArrayValues = [];
        foreach (String Parseable in Parseables) {
            if (Int32.TryParse(Parseable.Trim(), out Int32 Selected) == true) {
                ArrayValues.Add(Selected);
            }
            else {
                String[] SubParseables = Parseable.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (SubParseables.Length != 2) {
                    continue;
                }

                if (Int32.TryParse(SubParseables[0].Trim(), out Int32 Start) == false || Int32.TryParse(SubParseables[1].Trim(), out Int32 End) == false) {
                    continue;
                }

                for (Int32 Core = Start; Core <= End; Core++) {
                    ArrayValues.Add(Core);
                }
            }
        }

        return ArrayValues.ToArray();
    }
}