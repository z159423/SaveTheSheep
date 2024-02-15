using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    public Sound[] Bgm;
    public GameObject bgmPrefab;
    private AudioSource BGMaudioSource;
    private string previousBgmName = "";
    private float previousBgmTime = 0;

    private string nowBgmName = "";

    private bool isFadeOut = false;

    [Space]
    public Sound[] SFX;
    public GameObject SFXPrefab;
    private AudioSource SFXaudioSource;

    public static SoundManager instance;

    private static bool Initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (!Initialized)
        {
            Initialized = true;
        }

        BGMaudioSource = bgmPrefab.AddComponent<AudioSource>();
        SFXaudioSource = SFXPrefab.AddComponent<AudioSource>();
    }

    public void FindBGM(string name = "", bool isPreviousBgm = false)
    {
        Sound s;

        if (isPreviousBgm)
        {
            s = Array.Find(Bgm, sound => sound.name == previousBgmName);
        }
        else
        {
            s = Array.Find(Bgm, sound => sound.name == name);
        }

        PlayBGM(s, isPreviousBgm);
    }

    public void PlayPreviousBGM()
    {
        Sound s = Array.Find(Bgm, sound => sound.name == previousBgmName);

        PlayBGM(s, isPrevious: true);
    }


    public void PlayRandomBossFightBGM()
    {
        string name = "boss" + UnityEngine.Random.Range(2, 4).ToString();

        Sound s = Array.Find(Bgm, sound => sound.name == name);

        PlayBGM(s);
    }

    public void PlaySFX(string name)
    {

        Sound s = Array.Find(SFX, sound => sound.name == name);

        SFXaudioSource.clip = s.clip;

        SFXaudioSource.volume = s.volume;
        SFXaudioSource.pitch = s.pitch;
        SFXaudioSource.loop = s.loop;
        SFXaudioSource.outputAudioMixerGroup = s.audioMixerGroup;
        SFXaudioSource.time = s.time;

        SFXaudioSource.Play();
    }


    public void PlaySFXWithAudioSource(string name, AudioSource audio)
    {
        Sound s = Array.Find(SFX, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError("SFX 클립을 찾을 수 없습니다.");
            return;
        }

        if (audio == null)
        {
            Debug.LogError("Audio Source가 없습니다.");
            return;
        }

        audio.clip = s.clip;

        audio.volume = s.volume;
        audio.pitch = s.pitch;
        audio.loop = s.loop;
        audio.outputAudioMixerGroup = s.audioMixerGroup;
        audio.time = s.time;

        audio.Play();
    }

    public AudioSource GenerateAudioAndPlaySFX(string name, Vector3 position)
    {
        Sound s = Array.Find(SFX, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError("SFX 클립을 찾을 수 없습니다.");
            return null;
        }

        var audio = new GameObject();
        audio.AddComponent<AudioSource>();
        var audioSource = audio.GetComponent<AudioSource>();

        audioSource.clip = s.clip;

        audioSource.volume = s.volume;
        audioSource.pitch = s.pitch;
        audioSource.loop = s.loop;
        audioSource.outputAudioMixerGroup = s.audioMixerGroup;
        audioSource.time = s.time;

        audioSource.Play();

        Destroy(audio, s.clip.length + 1);

        return audioSource;
    }

    private void PlayBGM(Sound s, bool isPrevious = false)
    {
        if (BGMaudioSource.isPlaying)
        {
            if (s.name == nowBgmName)                   //같은 bgm이면 리턴
                return;

            float previousBgmTimeSave = previousBgmTime;

            previousBgmName = nowBgmName;

            if (!isFadeOut)
                previousBgmTime = BGMaudioSource.time;

            nowBgmName = s.name;

            StopAllCoroutines();
            StartCoroutine(FadeOutBgm(s, isPrevious, previousBgmTime: previousBgmTimeSave));
        }
        else
        {
            nowBgmName = s.name;

            StartCoroutine(FadeInBgm(s, isPrevious));
        }

    }


    private IEnumerator FadeInBgm(Sound s, bool isPreviousBgm = false, float previousBgmTime = 0)
    {
        float timeToFade = 1.25f;
        float timeElapsed = 0f;
        float maxVolume = s.volume;

        BGMaudioSource.clip = s.clip;
        BGMaudioSource.pitch = s.pitch;
        BGMaudioSource.loop = s.loop;
        BGMaudioSource.outputAudioMixerGroup = s.audioMixerGroup;

        if (isPreviousBgm)
        {
            //Debug.LogError(previousBgmTime);

            BGMaudioSource.time = previousBgmTime;
        }
        else
        {
            BGMaudioSource.time = s.time;
        }

        BGMaudioSource.Play();

        while (timeElapsed < timeToFade)
        {
            BGMaudioSource.volume = Mathf.Lerp(0, maxVolume, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime / 3;
            yield return null;
        }

    }

    private IEnumerator FadeOutBgm(Sound s, bool isPreviousBgm = false, float previousBgmTime = 0)
    {
        float timeToFade = 0.02f;

        isFadeOut = true;
        while (BGMaudioSource.volume > 0.05f)
        {
            //Debug.LogError("FadeOutBgm");
            BGMaudioSource.volume -= timeToFade;
            yield return new WaitForSeconds(0.1f);
        }
        isFadeOut = false;


        StartCoroutine(FadeInBgm(s, isPreviousBgm, previousBgmTime: previousBgmTime));

    }



}

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume;
    [Range(.1f, 3f)]
    public float pitch;

    public float time = 0;

    public bool loop;
    public bool bgm;

    public AudioMixerGroup audioMixerGroup;

    [HideInInspector]
    public AudioSource source;

}
