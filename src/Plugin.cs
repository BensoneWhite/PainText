using UnityEngine;

namespace PainText;

[BepInPlugin(GUID: PluginAuthors, Name: PluginName, Version: PluginVersion)]
public class PainText : BaseUnityPlugin
{
    public const string PluginAuthors = "BensoneWhite";
    public const string PluginName = "PainText";
    public const string PluginVersion = "1.0.1";

    public bool IsInit;

    private void LogInfo(object data)
    {
        Logger.LogInfo(data);
    }

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(IntPtr hwnd, string lpString);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

    public void OnEnable()
    {
        Debug.LogWarning($"{PluginName} is loading... {PluginVersion}");

        On.RainWorld.OnModsInit += ModsInitHook;
    }

    private void ModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld rw)
    {
        try
        {
            //Never call two times the same hooks or everything is going to get weird
            if (IsInit) return;
            IsInit = true;

            //Main Features for chaning all the text
            On.FLabel.CreateTextQuads += TextQuadsHook;
            On.HUD.DialogBox.Message.ctor += DialogBoxHook;
            //On.Menu.Remix.MixedUI.LabelTest.TrimText += LabelTest_TrimText;

            //Delete old logs on start
            File.Delete("exceptionLog.txt");
            File.Delete("consoleLog.txt");

            //Change the windows name
            StringBuilder stringBuilder = new();
            IntPtr activeWindow = GetActiveWindow();
            GetWindowText(activeWindow, stringBuilder, stringBuilder.Capacity);
            SetWindowText(activeWindow, TransformText(stringBuilder.ToString()));

            //Handle all the logs 
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
            LogInfo(ex);
            throw;
        }
        finally
        {
            orig(rw);
        }
    }

    private string LabelTest_TrimText(On.Menu.Remix.MixedUI.LabelTest.orig_TrimText orig, string text, float width, bool addDots, bool bigText)
    {
        try
        {
            Debug.LogWarning(orig(text, width, addDots, bigText));
            return orig(text, width, addDots, bigText);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
        return orig(text, width, addDots, bigText);
    }

    //This is for hooks never happened an error
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

    //This is for getting all the Logs data into custom one
    private void HandleLogHook(ILContext il)
    {
        try
        {
            ILCursor c = new(il);
            while (c.TryGotoNext(new Func<Instruction, bool>[1]
            {
        i => ILPatternMatchingExt.MatchLdstr(i, "exceptionLog.txt")
            }))
            {
                c.Next.Operand = "ifeisaticoifealpaftitiaocintQalaocgam.txt";
            }
            c.Goto(0, 0, false);
            while (c.TryGotoNext(new Func<Instruction, bool>[1]
            {
        i => ILPatternMatchingExt.MatchLdstr(i, "consoleLog.txt")
            }))
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

    private static readonly Dictionary<string, string> transformedTextCache = new();

    private static string TransformText(string input)
    {

        if (string.IsNullOrEmpty(input) || input.Length < 0)
        {
            return input; 
        } // Return if null or empty

        bool isTransformed = false;

        // Check if the transformed text for the input is already cached
        if (transformedTextCache.ContainsKey(input) && !(input.Length < 0))
        {
            // If cached, return the cached transformed text
            isTransformed = true;
            return transformedTextCache[input];
        }

        if(!isTransformed)
        {
            // If not cached, perform the transformation
            foreach (KeyValuePair<string, string> replacement in CharReplacements)
            {
                if (!(input.Length < 0))
                {
                    input = input.Replace(replacement.Key.ToString(), replacement.Value);
                }

            }
        }

        // Cache the transformed text
        transformedTextCache[input] = input;

        return input;
    }

    //Never had a issue in this hook
    private void DialogBoxHook(On.HUD.DialogBox.Message.orig_ctor orig, HUD.DialogBox.Message message, string text, float xPos, float yPos, int line)
    {
        try
        {
            text = TransformText(text);
            orig(message, text, xPos, yPos, line);
            message.longestLine = 1 + (int)Math.Floor(Custom.LerpMap((float)TransformText(text).Count((char f) => f == 'w' || f == 'W'), 0f, (float)text.Length, (float)message.longestLine * 0.95f, (float)message.longestLine * 1.5f));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
    }

    private void TextQuadsHook(On.FLabel.orig_CreateTextQuads orig, FLabel label)
    {
        orig(label);

        try
        {
            if (label._doesTextNeedUpdate || label._numberOfFacetsNeeded == 0)
            {
                //The String becomes null and resets the calculation
                label._text = TransformText(label._text);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogException(e);
        }
        finally
        {
            orig(label);
        }

    }

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
}