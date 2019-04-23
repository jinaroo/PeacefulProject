using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MasterSceneManager : MonoBehaviour
{
    public Transform racoonTransform;
    public Transform mainCamTransform;

    public AudioManager audioManager;
    
    public Vector3 nextTeleportPosition;

    public bool dogHasBeenRescued;
    public bool snakeHasBeenRescued;
    public bool birdHasBeenRescued;

    public float audioFadeTime = 1f;

    public float outdoorAudioVolume = 1f;
    public float outdoorSoftAudioVolume = 0.05f;

    public float undergroundAudioVolume = 1f;
    public float indoorAudioVolume = 1f;

    
    
    private AudioSource outdoorAudSrc;
    private AudioSource indoorAudSrc;
    private AudioSource undergroundAudSrc;

    
    private bool isUnderground;
    private bool isIndoors;
    
    // Start is called before the first frame update
    void Start()
    {
        if (audioManager == null)
            audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        
        outdoorAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.outdoor, outdoorAudioVolume, true).GetComponent<AudioSource>();
        indoorAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.interior, 0f, true).GetComponent<AudioSource>();
        undergroundAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.underground, 0f, true).GetComponent<AudioSource>();
        undergroundAudSrc.pitch = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TeleportRaccoon()
    {
        
    }

    public void TeleportDogToTunnel()
    {
        dogHasBeenRescued = true;
    }
    
    public void TeleportSnakeToTunnel()
    {
        snakeHasBeenRescued = true;
    }
    
    public void TeleportBirdToTunnel()
    {
        birdHasBeenRescued = true;
    }

    public void EnterUnderground()
    {
        if (isUnderground)
            return;
        isUnderground = true;

        outdoorAudSrc.DOFade(outdoorSoftAudioVolume, audioFadeTime);
        undergroundAudSrc.DOFade(undergroundAudioVolume, audioFadeTime);
    }

    public void ExitUnderground()
    {
        if (!isUnderground)
            return;
        isUnderground = false;
        
        outdoorAudSrc.DOFade(outdoorAudioVolume, audioFadeTime);
        undergroundAudSrc.DOFade(0f, audioFadeTime);
    }

    public void GoIndoors()
    {
        if (isIndoors)
            return;
        isIndoors = true;
        
        outdoorAudSrc.DOFade(outdoorSoftAudioVolume, audioFadeTime);
        indoorAudSrc.DOFade(indoorAudioVolume, audioFadeTime);
    }

    public void GoOutdoors()
    {
        if (!isIndoors)
            return;
        isIndoors = false;
        
        outdoorAudSrc.DOFade(outdoorAudioVolume, audioFadeTime);
        indoorAudSrc.DOFade(0f, audioFadeTime);
    }
}
