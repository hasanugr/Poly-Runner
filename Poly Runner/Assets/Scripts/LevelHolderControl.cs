using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHolderControl : MonoBehaviour
{
    [SerializeField] Coin[] coins;
    [SerializeField] FallingTree[] fallingTrees;
    [SerializeField] BreakingIce[] breakingIces;
    [SerializeField] JumpingShark[] jumpingSharks;
    [SerializeField] SeagulAttacker[] seagulAttackers;
    [SerializeField] SnowballRolling[] snowballRollings;
    [SerializeField] TreeTrunkRolling[] treeTrunkRollings;
    [SerializeField] DeerRunning[] deerRunnings;

    public void ResetLevel()
    {
        for (int i = 0; i < coins.Length; i++)
        {
            coins[i].ResetCoin();
        }
        
        for (int i = 0; i < fallingTrees.Length; i++)
        {
            fallingTrees[i].ResetObstacle();
        }
        
        for (int i = 0; i < breakingIces.Length; i++)
        {
            breakingIces[i].ResetObstacle();
        }
        
        for (int i = 0; i < jumpingSharks.Length; i++)
        {
            jumpingSharks[i].ResetObstacle();
        }
        
        for (int i = 0; i < seagulAttackers.Length; i++)
        {
            seagulAttackers[i].ResetObstacle();
        }
        
        for (int i = 0; i < snowballRollings.Length; i++)
        {
            snowballRollings[i].ResetObstacle();
        }
        
        for (int i = 0; i < treeTrunkRollings.Length; i++)
        {
            treeTrunkRollings[i].ResetObstacle();
        }
        
        for (int i = 0; i < deerRunnings.Length; i++)
        {
            deerRunnings[i].ResetObstacle();
        }
    }
}
