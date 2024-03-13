namespace PainText;

[BepInPlugin(GUID: AUTHORS, Name: MOD_NAME, Version: VERSION)]
class PainText : BaseUnityPlugin
{
    public const string AUTHORS = "BensoneWhite";
    public const string MOD_NAME = "PainText";
    public const string VERSION = "1.0.1";

    public bool IsInit;

    public void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        Debug.LogWarning($"{MOD_NAME} is loading... {VERSION}");
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        try
        {
            if (IsInit) return;
            IsInit = true;

            PainLogic.Apply();

        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError(e);
            throw;
        }
        finally
        {
            orig(self);
        }
    }
}