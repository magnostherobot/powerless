using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyer : MonoBehaviour {
  public Vector2 velocity = new Vector2(1, 0);

  private Rigidbody2D body;

  void Start() {
    body = GetComponent<Rigidbody2D>();
    body.velocity = velocity;
  }

  void OnTriggerEnter2D(Collider2D other) {
    switch (other.gameObject.tag) {
      case "Platform":
        Flip();
        break;
    }
  }

  void OnTriggerStay2D(Collider2D other) {
    GameObject ogo = other.gameObject;
    switch (ogo.tag) {
      case "Player":
        CharacterControl cc = GetComponent<CharacterControl>();
        if (cc) cc.Damaged();
        break;
    }
  }

  void Flip() {
    body.velocity = -body.velocity;
  }

  void Update() {
    transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), 1);
  }
}
