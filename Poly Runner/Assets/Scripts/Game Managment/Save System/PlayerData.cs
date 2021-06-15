using System;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PlayerData
{
    // Player Game Data
    private int[] _levels;
    private int _highScore;
    private int _gold;
    private int _selectedVehicleId;
    private List<List<int[]>> vehicleList;

    // Player Game Prefs
    private bool _music;
    private bool _sound;
    private int _vehicleControlType;


    public int selectedVehicleId {
        get
        {
            return _selectedVehicleId;
        }
        set
        {
            _selectedVehicleId = value;
        }
    }

    public int highScore { 
        get
        {
            return _highScore;
        }
        set
        {
            _highScore = value;
        }
    }

    public int gold { 
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
        }
    }

    public int[] levels 
    { 
        get
        {
            return _levels;
        }
    }

    public bool music
    {
        get
        {
            return _music;
        }
        set
        {
            _music = value;
        }
    }

    public bool sound
    {
        get
        {
            return _sound;
        }
        set
        {
            _sound = value;
        }
    }

    public int vehicleControlType
    {
        get
        {
            return _vehicleControlType;
        }
        set
        {
            _vehicleControlType = value;
        }
    }



    public void AddDefaultValues()
    {
        _levels = new int[1];
        _highScore = 0;
        _gold = 0;
        _selectedVehicleId = 1;
        ActivateVehicle(_selectedVehicleId);

        _music = true;
        _sound = true;
        _vehicleControlType = 2;
    }

    public void AddLoadedValues(PlayerData pd)
    {
        _levels = pd._levels;
        _highScore = pd._highScore;
        _gold = pd._gold;
        _selectedVehicleId = pd._selectedVehicleId;
        vehicleList = pd.vehicleList;

        _music = pd._music;
        _sound = pd._sound;
        _vehicleControlType = pd._vehicleControlType;
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

    public bool IsVehicleActive(int vehicleId)
    {
        if (vehicleList == null)
        {
            ActivateVehicle(_selectedVehicleId);
        }

        bool result = false;
        
        foreach (List<int[]> item in vehicleList)
        {
            if (item[0][0] == vehicleId)
            {
                result = true;
            }
        }

        return result;
    }

    public void ActivateVehicle(int vehicleId)
    {
        if (vehicleList == null)
        {
            vehicleList = new List<List<int[]>>();
        }

        vehicleList.Add(new List<int[]>(AddVehicleValues(vehicleId)));
    }

    public void ActivateColorVehicle(int vehicleIndex, int colorIndex)
    {
        vehicleList[vehicleIndex][1][colorIndex] = 1;
    }

    public void ChangeColorVehicle(int vehicleIndex, int colorIndex)
    {
        if (vehicleList[vehicleIndex][1][colorIndex] == 1)
        {
            vehicleList[vehicleIndex][0][1] = colorIndex;
        }
    }

    private List<int[]> AddVehicleValues(int vehicleId)
    {
        List<int[]> tempList = new List<int[]>();

        int[] vehiclePrefs = new int[2];
        vehiclePrefs[0] = vehicleId; // Vehicle ID
        vehiclePrefs[1] = 0; // selectedColorIndex

        int[] vehicleColors = new int[3];
        vehicleColors[0] = 1; // First Color
        vehicleColors[1] = 0; // Second Color
        vehicleColors[2] = 0; // Thirth Color

        tempList.Add(vehiclePrefs.ToArray<int>());
        tempList.Add(vehicleColors.ToArray<int>());

        return tempList;
    }

    private int FindVehicleIndex(int vehicleId)
    {
        int index = 0;

        foreach (List<int[]> item in vehicleList)
        {
            if (item[0][0] == vehicleId)
            {
                break;
            }
            index++;
        }

        return index;
    }

}
