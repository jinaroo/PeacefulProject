using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Intent: A script to allow us to access/change out sounds as needed
//Usage: Just let it hang out

public class AudioClips : MonoBehaviour
{


//Item sound effects
    public AudioClip pickUpItem;
    public AudioClip dropItem;
    
//player sound effects
    public AudioClip playerWalkdirt; //sounds different depending on surface.
    public AudioClip playerWalkindoor; //sounds different depending on surface.
    public AudioClip playerWalkgrass; //might wanna have the climb sound diferent/ more difficult if it takes longer
    public AudioClip Playerclimb; //sounds different depending on surface.
    
    //enviroment sound effects
    public AudioClip outdoor;
    public AudioClip underground;
    public AudioClip interior;
    
   
}




