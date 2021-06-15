using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum CollectedDataType { Roads, StaticObstacles, DynamicObstacles, Coins };
public class AddLevelDatas : MonoBehaviour
{
    public LevelDesign levelDesignScriptableObject;

    [Header("Holders")]
    public GameObject RoadObjectsHolder;
    public GameObject StaticObstacleObjectsHolder;
    public GameObject DynamicObstacleObjectsHolder;
    public GameObject CoinObjectsHolder;
    public GameObject StartLine;
    public GameObject FinishLine;

    [Header("Loaded Objects")]
    public int LevelNumber;
    public List<RoadObject> RoadObjects = new List<RoadObject>();
    public List<StaticObstalceObject> StaticObstacleObjects = new List<StaticObstalceObject>();
    public List<DynamicObstalceObject> DynamicObstacleObjects = new List<DynamicObstalceObject>();
    public List<CoinObject> CoinObjects = new List<CoinObject>();
    public Variables StartLineVariables = new Variables();
    public Variables FinishLineVariables = new Variables();

    public void LoadFromScene()
    {
        Clear(false);

        LevelNumber = 0;
        CollectRoadDataFromScene(RoadObjectsHolder);
        CollectStaticObstacleDataFromScene(StaticObstacleObjectsHolder);
        CollectDynamicObstacleDataFromScene(DynamicObstacleObjectsHolder);
        CollectCoinDataFromScene(CoinObjectsHolder);
        StartLineVariables = CollectAnObjectVariables(StartLine);
        FinishLineVariables = CollectAnObjectVariables(FinishLine);
    }

