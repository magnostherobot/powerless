using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
  public string colour = "yellow";
  public float damage = 1f;

  public AudioClip bounceNoise;
  public float bounceNoiseVolume = 1.0f;
  public AudioClip hitWallNoise;
  public float hitWallNoiseVolume = 1.0f;
  public AudioClip hitEnemyNoise;
  public float hitEnemyNoiseVolume = 1.0f;
  public AudioClip genericHitNoise;
  public float genericHitNoiseVolume = 1.0f;

  public bool bounceOnWrongDoors = true;

  private bool leftPlayer = false;

  private Rigidbody2D body;
  private AudioSource audio;

  void Start() {
    body = GetComponent<Rigidbody2D>();
    audio = GetComponent<AudioSource>();
  }

  void Update() {
    if (body.velocity.x < 0) {
      transform.localScale = new Vector2(-1, 1);
    } else if (body.velocity.x > 0) {
      transform.localScale = new Vector2(1, 1);
    } else {
      transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
    }
  }

  void OnTriggerEnter2D(Collider2D collider) {
    if (!leftPlayer) {
      leftPlayer = true;
      return;
    }

    if (genericHitNoise) audio.PlayOneShot(genericHitNoise, genericHitNoiseVolume);

    GameObject other = collider.gameObject;

    switch (other.tag) {
      case "Door":
        DoorLogic dl = other.GetComponent<DoorLogic>();
        if (dl.colour == this.colour) {
          dl.Open();
          Die();
        } else {
          if (bounceOnWrongDoors) { this.Bounce(); } else { Die(); }
        }
        break;

      case "Player":
        CharacterControl cc = other.GetComponent<CharacterControl>();
        cc.Damaged();
        Die();
        break;

      case "Enemy":
        if (hitEnemyNoise) audio.PlayOneShot(hitEnemyNoise, hitEnemyNoiseVolume);
        ICanDie icd = other.GetComponent<ICanDie>();
        if (icd) icd.Hit(damage);
        Die();
        break;

      case "Projectile":
        // do nothing
        break;

      default:
        if (hitWallNoise) audio.PlayOneShot(hitWallNoise, hitWallNoiseVolume);
        Die();
        break;
    }
  }

  void Die() {
    GetComponent<SpriteRenderer>().enabled = false;
    GetComponent<BoxCollider2D>().enabled = false;

    Invoke("ReallyDie", 3f);
  }

  void ReallyDie() {
    Destroy(this.gameObject);
  }

  void Bounce() {
    if (bounceNoise) audio.PlayOneShot(bounceNoise, bounceNoiseVolume);
    body.velocity = new Vector2(body.velocity.x * -1, body.velocity.y);
  }
}
