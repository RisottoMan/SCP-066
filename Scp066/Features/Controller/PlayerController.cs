﻿using Exiled.API.Features;
using LabApi.Features.Wrappers;
using MEC;
using ProjectMER.Features.Objects;
using Scp066.Configs;
using Scp066.Features.Manager;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace Scp066.Features.Controller;
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Register features for the player
    /// </summary>
    void Awake()
    {
        _player = Player.Get(gameObject);
        Config config = Plugin.Singleton.Config;
        
        _schematicObject = SchematicManager.AddSchematicByName(config.SchematicName);  // Create schematic
        _audioPlayer = AudioManager.AddAudioPlayer(_player, config.Volume);            // Create audioPlayer
        _audioPlayer.TryGetSpeaker("scp066-speaker", out Speaker speaker);        // Get speaker
        _textToy = TextToyManager.CreateTextForSchematic(_player, _schematicObject);   // Create TextToy

        _movementController = gameObject.AddComponent<MovementController>();
        _movementController.Init(_schematicObject, speaker, config.SchematicOffset);
        _cooldownController = gameObject.AddComponent<CooldownController>();

        Timing.CallDelayed(0.1f, () =>
        {
            _hintController = gameObject.AddComponent<HintController>();
            _hintController.Init(_player);
        });
        
        Log.Debug($"[PlayerController] Custom role granted for {_player.Nickname}");
    }
    
    /// <summary>
    /// Unregister features for the player
    /// </summary>
    void OnDestroy()
    {
        Destroy(_hintController);       // Destroy hints
        Destroy(_movementController);   // Destroy movement controller for schematic and audio
        Destroy(_cooldownController);   // Destroy cooldown for abilities

        _textToy.Destroy();             // Remove text toy
        _audioPlayer.RemoveAllClips();  // Remove all audio clips
        _audioPlayer.Destroy();         // Remove a AudioPlayer
        _schematicObject.Destroy();     // Remove schematic
        
        Log.Debug($"[PlayerController] Custom role removed for {_player.Nickname}");
    }
    
    // Properties
    public AudioPlayer GetCurrentAudioPlayer => _audioPlayer;

    // Fields
    private Player _player;
    private SchematicObject _schematicObject;
    private AudioPlayer _audioPlayer;
    private TextToy _textToy;
    private MovementController _movementController;
    private CooldownController _cooldownController;
    private HintController _hintController;
}