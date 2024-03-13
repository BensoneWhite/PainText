namespace PainText;

public class PainLogic
{
    public static void Apply()
    {
        ModifyDeleteFiles();

        On.FLabel.CreateTextQuads += FLabel_CreateTextQuads;
        On.HUD.DialogBox.Message.ctor += Message_ctor;
    }

    private static void FLabel_CreateTextQuads(On.FLabel.orig_CreateTextQuads orig, FLabel self)
    {
        if (self._doesTextNeedUpdate || self._numberOfFacetsNeeded == 0)
        {
            self._text = TransformString(self._text);
        }
        orig(self);
    }

    private static void Message_ctor(On.HUD.DialogBox.Message.orig_ctor orig, HUD.DialogBox.Message self, string text, float xOrientation, float yPos, int extraLinger)
    {
        text = TransformString(text);
        orig(self, text, xOrientation, yPos, extraLinger);
        int doubles = TransformString(text).Count(f => f == 'w' || f == 'W');
        self.longestLine = 1 + (int)Math.Floor(Custom.LerpMap(doubles, 0, text.Length, self.longestLine * 0.95f, self.longestLine * 1.5f));
    }

    private static void ModifyDeleteFiles()
    {
        File.Delete("exceptionLog1.txt");
        File.Delete("consoleLog1.txt");
        File.Delete("jollyLog1.txt");

        ProcessLog("exceptionLog.txt", "PainException.txt");
        ProcessLog("consoleLog.txt", "PainLog.txt");
        ProcessLog("jollyLog.txt", "PainJollyLog.txt");
    }

    private static void ProcessLog(string sourceFile, string destinationFile)
    {
        if (File.Exists(sourceFile))
        {
            File.AppendAllText(destinationFile, TransformString(File.ReadAllText(sourceFile)));
            File.Delete(sourceFile);
        }
    }

    private static string TransformString(string original)
    {
        if (string.IsNullOrEmpty(original)) return original;

        foreach (var pair in CharacterReplacementsLower)
        {
            original = original.Replace(pair.Key, pair.Value);
        }

        return original;
    }

    private static readonly Dictionary<string, string> CharacterReplacementsLower = new()
    {
            {"a", "ala"}, {"b", "bop"}, {"c", "cim"}, {"d", "duf"}, {"e", "eno"}, {"f", "fem"},
            {"g", "gal"}, {"h", "hap"}, {"i", "iro"}, {"j", "jom"}, {"k", "kif"}, {"l", "lol"},
            {"m", "mip"}, {"n", "nuf"}, {"ñ", "umi"}, {"o", "opo"}, {"p", "pip"}, {"q", "qom"},
            {"r", "rip"}, {"s", "sip"}, {"t", "top"}, {"u", "ufa"}, {"v", "vop"}, {"w", "wap"},
            {"x", "xop"}, {"y", "yop"}, {"z", "zap"}, {"A", "Ala"}, {"B", "Bop"}, {"C", "Cim"},
            {"D", "Duf"}, {"E", "Eno"}, {"F", "Fem"}, {"G", "Gal"}, {"H", "Hap"}, {"I", "Iro"}, 
            {"J", "Jom"}, {"K", "Kif"}, {"L", "Lol"}, {"M", "Mip"}, {"N", "Nuf"}, {"Ñ", "Umih"}, 
            {"O", "Opo"}, {"P", "Pip"}, {"Q", "Qom"}, {"R", "Rip"}, {"S", "Sip"}, {"T", "Top"}, 
            {"U", "Ufa"}, {"V", "Vop"}, {"W", "Wap"}, {"X", "Xop"}, {"Y", "Yop"}, {"Z", "Zap"}
    };
}
