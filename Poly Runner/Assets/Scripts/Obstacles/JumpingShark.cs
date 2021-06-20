using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingShark : MonoBehaviour
{
    [SerializeField] private Animator animationControl;
    [SerializeField] private GameObject particleEffect;

    CameraFollow _cameraFollow;
    Character _playerScript;

    private int triggerCounter;
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
            isTriggered = true;
            
            StartCoroutine(TriggeredDeactive(0.5f));

            switch (triggerCounter)
            {
                case 0:
                    _playerScript.runMode = RunMode.Bridge;
                    break;
                case 1:
                    animationControl.SetTrigger("SharkAttack");
                    particleEffect.SetActive(true);
                    break;
                case 2:
                    _playerScript.runMode = RunMode.Straight;
                    //StartCoroutine(DeactiveTheObstacle(5f));
                    break;
                default:
                    Debug.Log("There is no more action.");
                    break;
            }
            triggerCounter++;
            
        }
    }

    IEnumerator DeactiveTheObstacle(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        if (InGameManager.instance.isGameActive)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator TriggeredDeactive(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        isTriggered = false;
    }

    public void ResetObstacle()
    {
        if (animationControl.GetCurrentAnimatorClipInfo(0).Length > 0)
            animationControl.SetTrigger("Passive");
        isTriggered = false;
        triggerCounter = 0;
    }
}
