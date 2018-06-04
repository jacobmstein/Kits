using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Plugins
{
    [Info("Kits", "Jacob", "4.0.0-alpha.3", ResourceId = 668)]
    internal class Kits : RustPlugin
    {
        #region Fields

        private static Kits Instance;

        private readonly string[] _invalidNames =
        {
            "add",
            "admin",
            "cooldown",
            "create",
            "delete",
            "duplicate",
            "give",
            "help",
            "items",
            "limit",
            "max",
            "remove",
            "rename",
            "update"
        };

        private readonly Dictionary<int, string> _itemIdShortnameConversions = new Dictionary<int, string>
        {
            [-1461508848] = "rifle.ak",
            [2115555558] = "ammo.handmade.shell",
            [-533875561] = "ammo.pistol",
            [1621541165] = "ammo.pistol.fire",
            [-422893115] = "ammo.pistol.hv",
            [815896488] = "ammo.rifle",
            [805088543] = "ammo.rifle.explosive",
            [449771810] = "ammo.rifle.incendiary",
            [1152393492] = "ammo.rifle.hv",
            [1578894260] = "ammo.rocket.basic",
            [1436532208] = "ammo.rocket.fire",
            [542276424] = "ammo.rocket.hv",
            [1594947829] = "ammo.rocket.smoke",
            [-1035059994] = "ammo.shotgun",
            [1818890814] = "ammo.shotgun.fire",
            [1819281075] = "ammo.shotgun.slug",
            [1685058759] = "antiradpills",
            [93029210] = "apple",
            [-1565095136] = "apple.spoiled",
            [-1775362679] = "arrow.bone",
            [-1775249157] = "arrow.fire",
            [-1280058093] = "arrow.hv",
            [-420273765] = "arrow.wooden",
            [563023711] = "autoturret",
            [790921853] = "axe.salvaged",
            [-337261910] = "bandage",
            [498312426] = "barricade.concrete",
            [504904386] = "barricade.metal",
            [-1221200300] = "barricade.sandbags",
            [510887968] = "barricade.stone",
            [-814689390] = "barricade.wood",
            [1024486167] = "barricade.woodwire",
            [2021568998] = "battery.small",
            [97329] = "bbq",
            [1046072789] = "trap.bear",
            [97409] = "bed",
            [-1480119738] = "tool.binoculars",
            [1611480185] = "black.raspberries",
            [-1386464949] = "bleach",
            [93832698] = "blood",
            [-1063412582] = "blueberries",
            [-1887162396] = "blueprintbase",
            [-55660037] = "rifle.bolt",
            [919780768] = "bone.club",
            [-365801095] = "bone.fragments",
            [68998734] = "botabag",
            [-853695669] = "bow.hunting",
            [271534758] = "box.wooden.large",
            [-770311783] = "box.wooden",
            [-1192532973] = "bucket.water",
            [-307490664] = "building.planner",
            [707427396] = "burlap.shirt",
            [707432758] = "burlap.shoes",
            [-2079677721] = "cactusflesh",
            [-1342405573] = "tool.camera",
            [-139769801] = "campfire",
            [-1043746011] = "can.beans",
            [2080339268] = "can.beans.empty",
            [-171664558] = "can.tuna",
            [1050986417] = "can.tuna.empty",
            [-1693683664] = "candycaneclub",
            [523409530] = "candycane",
            [1300054961] = "cctv.camera",
            [-2095387015] = "ceilinglight",
            [1428021640] = "chainsaw",
            [94623429] = "chair",
            [1436001773] = "charcoal",
            [1711323399] = "chicken.burned",
            [1734319168] = "chicken.cooked",
            [-1658459025] = "chicken.raw",
            [-726947205] = "chicken.spoiled",
            [-341443994] = "chocholate",
            [1540879296] = "xmasdoorwreath",
            [94756378] = "cloth",
            [3059095] = "coal",
            [3059624] = "corn",
            [2045107609] = "clone.corn",
            [583366917] = "seed.corn",
            [2123300234] = "crossbow",
            [1983936587] = "crude.oil",
            [1257201758] = "cupboard.tool",
            [-1144743963] = "diving.fins",
            [-1144542967] = "diving.mask",
            [-1144334585] = "diving.tank",
            [1066729526] = "diving.wetsuit",
            [-1598790097] = "door.double.hinged.metal",
            [-933236257] = "door.double.hinged.toptier",
            [-1575287163] = "door.double.hinged.wood",
            [-2104481870] = "door.hinged.metal",
            [-1571725662] = "door.hinged.toptier",
            [1456441506] = "door.hinged.wood",
            [1200628767] = "door.key",
            [-778796102] = "door.closer",
            [1526866730] = "xmas.door.garland",
            [1925723260] = "dropbox",
            [1891056868] = "ducttape",
            [1295154089] = "explosive.satchel",
            [498591726] = "explosive.timed",
            [1755466030] = "explosives",
            [726730162] = "facialhair.style01",
            [-1034048911] = "fat.animal",
            [252529905] = "femalearmpithair.style01",
            [471582113] = "femaleeyebrow.style01",
            [-1138648591] = "femalepubichair.style01",
            [305916740] = "female_hairstyle_01",
            [305916742] = "female_hairstyle_03",
            [305916744] = "female_hairstyle_05",
            [1908328648] = "fireplace.stone",
            [-2078972355] = "fish.cooked",
            [-533484654] = "fish.raw",
            [1571660245] = "fishingrod.handmade",
            [1045869440] = "flamethrower",
            [1985408483] = "flameturret",
            [97513422] = "flare",
            [1496470781] = "flashlight.held",
            [1229879204] = "weapon.mod.flashlight",
            [-1722829188] = "floor.grill",
            [1849912854] = "floor.ladder.hatch",
            [-1266285051] = "fridge",
            [-1749787215] = "boots.frog",
            [28178745] = "lowgradefuel",
            [-505639592] = "furnace",
            [1598149413] = "furnace.large",
            [-1779401418] = "gates.external.high.stone",
            [-57285700] = "gates.external.high.wood",
            [98228420] = "gears",
            [1422845239] = "geiger.counter",
            [277631078] = "generator.wind.scrap",
            [115739308] = "burlap.gloves",
            [-522149009] = "gloweyes",
            [3175989] = "glue",
            [718197703] = "granolabar",
            [384204160] = "grenade.beancan",
            [-1308622549] = "grenade.f1",
            [-217113639] = "fun.guitar",
            [-1580059655] = "gunpowder",
            [-1832205789] = "male_hairstyle_01",
            [305916741] = "female_hairstyle_02",
            [936777834] = "attire.hide.helterneck",
            [-1224598842] = "hammer",
            [-1976561211] = "hammer.salvaged",
            [-1406876421] = "hat.beenie",
            [-1397343301] = "hat.boonie",
            [1260209393] = "bucket.helmet",
            [-1035315940] = "burlap.headwrap",
            [-1381682752] = "hat.candle",
            [696727039] = "hat.cap",
            [-2128719593] = "coffeecan.helmet",
            [-1178289187] = "deer.skull.mask",
            [1351172108] = "heavy.plate.helmet",
            [-450738836] = "hat.miner",
            [-966287254] = "attire.reindeer.headband",
            [340009023] = "riot.helmet",
            [124310981] = "hat.wolf",
            [1501403549] = "wood.armor.helmet",
            [698310895] = "hatchet",
            [523855532] = "hazmatsuit",
            [2045246801] = "clone.hemp",
            [583506109] = "seed.hemp",
            [-148163128] = "attire.hide.boots",
            [-132588262] = "attire.hide.skirt",
            [-1666761111] = "attire.hide.vest",
            [-465236267] = "weapon.mod.holosight",
            [-1211618504] = "hoodie",
            [2133577942] = "hq.metal.ore",
            [-1014825244] = "humanmeat.burned",
            [-991829475] = "humanmeat.cooked",
            [-642008142] = "humanmeat.raw",
            [661790782] = "humanmeat.spoiled",
            [-1440143841] = "icepick.salvaged",
            [569119686] = "bone.armor.suit",
            [1404466285] = "heavy.plate.jacket",
            [-1616887133] = "jacket.snow",
            [-1167640370] = "jacket",
            [-1284735799] = "jackolantern.angry",
            [-1278649848] = "jackolantern.happy",
            [776005741] = "knife.bone",
            [108061910] = "ladder.wooden.wall",
            [255101535] = "trap.landmine",
            [-51678842] = "lantern",
            [-789202811] = "largemedkit",
            [516382256] = "weapon.mod.lasersight",
            [50834473] = "leather",
            [-975723312] = "lock.code",
            [1908195100] = "lock.key",
            [-1097452776] = "locker",
            [146685185] = "longsword",
            [-1716193401] = "rifle.lr300",
            [193190034] = "lmg.m249",
            [371156815] = "pistol.m92",
            [3343606] = "mace",
            [825308669] = "machete",
            [830965940] = "mailbox",
            [1662628660] = "male.facialhair.style02",
            [1662628661] = "male.facialhair.style03",
            [1662628662] = "male.facialhair.style04",
            [-1832205788] = "male_hairstyle_02",
            [-1832205786] = "male_hairstyle_04",
            [1625090418] = "malearmpithair.style01",
            [-1269800768] = "maleeyebrow.style01",
            [429648208] = "malepubichair.style01",
            [-1832205787] = "male_hairstyle_03",
            [-1832205785] = "male_hairstyle_05",
            [107868] = "map",
            [997973965] = "mask.balaclava",
            [-46188931] = "mask.bandana",
            [-46848560] = "metal.facemask",
            [-2066726403] = "bearmeat.burned",
            [-2043730634] = "bearmeat.cooked",
            [1325935999] = "bearmeat",
            [-225234813] = "deermeat.burned",
            [-202239044] = "deermeat.cooked",
            [-322501005] = "deermeat.raw",
            [-1851058636] = "horsemeat.burned",
            [-1828062867] = "horsemeat.cooked",
            [-1966381470] = "horsemeat.raw",
            [968732481] = "meat.pork.burned",
            [991728250] = "meat.pork.cooked",
            [-253819519] = "meat.boar",
            [-1714986849] = "wolfmeat.burned",
            [-1691991080] = "wolfmeat.cooked",
            [179448791] = "wolfmeat.raw",
            [431617507] = "wolfmeat.spoiled",
            [688032252] = "metal.fragments",
            [-1059362949] = "metal.ore",
            [1265861812] = "metal.plate.torso",
            [374890416] = "metal.refined",
            [1567404401] = "metalblade",
            [-1057402571] = "metalpipe",
            [-758925787] = "mining.pumpjack",
            [-1411620422] = "mining.quarry",
            [88869913] = "fish.minnows",
            [-2094080303] = "smg.mp5",
            [843418712] = "mushroom",
            [-1569356508] = "weapon.mod.muzzleboost",
            [-1569280852] = "weapon.mod.muzzlebrake",
            [449769971] = "pistol.nailgun",
            [590532217] = "ammo.nailgun.nails",
            [3387378] = "note",
            [1767561705] = "burlap.trousers",
            [106433500] = "pants",
            [-1334615971] = "heavy.plate.pants",
            [-135651869] = "attire.hide.pants",
            [-1595790889] = "roadsign.kilt",
            [-459156023] = "pants.shorts",
            [106434956] = "paper",
            [-578028723] = "pickaxe",
            [-586116979] = "jar.pickle",
            [-1379225193] = "pistol.eoka",
            [-930579334] = "pistol.revolver",
            [548699316] = "pistol.semiauto",
            [142147109] = "planter.large",
            [148953073] = "planter.small",
            [102672084] = "attire.hide.poncho",
            [640562379] = "pookie.bear",
            [-1732316031] = "xmas.present.large",
            [-2130280721] = "xmas.present.medium",
            [-1725510067] = "xmas.present.small",
            [1974032895] = "propanetank",
            [-225085592] = "pumpkin",
            [509654999] = "clone.pumpkin",
            [466113771] = "seed.pumpkin",
            [2033918259] = "pistol.python",
            [2069925558] = "target.reactive",
            [-1026117678] = "box.repair.bench",
            [1987447227] = "research.table",
            [540154065] = "researchpaper",
            [1939428458] = "riflebody",
            [-288010497] = "roadsign.jacket",
            [-847065290] = "roadsigns",
            [3506021] = "rock",
            [649603450] = "rocket.launcher",
            [3506418] = "rope",
            [569935070] = "rug.bear",
            [113284] = "rug",
            [1916127949] = "water.salt",
            [-1775234707] = "salvaged.cleaver",
            [-388967316] = "salvaged.sword",
            [2007564590] = "santahat",
            [-1705696613] = "scarecrow",
            [670655301] = "hazmatsuit_scientist",
            [1148128486] = "hazmatsuit_scientist_peacekeeper",
            [-141135377] = "weapon.mod.small.scope",
            [109266897] = "scrap",
            [-527558546] = "searchlight",
            [-1745053053] = "rifle.semiauto",
            [1223860752] = "semibody",
            [-419069863] = "sewingkit",
            [-1617374968] = "sheetmetal",
            [2057749608] = "shelves",
            [24576628] = "shirt.collared",
            [-1659202509] = "shirt.tanktop",
            [2107229499] = "shoes.boots",
            [191795897] = "shotgun.double",
            [-1009492144] = "shotgun.pump",
            [2077983581] = "shotgun.waterpipe",
            [378365037] = "guntrap",
            [-529054135] = "shutter.metal.embrasure.a",
            [-529054134] = "shutter.metal.embrasure.b",
            [486166145] = "shutter.wood.a",
            [1628490888] = "sign.hanging.banner.large",
            [1498516223] = "sign.hanging",
            [-632459882] = "sign.hanging.ornate",
            [-626812403] = "sign.pictureframe.landscape",
            [385802761] = "sign.pictureframe.portrait",
            [2117976603] = "sign.pictureframe.tall",
            [1338515426] = "sign.pictureframe.xl",
            [-1455694274] = "sign.pictureframe.xxl",
            [1579245182] = "sign.pole.banner.large",
            [-587434450] = "sign.post.double",
            [-163742043] = "sign.post.single",
            [-1224714193] = "sign.post.town",
            [644359987] = "sign.post.town.roof",
            [-1962514734] = "sign.wooden.huge",
            [-705305612] = "sign.wooden.large",
            [-357728804] = "sign.wooden.medium",
            [-698499648] = "sign.wooden.small",
            [1213686767] = "weapon.mod.silencer",
            [386382445] = "weapon.mod.simplesight",
            [1859976884] = "skull_fire_pit",
            [960793436] = "skull.human",
            [1001265731] = "skull.wolf",
            [1253290621] = "sleepingbag",
            [470729623] = "small.oil.refinery",
            [1051155022] = "stash.small",
            [865679437] = "fish.troutsmall",
            [927253046] = "smallwaterbottle",
            [109552593] = "smg.2",
            [-2092529553] = "smgbody",
            [691633666] = "snowball",
            [-2055888649] = "snowman",
            [621575320] = "shotgun.spas12",
            [-2118132208] = "spear.stone",
            [-1127699509] = "spear.wooden",
            [-685265909] = "spikes.floor",
            [552706886] = "spinner.wheel",
            [1835797460] = "metalspring",
            [-892259869] = "sticks",
            [-1623330855] = "stocking.large",
            [-1616524891] = "stocking.small",
            [789892804] = "stone.pickaxe",
            [-1289478934] = "stonehatchet",
            [-892070738] = "stones",
            [-891243783] = "sulfur",
            [889398893] = "sulfur.ore",
            [-1625468793] = "supply.signal",
            [1293049486] = "surveycharge",
            [1369769822] = "fishtrap.small",
            [586484018] = "syringe.medical",
            [110115790] = "table",
            [1490499512] = "targeting.computer",
            [3552619] = "tarp",
            [1471284746] = "techparts",
            [456448245] = "smg.thompson",
            [110547964] = "torch",
            [1588977225] = "xmas.decoration.baubels",
            [918540912] = "xmas.decoration.candycanes",
            [-471874147] = "xmas.decoration.gingerbreadmen",
            [205978836] = "xmas.decoration.lights",
            [-1044400758] = "xmas.decoration.pinecone",
            [-2073307447] = "xmas.decoration.star",
            [435230680] = "xmas.decoration.tinsel",
            [-864578046] = "tshirt",
            [1660607208] = "tshirt.long",
            [260214178] = "tunalight",
            [-1847536522] = "vending.machine",
            [-496055048] = "wall.external.high.stone",
            [-1792066367] = "wall.external.high",
            [562888306] = "wall.frame.cell.gate",
            [-427925529] = "wall.frame.cell",
            [995306285] = "wall.frame.fence.gate",
            [-378017204] = "wall.frame.fence",
            [447918618] = "wall.frame.garagedoor",
            [313836902] = "wall.frame.netting",
            [1175970190] = "wall.frame.shopfront",
            [525244071] = "wall.frame.shopfront.metal",
            [-1021702157] = "wall.window.bars.metal",
            [-402507101] = "wall.window.bars.toptier",
            [-1556671423] = "wall.window.bars.wood",
            [61936445] = "wall.window.glass.reinforced",
            [112903447] = "water",
            [1817873886] = "water.catcher.large",
            [1824679850] = "water.catcher.small",
            [-1628526499] = "water.barrel",
            [547302405] = "waterjug",
            [1840561315] = "water.purifier",
            [-460592212] = "xmas.window.garland",
            [3655341] = "wood",
            [1554697726] = "wood.armor.jacket",
            [-1883959124] = "wood.armor.pants",
            [-481416622] = "workbench1",
            [-481416621] = "workbench2",
            [-481416620] = "workbench3",
            [-1151126752] = "xmas.lightstring",
            [-1926458555] = "xmas.tree"
        };

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

        private string[] GetKits(Func<string, bool> predicate = null) =>
            _data.Kits.Where(x => predicate?.Invoke(x.Name) ?? true).Select(x => x.Name).ToArray();

        private void GiveKit(BasePlayer player, string name) => _data[name]?.Give(player);

        // Deprecated
        private string[] GetAllKits()
        {
            PrintWarning("WARNING: The \"isKit\" API method is deprecated and will be removed on July 1st, 2018.");
            return _data.Kits.Select(x => x.Name).ToArray();
        }

        // Deprecated
        private bool isKit(string name)
        {
            PrintWarning("WARNING: The \"isKit\" API method is deprecated and will be removed on July 1st, 2018.");
            return IsKit(name);
        }

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
                            || _invalidNames.Contains(name))
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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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

                case "give":
                    {
                        if (!HasPermission(player))
                        {
                            Message(player, "PermissionError");
                            return;
                        }

                        if (args.Length < 2)
                        {
                            goto case "help";
                        }

                        var kit = _data[args[1].ToLower()];
                        if (kit == null)
                        {
                            Message(player, "Doesn'tExistError", args[1].ToLower());
                            return;
                        }

                        if (args.Length > 2)
                        {
                            var name = string.Join(" ", args.Skip(2).ToArray());
                            var players = BasePlayer.activePlayerList.Where(x => x.displayName.Contains(name)
                                                                            && x != player).ToArray();
                            if (players.Length != 1)
                            {
                                Message(player, players.Any() ? "MultiplePlayers" : "NoPlayers", name, players.Select(x => x.displayName).ToSentence());
                                return;
                            }

                            var target = players[0];
                            Message(player, "Given", target.displayName, kit.Name);
                            Message(target, "GivenToYou", player.displayName, kit.Name);
                            kit.Give(target);
                            return;
                        }

                        Message(player, "GivenAll", kit.Name);
                        foreach (var target in BasePlayer.activePlayerList.Where(x => x != player))
                        {
                            Message(target, "GivenToYou", player.displayName, kit.Name);
                            kit.Give(target);
                        }

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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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
                            Message(player, "Doesn'tExistError", args[1].ToLower());
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
                            Message(player, "Doesn'tExistError", args[0].ToLower());
                            return;
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
            Instance.permission.UserHasPermission(player.UserIDString, permission)
            || player.IsAdmin;

        private void Message(BasePlayer player, string key, params object[] args) =>
            PrintToChat(player, covalence.FormatText(lang.GetMessage(key, this, player.UserIDString)), args);

        private void MigrateData()
        {
            if (!Interface.Oxide.DataFileSystem.ExistsDatafile("Kits")
                || _data.Kits.Any())
            {
                return;
            }

            PrintWarning("Data successfully migrated. For full functionality it's suggested you refer to https://github.com/jacobmstein/Kits/blob/master/README.md#migrating.");
            var kits = Interface.Oxide.DataFileSystem.ReadObject<Dictionary<string, Dictionary<string, OldKit>>>("Kits");
            _data.Kits = new HashSet<Kit>(kits["Kits"].Select(x =>
            {
                var kit = x.Value.ToKit();
                kit.Name = x.Key.ToLower();
                return kit;
            }));

            foreach (var kit in _data.Kits)
            {
                permission.RegisterPermission($"kits.{kit.Name}", this);
            }
        }

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
            ["Given"] = "Kit [#ADD8E6]{0}[/#] successfully given to player [#ADD8E6]{1}[/#].",
            ["GivenAll"] = "Kit [#ADD8E6]{0}[/#] successfully given to all players.",
            ["GivenToYou"] = "[#ADD8E6]{0}[/#] gave you kit [#ADD8E6]{1}[/#].",
            ["Help"] = "...",
            ["InvalidName"] = "Error, invalid name.",
            ["LimitError"] = "Error, you reached the usage limit for kit [#ADD8E6]{0}[/#].",
            ["LimitSet"] = "Limit for kit [#ADD8E6]{0}[/#] successfully set to [#ADD8E6]{1}[/#].",
            ["List"] = "<size=16>Kit List</size>\nThe following kit{0} are available: [#ADD8E6]{1}[/#].",
            ["MultiplePlayers"] = "Error, the following players were found by the name [#ADD8E6]{0}[/#]: [#ADD8E6]{1}[/#].",
            ["NoPlayers"] = "Error, no players were found by the name [#ADD8E6]{0}[/#].",
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

            var result = Interface.CallHook("CanGiveDefaultKit", player);
            result = Interface.CallHook("CanGiveDefaultKit", player, kit.Name) ?? result;
            if (result != null)
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

        private void OnServerInitialized() => MigrateData();

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

            public Kit this[string name] => Kits.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

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

        private class OldKit
        {
            [JsonProperty("cooldown")]
            public double Cooldown { get; set; }

            [JsonProperty("items")]
            public HashSet<OldKitItem> Items { get; set; }

            [JsonProperty("max")]
            public int Max { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            public Kit ToKit() => new Kit
            {
                Cooldown = Convert.ToInt64(Cooldown),
                Items = new HashSet<KitItem>(Items.Select(x => x.ToKitItem())),
                Limit = Max
            };
        }

        private class OldKitItem
        {
            [JsonProperty("amount")]
            public int Amount { get; set; }

            [JsonProperty("mods")]
            public HashSet<int> Attachments { get; set; } = new HashSet<int>();

            [JsonProperty("container")]
            public Container Container { get; set; }

            [JsonProperty("itemid")]
            public int ItemId { get; set; }

            [JsonProperty("skinId")]
            public ulong SkinId { get; set; }

            public KitItem ToKitItem()
            {
                var item = ItemManager.FindItemDefinition(Instance._itemIdShortnameConversions[ItemId]);
                return new KitItem
                {
                    Amount = Amount,
                    Container = Container,
                    Contents = new HashSet<KitItem>(Attachments.Select(x => new KitItem
                    {
                        Amount = 1,
                        Container = Container.Main,
                        Shortname = Instance._itemIdShortnameConversions[x]
                    })),
                    Shortname = item.shortname,
                    SkinId = SkinId
                };
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
                var magazine = (item.GetHeldEntity() as BaseProjectile)?.primaryMagazine;
                if (magazine != null)
                {
                    AmmoType = magazine.ammoType.shortname;
                }

                Amount = item.amount;
                Blueprint = item.IsBlueprint();
                var parent = item.parent;
                Container = parent.HasFlag(ItemContainer.Flag.Belt)
                                    ? Container.Belt
                                    : (parent.HasFlag(ItemContainer.Flag.Clothing)
                                        ? Container.Wear
                                        : Container.Main);
                if (item.contents != null)
                {
                    foreach (var content in item.contents.itemList)
                    {
                        Contents.Add(new KitItem(content));
                    }
                }

                Position = item.position;
                Shortname = Blueprint
                                ? item.blueprintTargetDef.shortname
                                : item.info.shortname;
                SkinId = item.skin;
            }

            [JsonProperty("ammoType")]
            public string AmmoType { get; set; }

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
                var item = ItemManager.CreateByName(Blueprint
                                                        ? "blueprintbase"
                                                        : Shortname, Amount, SkinId);
                if (Blueprint)
                {
                    item.blueprintTarget = ItemManager.FindItemDefinition(Shortname).itemid;
                    return item;
                }

                if (!string.IsNullOrEmpty(AmmoType))
                {
                    var magazine = (item.GetHeldEntity() as BaseProjectile)?.primaryMagazine;
                    magazine.ammoType = ItemManager.FindItemDefinition(AmmoType);
                    magazine.contents = magazine.capacity;
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
                if (item.MoveToContainer(container, Position) || item.MoveToContainer(container))
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
                    return;
                }

                Redemptions.Add(name, 1);
            }

            public bool HasCooldown(string name, out long seconds)
            {
                if (Cooldowns.TryGetValue(name, out seconds)
                    && seconds > Instance.Epoch())
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
