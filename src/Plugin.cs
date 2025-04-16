namespace PainText;

[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]
public class PainText : BaseUnityPlugin
{
    public const string MOD_ID = "Paintext";
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "PainText";
    public const string VERSION = "1.1.0";

    public bool IsInit;

    public new static ManualLogSource Logger;

    public RemixMenu remixMenuInstance;

    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

    //Caching text, so it's not calculated every time user moves from UI's labels
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

            On.Menu.Remix.MixedUI.LabelTest.TrimText += LabelTest_TrimText;

            StringBuilder stringBuilder = new();
            IntPtr activeWindow = GetActiveWindow();
            GetWindowText(activeWindow, stringBuilder, stringBuilder.Capacity);
            SetWindowText(activeWindow, TransformText(stringBuilder.ToString()));

            On.ProcessManager.RequestMainProcessSwitch_ProcessID += ProcessManager_RequestMainProcessSwitch_ProcessID;

            MachineConnector.SetRegisteredOI(MOD_ID, remixMenuInstance = new RemixMenu());
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void ProcessManager_RequestMainProcessSwitch_ProcessID(On.ProcessManager.orig_RequestMainProcessSwitch_ProcessID orig, ProcessManager self, ProcessManager.ProcessID ID)
    {
        if(ID == ProcessManager.ProcessID.Game)
        {
            transformedTextCache.Clear();
        }
        orig(self, ID);
    }

    private string LabelTest_TrimText(On.Menu.Remix.MixedUI.LabelTest.orig_TrimText orig, string text, float width, bool addDots, bool bigText)
    {
        try
        {
            if (width <= 0)
                return string.Empty;

            text = TransformText(text);

            return orig(text, width, addDots, bigText);
        }
        catch (Exception ex)
        {
            DebugError(ex);

            return orig(text, width, addDots, bigText);
        }

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

    private static string TransformText(string input)
    {
        if (!ShouldTransformText(input) || string.IsNullOrEmpty(input))
            return input;


        string originalInput = input;
        if (transformedTextCache.ContainsKey(originalInput))
            return transformedTextCache[originalInput];

        foreach (KeyValuePair<string, string> replacement in CharReplacements)
        {
            input = input.Replace(replacement.Key, replacement.Value);
        }

        if (_Debug) DebugWarning($"Input string: '{input}', Length: {input.Length}");

        //Remix menu value is null
        //int desiredLenght = RemixMenu.OptimizedMode.Value ? 500 : 100;

        if (input.Length > 100)
        {
            input = input.Substring(0, Math.Min(100, input.Length));
        }

        transformedTextCache[originalInput] = input;
        return input;
    }

    //Placeholder security check 
    private static bool ShouldTransformText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        //foreach (string region in Region.GetFullRegionOrder())
        //{
        //    if (input.Contains(region)) return false;
        //}

        return true;
    }
}