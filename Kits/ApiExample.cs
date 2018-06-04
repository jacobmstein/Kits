using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("Api Example", "Jacob", "1.0.0")]
    internal class ApiExample : RustPlugin
    {
        [PluginReference]
        private Plugin Kits;

        [ChatCommand("givekit")]
        private void GiveKitCommand(BasePlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            Kits.Call("GiveKit", player, args[0]);
        }

        [ChatCommand("iskit")]
        private void IsKitCommand(BasePlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            PrintToChat(player, $"{args[0].ToLower()} is{(Kits.Call<bool>("IsKit", args[0].ToLower()) ? "" : " not")} a kit.");
        }

        [ChatCommand("iskitredeemable")]
        private void IsKitRedeemableCommand(BasePlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            PrintToChat(player, $" You may{(Kits.Call<bool>("IsKitRedeemable", player, args[0].ToLower()) ? "" : " not")} redeem kit {args[0].ToLower()}.");
        }

        private object CanGiveDefaultKit(BasePlayer player, string name) => name == "blocked" 
                                                                                ? false 
                                                                                : (object)null;

        private object CanRedeemKit(BasePlayer player, string name) => name == "blocked"
                                                                           ? $"{Title} is preventing you from using that kit."
                                                                           : null;
    }
}
