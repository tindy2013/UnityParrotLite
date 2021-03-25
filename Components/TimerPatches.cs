using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MU3;
using MU3.CustomUI;
using UnityEngine;
using static MU3.UITimer;

namespace UnityParrotLite.Components
{
    public class TimerPatches
    {
        public static void Patch()
        {
            Harmony.PatchAllInType(typeof(TimerPatches));
            Harmony.MakeRET(typeof(UITimer), "set_Counter");
            Harmony.MakeRET(typeof(SystemUI.Timer), "start");
        }

        [MethodPatch(PatchType.Prefix, typeof(UITimer), "initialize", new Type[] { typeof(float), typeof(UIInput.UILayeredInput) })]
        public static bool initialize(ref UITimer __instance, ref float counter, ref UIInput.UILayeredInput input)
        {
            return initialize(ref __instance, ref counter);
        }

        [MethodPatch(PatchType.Prefix, typeof(UITimer), "initialize", new Type[] { typeof(float) })]
        public static bool initialize(ref UITimer __instance, ref float counter)
        {
            __instance.gameObject.SetActive(false);
            typeof(UITimer).GetProperty("counter_").SetValue(__instance, 60f, null);
            MU3UICounter normal = (MU3UICounter)typeof(UITimer).GetProperty("counterNormal_").GetValue(__instance, null);
            normal.CounterAsInt = 99;
            typeof(UITimer).GetMethod("setAnimatorStatus", (BindingFlags)62).Invoke(__instance, new object[] { 0 });
            Animator animator = (Animator)typeof(UITimer).GetProperty("animator_", (BindingFlags)62).GetValue(__instance, null);
            animator.speed = 0f;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(SystemUI.Timer), "get_remainTime")]
        public static bool remainTime(ref float __result)
        {
            __result = 60.0f;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(UITimer), "get_IsTimeout")]
        public static bool IsTimeout(ref bool __result)
        {
            __result = false;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(UITimer), "get_Counter")]
        public static bool Counter(ref float __result)
        {
            __result = 60f;
            return false;
        }
    }
}
