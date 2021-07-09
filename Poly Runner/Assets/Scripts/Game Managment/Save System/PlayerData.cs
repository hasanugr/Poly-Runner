using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{
    // Player Game Data
    private int[] _levels;
    private int _gold;
    private int _selectedCharacterId;
    private int[] _activatedCharactersId;

    // Player Game Prefs
    private bool _music;
    private bool _sound;


    public int[] Levels { get => _levels; set => _levels = value; }
    public int Gold { get => _gold; set => _gold = value; }
    public int SelectedCharacterId { get => _selectedCharacterId; set => _selectedCharacterId = value; }
    public int[] ActivatedCharactersId { get => _activatedCharactersId; set => _activatedCharactersId = value; }

    public bool Music { get => _music; set => _music = value; }
    public bool Sound { get => _sound; set => _sound = value; }

    public void AddDefaultValues()
    {
        _levels = new int[1]; // 1
        _gold = 0; // 0
        _selectedCharacterId = 0; // 0
        _activatedCharactersId = new int[1] { 0 }; // [1] { 0 }

        _music = true; // true
        _sound = true; // true
    }

    public void AddLoadedValues(PlayerData pd)
    {
        Levels = pd.Levels;
        Gold = pd.Gold;
        SelectedCharacterId = pd.SelectedCharacterId;
        ActivatedCharactersId = pd.ActivatedCharactersId;

        Music = pd.Music;
        Sound = pd.Sound;
    }

    public void NextLevel(int level, int star)
    {
        // Change the level star if level star bigger than older.
        if (star > _levels[level - 1])
        {
            _levels[level - 1] = star;
            // Open new level if this level is the last active level.
            if (level == _levels.Length)
            {
                Array.Resize(ref _levels, _levels.Length + 1);
            }
        }
    }

    public bool IsCharacterActive(int characterId)
    {
        if (_activatedCharactersId == null)
        {
            ActivateCharacter(_selectedCharacterId);
        }

        bool result = false;
        
        foreach (int itemId in _activatedCharactersId)
        {
            if (itemId == characterId)
            {
                result = true;
            }
        }

        return result;
    }

    public void ActivateCharacter(int characterId)
    {
        if (!IsCharacterActive(characterId))
        {
            Array.Resize(ref _activatedCharactersId, _activatedCharactersId.Length + 1);
            _activatedCharactersId[_activatedCharactersId.Length - 1] = characterId;
        }
    }

}