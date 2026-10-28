using HarmonyLib;
using RimWorld;
using Verse;

namespace Psychopath
{
    /// <summary>
    /// What else is installed, reported at startup so a log tells you the whole picture.
    ///
    /// This mod needs none of it. It is worth saying out loud, though, because the psychopath is
    /// only conspicuous when other colonists have something to be conspicuous against — and that
    /// is what the rest of the set provides.
    /// </summary>
    [StaticConstructorOnStartup]
    public static class Bridges
    {
        private static readonly string[] Watched =
        {
            "Grief.GameComponent_Grief",
            "Guilt.GameComponent_Guilt",
            "Mettle.GameComponent_Mettle",
            "Company.GameComponent_Company",
        };

        private static int found;

        public static void Connect()
        {
            found = 0;
            foreach (string t in Watched)
            {
                try { if (AccessTools.TypeByName(t) != null) { found++; } }
                catch { }
            }
        }

        public static string Report()
        {
            if (found == 0)
            {
                return " Nothing else from the set is loaded: they will have less to be unlike.";
            }
            return $" {found} of the rest of the set found: plenty for them to be unlike.";
        }
    }
}
