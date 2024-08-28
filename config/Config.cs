using CounterStrikeSharp.API;
using System.Reflection;
using Tomlyn;
using Tomlyn.Model;

namespace BoxConfig;
public static class Config_Config
{
    public static Cfg Config { get; set; } = new Cfg();
    public static void Load()
    {
        string assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? "";
        string cfgPath = $"{Server.GameDirectory}/csgo/addons/counterstrikesharp/configs/plugins/{assemblyName}";

        LoadConfig($"{cfgPath}/config.toml");
    }

    private static void LoadConfig(string configPath)
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        string configText = File.ReadAllText(configPath);
        TomlTable model = Toml.ToModel(configText);

        TomlTable tb = (TomlTable)model["Sound"];
        Config.Sound = tb["Sound"].ToString()!;
    }
    public class Cfg
    {
        public string Sound { get; set; } = string.Empty;
    }
}
