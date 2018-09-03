using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {
  public AudioClip obtainNoise;
  public float obtainNoiseVolume = 1f;

  public float messageDisplayTime = 3f;

  private Animator anim;
  private AudioSource audio;

  void Start() {
    anim = GetComponent<Animator>();
    audio = GetComponent<AudioSource>();
  }

  void OnTriggerEnter2D(Collider2D other) {
    switch (other.gameObject.tag) {
      case "Projectile":
        Break();
        break;
    }
  }

  void Break() {
    anim.SetBool("broken", true);

    if (obtainNoise) audio.PlayOneShot(obtainNoise, obtainNoiseVolume);
    GetComponent<BoxCollider2D>().enabled = false;

    Time.timeScale = 0;

    GameObject.FindGameObjectsWithTag("Message")[0].GetComponent<SpriteRenderer>().enabled = true;
  }

  void Update() {
    Debug.Log("woa");
    if (Input.GetButtonDown("Shoot")) {
      Debug.Log("wow");
      RemoveMessage();
    }
  }

  void RemoveMessage() {
    GameObject.FindGameObjectsWithTag("Message")[0].GetComponent<SpriteRenderer>().enabled = false;
    Time.timeScale = 1;
  }
}
