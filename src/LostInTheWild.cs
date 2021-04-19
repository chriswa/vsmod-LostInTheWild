using HarmonyLib;
using System.Reflection;
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
      harmony.PatchAll(Assembly.GetExecutingAssembly());

      sapi.RegisterCommand("getplayerpos", "Outputs player's position to server console", "/getplayerpos PlayerName", (IServerPlayer player, int groupId, CmdArgs args) => {
        string playerName = args.PopWord();
        foreach (IServerPlayer onlinePlayer in sapi.World.AllOnlinePlayers) {
          if (onlinePlayer.PlayerName.Equals(playerName)) {
            var pos = onlinePlayer?.Entity?.Pos?.AsBlockPos;
            if (pos != null) {
              sapi.Logger.Notification("getplayerpos: Player {0} is at {1}", onlinePlayer.PlayerName, pos);
              return;
            }
          }
        }
        sapi.Logger.Notification("getplayerpos: Could not find player named {0}", playerName);

      }, Privilege.tp);
    }
    public override void Dispose() {
      harmony.UnpatchAll(harmony.Id);
    }
  }

  [HarmonyPatch(typeof(WaypointMapLayer))]
  [HarmonyPatch("OnCmdWayPoint")]
  public class Patch_WaypointMapLayer_OnCmdWayPoint {
    public static bool Prefix(IServerPlayer player, int groupId, CmdArgs args) {
      player.SendMessage(groupId, Lang.Get("Waypoints have been disabled by LostInTheWild"), EnumChatType.CommandSuccess);
      return false; // skip original method
    }
  }

  [HarmonyPatch(typeof(ServerLogger))]
  [HarmonyPatch("Log")]
  public class Patch_ServerLogger_Log {
    public static bool Prefix(EnumLogType logType, ref string message, params object[] args) {
      if (message == "Placing player at {0} {1} {2}") {
        message = "Placing player";
      }
      else if (message == ("Teleporting player {0} to {1}")) {
        message = "Teleporting player {0}";
      }
      else if (message == ("Teleporting entity {0} to {1}")) {
        message = "Teleporting entity {0}";
      }
      return true; // run original method
    }
  }

}
