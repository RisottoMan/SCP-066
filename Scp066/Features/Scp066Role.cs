﻿using System.Collections.Generic;
using System.Text;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pools;
using Exiled.API.Features.Spawn;
using Exiled.CustomRoles.API.Features;
using PlayerRoles;
using Scp066.Features.Controller;
using UnityEngine;

namespace Scp066.Features;
public class Scp066Role : CustomRole
{
    public override string Name { get; set; } = "SCP-066";
    public override string Description { get; set; } = "Eric's Toy";
    public override string CustomInfo { get; set; } = "SCP-066";
    public override uint Id { get; set; } = 660;
    public override int MaxHealth { get; set; } = 2000;
    public override SpawnProperties SpawnProperties { get; set; } = new()
    {
        Limit = 1,
        DynamicSpawnPoints = [new DynamicSpawnPoint { Chance = 100, Location = SpawnLocationType.Inside173Gate }]
    };
    
    public override bool KeepPositionOnSpawn { get; set; } = true;
    public override bool KeepInventoryOnSpawn { get; set; } = false;
    public override bool RemovalKillsPlayer { get; set; } = true;
    public override bool KeepRoleOnDeath { get; set; } = false;
    public override bool IgnoreSpawnSystem { get; set; } = true;
    public override bool KeepRoleOnChangingRole { get; set; } = false;
    public override RoleTypeId Role { get; set; } = RoleTypeId.Scp0492;
    
    public override Exiled.API.Features.Broadcast Broadcast { get; set; } = new()
    {
        Show = true,
        Content = 
            "<color=red>\ud83c\udfb5 You are SCP-066 - Eric's Toy \ud83c\udfb5\n" +
            "Play sounds to kill humans\n" +
            "Use abilities by clicking on the buttons</color>",
        Duration = 15
    };

    protected override void ShowMessage(Player player) {}
    
    public override string ConsoleMessage { get; set; } =
        "You are SCP-066 - Eric's Toy!\n" +
        "Play sounds to kill humans\n" +
        "Configure your buttons in the settings. Remove the stars.";
    
    /// <summary>
    /// Adding the SCP-066 role to the player
    /// </summary>
    /// <param name="player">The player who should become SCP-066</param>
    public override void AddRole(Player player)
    {
	  player.Role.Set(this.Role, SpawnReason.ForceClass, RoleSpawnFlags.All);
      player.Position = SpawnProperties.DynamicSpawnPoints.RandomItem().Position;
      
      player.ClearItems();
      player.ClearAmmo();
      player.UniqueRole = this.Name;
      this.TrackedPlayers.Add(player);
      player.Health = this.MaxHealth;
      player.MaxHealth = this.MaxHealth;
      player.Scale = this.Scale;
      player.CustomName = this.Name;
      player.CustomInfo = player.CustomName + "\n" + this.CustomInfo;
      
      this.ShowMessage(player);
      this.ShowBroadcast(player);
      this.RoleAdded(player);
      player.SendConsoleMessage(this.ConsoleMessage, "green");
      
      player.EnableEffect<Disabled>();
      player.EnableEffect<Slowness>(intensity: 50);
      player.EnableEffect<SilentWalk>(intensity: 50);
      
      // Register PlayerComponent for player
      player.GameObject.AddComponent<PlayerController>();
    }

    /// <summary>
    /// Remove the role from the player
    /// </summary>
    /// <param name="player">A player who should become normal role</param>
    public override void RemoveRole(Player player)
    {
        // Remove a custom role
        base.RemoveRole(player);
        player.CustomName = null;
        
        // Unregister PlayerComponent for player
        Object.Destroy(player.GameObject.GetComponent<PlayerController>());
    }
}