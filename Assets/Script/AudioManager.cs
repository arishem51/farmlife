using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;  // Singleton

    [Header("Background Music")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;  // Assign BGM file ở Inspector

    [Header("Sound Effects")]
    public AudioSource sfxSource;  // Một source cho tất cả SFX
    public List<AudioClip> sfxClips = new List<AudioClip>();  // 0: Dig, 1: Plant, 2: Water, 3: Harvest, 4: Milk, 5: Egg, 6: Feed, 7: UI Click

    [Header("Volumes")]
    [Range(0f, 1f)] public float bgmVolume = 0.4f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    private Dictionary<string, AudioClip> sfxDict = new Dictionary<string, AudioClip>();  // Tên → Clip (dễ gọi)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist qua scenes
            InitializeSFX();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM();
    }

    private void InitializeSFX()
    {
        // Map tên SFX với index trong List (assign ở Inspector)
        //sfxDict["dig"] = sfxClips[0];
        //sfxDict["plant"] = sfxClips[1];
        //sfxDict["water"] = sfxClips[2];
        //sfxDict["harvest"] = sfxClips[3];
        //sfxDict["milk"] = sfxClips[4];
        //sfxDict["egg"] = sfxClips[5];
        //sfxDict["feed"] = sfxClips[6];
        sfxDict["ui_click"] = sfxClips[0];
        sfxDict["gameover"] = sfxClips[1];
        sfxDict["dig"] = sfxClips[2];
        sfxDict["water"] = sfxClips[3];


    }

    public void PlayBGM()
    {
        if (bgmSource && bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void PlaySFX(string sfxName)
    {
        if (sfxSource && sfxDict.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxDict[sfxName], sfxVolume);  // Không stop SFX cũ
        }
    }

    public void StopBGM() { if (bgmSource) bgmSource.Stop(); }
    public void PauseBGM() { if (bgmSource) bgmSource.Pause(); }
}