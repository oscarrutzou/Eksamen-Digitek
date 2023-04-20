using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    //Laver et array ud fra et script.
    
    public  Sound[] sounds;

    //For at der kun kan kører en AudioManager.
    public static AudioManager instance;

    public AudioMixer masterMixer;

    void Awake()
    {
        //For at sikre der kun kører en AuidoManager.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            //For at sikre at der ikke bliver kaldt mere kode før den er slettet.
            return;
        }

        //For at den ikke bliver slettet når scenerne skifter.
        DontDestroyOnLoad(gameObject);

        //Kalder vores Sound script og gør så de variabler ren faktisk gør noget.
        foreach (Sound s in sounds)
        {
            //Så man kan finde hvor musikken er.
            s.sorce = gameObject.AddComponent<AudioSource>();
            s.sorce.clip = s.clip;

            //For at kunne ændre volume, pitch og spørge om den skal være i et loop.
            s.sorce.volume = s.volume;
            s.sorce.pitch = s.pitch;
            s.sorce.loop = s.loop;

            AudioMixerGroup[] groups = masterMixer.FindMatchingGroups(s.mixerGroup);
            if (groups.Length > 0)
            {
                // Assign the first matching group to the outputAudioMixerGroup of the AudioSource
                s.sorce.outputAudioMixerGroup = groups[0];
            }
        }
    }

    public static AudioManager GetInstance()
    {
        return instance;
    }


    void Start()
    {
        //Starter den der hedder "Theme" i vores array.

        //Når man kalder den, tag og reference AudioManager før man ka spille lyden self:d

        Play("MainTheme");
    }


    //For at nemmere kunne kalde sounds igennem andre scipts, eller hvis man har mange sounds.
    public void Play(string name)
    {
        //Finder den sound med det samme man har skrevet ind i Play.
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //Hvis man har skrevet forkert, så fortæller den det.
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found;(");
            //For at ikke blive ved at søge efter den sound.
            return;
        }
        //Starter den sound.
        s.sorce.Play();
    }


    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.sorce.Stop();
    }

}
