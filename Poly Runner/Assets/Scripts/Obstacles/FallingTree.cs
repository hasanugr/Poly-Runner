using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MilkShake;

public class FallingTree : MonoBehaviour
{
    [SerializeField] private Animator animationControl;
    [SerializeField] private GameObject dustEffect;
    [SerializeField] private ShakePreset fallShake;

    private bool isTriggered;

    private AudioManager _audioManager;

    private void Start()
    {
        _audioManager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            animationControl.ResetTrigger("FallTreeDefault");
            animationControl.SetTrigger("FallTree");
            StartCoroutine(ParticleEffect(1f));
            StartCoroutine(PlaySoundWithDelay("FallTree", 1f));
        }
    }

    IEnumerator ParticleEffect(float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        dustEffect.SetActive(true);
        Shaker.ShakeAllSeparate(fallShake);
    }

    IEnumerator PlaySoundWithDelay(string soundName, float time)
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(time);
        _audioManager.PlayOneShot(AudioManager.AudioSoundTypes.Environment, soundName);
    }

    public void ResetObstacle()
    {
        //if (animationControl.GetCurrentAnimatorClipInfo(0).Length > 0)
            
        animationControl.SetTrigger("FallTreeDefault");
        isTriggered = false;
    }
}
