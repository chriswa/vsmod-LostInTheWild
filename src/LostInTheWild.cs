using HarmonyLib;
using System.Text.RegularExpressions;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.GameContent;
using Vintagestory.Server;

[assembly: ModInfo("LostInTheWild")]

namespace LostInTheWild {
  public class LostInTheWildMod : ModSystem {
    private Harmony harmony;
    public override void StartServerSide(ICoreServerAPI sapi) {
      harmony = new Harmony("goxmeor.lostinthewild");
      harmony.PatchAll();
    }
    // public override void Dispose() {
    //   harmony.UnpatchAll(harmony.Id);
    //   base.Dispose();
    // }
  }

  [HarmonyPatch(typeof(WaypointMapLayer))]
  [HarmonyPatch("OnCmdWayPoint")]
  public class Patch_WaypointMapLayer_OnCmdWayPoint {
    public static bool Prefix(IServerPlayer player, int groupId, CmdArgs args) {
      player.SendMessage(groupId, Lang.Get("Waypoints have been disabled by LostInTheWild"), EnumChatType.CommandSuccess);
      return false; // skip original method
    }
  }

  [HarmonyPatch(typeof(BaseServerChatCommandDelegateProvider))]
  [HarmonyPatch("Success")]
  public class Patch_BaseServerChatCommandDelegateProvider_Success {
    public static bool Prefix(BaseServerChatCommandDelegateProvider __instance, IServerPlayer player, int groupId, string message) {
      if (__instance.GetType().ToString() == "Vintagestory.Server.CmdLandRights") {
        string censoredMessage = message;
        // first, try to remove english phrases including coords
        censoredMessage = Regex.Replace(censoredMessage, " at -?\\d+, -?\\d+, -?\\d+", "");
        censoredMessage = Regex.Replace(censoredMessage, " position -?\\d+, -?\\d+, -?\\d+", "");
        // if this isn't english, simply replace coords with a bogus string
        censoredMessage = Regex.Replace(censoredMessage, "-?\\d+, -?\\d+, -?\\d+", "[LostInTheWild]");
        player.SendMessage(groupId, censoredMessage, EnumChatType.CommandSuccess);
        return false; // skip original method
      }
      return true; // run original method
    }
  }

}
