using Mlie;
using UnityEngine;
using Verse;

namespace RimDisorders;

public class Controller : Mod
{
    public static Settings Settings;
    public static string currentVersion;

    public Controller(ModContentPack content) : base(content)
    {
        Settings = GetSettings<Settings>();
        currentVersion =
            VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public override string SettingsCategory()
    {
        return "RRD.RimDisorders".Translate();
    }

    public override void DoSettingsWindowContents(Rect canvas)
    {
        Settings.DoWindowContents(canvas);
    }
}