using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Psychopath
{
    /// <summary>
    /// What the rest of the colony makes of the one it never touches.
    ///
    /// This is the half of psychopathy the game has never modelled. Vanilla gives the psychopath a
    /// short list of things that do not upset them and stops there — it is entirely a private
    /// discount. But a colonist who does not flinch while everyone around them is in pieces is
    /// visible, and people form a view about it.
    ///
    /// Nothing here is about anything the psychopath does. It is about what they conspicuously
    /// do not do.
    /// </summary>
    public class GameComponent_Noticing : GameComponent
    {
        private static GameComponent_Noticing instance;

        public GameComponent_Noticing(Game game)
        {
            instance = this;
        }

        public static GameComponent_Noticing Instance =>
            instance ?? Current.Game?.GetComponent<GameComponent_Noticing>();

        public override void FinalizeInit() => instance = this;

        public override void GameComponentTick()
        {
            if (Find.TickManager.TicksGame % Tuning.CheckInterval != 0)
            {
                return;
            }
            Look();
        }

        /// <summary>Exposed so the tests can run a pass without waiting on the tick.</summary>
        public void Look()
        {
            if (!Tuning.TheyNotice)
            {
                return;
            }
            List<Map> maps = Find.Maps;
            for (int m = 0; m < maps.Count; m++)
            {
                LookAtMap(maps[m]);
            }
        }

        private void LookAtMap(Map map)
        {
            List<Pawn> psychopaths = new List<Pawn>();
            List<Pawn> suffering = new List<Pawn>();

            foreach (Pawn p in map.mapPawns.FreeColonistsSpawned)
            {
                if (!PS.Counts(p))
                {
                    continue;
                }
                if (PS.IsPsychopath(p))
                {
                    psychopaths.Add(p);
                }
                else if (p.needs.mood.CurLevelPercentage <= Tuning.NoticingBelow)
                {
                    suffering.Add(p);
                }
            }
            if (psychopaths.Count == 0 || suffering.Count == 0)
            {
                return;
            }

            for (int i = 0; i < suffering.Count; i++)
            {
                Pawn hurt = suffering[i];
                for (int j = 0; j < psychopaths.Count; j++)
                {
                    Pawn steady = psychopaths[j];
                    // They have to actually be around each other for it to register.
                    if (!hurt.Position.InHorDistOf(steady.Position, 24f))
                    {
                        continue;
                    }
                    hurt.needs.mood.thoughts.memories.TryGainMemory(PS_DefOf.PS_DidntEvenFlinch, steady);
                }
            }
        }
    }
}
