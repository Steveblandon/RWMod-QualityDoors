namespace SteveZero
{
    using HarmonyLib;
    using RimWorld;
    using Verse;

    [StaticConstructorOnStartup]
    public static class QualityDoorsHarmonyPatch
    {
        static QualityDoorsHarmonyPatch()
        {
            Harmony harmony = new Harmony("SteveZero.HarmonyPatch_QualityDoors");

            harmony.PatchAll();

            harmony.Patch(
                AccessTools.Method(typeof(CompQuality), nameof(CompQuality.SetQuality)),
                postfix: new HarmonyMethod(typeof(QualityDoorsHarmonyPatch), nameof(PatchSetQualityMethod)));

            harmony.Patch(
                AccessTools.DeclaredPropertyGetter(typeof(Thing), nameof(Thing.MaxHitPoints)),
                postfix: new HarmonyMethod(typeof(QualityDoorsHarmonyPatch), nameof(PatchMaxHitpointsProperty)));
        }

        public static void PatchSetQualityMethod(QualityCategory q, ArtGenerationContext? source, CompQuality __instance)
        {
            __instance.parent.HitPoints = __instance.parent.MaxHitPoints;
        }

        public static void PatchMaxHitpointsProperty(Thing __instance, ref int __result)
        {
            if (__instance.def.IsDoor)
            {
                CompQuality compQuality = ThingCompUtility.TryGetComp<CompQuality>(__instance);

                if (compQuality != null)
                {
                    __result = (int)((float)__result * GetQualityFactor(compQuality.Quality));
                }
            }
        }

        private static float GetQualityFactor(QualityCategory q)
        {
            float result;
            switch (q)
            {
                case QualityCategory.Awful:
                    result = 0.6f;
                    break;
                case QualityCategory.Poor:
                    result = 0.9f;
                    break;
                case QualityCategory.Normal:
                    result = 1f;
                    break;
                case QualityCategory.Good:
                    result = 1.2f;
                    break;
                case QualityCategory.Excellent:
                    result = 1.5f;
                    break;
                case QualityCategory.Masterwork:
                    result = 2f;
                    break;
                case QualityCategory.Legendary:
                    result = 3f;
                    break;
                default:
                    result = 1f;
                    break;
            }
            return result;
        }
    }
}
