using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour {
  public string colour = "yellow";
  public bool open = false;

  public GameObject player;
  public GameObject exit;

  public AudioClip openNoise;
  public float openNoiseVolume = 1f;

  public float playerProximity = 0.6f;
  public float playerPlacedOffset = 2f;

  private Animator anim;
  private BoxCollider2D coll;
  private AudioSource audio;

  void OnDrawGizmos() {
    if (exit != null) {
      Gizmos.color = Color.white;
      Gizmos.DrawLine(transform.position, exit.transform.position);
    }
  }

  void Start() {
    anim = GetComponent<Animator>();
    coll = GetComponent<BoxCollider2D>();
    audio = GetComponent<AudioSource>();

    if (exit) {
      DoorLogic dl = exit.GetComponent<DoorLogic>();
      dl.exit = this.gameObject;
    }

    if (!player) {
      player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    SetAnimationColour();
    OpenOrClosed(open);
  }

  public void Open() {
    if (openNoise) audio.PlayOneShot(openNoise, openNoiseVolume);

    OpenOrClosed(true);
  }

  void Close() {
    OpenOrClosed(false);
  }

  void OpenOrClosed(bool which) {
    open = which;
    anim.SetBool("open", which);
    coll.isTrigger = which;
  }

  void SetAnimationColour() {
    switch (colour) {
      case "yellow":
        anim.SetInteger("colour", 0);
        break;
      case "blue":
        anim.SetInteger("colour", 1);
        break;
      case "green":
        anim.SetInteger("colour", 2);
        break;
      case "red":
        anim.SetInteger("colour", 3);
        break;
    }
  }

  void FixedUpdate() {
    if (exit) {
      if (Vector2.Distance(player.transform.position, this.transform.position)
          < playerProximity) {
        Teleport(player, exit);
      }
    }
  }

  void Teleport(GameObject player, GameObject exit) {
    float yDiff = this.transform.position.y - player.transform.position.y;

    player.transform.position = exit.transform.position +
      (Vector3.right * (exit.GetComponent<SpriteRenderer>().flipX ? -1 : 1) *
       playerPlacedOffset) - Vector3.up * yDiff;

    ResetAll();
  }

  public void Reset() {
    Close();
  }

  void ResetAll() {
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    foreach (GameObject enemy in enemies) {
      Enemy script = enemy.GetComponent<Enemy>();
      script.Reset();
    }

    GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
    foreach (GameObject door in doors) {
      DoorLogic script = door.GetComponent<DoorLogic>();
      script.Reset();
    }
  }
}
