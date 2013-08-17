using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager logic { get; private set; }

    void Awake() {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    void Start() {
        logic = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public AudioSource PlayAudio(AudioClip clip, Transform emitter, float volume = 1.0f, float pitch = 1.0f) {
        GameObject go = new GameObject("Audio (" + clip.name + ")");
        go.transform.position = emitter.position;
        go.transform.parent = emitter;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
}
