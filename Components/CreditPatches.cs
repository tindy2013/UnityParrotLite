using MU3.AM;
using MU3.DB;
using System;
using System.Collections.Generic;
using System.Reflection;
using static MU3.AM.Credit;

namespace UnityParrotLite.Components
{
    public class CreditPatches
    {
        public static void Patch()
        {
            Harmony.PatchAllInType(typeof(CreditPatches));
            Harmony.MakeRET(typeof(Credit), "clearAimeLog");
            //Harmony.MakeRET(typeof(Credit), "initialize");
            //Harmony.MakeRET(typeof(Credit), "isGameCostEnough", true);
            //Harmony.MakeRET(typeof(Credit), "onCoinIn", true);
            //Harmony.MakeRET(typeof(Credit), "payGameCost", true);
            Harmony.MakeRET(typeof(Credit), "sendAimeLog");
            //Harmony.MakeRET(typeof(Credit), "terminate");
        }

        static int currentCredit = 0;

        [MethodPatch(PatchType.Prefix, typeof(Credit), "get_addableCoin")]
        static bool addableCoin(ref int __result)
        {
            __result = 0;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(Credit), "get_coin")]
        static bool coin(ref int __result)
        {
            __result = 0;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(Credit), "get_CoinAmount")]
        static bool CoinAmount(ref int __result)
        {
            __result = 1;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(Credit), "get_credit")]
        static bool credit(ref Credit __instance, ref int __result)
        {
            byte[] result = SharedMemory.Read((int)SharedMemory.MapPosition.Function_Coin, 1);
            if (result[0] == 0x01)
            {
                currentCredit++;
                SharedMemory.Write(new byte[1] { 0 }, (int)SharedMemory.MapPosition.Function_Coin, 1);
                typeof(Credit).GetMethod("onCoinIn", (BindingFlags)62).Invoke(__instance, new object[] { AMDaemon.CreditSound.Credit });
            }
            __result = currentCredit;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(Credit), "isGameCostEnough")]
        static bool isGameCostEnough(ref bool __result, GameCost gameCost, int count)
        {
            __result = currentCredit >= count;
            return false;
        }

        [MethodPatch(PatchType.Prefix, typeof(Credit), "payGameCost")]
        static bool payGameCost(ref bool __result, GameCost __0, int __1)
        {
            __result = true;
            currentCredit -= __1;
            return false;
        }


        [MethodPatch(PatchType.Prefix, typeof(Credit), "toCoins")]
        static bool toCoins(ref int __result, int __0)
        {
            __result = __0;
            return false;
        }
    }
}
