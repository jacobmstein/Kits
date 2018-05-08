using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("Api Example", "Jacob", "1.0.0")]
    internal class ApiExample : RustPlugin
    {
        [PluginReference]
        private Plugin Kits;

        [ChatCommand("canuse")]
        private void CanUseCommand(BasePlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }

            PrintToChat(player, $" You {(Kits.Call<bool>("IsKitRedeemable", player.userID, args[0].ToLower()) ? "may" : "may not")} use kit {args[0].ToLower()}.");
        }

        private object CanRedeemKit(BasePlayer player, string name) => name == "blocked" 
            ? $"{Title} is preventing you from using that kit." 
            : null;
    }
}
