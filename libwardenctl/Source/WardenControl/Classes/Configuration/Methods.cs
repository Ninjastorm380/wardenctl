using System.Globalization;
using System.Text;
using IniParser;
using IniParser.Model;

namespace WardenControl;

public partial class Configuration {
    public Configuration() {
        BaseEnabled                  = BaseDefaultEnabled;
        BaseLocked                   = BaseDefaultLocked;
        BaseMaximumMemoryCapacity    = BaseDefaultMaximumMemoryCapacity;
        BaseMaximumNetworkReadSpeed  = BaseDefaultMaximumNetworkReadSpeed;
        BaseMaximumNetworkWriteSpeed = BaseDefaultMaximumNetworkWriteSpeed;
        BaseMaximumStorageCapacity   = BaseDefaultMaximumStorageCapacity;
        BaseMaximumStorageReadIOPS   = BaseDefaultMaximumStorageReadIOPS;
        BaseMaximumStorageWriteIOPS  = BaseDefaultMaximumStorageWriteIOPS;
        BaseMaximumStorageReadSpeed  = BaseDefaultMaximumStorageReadSpeed;
        BaseMaximumStorageWriteSpeed = BaseDefaultMaximumStorageWriteSpeed;
        BaseLogicalCoreCount         = BaseDefaultLogicalCoreCount;

        BaseIPv4Addresses = BaseDefaultIPv4Addresses;
        BaseIPv4Gateway   = BaseDefaultIPv4Gateway;
        BaseIPv6Addresses = BaseDefaultIPv6Addresses;
        BaseIPv6Gateway   = BaseDefaultIPv6Gateway;
    }

    public static Configuration Load(String FilePath) {
        Configuration     Configuration = new Configuration();
        FileIniDataParser Parser        = new FileIniDataParser();
        IniData           Data          = Parser.ReadFile(FilePath);

        const String Section = "Server"; if (Data.Sections.Contains(Section) == false) {
            Data.Sections.Add(Section);
        }
        
        Configuration.Enabled                  = DefensiveParse(Data, Section, "Enabled",                  BaseDefaultEnabled);
        Configuration.Locked                   = DefensiveParse(Data, Section, "Locked",                   BaseDefaultLocked);
        Configuration.MaximumMemoryCapacity    = DefensiveParse(Data, Section, "MaximumMemoryCapacity",    BaseDefaultMaximumMemoryCapacity);
        Configuration.MaximumStorageCapacity   = DefensiveParse(Data, Section, "MaximumStorageCapacity",   BaseDefaultMaximumStorageCapacity);
        Configuration.MaximumStorageReadSpeed  = DefensiveParse(Data, Section, "MaximumStorageReadSpeed",  BaseDefaultMaximumStorageReadSpeed);
        Configuration.MaximumStorageWriteSpeed = DefensiveParse(Data, Section, "MaximumStorageWriteSpeed", BaseDefaultMaximumStorageWriteSpeed);
        Configuration.MaximumStorageReadIOPS   = DefensiveParse(Data, Section, "MaximumStorageReadIOPS",   BaseDefaultMaximumStorageReadIOPS);
        Configuration.MaximumStorageWriteIOPS  = DefensiveParse(Data, Section, "MaximumStorageWriteIOPS",  BaseDefaultMaximumStorageWriteIOPS);
        Configuration.MaximumNetworkReadSpeed  = DefensiveParse(Data, Section, "MaximumNetworkReadSpeed",  BaseDefaultMaximumNetworkReadSpeed);
        Configuration.MaximumNetworkWriteSpeed = DefensiveParse(Data, Section, "MaximumNetworkWriteSpeed", BaseDefaultMaximumNetworkWriteSpeed);
        Configuration.LogicalCoreCount         = DefensiveParse(Data, Section, "LogicalCoreCount",         BaseDefaultLogicalCoreCount);
        Configuration.IPv4Addresses            = DefensiveParse(Data, Section, "IPv4Addresses",            BaseDefaultIPv4Addresses);
        Configuration.IPv4Gateway              = DefensiveParse(Data, Section, "IPv4Gateway",              BaseDefaultIPv4Gateway);
        Configuration.IPv6Addresses            = DefensiveParse(Data, Section, "IPv6Addresses",            BaseDefaultIPv6Addresses);
        Configuration.IPv6Gateway              = DefensiveParse(Data, Section, "IPv6Gateway",              BaseDefaultIPv6Gateway);

        Parser.WriteFile(FilePath, Data);
        
        return Configuration;
    }

