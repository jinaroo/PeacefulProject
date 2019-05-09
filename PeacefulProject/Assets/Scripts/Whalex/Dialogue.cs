using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Whalex
{
    public class Dialogue : MonoBehaviour
    {
       [SerializeField] private NpcTalkStatus status;
        
        private GameObject npcIndicator;
        private GameObject npcHeart;
        private GameObject npcDialogueCloud;

        public float rescueTeleportDelay = 4;
        private MasterSceneManager masterSceneManager;
        public CharacterType charType;
        public bool hasBeenRescued;
        public bool questComplete;

        public int totalQuestSteps;
        public int numQuestStepsCompleted;

        public int numPhasesComplete;

        public float growTime = 1f;
        public Ease growEase;
        public Ease shrinkEase;

        public SpriteRenderer[] questItemIcons;
        public Color itemCompleteColor;

        private bool playerInRange = false;
        
        private void Start()
        {
            UpdateStatus(NpcTalkStatus.P0_FAR);
            
            npcIndicator = Instantiate((Resources.Load<GameObject>("heart_prefab_prepare")));
            npcIndicator.transform.SetParent(transform.Find("Heart"));
            npcIndicator.transform.localPosition = Vector3.zero;
            npcIndicator.SetActive(false);
            
            npcHeart = Instantiate(Resources.Load<GameObject>("heart_prefab"));
            npcHeart.transform.SetParent(transform.Find("Heart"));
            npcHeart.transform.localPosition = Vector3.zero;
            //npcHeart.SetActive(false);
            npcHeart.transform.localScale = Vector3.zero;
            
            npcDialogueCloud = Instantiate(Resources.Load<GameObject>("npc_cloud"));
            npcDialogueCloud.transform.SetParent(transform.Find("Dialogue"));
            npcDialogueCloud.transform.localPosition = Vector3.zero;
            //npcDialogueCloud.SetActive(false);
            npcDialogueCloud.transform.localScale = Vector3.zero;

            GameObject masterSceneObj = GameObject.FindWithTag("MasterSceneManager");
            if (masterSceneObj)
            {
                masterSceneManager = masterSceneObj.GetComponent<MasterSceneManager>();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                UpdateStatus(NpcTalkStatus.P1_NEAR);
                playerInRange = true;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E) && playerInRange)
            {
                switch (status)
                {
                    case NpcTalkStatus.P1_NEAR:
                        if (hasBeenRescued)
                        {
                            if (questComplete)
                            {
                                UpdateStatus(NpcTalkStatus.P2_HAPPY);
                            }
                            else
                            {
                                UpdateStatus(NpcTalkStatus.P3_QUEST);
                            }
                        }
                        else
                        {
                            UpdateStatus(NpcTalkStatus.P2_HAPPY);
                            switch (charType)
                            {
                                case CharacterType.DOG:
                                    GetComponent<DogRescueEvent>().TriggerBowEvent();
                                    break;
                                case CharacterType.SNAKE:
                                    GetComponent<SnakeRescueEvent>().TriggerSewerEvent();
                                    
                                    break;
                                case CharacterType.BIRD:
                                    GetComponent<BirdRescueEvent>().TriggerDirtClumpEvent();
                                    break;
                                default:
                                    Debug.Log("this dialogue has no rescue event!");
                                    break;
                            }
                        }
                        break;
                        
                    case NpcTalkStatus.P2_HAPPY:
                        //UpdateStatus(NpcTalkStatus.P1_NEAR);
                        break;
                        
                    default:
                        print("No behavior specified for this talking status!");
                        break;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                UpdateStatus(NpcTalkStatus.P0_FAR);
                playerInRange = false;
            }
        }

        void UpdateStatus(NpcTalkStatus newStatus)
        {
            if (status == newStatus)
                return;

            status = newStatus;
            switch (status)
            {
                case NpcTalkStatus.P0_FAR:
                    if (npcDialogueCloud.transform.localScale != Vector3.zero)
                    {
                        npcDialogueCloud.transform.DOScale(0f, growTime).SetEase(shrinkEase);
                    }

                    if (questComplete)
                    {
                        if (npcHeart.transform.localScale != Vector3.zero)
                        {
                            npcHeart.transform.DOScale(0f, growTime).SetEase(shrinkEase);
                        }
                    }
                    break;
                
                case NpcTalkStatus.P1_NEAR:
                    if (questComplete)
                    {
                        if (npcHeart.transform.localScale == Vector3.zero)
                        {
                            npcHeart.transform.DOScale(1f, growTime).SetEase(growEase);
                        }
                    }
                    else
                    {
                        // display button prompt
                    }
                    break;
                    
                case NpcTalkStatus.P2_HAPPY:
                    if(!hasBeenRescued)
                    {
                        hasBeenRescued = true;
                        if(masterSceneManager)
                            Invoke("Teleport", rescueTeleportDelay);
                    }
                    if (npcHeart.transform.localScale == Vector3.zero)
                    {
                        npcHeart.transform.DOScale(1f, growTime).SetEase(growEase);
                    }
                    break;
                case NpcTalkStatus.P3_QUEST:
                    if (npcDialogueCloud.transform.localScale == Vector3.zero)
                    {
                        npcDialogueCloud.transform.DOScale(1f, growTime).SetEase(growEase);
                        npcHeart.transform.DOScale(0f, growTime).SetEase(shrinkEase);
                    }

                    break;
            }
        }

        public void AcceptItem(int itemIndex)
        {
            //questItemIcons[itemIndex].color = itemCompleteColor;
            numQuestStepsCompleted++;
            
            
            if (numQuestStepsCompleted == totalQuestSteps)
            {
                questComplete = true;
                UpdateStatus(NpcTalkStatus.P2_HAPPY);
            }
        }
        
        void Teleport()
        {
            if (npcHeart.transform.localScale != Vector3.zero)
            {
                npcHeart.transform.DOScale(0f, growTime).SetEase(shrinkEase);
            }
            switch (charType)
            {
                case CharacterType.DOG:
                    masterSceneManager.TeleportDogToTunnel();
                    break;
                case CharacterType.SNAKE:
                    masterSceneManager.TeleportSnakeToTunnel();
                    break;
                case CharacterType.BIRD:
                    masterSceneManager.TeleportBirdToTunnel();
                    break;
                default:
                    Debug.Log("charType has not been set for this character!");
                    break;
            }
        }
        
        enum NpcTalkStatus
        {
            P0_FAR,
            P1_NEAR,
            P2_HAPPY,
            P3_QUEST 
        }

        public enum CharacterType
        {
            DOG, 
            SNAKE,
            BIRD
        }
    }
}