    private void CollectRoadDataFromScene(GameObject holder)
    {
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            RoadObject newObject = new RoadObject();

            newObject.type = (LevelDesign.RoadType)i;
            Transform thisObject = holder.transform.GetChild(i);

            for (int j = 0; j < thisObject.childCount; j++)
            {
                GameObject thisSubObject = thisObject.transform.GetChild(j).gameObject;

                if (thisSubObject.activeSelf)
                {
                    LevelDesign.Variables thisObjectVariables = new LevelDesign.Variables();

                    thisObjectVariables.position = thisSubObject.transform.localPosition;
                    thisObjectVariables.rotation = thisSubObject.transform.localRotation;
                    thisObjectVariables.scale = thisSubObject.transform.localScale;
                    newObject.objects.Add(thisObjectVariables);
                }
            }

            RoadObjects.Add(newObject);
        }
    }
    private void CollectStaticObstacleDataFromScene(GameObject holder)
    {
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            StaticObstalceObject newObject = new StaticObstalceObject();

            newObject.type = (LevelDesign.StaticObstacleType)i;
            Transform thisObject = holder.transform.GetChild(i);

            for (int j = 0; j < thisObject.childCount; j++)
            {
                GameObject thisSubObject = thisObject.transform.GetChild(j).gameObject;

                if (thisSubObject.activeSelf)
                {
                    LevelDesign.Variables thisObjectVariables = new LevelDesign.Variables();

                    thisObjectVariables.position = thisSubObject.transform.localPosition;
                    thisObjectVariables.rotation = thisSubObject.transform.localRotation;
                    thisObjectVariables.scale = thisSubObject.transform.localScale;
                    newObject.objects.Add(thisObjectVariables);
                }
            }

            StaticObstacleObjects.Add(newObject);
        }
    }
    private void CollectDynamicObstacleDataFromScene(GameObject holder)
    {
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            DynamicObstalceObject newObject = new DynamicObstalceObject();

            newObject.type = (LevelDesign.DynamicObstacleType)i;
            Transform thisObject = holder.transform.GetChild(i);

            for (int j = 0; j < thisObject.childCount; j++)
            {
                GameObject thisSubObject = thisObject.transform.GetChild(j).gameObject;

                if (thisSubObject.activeSelf)
                {
                    LevelDesign.Variables thisObjectVariables = new LevelDesign.Variables();

                    thisObjectVariables.position = thisSubObject.transform.localPosition;
                    thisObjectVariables.rotation = thisSubObject.transform.localRotation;
                    thisObjectVariables.scale = thisSubObject.transform.localScale;
                    newObject.objects.Add(thisObjectVariables);
                }
            }

            DynamicObstacleObjects.Add(newObject);
        }
    }
    private void CollectCoinDataFromScene(GameObject holder)
    {
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            CoinObject newObject = new CoinObject();

            newObject.type = (LevelDesign.CoinType)i;
            Transform thisObject = holder.transform.GetChild(i);

            for (int j = 0; j < thisObject.childCount; j++)
            {
                GameObject thisSubObject = thisObject.transform.GetChild(j).gameObject;

                if (thisSubObject.activeSelf)
                {
                    LevelDesign.Variables thisObjectVariables = new LevelDesign.Variables();

                    thisObjectVariables.position = thisSubObject.transform.localPosition;
                    thisObjectVariables.rotation = thisSubObject.transform.localRotation;
                    thisObjectVariables.scale = thisSubObject.transform.localScale;
                    newObject.objects.Add(thisObjectVariables);
                }
            }

            CoinObjects.Add(newObject);
        }
    }
    private Variables CollectAnObjectVariables(GameObject objectItem)
    {
        Variables newVariable = new Variables();
        newVariable.position = objectItem.transform.localPosition;
        newVariable.rotation = objectItem.transform.localRotation;
        newVariable.scale = objectItem.transform.localScale;

        return newVariable;
    }

    public void SaveLevel()
    {
        levelDesignScriptableObject.Clear();

        levelDesignScriptableObject.LevelNumber = LevelNumber;
        levelDesignScriptableObject.AddStartLine(StartLineVariables.position, StartLineVariables.rotation, StartLineVariables.scale);
        levelDesignScriptableObject.AddFinishLine(FinishLineVariables.position, FinishLineVariables.rotation, FinishLineVariables.scale);
        for (int i = 0; i < RoadObjects.Count; i++)
        {
            levelDesignScriptableObject.AddRoadObject(RoadObjects[i].type, RoadObjects[i].objects);
        }
        for (int i = 0; i < StaticObstacleObjects.Count; i++)
        {
            levelDesignScriptableObject.AddStaticObstacleObject(StaticObstacleObjects[i].type, StaticObstacleObjects[i].objects);
        }
        for (int i = 0; i < DynamicObstacleObjects.Count; i++)
        {
            levelDesignScriptableObject.AddDynamicObstacleObject(DynamicObstacleObjects[i].type, DynamicObstacleObjects[i].objects);
        }
        for (int i = 0; i < CoinObjects.Count; i++)
        {
            levelDesignScriptableObject.AddCoinObject(CoinObjects[i].type, CoinObjects[i].objects);
        }
    }


    public void LoadFromObject()
    {
        LevelNumber = levelDesignScriptableObject.LevelNumber;
        // Load From Scriptableobject..
    }
    public void Clear(bool isAll)
    {
        if (isAll)
        {
            levelDesignScriptableObject = null;
        }
        LevelNumber = 0;
        StartLineVariables = null;
        FinishLineVariables = null;
        RoadObjects.Clear();
        StaticObstacleObjects.Clear();
        DynamicObstacleObjects.Clear();
        CoinObjects.Clear();
    }

    #region Special Classes
    [System.Serializable]
    public class RoadObject
    {
        public LevelDesign.RoadType type;
        public List<LevelDesign.Variables> objects = new List<LevelDesign.Variables>();
    }

    [System.Serializable]
    public class StaticObstalceObject
    {
        public LevelDesign.StaticObstacleType type;
        public List<LevelDesign.Variables> objects = new List<LevelDesign.Variables>();
    }

    [System.Serializable]
    public class DynamicObstalceObject
    {
        public LevelDesign.DynamicObstacleType type;
        public List<LevelDesign.Variables> objects = new List<LevelDesign.Variables>();
    }

    [System.Serializable]
    public class CoinObject
    {
        public LevelDesign.CoinType type;
        public List<LevelDesign.Variables> objects = new List<LevelDesign.Variables>();
    }

    [System.Serializable]
    public class Variables
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.zero;
    }
    #endregion

    /*private void AddRoadObjects()
    {
        for (int i = 0; i < RoadObjects.Length; i++)
        {
            LevelDesign.RoadObject thisObject = new LevelDesign.RoadObject();

            thisObject.type = GetRoadType(RoadObjects[i].name);
            thisObject.position = RoadObjects[i].transform.localPosition;
            thisObject.rotation = RoadObjects[i].transform.localRotation;
            thisObject.scale = RoadObjects[i].transform.localScale;

            levelDesignScriptableObject.RoadObjects.Add(thisObject);
        }
    }
    private LevelDesign.RoadType GetRoadType(string name)
    {

        string[] splittedName = name.Split('-');
        switch (splittedName[0])
        {
            case "1":
                return LevelDesign.RoadType.SnowyRoad;
            case "2":
                return LevelDesign.RoadType.IceRoad;
            case "3":
                return LevelDesign.RoadType.SlopeRoad;
            case "4":
                return LevelDesign.RoadType.CaveEnter;
            default:
                return LevelDesign.RoadType.SnowyRoad;
        }
    }

    private void AddCoinObjects()
    {
        for (int i = 0; i < CoinObjects.Length; i++)
        {
            LevelDesign.CoinObject thisObject = new LevelDesign.CoinObject();

            thisObject.type = GetCoinType(CoinObjects[i].name);
            thisObject.position = CoinObjects[i].transform.localPosition;
            thisObject.rotation = CoinObjects[i].transform.localRotation;
            thisObject.scale = CoinObjects[i].transform.localScale;

            levelDesignScriptableObject.CoinObjects.Add(thisObject);
        }
    }
    private LevelDesign.CoinType GetCoinType(string name)
    {

        string[] splittedName = name.Split('-');
        switch (splittedName[0])
        {
            case "1":
                return LevelDesign.CoinType.Diamond;
            default:
                return LevelDesign.CoinType.Diamond;
        }
    }

    private void AddStaticObstacleObjects()
    {
        for (int i = 0; i < StaticObstacleObjects.Length; i++)
        {
            LevelDesign.StaticObstalceObject thisObject = new LevelDesign.StaticObstalceObject();

            thisObject.type = GetStaticObstacleType(StaticObstacleObjects[i].name);
            thisObject.position = StaticObstacleObjects[i].transform.localPosition;
            thisObject.rotation = StaticObstacleObjects[i].transform.localRotation;
            thisObject.scale = StaticObstacleObjects[i].transform.localScale;

            levelDesignScriptableObject.StaticObstacleObjects.Add(thisObject);
        }
    }
    private LevelDesign.StaticObstacleType GetStaticObstacleType(string name)
    {
        string[] splittedName = name.Split('-');
        switch (splittedName[0])
        {
            case "1":
                return LevelDesign.StaticObstacleType.BarricadeTank;
            case "2":
                return LevelDesign.StaticObstacleType.Bulldozer;
            case "3":
                return LevelDesign.StaticObstacleType.CargoShipping;
            case "4":
                return LevelDesign.StaticObstacleType.HighRoad;
            case "5":
                return LevelDesign.StaticObstacleType.HighRoadWithRamp;
            case "6":
                return LevelDesign.StaticObstacleType.HorseRest;
            case "7":
                return LevelDesign.StaticObstacleType.HorseWagon;
            case "8":
                return LevelDesign.StaticObstacleType.SandBags;
            case "9":
                return LevelDesign.StaticObstacleType.Timber;
            case "10":
                return LevelDesign.StaticObstacleType.TreeTrunk;
            case "11":
                return LevelDesign.StaticObstacleType.TrenchesWood;
            case "12":
                return LevelDesign.StaticObstacleType.Snowman;
            case "13":
                return LevelDesign.StaticObstacleType.RockPiller;
            case "14":
                return LevelDesign.StaticObstacleType.RockSharp;
            case "15":
                return LevelDesign.StaticObstacleType.Stalags;
            default:
                return LevelDesign.StaticObstacleType.Timber;
        }
    }

    private void AddDynamicObstacleObjects()
    {
        for (int i = 0; i < DynamicObstacleObjects.Length; i++)
        {
            LevelDesign.DynamicObstalceObject thisObject = new LevelDesign.DynamicObstalceObject();

            thisObject.type = GetDynamicObstacleType(DynamicObstacleObjects[i].name);
            thisObject.position = DynamicObstacleObjects[i].transform.localPosition;
            thisObject.rotation = DynamicObstacleObjects[i].transform.localRotation;
            thisObject.scale = DynamicObstacleObjects[i].transform.localScale;

            levelDesignScriptableObject.DynamicObstacleObjects.Add(thisObject);
        }
    }
    private LevelDesign.DynamicObstacleType GetDynamicObstacleType(string name)
    {

        string[] splittedName = name.Split('-');
        switch (splittedName[0])
        {
            case "1":
                return LevelDesign.DynamicObstacleType.FallingTree;
            case "2":
                return LevelDesign.DynamicObstacleType.BreakibgIce;
            case "3":
                return LevelDesign.DynamicObstacleType.JumpingShark;
            case "4":
                return LevelDesign.DynamicObstacleType.BigSeagulAttack;
            case "5":
                return LevelDesign.DynamicObstacleType.RunningDeer;
            case "6":
                return LevelDesign.DynamicObstacleType.MultipleSeagulAttack;
            case "7":
                return LevelDesign.DynamicObstacleType.RollingSnowball;
            case "8":
                return LevelDesign.DynamicObstacleType.RollingTreeTrunk;
            default:
                return LevelDesign.DynamicObstacleType.FallingTree;
        }
    }*/
}