    public void Save(String FilePath) {
        FileIniDataParser Parser        = new FileIniDataParser();
        IniData           Data          = Parser.ReadFile(FilePath);
        
        const String Section = "Server"; if (Data.Sections.Contains(Section) == false) {
            Data.Sections.Add(Section);
        }

        DefensiveStore(Data, Section, "Enabled",                  BaseEnabled);
        DefensiveStore(Data, Section, "Locked",                   BaseLocked);
        DefensiveStore(Data, Section, "MaximumMemoryCapacity",    BaseMaximumMemoryCapacity);
        DefensiveStore(Data, Section, "MaximumStorageCapacity",   BaseMaximumStorageCapacity);
        DefensiveStore(Data, Section, "MaximumStorageReadSpeed",  BaseMaximumStorageReadSpeed);
        DefensiveStore(Data, Section, "MaximumStorageWriteSpeed", BaseMaximumStorageWriteSpeed);
        DefensiveStore(Data, Section, "MaximumStorageReadIOPS",   BaseMaximumStorageReadIOPS);
        DefensiveStore(Data, Section, "MaximumStorageWriteIOPS",  BaseMaximumStorageWriteIOPS);
        DefensiveStore(Data, Section, "MaximumNetworkReadSpeed",  BaseMaximumNetworkReadSpeed);
        DefensiveStore(Data, Section, "MaximumNetworkWriteSpeed", BaseMaximumNetworkWriteSpeed);
        DefensiveStore(Data, Section, "LogicalCoreCount",         BaseLogicalCoreCount);
        DefensiveStore(Data, Section, "IPv4Addresses",            BaseIPv4Addresses);
        DefensiveStore(Data, Section, "IPv4Gateway",              BaseIPv4Gateway);
        DefensiveStore(Data, Section, "IPv6Addresses",            BaseIPv6Addresses);
        DefensiveStore(Data, Section, "IPv6Gateway",              BaseIPv6Gateway);
        
        Parser.WriteFile(FilePath, Data);
    }
    
    private static void DefensiveStore<T>(IniData Data, String Section, String Key, T   Value) where T : IParsable<T> {
        if (Data[Section].ContainsKey(Key) == false) {
            Data[Section].Add(Key, Convert(Value));
        }
        else {
            Data[Section][Key] = Convert(Value);
        }
    }
    private static void DefensiveStore<T>(IniData Data, String Section, String Key, T[] Value) where T : IParsable<T> {
        if (Data[Section].ContainsKey(Key) == false) {
            Data[Section].Add(Key, Convert(Value));
        }
        else {
            Data[Section][Key] = Convert(Value);
        }
    }
    
    private static T   DefensiveParse<T>(IniData Data, String Section, String Key, T   Default) where T : IParsable<T> {
        if (Data[Section].ContainsKey(Key) == false) {
            Data[Section].Add(Key, Convert(Default));

            return Default;
        }

        Validate(Data[Section][Key], Default, out T Result);
        
        if (Result.Equals(Default) == true) {
            Data[Section][Key] = Convert(Result);
        }

        return Result;
    }
    private static T[] DefensiveParse<T>(IniData Data, String Section, String Key, T[] Default) where T : IParsable<T> {
        if (Data[Section].ContainsKey(Key) == false) {
            Data[Section].Add(Key, Convert(Default));

            return Default;
        }

        Validate(Data[Section][Key], Default, out T[] Result);
        if (Result.Equals(Default) == true) {
            Data[Section][Key] = Convert(Result);
        }

        return Result;
    }

    private static void Validate<T>(String Input, T   Default, out T   Output) where T : IParsable<T> {
        if (T.TryParse(Input, CultureInfo.InvariantCulture, out Output!) == false) {
            Output = Default;
        }
    }
    private static void Validate<T>(String Input, T[] Default, out T[] Output) where T : IParsable<T> {
        String[] RawValues = Input.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        T[] Values = new T[RawValues.Length];

        for (Int32 Index = 0; Index < RawValues.Length; Index++) {
            if (T.TryParse(RawValues[Index], CultureInfo.InvariantCulture, out T? Parsed) == false) {
                Output = Default;
                return;
            }

            Values[Index] = Parsed;
        }

        Output = Values;
    }

    private static String Convert<T>(T   Input) where T : IParsable<T> {
        return Input.ToString()!;
    }
    private static String Convert<T>(T[] Input) where T : IParsable<T> {
        StringBuilder Builder = new StringBuilder();
        Builder.Append(Input[0].ToString());
        for (Int32 Index = 1; Index < Input.Length; Index++) {
            Builder.Append(", ").Append(Input[Index].ToString());
        } return Builder.ToString();
    }
}