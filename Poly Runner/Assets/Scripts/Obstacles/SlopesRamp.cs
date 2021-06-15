using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopesRamp : MonoBehaviour
{
    public enum SlopesRampType { Enter, Exit };
    [SerializeField] SlopesRampType slopesRampType;
    CameraFollow _cameraFollow;
    Character _playerScript;

    private bool isTriggered;
    // Start is called before the first frame update
    void Start()
    {
        _cameraFollow = FindObjectOfType<CameraFollow>();
        _playerScript = FindObjectOfType<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            StartCoroutine(TriggeredDeactive(0.5f));
            switch (slopesRampType)
            {
                case SlopesRampType.Enter:
                    _playerScript.runMode = RunMode.Slope;
                    string animateName = _playerScript.isJumping ? "FallingIdleFromJump" : "JumpToSlope";
                    _playerScript.SetAnimate(animateName);
                    _cameraFollow.isLookAtPlayer = true;
                    break;
                case SlopesRampType.Exit:
                    _playerScript.SetAnimate("JumpToSlope");
                    _cameraFollow.isLookAtPlayer = false;
                    _cameraFollow.isRampSlidingMode = false;
                    break;
                default:
                    Debug.LogWarning("Slopes Ramp Type not selected.!");
                    break;
            }
        }
    }

    IEnumerator TriggeredDeactive(float time)
    {
        //yield on a new YieldInstruction that waits for X seconds.
        yield return new WaitForSeconds(time);
        isTriggered = false;
    }
}
