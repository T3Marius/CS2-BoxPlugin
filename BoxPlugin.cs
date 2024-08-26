using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace BoxPlugin
{
    public class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("DingSound")]
        public string sound { get; set; } = "sounds/sankysounds/box.vsnd_c";
    }
    public class BoxPlugin : BasePlugin, IPluginConfig<PluginConfig>
    {
        public override string ModuleName => "cs2-box";
        public override string ModuleAuthor => "T3Marius";
        public override string ModuleVersion => "1.0.0";
        public PluginConfig Config { get; set; } = new PluginConfig();
        public void OnConfigParsed(PluginConfig config)
        {
            Config = config;
        }

        private bool isBoxModeActive = false;

        public override void Load(bool hotReload)
        {
            RegisterEventHandler<EventPlayerHurt>(OnPlayerHurt); 
        }
        public override void Unload(bool hotReload)
        {
            RemoveListener(OnPlayerHurt);
        }

        [ConsoleCommand("css_box")]
        public void OnBoxCommand(CCSPlayerController player, CommandInfo info)
        {
            if (player != null && player.Team == CsTeam.CounterTerrorist)
            {               
                isBoxModeActive = !isBoxModeActive;
          
                if (isBoxModeActive)
                {
                    Server.PrintToChatAll(Localizer["tag.prefix"] + Localizer["box.enabled"]);
                    player.ExecuteClientCommand($"play {Config.sound}");
                }
                else
                {
                    Server.PrintToChatAll(Localizer["tag.prefix"] + Localizer["box.disabled"]);
                }
            }
            else if (player != null && player.Team == CsTeam.Terrorist)
            {
                
                player.PrintToChat(Localizer["tag.prefix"] + Localizer["t.warning"]);
            }
        }

        private HookResult OnPlayerHurt(EventPlayerHurt @event, GameEventInfo info)
        {
            var attacker = @event.Attacker;
            var victim = @event.Userid;

            if (isBoxModeActive)
            {
                
                if (attacker != null && victim != null &&
                    attacker.Team == CsTeam.Terrorist &&
                    victim.Team == CsTeam.Terrorist)
                {
                    return HookResult.Continue;  
                }
                else
                {
                    return HookResult.Handled;  
                }
            }

            return HookResult.Continue; 
        }

        [ConsoleCommand("css_ding")]
        public void OnDingCommand(CCSPlayerController player, CommandInfo info)
        {
            if (player != null && player.Team == CsTeam.CounterTerrorist)
            {
                player.ExecuteClientCommand($"play {Config.sound}");
            }
            else if (player != null && player.Team == CsTeam.Terrorist)
            {
                player.PrintToChat(Localizer["tag.prefix"] + Localizer["t.warning"]);
            }
        }
    }
}
