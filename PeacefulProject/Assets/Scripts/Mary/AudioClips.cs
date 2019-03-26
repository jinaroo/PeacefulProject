using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Intent: A script to allow us to access/change out sounds as needed
//Usage: Just let it hang out

public class AudioClips : MonoBehaviour
{


//Item sound effects
    public AudioClip pickUpItem;
    public AudioClip pushItem;
    public AudioClip[] breakItem;
    
//player sound effects
    public AudioClip[] PlayerWalk; //sounds different depending on surface.
    public AudioClip[] playerJump; //sounds different depending on surface.
    public AudioClip[] playerClimb; //might wanna have the climb sound diferent/ more difficult if it takes longer
    public AudioClip[] PlayerSlide; //sounds different depending on surface.
    
    //dog sound effects
    public AudioClip[] dogHappy;
    public AudioClip[] dogSad;
    public AudioClip[] dogQuest;
    public AudioClip[] dogReward;
    
    //bird sound effects
    public AudioClip[] birdHappy;
    public AudioClip[] birdSad;
    public AudioClip[] birdQuest;
    public AudioClip[] birdReward;
    
    //hamster sound effects
    public AudioClip[] hamsterHappy;
    public AudioClip[] hamsterSad;
    public AudioClip[] hamsterQuest;
    public AudioClip[] hamsterReward;
    
    //enviroment sound effects
    public AudioClip treeShuffle;
    public AudioClip[] rockShuffle;
    public AudioClip rockHitSword;
    public AudioClip daggerHitSword;
    
    //narrative sound effect for thought bubble stories
    public AudioClip dogStoryBubble;
    public AudioClip birdStoryBubble;
    public AudioClip hamsterStoryBubble;
}




