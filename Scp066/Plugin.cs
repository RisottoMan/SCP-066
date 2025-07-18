﻿using System;
using System.IO;
using System.Linq;
using HarmonyLib;
using Exiled.API.Features;
using Exiled.CustomRoles.API;
using PlayerRoles;
using Scp066.Configs;
using Scp066.Features.Manager;

namespace Scp066;
public class Plugin : Plugin<Config>
{
    public override string Name => "Scp066";
    public override string Author => "RisottoMan";
    public override Version Version => new(1, 2, 0);
    public override Version RequiredExiledVersion => new(9, 6, 1);
    
    private Harmony _harmony;
    private EventHandler _eventHandler;
    public static Plugin Singleton;
    
    // Configs path
    public string BasePath { get; set; }
    public string SchematicPath { get; set; }
    public string AudioPath { get; set; }

    public override void OnEnabled()
    {
        Singleton = this;
        _eventHandler = new EventHandler(this);

        // Patch
        _harmony = new Harmony($"risottoman.scp066");
        _harmony.PatchAll();
        
        // Checking that the ProjectMER plugin is loaded on the server
        if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.FullName.ToLower().Contains("projectmer")))
        {
            Log.Error("ProjectMER is not installed. Schematics can't spawn the SCP-066 game model.");
            return;
        }
        
        // Register the custom scp066 role
        Config.Scp066RoleConfig.Register();
        
        // Create config folders
        BasePath = Path.Combine(Paths.IndividualConfigs, Name.ToLower());
        SchematicPath = Path.Combine(BasePath, "Schematics");
        AudioPath = Path.Combine(BasePath, "Audio");
        CreatePluginDirectory(SchematicPath);
        CreatePluginDirectory(AudioPath);
        
        AbilityManager.RegisterAbilities();
        KeybindManager.RegisterKeybinds();

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        AbilityManager.UnregisterAbilities();
        KeybindManager.UnregisterKeybinds();

        Config.Scp066RoleConfig.Unregister();
        _harmony.UnpatchAll();

        _eventHandler = null;
        Singleton = null;
        
        base.OnDisabled();
    }

    private void CreatePluginDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}