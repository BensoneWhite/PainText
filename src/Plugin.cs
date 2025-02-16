namespace PainText;

[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class PainText : BaseUnityPlugin
{
    public const string MOD_ID = "PainText";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "PainText";
    public const string VERSION = "0.0.5";

    public bool IsInit;

    public new static ManualLogSource Logger;

    public static void DebugLog(object ex) => Logger.LogInfo(ex);
    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);
    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

    private static readonly Dictionary<string, string> transformedTextCache = new();

    private static readonly Dictionary<string, string> CharReplacements = new()
    {
        { "a", "ala" },
        { "b", "bop" },
        { "c", "cim" },
        { "d", "duf" },
        { "e", "eno" },
        { "f", "fem" },
        { "g", "gal" },
        { "h", "hap" },
        { "i", "iro" },
        { "j", "jom" },
        { "k", "kif" },
        { "l", "lol" },
        { "m", "mip" },
        { "n", "nuf" },
        { "ñ", "umi" },
        { "o", "opo" },
        { "p", "pip" },
        { "q", "qom" },
        { "r", "rip" },
        { "s", "sip" },
        { "t", "top" },
        { "u", "ufa" },
        { "v", "vop" },
        { "w", "wap" },
        { "x", "xop" },
        { "y", "yop" },
        { "z", "zap" },
        { "A", "Ala" },
        { "B", "Bop" },
        { "C", "Cim" },
        { "D", "Duf" },
        { "E", "Eno" },
        { "F", "Fem" },
        { "H", "Hap" },
        { "I", "Iro" },
        { "J", "Jom" },
        { "K", "Kif" },
        { "L", "Lol" },
        { "M", "Mip" },
        { "N", "Nuf" },
        { "Ñ", "Umih" },
        { "O", "Opo" },
        { "P", "Pip" },
        { "Q", "Qom" },
        { "R", "Rip" },
        { "S", "Sip" },
        { "T", "Top" },
        { "U", "Ufa" },
        { "V", "Vop" },
        { "W", "Wap" },
        { "X", "Xop" },
        { "Y", "Yop" },
        { "Z", "Zap" }
    };

    private static bool _Debug = false;

    public void OnEnable()
    {
        Logger = base.Logger;
        DebugWarning($"{MOD_NAME} is loading.... {VERSION}");
        On.RainWorld.OnModsInit += ModsInitHook;
    }

    private void ModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            IsInit = true;

            On.FLabel.CreateTextQuads += TextQuadsHook;
            On.HUD.DialogBox.Message.ctor += DialogBoxHook;

            if(_Debug) On.Menu.Remix.MixedUI.LabelTest.TrimText += LabelTest_TrimText;

            File.Delete("exceptionLog.txt");
            File.Delete("consoleLog.txt");

            StringBuilder stringBuilder = new();
            IntPtr activeWindow = GetActiveWindow();
            GetWindowText(activeWindow, stringBuilder, stringBuilder.Capacity);
            SetWindowText(activeWindow, TransformText(stringBuilder.ToString()));

            IL.RainWorld.HandleLog += HandleLogHook;
            On.RainWorld.HandleLog += HandleLogHook1;
            if (File.Exists("exceptionLog.txt"))
            {
                File.AppendAllText("exceptionLog.txt", TransformText(File.ReadAllText("exceptionLog.txt")));
                File.Delete("exceptionLog.txt");
            }
            if (File.Exists("consoleLog.txt"))
            {
                File.AppendAllText("consoleLog.txt", TransformText(File.ReadAllText("consoleLog.txt")));
                File.Delete("consoleLog.txt");
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private string LabelTest_TrimText(On.Menu.Remix.MixedUI.LabelTest.orig_TrimText orig, string text, float width, bool addDots, bool bigText)
    {
        DebugWarning(orig(text, width, addDots, bigText));

        return orig(text, width, addDots, bigText);
    }

    private void HandleLogHook1(On.RainWorld.orig_HandleLog orig, RainWorld rw, string logString, string stackTrace, LogType logType)
    {
        try
        {
            orig(rw, TransformText(logString), TransformText(stackTrace), logType);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
    }

    private void HandleLogHook(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            while (c.TryGotoNext(new Func<Instruction, bool>[] { i => ILPatternMatchingExt.MatchLdstr(i, "exceptionLog.txt") }))
            {
                c.Next.Operand = "ifeisaticoifealpaftitiaocintQalaocgam.txt";
            }
            c.Goto(0, 0, false);
            while (c.TryGotoNext(new Func<Instruction, bool>[] { i => ILPatternMatchingExt.MatchLdstr(i, "consoleLog.txt") }))
            {
                c.Next.Operand = "IcohaocintimsaocqalifeQalaocgam.txt";
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
    }

    private static string TransformText(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string originalInput = input;
        if (transformedTextCache.ContainsKey(originalInput))
            return transformedTextCache[originalInput];

        foreach (KeyValuePair<string, string> replacement in CharReplacements)
        {
            input = input.Replace(replacement.Key, replacement.Value);
        }

        if (input.Length > 100)
        {
            input = input.Substring(0, 100);
        }

        transformedTextCache[originalInput] = input;
        return input;
    }


    private void DialogBoxHook(On.HUD.DialogBox.Message.orig_ctor orig, HUD.DialogBox.Message message, string text, float xPos, float yPos, int line)
    {
        try
        {
            text = TransformText(text);
            orig(message, text, xPos, yPos, line);
            message.longestLine = 1 + (int)Math.Floor(
                Custom.LerpMap(
                    (float)TransformText(text).Count(f => f == 'w' || f == 'W'),
                    0f,
                    (float)text.Length,
                    message.longestLine * 0.95f,
                    message.longestLine * 1.5f)
            );
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
    }

    private void TextQuadsHook(On.FLabel.orig_CreateTextQuads orig, FLabel label)
    {
        try
        {
            if (label._doesTextNeedUpdate || label._numberOfFacetsNeeded == 0)
            {
                label._text = TransformText(label._text);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
        orig(label);
    }
}