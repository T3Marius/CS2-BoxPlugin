using BoxConfig;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Capabilities;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using static BoxConfig.Config_Config;

namespace BoxPlugin
{
    public class BoxPlugin : BasePlugin
    {
        public override string ModuleName => "cs2-box";
        public override string ModuleAuthor => "T3Marius";
        public override string ModuleVersion => "1.0.0";

        private bool isFriendlyFireForTerroristsActive = false;

        public static PluginCapability<IWardenService> wardenService { get; set; } = new("jailbreak:warden_service");

        public override void Load(bool hotReload)
        {
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Hook(OnTakeDamage, HookMode.Pre);
            Config_Config.Load();
        }
        public override void Unload(bool hotReload)
        {
            VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Unhook(OnTakeDamage, HookMode.Pre);
        }

        [ConsoleCommand("css_box")]
        public void OnBoxCommand(CCSPlayerController player, CommandInfo info)
        {
            var service = wardenService.Get();

            if (service != null && player != null && service.IsWarden(player))
            {
                isFriendlyFireForTerroristsActive = !isFriendlyFireForTerroristsActive;

                if (isFriendlyFireForTerroristsActive)
                {
                    ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() = true;
                    Server.PrintToChatAll(Localizer["tag.prefix"] + Localizer["ff_for_terrorists.enabled"]);
                    Utilities.GetPlayers().ForEach(p => p.ExecuteClientCommand($"play {Config.Sound}"));
                }
                else
                {
                    Server.PrintToChatAll(Localizer["tag.prefix"] + Localizer["ff_for_terrorists.disabled"]);
                    ConVar.Find("mp_teammates_are_enemies")!.GetPrimitiveValue<bool>() = false;
                }               
            }
            else if (player != null && (player.Team == CsTeam.Terrorist || player.Team == CsTeam.CounterTerrorist))
            {
                player.PrintToChat(Localizer["tag.prefix"] + Localizer["t.warning"]);
            }
        }

        public HookResult OnTakeDamage(DynamicHook hook)
        {
            CEntityInstance entity = hook.GetParam<CEntityInstance>(0);
            CTakeDamageInfo info = hook.GetParam<CTakeDamageInfo>(1);

            var ability = info.Ability.Value;
            if (ability == null)
            {
                return HookResult.Continue;
            }

            if (entity.DesignerName != "player")
            {
                return HookResult.Continue;
            }

            var attacker = new CCSPlayerController(ability.Handle);
            var victim = new CCSPlayerController(entity.Handle);

            // Handle friendly fire logic for Terrorists only
            if (isFriendlyFireForTerroristsActive && attacker.Team == victim.Team && victim.Team != CsTeam.Terrorist)
            {
                return HookResult.Handled;
            }

            return HookResult.Continue;
        }

        [ConsoleCommand("css_ding")]
        public void OnDingCommand(CCSPlayerController player, CommandInfo info)
        {
            var service = wardenService.Get();

            if (service != null && player != null && service.IsWarden(player))
            {
                Utilities.GetPlayers().ForEach(p =>
                {
                    p.ExecuteClientCommand($"play {Config.Sound}");
                });
            }
            else if (player != null && (player.Team == CsTeam.Terrorist || player.Team == CsTeam.CounterTerrorist))
            {
                player.PrintToChat(Localizer["tag.prefix"] + Localizer["t.warning"]);
            }
        }
    }
}