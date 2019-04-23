using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Intent: To have a script that manages the audio for the game
//Usage: Place on an empty Gameobject in the scene named Audio Manager

public class AudioManager : MonoBehaviour
{
	public AudioClips Clips;
	public static AudioManager Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Clips = Resources.Load<GameObject>("Clips").GetComponent<AudioClips>();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void Start()
	{
		//Clips = Resources.Load<GameObject>("Clips").GetComponent<AudioClips>();
	}

	public GameObject PlaySoundEffect(AudioClip audioClip, float volume = 1.0f, bool looping = false, bool randomPitch = false, float pitchVariation = 0.1f, bool keepAlive = false)
	{
		var newGameObject = new GameObject("Audio Effect");
		newGameObject.transform.position = Camera.main.transform.position;
		
		var newAudioSource = newGameObject.AddComponent<AudioSource>();
		newAudioSource.spatialize = false;

		newAudioSource.clip = audioClip;
		newAudioSource.volume = volume;
		newAudioSource.loop = looping;

		if (randomPitch)
			newAudioSource.pitch = Random.Range(1f - pitchVariation, 1f + pitchVariation);

		newAudioSource.Play();

		if (!looping && !keepAlive)
		{
			Destroy(newGameObject, audioClip.length * 3.0f);
		}

		return newGameObject;
	}
}

