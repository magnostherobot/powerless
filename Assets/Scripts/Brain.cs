using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour {
  public float chargeInterval = 2f;
  public float chargeDuration = 1f;

  public GameObject wavePrefab;
  public Vector2 waveSpawnOffset = new Vector2(0, 0);
  public Vector2 waveVelocity = new Vector2(1, 0);

  private bool charging = false;
  private bool facingRight;

  private Animator anim;

  public bool Charging {
    get {
      return charging;
    }

    set {
      charging = value;
      if (anim) anim.SetBool("charging", value);
    }
  }

  void Start() {
    anim = GetComponent<Animator>();

    facingRight = !GetComponent<SpriteRenderer>().flipX;
  }

  void Awake() {
    Idle();
  }

  void Idle() {
    Charging = false;
    Invoke("Charge", chargeInterval);
  }

  void Charge() {
    Charging = true;
    Invoke("Fire", chargeDuration);
  }

  void Fire() {
    Vector2 spawnPosition = this.transform.position + (Vector3)((facingRight ? 1 : -1) * waveSpawnOffset);
    GameObject wave = Instantiate(wavePrefab, spawnPosition, Quaternion.identity);
    Rigidbody2D wrb = wave.GetComponent<Rigidbody2D>();
    wrb.velocity = waveVelocity * (facingRight ? 1 : -1);

    Idle();
  }
}
