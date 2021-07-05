using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum AudioSoundTypes { Music, UI, Player, Environment, Animals };
    public Sound[] Musics;
    public Sound[] UISounds;
    public Sound[] PlayerSounds;
    public Sound[] EnvironmentSounds;
    public Sound[] AnimalSounds;

    [SerializeField] GameManager _gm;

    // Start is called before the first frame update
    void Awake()
    {
        MakeSingleton();

        foreach (Sound s in Musics)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in UISounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in PlayerSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in EnvironmentSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        foreach (Sound s in AnimalSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    public void Play(AudioSoundTypes audioSoundTypes, string name, float customVolume = -1)
    {
        Sound s = Array.Find(GetSounds(audioSoundTypes), sound => sound.name == name);
        if (s== null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (customVolume >= 0)
        {
            s.source.volume = customVolume;
        }

        if (audioSoundTypes == AudioSoundTypes.Music)
        {
            if (_gm.pd.Music)
            {
                s.source.Play();
            }
        }else
        {
            if (_gm.pd.Sound)
            {
                s.source.Play();
            }
        }
    }

    public void PlayOneShot(AudioSoundTypes audioSoundTypes, string name, float customVolume = -1)
    {
        Sound s = Array.Find(GetSounds(audioSoundTypes), sound => sound.name == name);
        if (s== null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        if (customVolume >= 0)
        {
            s.source.volume = customVolume;
        }

        if (audioSoundTypes == AudioSoundTypes.Music)
        {
            if (_gm.pd.Music)
            {
                s.source.PlayOneShot(s.clip);
            }
        }
        else
        {
            if (_gm.pd.Sound)
            {
                s.source.PlayOneShot(s.clip);
            }
        }
    }

    public void Stop(AudioSoundTypes audioSoundTypes, string name)
    {
        Sound s = Array.Find(GetSounds(audioSoundTypes), sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    private Sound[] GetSounds(AudioSoundTypes type)
    {
        switch (type)
        {
            case AudioSoundTypes.Music:
                return Musics;
            case AudioSoundTypes.UI:
                return UISounds;
            case AudioSoundTypes.Player:
                return PlayerSounds;
            case AudioSoundTypes.Environment:
                return EnvironmentSounds;
            case AudioSoundTypes.Animals:
                return AnimalSounds;
            default:
                return Musics;
        }
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
