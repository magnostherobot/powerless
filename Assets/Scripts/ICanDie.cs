using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICanDie : MonoBehaviour {
  public float health = 3;

  public AudioClip deathNoise;
  public float deathNoiseVolume = 1f;

  private AudioSource audio;

  void Start() {
    audio = GetComponent<AudioSource>();
  }

  public void Hit(float damage) {
    health -= damage;

    if (health <= 0) {
      Die();
    }
  }

  public void Die() {
    if (deathNoise) audio.PlayOneShot(deathNoise, deathNoiseVolume);

    GetComponent<SpriteRenderer>().enabled = false;
    GetComponent<BoxCollider2D>().enabled = false;
  }
}
