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
		if(soundClips.ContainsKey(soundName)) {
			AudioSource sound;
			if(soundClips.TryGetValue(soundName, out sound)) {
				sound.Play();
			}
		}
	}
}
