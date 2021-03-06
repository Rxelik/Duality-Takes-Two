using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
public class AudioManager : MonoBehaviour
{
    [SerializeField] public List<SFXClass> SfxList = new List<SFXClass>();
    public const string LIGHT_DARK_LIGHTER_DARKER_PARAM_NAME = "LightDark";
    [Header("BGM")]
    public const string BGM_PARAM_NAME = "Intensity";
    private EventInstance bgmEvent;
    [EventRef]public string bgmRef;
    [SerializeField] private int bgmIntensity;
    [SerializeField] private int bgmIntensityRate;
    private EventDescription bgmDescription;
    PLAYBACK_STATE playbackState;
    [SerializeField] private float OneShotTimer;
    private bool releaseIsOn;



    public static AudioManager instance;
    private void Awake()
    {
        bgmIntensity = 0;
        instance = this;
        bgmEvent = RuntimeManager.CreateInstance(bgmRef);
    }
    PLAYBACK_STATE PlaybackState(EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE pS;
        instance.getPlaybackState(out pS);
        return pS;
    }

    private void Start()
    {
        bgmIntensity = 0;
        if (PlaybackState(bgmEvent) == PLAYBACK_STATE.PLAYING)
        {
            StopBGM();
        }
        PlayBGM();
        InvokeRepeating("IncreaseIntensity", 0 ,bgmIntensityRate);
    }

    public void ReleaseOneShot(EventInstance instance)
    {
        if(PlaybackState(instance) != PLAYBACK_STATE.PLAYING && !releaseIsOn)
        {
            instance.release();
            releaseIsOn = true;
        }
    }
    public EventInstance PlayOneShot(string path, string parameterName, float parameterValue, Vector3 position = new Vector3())
    {
        var instance = RuntimeManager.CreateInstance(path);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.setParameterByName(parameterName, parameterValue);
        instance.start();
        return instance;
    }    
    public void PlayOneShotByName(string sfxName)
    {
        var sfxToPlay = SfxList.Find(name => name.sfxName == sfxName);
        RuntimeManager.PlayOneShot(sfxToPlay.path);
    }
    public IEnumerator ReleaseOneShotSource(EventInstance evInstance)
    {
        releaseIsOn = true;
        yield return new WaitForSeconds(10);
        if (PlaybackState(evInstance) != PLAYBACK_STATE.PLAYING)
        ReleaseOneShot(evInstance);
    }
    public void PlayBGM()
    {
        if (PlaybackState(bgmEvent) == PLAYBACK_STATE.PLAYING)
        {
            StopBGM();
        }
       bgmEvent.start();
    }
    
    public void StopBGM()
    {
        bgmEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bgmEvent.release();
     }
    public void bgmParameterChange(string parameterName,int parameterValue)
    {
        bgmEvent.setParameterByName(parameterName, parameterValue, true);
    }
    public void IncreaseIntensity()
    {
        if(bgmIntensity < 4)
        {
            bgmIntensity++;
        }
        bgmParameterChange(BGM_PARAM_NAME, bgmIntensity);
    }

    public void OnDeath()
    {
        //snapshot to death snapshot
        //Play one shot of death seq

    }

    public void PlayStartUI() => RuntimeManager.PlayOneShot("event:/uiStartGame", transform.position);

    public void WhooshUI() => RuntimeManager.PlayOneShot("event:/uiWhoosh", transform.position);
}