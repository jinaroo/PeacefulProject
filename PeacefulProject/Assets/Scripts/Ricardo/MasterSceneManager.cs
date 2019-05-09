using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterSceneManager : MonoBehaviour
{
    public Transform raccoonTransform;
    public Transform mainCamTransform;

    public AudioManager audioManager;
    
    public Vector3 nextTeleportPosition;

    public bool dogHasBeenRescued;
    public bool snakeHasBeenRescued;
    public bool birdHasBeenRescued;

    public Vector3 dogTunnelPosition;
    public Vector3 snakeTunnelPosition;
    public Vector3 birdTunnelPosition;

    public Transform dogTransform;
    public Transform snakeTransform;
    public Transform birdTransform;

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

    private SpriteRenderer blackScreenSprite;
    public float teleportFadeTime = 1f;
    public Ease teleportFadeOutEase;
    public Ease teleportFadeInEase;
    public float teleportDelay = 0.1f;

    private bool isTeleporting;

    public Transform currentTeleportingTransform;
    
    // Start is called before the first frame update
    void Start()
    {
        audioManager = GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>();
        
        outdoorAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.outdoor, outdoorAudioVolume, true).GetComponent<AudioSource>();
        indoorAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.interior, 0f, true).GetComponent<AudioSource>();
        undergroundAudSrc = audioManager.PlaySoundEffect(audioManager.Clips.underground, 0f, true).GetComponent<AudioSource>();
        undergroundAudSrc.pitch = 0.5f;

        if(raccoonTransform == null)
            raccoonTransform = GameObject.FindWithTag("Player").transform;
        mainCamTransform = Camera.main.transform;
        
        blackScreenSprite = mainCamTransform.GetChild(0).GetComponent<SpriteRenderer>();

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void TeleportRaccoon()
    {
        if (isTeleporting)
            return;
        isTeleporting = true;
        currentTeleportingTransform = raccoonTransform;
        raccoonTransform.GetComponent<PlayerInput>().isFrozen = true;
        TeleportFadeOut();
    }

    void TeleportFadeOut()
    {
        blackScreenSprite.DOFade(1f, teleportFadeTime).OnComplete(TranslateAnimal);
        DOTween.To(()=> AudioListener.volume , x=> AudioListener.volume  = x, 0f, teleportFadeTime);
    }

    void TeleportFadeIn()
    {
        blackScreenSprite.DOFade(0f, teleportFadeTime).OnComplete(FinishTeleporting);
        if (currentTeleportingTransform == raccoonTransform)
        {
            raccoonTransform.GetComponent<PlayerInput>().isFrozen = false;
        }
        DOTween.To(()=> AudioListener.volume , x=> AudioListener.volume  = x, 1f, teleportFadeTime);

    }

    void TranslateAnimal()
    {
        if (currentTeleportingTransform == raccoonTransform)
        {
            mainCamTransform.position = nextTeleportPosition;
            raccoonTransform.position = nextTeleportPosition;
        } else if (currentTeleportingTransform == dogTransform)
        {
            dogTransform.position = dogTunnelPosition;
        } else if (currentTeleportingTransform == dogTransform)
        {
            birdTransform.position = birdTunnelPosition;
        } else if (currentTeleportingTransform == dogTransform)
        {
            snakeTransform.position = snakeTunnelPosition;
        }
        
        Invoke("TeleportFadeIn", teleportDelay);
    }

    void FinishTeleporting()
    {
        isTeleporting = false;
        currentTeleportingTransform = null;
    }
    
    public void TeleportDogToTunnel()
    {
        if(dogHasBeenRescued)
            return;
        dogHasBeenRescued = true;
        currentTeleportingTransform = dogTransform;
        TeleportFadeOut();
    }
    
    public void TeleportSnakeToTunnel()
    {
        if (snakeHasBeenRescued)
            return;
        snakeHasBeenRescued = true;
        currentTeleportingTransform = snakeTransform;
        TeleportFadeOut();
    }
    
    public void TeleportBirdToTunnel()
    {
        if (birdHasBeenRescued)
            return;
        birdHasBeenRescued = true;
        currentTeleportingTransform = birdTransform;
        TeleportFadeOut();
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
