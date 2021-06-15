using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    /**
     * Level kontroller tamamen index kontrolü ile eþleme yapar, LevelDesign.cs üzerindeki ENUM index'leri temel kabul edilir.
     * ENUM'lardaki sýralamalarý sahne içerisindeki sýralamada da kullanmamýz gerekiyor. (Level Controller > Roads == LevelDesign.cs > RoadType)
     */
    public LevelDesign[] Levels;

    public List<RoadClass> Roads = new List<RoadClass>();
    public List<CoinClass> Coins = new List<CoinClass>();
    public List<StaticObstacleClass> StaticObstacles = new List<StaticObstacleClass>();
    public List<DynamicObstacleClass> DynamicObstacles = new List<DynamicObstacleClass>();
    public GameObject StartLine;
    public GameObject FinishLine;

    public int TestLevel;
    public void TestCreateLoadedLevel()
    {
        RoadsCreate(Levels[TestLevel - 1].RoadObjects);
        StaticObstacleCreate(Levels[TestLevel - 1].StaticObstacleObjects);
        DynamicObstacleCreate(Levels[TestLevel - 1].DynamicObstacleObjects);
        CoinsCreate(Levels[TestLevel - 1].CoinObjects);
        OneObjectCreate(StartLine, Levels[TestLevel - 1].StartLine, 0);
        OneObjectCreate(FinishLine, Levels[TestLevel - 1].FinishLine, 1);
    }

    public void CreateLoadedLevel(int levelInt)
    {
        RoadsCreate(Levels[levelInt - 1].RoadObjects);
        StaticObstacleCreate(Levels[levelInt - 1].StaticObstacleObjects);
        DynamicObstacleCreate(Levels[levelInt - 1].DynamicObstacleObjects);
        CoinsCreate(Levels[levelInt - 1].CoinObjects);
        OneObjectCreate(StartLine, Levels[levelInt - 1].StartLine, 0);
        OneObjectCreate(FinishLine, Levels[levelInt - 1].FinishLine, 1);
    }

    private void RoadsCreate(List<LevelDesign.RoadObject> loadedLevelObjects)
    {
        // Road Types
        for (int i = 0; i < Roads.Count; i++)
        {
            // Objects of Types
            for (int j = 0; j < Roads[i].objects.Count; j++)
            {
                // Check activate or deactivate object
                if (j < loadedLevelObjects[i].objects.Count)
                {
                    Roads[i].objects[j].transform.localPosition = loadedLevelObjects[i].objects[j].position;
                    Roads[i].objects[j].transform.localRotation = loadedLevelObjects[i].objects[j].rotation;
                    Roads[i].objects[j].transform.localScale = loadedLevelObjects[i].objects[j].scale;
                    Roads[i].objects[j].SetActive(true);
                }
                else
                {
                    Roads[i].objects[j].SetActive(false);
                }
            }
        }
    }

    private void StaticObstacleCreate(List<LevelDesign.StaticObstalceObject> loadedLevelObjects)
    {
        // Road Types
        for (int i = 0; i < StaticObstacles.Count; i++)
        {
            // Objects of Types
            for (int j = 0; j < StaticObstacles[i].objects.Count; j++)
            {
                // Check activate or deactivate object
                if (j < loadedLevelObjects[i].objects.Count)
                {
                    StaticObstacles[i].objects[j].transform.localPosition = loadedLevelObjects[i].objects[j].position;
                    StaticObstacles[i].objects[j].transform.localRotation = loadedLevelObjects[i].objects[j].rotation;
                    StaticObstacles[i].objects[j].transform.localScale = loadedLevelObjects[i].objects[j].scale;
                    StaticObstacles[i].objects[j].SetActive(true);
                }
                else
                {
                    StaticObstacles[i].objects[j].SetActive(false);
                }
            }
        }
    }

    private void DynamicObstacleCreate(List<LevelDesign.DynamicObstalceObject> loadedLevelObjects)
    {
        // Road Types
        for (int i = 0; i < DynamicObstacles.Count; i++)
        {
            // Objects of Types
            for (int j = 0; j < DynamicObstacles[i].objects.Count; j++)
            {
                // Check activate or deactivate object
                if (j < loadedLevelObjects[i].objects.Count)
                {
                    DynamicObstacles[i].objects[j].transform.localPosition = loadedLevelObjects[i].objects[j].position;
                    DynamicObstacles[i].objects[j].transform.localRotation = loadedLevelObjects[i].objects[j].rotation;
                    DynamicObstacles[i].objects[j].transform.localScale = loadedLevelObjects[i].objects[j].scale;
                    DynamicObstacles[i].objects[j].SetActive(true);

                    switch (i)
                    {
                        case 0: // FallingTree
                            DynamicObstacles[i].objects[j].GetComponent<FallingTree>().ResetObstacle();
                            break;
                        case 1: // BreakibgIce
                            DynamicObstacles[i].objects[j].GetComponent<BreakingIce>().ResetObstacle();
                            break;
                        case 2: // JumpingShark
                            DynamicObstacles[i].objects[j].GetComponent<JumpingShark>().ResetObstacle();
                            break;
                        case 3: // BigSeagulAttack
                            DynamicObstacles[i].objects[j].GetComponent<SeagulAttacker>().ResetObstacle();
                            break;
                        case 4: // MultipleSeagulAttack
                            DynamicObstacles[i].objects[j].GetComponent<SeagulAttacker>().ResetObstacle();
                            break;
                        case 5: // RollingSnowball
                            DynamicObstacles[i].objects[j].GetComponent<SnowballRolling>().ResetObstacle();
                            break;
                        case 6: // RollingTreeTrunk
                            DynamicObstacles[i].objects[j].GetComponent<TreeTrunkRolling>().ResetObstacle();
                            break;
                        case 7: // RunningDeer
                            DynamicObstacles[i].objects[j].GetComponent<DeerRunning>().ResetObstacle();
                            break;
                        default:
                            Debug.LogWarning("Undefined Dynamic Obstacle..");
                            break;
                    }
                }
                else
                {
                    DynamicObstacles[i].objects[j].SetActive(false);
                }
            }
        }
    }
    
    private void CoinsCreate(List<LevelDesign.CoinObject> loadedLevelObjects)
    {
        // Road Types
        for (int i = 0; i < Coins.Count; i++)
        {
            // Objects of Types
            for (int j = 0; j < Coins[i].objects.Count; j++)
            {
                // Check activate or deactivate object
                if (j < loadedLevelObjects[i].objects.Count)
                {
                    Coins[i].objects[j].transform.localPosition = loadedLevelObjects[i].objects[j].position;
                    Coins[i].objects[j].transform.localRotation = loadedLevelObjects[i].objects[j].rotation;
                    Coins[i].objects[j].transform.localScale = loadedLevelObjects[i].objects[j].scale;
                    Coins[i].objects[j].SetActive(true);
                    Coins[i].objects[j].GetComponent<Coin>().ResetCoin();
                }
                else
                {
                    Coins[i].objects[j].SetActive(false);
                }
            }
        }
    }

    private void OneObjectCreate(GameObject objectItem, LevelDesign.Variables variables, int objectTypeNumber)
    {
        objectItem.transform.localPosition = variables.position;
        objectItem.transform.localRotation = variables.rotation;
        objectItem.transform.localScale = variables.scale;

        if (objectTypeNumber == 0)
        {
            objectItem.GetComponent<StartLine>().ResetObstacle();
        }else if (objectTypeNumber == 1)
        {
            objectItem.GetComponent<FinishLine>().ResetObstacle();
        }
    }

    [System.Serializable]
    public class RoadClass
    {
        public LevelDesign.RoadType type;
        public GameObject holder;
        public List<GameObject> objects = new List<GameObject>();
    }

    [System.Serializable]
    public class CoinClass
    {
        public LevelDesign.CoinType type;
        public GameObject holder;
        public List<GameObject> objects = new List<GameObject>();
    }

    [System.Serializable]
    public class StaticObstacleClass
    {
        public LevelDesign.StaticObstacleType type;
        public GameObject holder;
        public List<GameObject> objects = new List<GameObject>();
    }

    [System.Serializable]
    public class DynamicObstacleClass
    {
        public LevelDesign.DynamicObstacleType type;
        public GameObject holder;
        public List<GameObject> objects = new List<GameObject>();
    }
}
