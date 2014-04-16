using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
	private Dictionary<string, AudioSource> soundClips = new Dictionary<string, AudioSource>(); 

	void Start () {
		AudioSource[] sounds = transform.root.GetComponentsInChildren<AudioSource>();
		foreach(AudioSource sound in sounds) {
			soundClips.Add(sound.name, sound);
		}
	}

	void Update () {
	
	}

	public void PlaySound(string soundName) {
		AudioSource sound;
		if(soundClips.TryGetValue(soundName, out sound)) {
			if(!sound.isPlaying) sound.Play();
		}
	}

	public void StopSound(string soundName) {
		AudioSource sound;
		if(soundClips.TryGetValue(soundName, out sound)) {
			sound.Stop();
		}
	}

	public bool IsPlayingSound(string soundName) {
		AudioSource sound;
		if(soundClips.TryGetValue(soundName, out sound)) {
			return sound.isPlaying;
		}
		return false;
	}
}
