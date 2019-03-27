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
    public AudioClip[] droppedItem;
    
//player sound effects
    public AudioClip[] playerWalk; //sounds different depending on surface.
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
    
    //snake sound effects
    public AudioClip[] snakeHappy;
    public AudioClip[] snakeSad;
    public AudioClip[] snakeQuest;
    public AudioClip[] snakeReward;
    
    //enviroment sound effects
    public AudioClip treeShuffle;
    public AudioClip[] rockShuffle;
    public AudioClip dirtShuffle;
    public AudioClip pavementShuffle;
    public AudioClip gravelShuffle;
    
    //narrative sound effect for thought bubble stories after they are in the nest,
    //if we still want a mini story about what happened to the humans
    public AudioClip dogStoryBubble;
    public AudioClip birdStoryBubble;
    public AudioClip snakeStoryBubble;
}




