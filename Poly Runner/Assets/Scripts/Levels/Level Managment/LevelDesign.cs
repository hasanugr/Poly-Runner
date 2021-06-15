using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Cytris/Level Design")]
public class LevelDesign : ScriptableObject
{
    public int LevelNumber;
    public List<RoadObject> RoadObjects = new List<RoadObject>();
    public List<StaticObstalceObject> StaticObstacleObjects = new List<StaticObstalceObject>();
    public List<DynamicObstalceObject> DynamicObstacleObjects = new List<DynamicObstalceObject>();
    public List<CoinObject> CoinObjects = new List<CoinObject>();
    public Variables StartLine = new Variables();
    public Variables FinishLine = new Variables();

    public enum RoadType { SnowyRoad, IceRoad, SlopeRoad, CaveEnter };
    public enum CoinType { Diamond };
    public enum StaticObstacleType { BarricadeTank, Bulldozer, CargoShipping, HighRoad, HighRoadWithRamp, HorseRest, HorseWagon, RockPiller, RockSharp, SandBags, Snowman, Stalags, Timber, TreeTrunk, TrenchesWood };
    public enum DynamicObstacleType { FallingTree, BreakibgIce, JumpingShark, BigSeagulAttack, MultipleSeagulAttack, RollingSnowball, RollingTreeTrunk, RunningDeer };


    public void AddRoadObject(RoadType type, List<Variables> objects)
    {
        RoadObject thisObject = new RoadObject();
        thisObject.type = type;
        thisObject.objects = objects;
        RoadObjects.Add(thisObject);
    }
    public void AddStaticObstacleObject(StaticObstacleType type, List<Variables> objects)
    {
        StaticObstalceObject thisObject = new StaticObstalceObject();
        thisObject.type = type;
        thisObject.objects = objects;
        StaticObstacleObjects.Add(thisObject);
    }
    public void AddDynamicObstacleObject(DynamicObstacleType type, List<Variables> objects)
    {
        DynamicObstalceObject thisObject = new DynamicObstalceObject();
        thisObject.type = type;
        thisObject.objects = objects;
        DynamicObstacleObjects.Add(thisObject);
    }
    public void AddCoinObject(CoinType type, List<Variables> objects)
    {
        CoinObject thisObject = new CoinObject();
        thisObject.type = type;
        thisObject.objects = objects;
        CoinObjects.Add(thisObject);
    }
    public void AddStartLine(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        StartLine.position = pos;
        StartLine.rotation = rot;
        StartLine.scale = scale;
    }
    public void AddFinishLine(Vector3 pos, Quaternion rot, Vector3 scale)
    {
        FinishLine.position = pos;
        FinishLine.rotation = rot;
        FinishLine.scale = scale;
    }

    public void Clear()
    {
        RoadObjects.Clear();
        StaticObstacleObjects.Clear();
        DynamicObstacleObjects.Clear();
        CoinObjects.Clear();
    }


    [System.Serializable]
    public class RoadObject
    {
        public RoadType type;
        public List<Variables> objects = new List<Variables>();
    }

    [System.Serializable]
    public class StaticObstalceObject
    {
        public StaticObstacleType type;
        public List<Variables> objects = new List<Variables>();
    }

    [System.Serializable]
    public class DynamicObstalceObject
    {
        public DynamicObstacleType type;
        public List<Variables> objects = new List<Variables>();
    }

    [System.Serializable]
    public class CoinObject
    {
        public CoinType type;
        public List<Variables> objects = new List<Variables>();
    }

    [System.Serializable]
    public class Variables
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.zero;
    }
}
