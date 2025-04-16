using Menu.Remix.MixedUI;

namespace PainText;

public class RemixMenu : OptionInterface
{
    public static Configurable<bool> OptimizedMode;
    public RemixMenu()
    {
        OptimizedMode = config.Bind(nameof(OptimizedMode), false);
    }

    public override void Initialize()
    {
        var opTab1 = new OpTab(this, "𝓞𝓹𝓽𝓲𝓸𝓷𝓼");

        Tabs = ([opTab1]);

        var tab1Container = new OpContainer(new Vector2(0f, 0f));

        opTab1.AddItems([tab1Container]);

        UIelement[] arrayElementsTab =
        [
            new OpCheckBox(OptimizedMode, 10f, 540f) { description = Translate("𝕮𝖆𝖕𝖘 𝖙𝖍𝖊 𝖆𝖒𝖔𝖚𝖓𝖙 𝖔𝖋 𝖙𝖊𝖝𝖙 𝖎𝖘 𝖕𝖗𝖔𝖈𝖊𝖘𝖘𝖊𝖉") },
            new OpLabel(45f, 540f, "Öṗẗïṁïżëḋ Ṁöḋë", false),
        ];

        opTab1.AddItems(arrayElementsTab);
    }
}