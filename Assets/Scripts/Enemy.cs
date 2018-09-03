using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
  private Vector2 start;

  void OnTriggerStay2D(Collider2D other) {
    GameObject ogo = other.gameObject;
    switch (ogo.tag) {
      case "Player":
        CharacterControl script = ogo.GetComponent<CharacterControl>();
        script.Damaged();
        break;
    }
  }

  void Start() {
    start = this.transform.position;
  }

  public void Reset() {
    Debug.Log(this);
    this.transform.position = start;
    GetComponent<SpriteRenderer>().enabled = true;
    GetComponent<BoxCollider2D>().enabled = true;
  }
}
