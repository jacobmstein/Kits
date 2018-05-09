﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Kits", "Jacob", "4.0.0-alpha", ResourceId = 668)]
    internal class Kits : RustPlugin
    {
        #region Fields

        private static Kits Instance;

        private readonly Data _data = new Data();

        private Configuration _configuration;

        #endregion

        #region Enums

        private enum Container
        {
            Belt,
            Main,
            Wear
        }

        #endregion

        #region API

        private bool IsKit(string name) => _data[name] != null;

        private bool IsKitRedeemable(BasePlayer player, string name)
        {
            var kit = _data[name];
            if (kit == null)
            {
                return false;
            }

            string message;
            return IsKitRedeemable(player, kit, out message);
        }

        private void GiveKit(BasePlayer player, string name) => _data[name]?.Give(player);

        #endregion

        #region Chat Commands

        [ChatCommand("kit")]
        private void KitCommand(BasePlayer player, string command, string[] args)
        {
            if (args.Length == 0)
            {
                var kits = _data.Kits.Where(x => HasPermission(player, $"kits.{x.Name}")).ToArray();
                Message(player, "List", kits.Length == 1 ? string.Empty : "s", kits.Any()
                                                                                   ? kits.Select(x => x.Name).ToSentence()
                                                                                   : "None");
                return;
            }

            switch (args[0].ToLower())
            {
                case "add":
                case "create":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 2)
                        {
                            goto case "help";
                        }

                        var name = args[1].ToLower();
                        if (!Regex.IsMatch(name, "[a-zA-Z]")
                            || new[] { "add", "admin", "cooldown", "create", "delete", "duplicate", "help", "items", "limit", "max", "remove", "rename", "update" }.Contains(name))
                        {
                            Message(player, "InvalidName");
                            return;
                        }

                        if (_data[name] != null)
                        {
                            Message(player, "ExistsError", name);
                            return;
                        }

                        Message(player, "Created", name);
                        _data.Kits.Add(new Kit(player, name));
                        permission.RegisterPermission($"kits.{name}", this);
                        break;
                    }

                case "cooldown":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length < 3)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        var seconds = ParseTime(string.Join(string.Empty, args.Skip(2).ToArray()));
                        Message(player, "CooldownSet", kit.Name, seconds == 0
                                                                     ? "Nothing"
                                                                     : FormatTime(seconds));
                        kit.Cooldown = seconds;
                        break;
                    }

                case "delete":
                case "remove":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 2)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        Message(player, "Removed", kit.Name);
                        _data.Kits.Remove(kit);
                        foreach (var target in _data.Players)
                        {
                            target.Cooldowns.Remove(kit.Name);
                            target.Redemptions.Remove(kit.Name);
                        }

                        break;
                    }

                case "duplicate":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 3)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        var name = args[2].ToLower();
                        if (_data[name] != null)
                        {
                            Message(player, "ExistsError", name);
                            return;
                        }

                        Message(player, "Duplicated", kit.Name, name);
                        _data.Kits.Add(new Kit
                        {
                            Cooldown = kit.Cooldown,
                            Items = kit.Items,
                            Limit = kit.Limit,
                            Name = name
                        });
                        permission.RegisterPermission($"kits.{name}", this);
                        break;
                    }

                case "help":
                    {
                        Message(player, "Help");
                        break;
                    }

                case "items":
                case "update":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 2)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        Message(player, "Updated", kit.Name);
                        kit.Items = new HashSet<KitItem>(player.inventory.AllItems().Select(x => new KitItem(x)));
                        break;
                    }

                case "max":
                case "limit":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 3)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        int limit;
                        if (!int.TryParse(args[2], out limit))
                        {
                            goto case "help";
                        }

                        Message(player, "LimitSet", kit.Name, limit);
                        kit.Limit = limit;
                        break;
                    }

                case "rename":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length != 3)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", _data[args[1].ToLower()]);
                            return;
                        }

                        var newName = args[2].ToLower();
                        if (_data[newName] != null)
                        {
                            Message(player, "ExistsError", newName);
                            return;
                        }

                        Message(player, "Renamed", kit.Name, newName);
                        foreach (var target in _data.Players)
                        {
                            long time;
                            if (target.Cooldowns.TryGetValue(kit.Name, out time))
                            {
                                target.Cooldowns.Remove(kit.Name);
                                target.Cooldowns.Add(newName, time);
                            }

                            int redemptions;
                            if (!target.Redemptions.TryGetValue(kit.Name, out redemptions))
                            {
                                continue;
                            }

                            target.Redemptions.Remove(kit.Name);
                            target.Redemptions.Add(newName, redemptions);
                        }

                        kit.Name = newName;
                        permission.RegisterPermission($"kits.{newName}", this);
                        break;
                    }

                default:
                    {
                        var kit = _data[args[0].ToLower()];
                        if (kit == null)
                        {
                            goto case "help";
                        }

                        string message;
                        if (!IsKitRedeemable(player, kit, out message))
                        {
                            PrintToChat(player, message);
                            return;
                        }

                        var result = Interface.CallHook("CanRedeemKit", player);
                        result = Interface.CallHook("CanRedeemKit", player, kit.Name) ?? result;
                        if (result != null)
                        {
                            if (result is string)
                            {
                                PrintToChat(player, result.ToString());
                            }

                            return;
                        }

                        kit.Give(player);
                        var playerData = _data[player.userID];
                        playerData.AddRedemption(kit.Name);
                        playerData.SetCooldown(kit.Name, kit.Cooldown);
                        LogToFile("RedemptionLog", $"{DateTime.UtcNow.ToShortTimeString()} {player.displayName,-32} {player.userID} {kit.Name}", this, false);
                        break;
                    }
            }
        }

        #endregion

        #region Methods

        private void ClearInventory(BasePlayer player)
        {
            foreach (var item in player.inventory.AllItems())
            {
                item.Remove();
                item.DoRemove();
            }
        }

        private bool IsKitRedeemable(BasePlayer player, Kit kit, out string message)
        {
            if (!HasPermission(player, $"kits.{kit.Name}"))
            {
                message = lang.GetMessage("PermissionError", this, player.ToString());
                return false;
            }

            var playerData = _data[player.userID];
            if (kit.Limit != 0
                && playerData.Redemptions.ContainsKey(kit.Name)
                && playerData.Redemptions[kit.Name] >= kit.Limit)
            {
                message = string.Format(covalence.FormatText(lang.GetMessage("LimitError", this, player.ToString())), kit.Name);
                return false;
            }

            long seconds;
            if (playerData.HasCooldown(kit.Name, out seconds))
            {
                message = string.Format(covalence.FormatText(lang.GetMessage("CooldownError", this, player.ToString())), kit.Name, FormatTime(seconds));
                return false;
            }

            message = string.Empty;
            return true;
        }

        private long Epoch() =>
            (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

        private string FormatTime(long seconds)
        {
            var phrases = new List<string>();
            var time = TimeSpan.FromSeconds(seconds);
            if (time.Days > 0)
            {
                phrases.Add($"{time.Days} day{(time.Days == 1 ? string.Empty : "s")}");
            }

            if (time.Hours > 0)
            {
                phrases.Add($"{time.Hours} hour{(time.Hours == 1 ? string.Empty : "s")}");
            }

            if (time.Minutes > 0)
            {
                phrases.Add($"{time.Minutes} minute{(time.Minutes == 1 ? string.Empty : "s")}");
            }

            if (time.Seconds > 0)
            {
                phrases.Add($"{time.Seconds} second{(time.Seconds == 1 ? string.Empty : "s")}");
            }

            return phrases.ToSentence();
        }

        private bool HasPermission(BasePlayer player, string permission = "kits.admin") =>
            Instance.permission.UserHasPermission(player.UserIDString, permission) || player.IsAdmin;

        private void Message(BasePlayer player, string key, params object[] args) =>
            PrintToChat(player, covalence.FormatText(lang.GetMessage(key, this, player.UserIDString)), args);

        private long ParseTime(string input)
        {
            var conversions = new Dictionary<char, int>
            {
                ['s'] = 1,
                ['m'] = 60,
                ['h'] = 3600,
                ['d'] = 86400
            };

            return (from @group in Regex.Matches(input.Replace(" ", string.Empty), @"(?'number'\d+)(?'identifier'[a-zA-Z])").Cast<Match>().Select(x => x.Groups)
                    let keyValuePair = conversions.SingleOrDefault(x => x.Key.ToString() == @group["identifier"].Value.ToLower())
                    select Convert.ToInt64(@group["number"].Value) * keyValuePair.Value).Sum();
        }

        #endregion

        #region Oxide Hooks

        protected override void LoadConfig()
        {
            base.LoadConfig();
            _configuration = Config.ReadObject<Configuration>();
        }

        protected override void LoadDefaultConfig() => _configuration = new Configuration();

        protected override void LoadDefaultMessages() => lang.RegisterMessages(new Dictionary<string, string>
        {
            ["CooldownError"] = "Error, you may not use kit [#ADD8E6]{0}[/#] for [#ADD8E6]{1}[/#].",
            ["CooldownSet"] = "Cooldown for kit [#ADD8E6]{0}[/#] successfully set to [#ADD8E6]{1}[/#].",
            ["Created"] = "Kit [#ADD8E6]{0}[/#] successfully created.",
            ["Doesn'tExistError"] = "Error, no kit exists by the name [#ADD8E6]{0}[/#].",
            ["Duplicated"] = "Kit [#ADD8E6]{0}[/#] successfully duplicated as [#ADD8E6]{1}[/#].",
            ["ExistsError"] = "Error, a kit already exists by the name [#ADD8E6]{0}[/#].",
            ["Help"] = "...",
            ["InvalidName"] = "Error, invalid name.",
            ["LimitError"] = "Error, you reached the usage limit for kit [#ADD8E6]{0}[/#].",
            ["LimitSet"] = "Limit for kit [#ADD8E6]{0}[/#] successfully set to [#ADD8E6]{1}[/#].",
            ["List"] = "<size=16>Kit List</size>\nThe following kit{0} are available: [#ADD8E6]{1}[/#].",
            ["PermissionError"] = "Error, you lack permission.",
            ["Removed"] = "Kit [#ADD8E6]{0}[/#] successfully removed.",
            ["Renamed"] = "Kit [#ADD8E6]{0}[/#] successfully renamed to [#ADD8E6]{1}[/#].",
            ["Updated"] = "Kit [#ADD8E6]{0}[/#] successfully updated."
        }, this);

        protected override void SaveConfig() => Config.WriteObject(_configuration);

        private void Init()
        {
            Instance = this;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            _data.Kits = Interface.Oxide.DataFileSystem.ReadObject<HashSet<Kit>>("Kits/Kits");
            _data.Players = Interface.Oxide.DataFileSystem.ReadObject<HashSet<PlayerData>>("Kits/PlayerData");

            permission.RegisterPermission("kits.admin", this);
            foreach (var kit in _data.Kits)
            {
                permission.RegisterPermission($"kits.{kit.Name}", this);
            }
        }

        private void OnPlayerRespawned(BasePlayer player)
        {
            var kit = _configuration.DefaultKits.Select(x => _data[x]).Where(x => x != null).LastOrDefault(x => HasPermission(player, $"kits.{x.Name}"));
            if (kit == null)
            {
                return;
            }

            ClearInventory(player);
            kit.Give(player);
        }

        private void OnNewSave()
        {
            if (_configuration.WipePlayerData)
            {
                _data.Players.Clear();
            }
        }

        private void OnServerSave()
        {
            Interface.Oxide.DataFileSystem.WriteObject("Kits/Kits", _data.Kits);
            Interface.Oxide.DataFileSystem.WriteObject("Kits/PlayerData", _data.Players);
        }

        private void Unload() => OnServerSave();

        #endregion

        #region Classes

        private class Configuration
        {
            [JsonProperty("Default kits (lowest to highest priority)")]
            public List<string> DefaultKits { get; set; } = new List<string>();

            [JsonProperty("Wipe player data on new save (true/false)")]
            public bool WipePlayerData { get; set; } = true;
        }

        private class Data
        {
            [JsonProperty("kits")]
            public HashSet<Kit> Kits { get; set; } = new HashSet<Kit>();

            [JsonProperty("players")]
            public HashSet<PlayerData> Players { get; set; } = new HashSet<PlayerData>();

            public Kit this[string name] => Kits.SingleOrDefault(x => x.Name == name);

            public PlayerData this[ulong userId]
            {
                get
                {
                    var player = Players.SingleOrDefault(x => x.UserId == userId) ?? new PlayerData(userId);

                    Players.Add(player);
                    return player;
                }
            }
        }

        private class Kit
        {
            public Kit()
            {
            }

            public Kit(BasePlayer player, string name)
            {
                Items = new HashSet<KitItem>(player.inventory.AllItems().Select(x => new KitItem(x)));
                Name = name;
            }

            [JsonProperty("cooldown")]
            public long Cooldown { get; set; }

            [JsonProperty("items")]
            public HashSet<KitItem> Items { get; set; }

            [JsonProperty("limit")]
            public int Limit { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            public void Give(BasePlayer player)
            {
                foreach (var item in Items)
                {
                    item.Give(player);
                }
            }
        }

        private class KitItem
        {
            public KitItem()
            {
            }

            public KitItem(Item item)
            {
                Amount = item.amount;
                Blueprint = item.info.shortname == "blueprintbase";
                var parent = item.parent;
                Container = parent.HasFlag(ItemContainer.Flag.Belt)
                                    ? Container.Belt
                                    : (parent.HasFlag(ItemContainer.Flag.Clothing)
                                        ? Container.Wear
                                        : Container.Main);
                Position = item.position;
                Shortname = Blueprint ? item.blueprintTargetDef.shortname : item.info.shortname;
                SkinId = item.skin;
                if (item.contents == null)
                {
                    return;
                }

                foreach (var content in item.contents.itemList)
                {
                    Contents.Add(new KitItem(content));
                }
            }

            [JsonProperty("amount")]
            public int Amount { get; set; }

            [JsonProperty("blueprint")]
            public bool Blueprint { get; set; }

            [JsonProperty("container")]
            public Container Container { get; set; }

            [JsonProperty("contents")]
            public HashSet<KitItem> Contents { get; set; } = new HashSet<KitItem>();

            [JsonProperty("position")]
            public int Position { get; set; }

            [JsonProperty("shortname")]
            public string Shortname { get; set; }

            [JsonProperty("skinId")]
            public ulong SkinId { get; set; }

            public Item Create()
            {
                var item = ItemManager.CreateByName(Blueprint ? "blueprintbase" : Shortname, Amount, SkinId);
                if (Blueprint)
                {
                    item.blueprintTarget = ItemManager.FindItemDefinition(Shortname).itemid;
                    return item;
                }

                foreach (var content in Contents)
                {
                    content.Create().MoveToContainer(item.contents);
                }

                return item;
            }

            public void Give(BasePlayer player)
            {
                var container = Container == Container.Belt
                                    ? player.inventory.containerBelt
                                    : (Container == Container.Wear
                                        ? player.inventory.containerWear
                                        : player.inventory.containerMain);
                var item = Create();
                if (item.MoveToContainer(container, Position))
                {
                    player.Command("note.inv", item.info.itemid, item.amount, item.name);
                    return;
                }

                player.GiveItem(item);
            }
        }

        private class PlayerData
        {
            public PlayerData()
            {
            }

            public PlayerData(ulong userId)
            {
                UserId = userId;
            }

            [JsonProperty("userId")]
            public ulong UserId { get; set; }

            [JsonProperty("cooldowns")]
            public Dictionary<string, long> Cooldowns { get; set; } = new Dictionary<string, long>();

            [JsonProperty("redemptions")]
            public Dictionary<string, int> Redemptions = new Dictionary<string, int>();

            public void AddRedemption(string name)
            {
                if (Redemptions.ContainsKey(name))
                {
                    Redemptions[name]++;
                }
                else
                {
                    Redemptions.Add(name, 1);
                }
            }

            public bool HasCooldown(string name, out long seconds)
            {
                if (Cooldowns.TryGetValue(name, out seconds) && seconds > Instance.Epoch())
                {
                    seconds -= Instance.Epoch();
                    return true;
                }

                return false;
            }

            public void SetCooldown(string name, long seconds) => Cooldowns[name] = Instance.Epoch() + seconds;
        }

        #endregion
    }
}
