using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour {
  public Vector2 velocity = new Vector2(1, 0);

  public float raycastLength = 0.1f;

  private BoxCollider2D coll;
  private Rigidbody2D body;

  void Start() {
    coll = GetComponent<BoxCollider2D>();
    body = GetComponent<Rigidbody2D>();

    body.velocity = velocity;
  }

  void FixedUpdate() {
    Vector2 bounds = coll.bounds.extents;

    RaycastHit2D leftHit = Physics2D.Raycast(
        transform.position + Vector3.left * bounds.x + Vector3.down * bounds.y,
        Vector2.down, raycastLength);

    RaycastHit2D rightHit = Physics2D.Raycast(
        transform.position + Vector3.right * bounds.x + Vector3.down * bounds.y,
        Vector2.down, raycastLength);

    Debug.DrawRay(
        transform.position + Vector3.left * bounds.x + Vector3.down * bounds.y,
        Vector2.down, Color.blue);

    Debug.DrawRay(
        transform.position + Vector3.right * bounds.x + Vector3.down * bounds.y,
        Vector2.down, Color.red);

    if (!leftHit || !rightHit) {
      Flip();
    }
  }

  void Flip() {
    body.velocity = -body.velocity;
  }

  void Update() {
    transform.localScale = new Vector2(Mathf.Sign(body.velocity.x), 1);
  }
}
