﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Whalex
{
    public class Dialogue : MonoBehaviour
    {
       [SerializeField] private NpcTalkStatus status;
        
        private GameObject npcIndicator;
        private GameObject npcPrompt;
        private GameObject npcDialogue;

        public float rescueTeleportDelay = 4;
        private MasterSceneManager masterSceneManager;
        public CharacterType charType;
        
        private void Start()
        {
            status = NpcTalkStatus.P0_FAR;
            
            npcIndicator = Instantiate((Resources.Load<GameObject>("heart_prefab_prepare")));
            npcIndicator.transform.SetParent(transform.Find("Prompt"));
            npcIndicator.transform.localPosition = Vector3.zero;
            npcIndicator.SetActive(false);
            
            npcPrompt = Instantiate(Resources.Load<GameObject>("heart_prefab"));
            npcPrompt.transform.SetParent(transform.Find("Prompt"));
            npcPrompt.transform.localPosition = Vector3.zero;
            npcPrompt.SetActive(false);
            
            npcDialogue = Instantiate(Resources.Load<GameObject>("npc_cloud"));
            npcDialogue.transform.SetParent(transform.Find("Dialogue"));
            npcDialogue.transform.localPosition = Vector3.zero;
            npcDialogue.SetActive(false);

            GameObject masterSceneObj = GameObject.FindWithTag("MasterSceneManager");
            if (masterSceneObj)
            {
                masterSceneManager = masterSceneObj.GetComponent<MasterSceneManager>();
            }
        }

        private void Update()
        {
            switch (status)
            {
                case NpcTalkStatus.P0_FAR:
                    if (!npcIndicator.activeSelf)
                    {
                        npcIndicator.SetActive(true);
                        npcPrompt.SetActive(false);
                        npcDialogue.SetActive(false);
                    }
                    break;
                
                case NpcTalkStatus.P1_NEAR:
                    if (!npcPrompt.activeSelf)
                    {
                        npcIndicator.SetActive(false);
                        npcPrompt.SetActive(true);
                        npcDialogue.SetActive(false);
                    }
                    break;
                    
                case NpcTalkStatus.P2_TALK_:
                    if (!npcDialogue.activeSelf)
                    {
                        npcIndicator.SetActive(false);
                        npcPrompt.SetActive(false);
                        npcDialogue.SetActive(true);
                        
                        if(masterSceneManager)
                            Invoke("Teleport", rescueTeleportDelay);
                    }
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                status = NpcTalkStatus.P1_NEAR;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    switch (status)
                    {
                        case NpcTalkStatus.P1_NEAR:
                            status = NpcTalkStatus.P2_TALK_;
                            break;
                        
                        case NpcTalkStatus.P2_TALK_:
                            status = NpcTalkStatus.P1_NEAR;
                            break;
                        
                        default:
                            print("Wrong NPC talking status!");
                            break;
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                status = NpcTalkStatus.P0_FAR;
            }
        }

        void Teleport()
        {
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
            P2_TALK_,
        }

        public enum CharacterType
        {
            DOG, 
            SNAKE,
            BIRD
        }
    }
}