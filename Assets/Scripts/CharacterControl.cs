using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour {
  public float moveForce = 365f;
  public AudioClip walkNoise;
  public float walkNoiseVolume = 1f;
  public float walkNoiseFrequency = 1f;

  public float jumpForce = 1000f;
  public float jumpSustainForce = 0f;
  public AudioClip jumpNoise;
  public float jumpNoiseVolume = 1f;
  public AudioClip landNoise;
  public float landNoiseVolume = 1f;

  public bool limitHorizontalSpeed = true;
  public float maxHorizontalSpeed = 5f;
  public bool limitHorizontalSpeedWhenDamaged = false;

  public bool limitVerticalSpeed = true;
  public float maxVerticalSpeed = 5f;
  public bool limitVerticalSpeedWhenDamaged = false;
  public bool limitJumpSpeed = false;

  public float damageDuration = 1f;
  public Vector2 knockbackForce = new Vector2(-400f, 200f);
  public AudioClip damageNoise;
  public float damageNoiseVolume = 1f;

  public GameObject bulletPrefab;
  public AudioClip shootNoise;
  public float shootNoiseVolume = 1f;
  public float bulletSpawnHeightOffset = 0f;
  public float bulletSpeed = 1f;
  public float shootCooldown;

  private bool grounded;
  private bool damaged;
  private bool facingRight;
  private bool shotRecently;

  private Rigidbody2D body;
  private BoxCollider2D coll;
  private Animator anim;
  private Transform trans;
  private AudioSource audio;

  private float distToGround;

	void Start() {
    body = GetComponent<Rigidbody2D>();
    coll = GetComponent<BoxCollider2D>();
    anim = GetComponent<Animator>();
    trans = GetComponent<Transform>();
    audio = GetComponent<AudioSource>();

    distToGround = coll.bounds.extents.y;
	}

  void Awake() {
    Invoke("WalkNoisily", 0.1f);
  }

  void WalkNoisily() {
    if (grounded && !damaged && (Input.GetButton("Move Left") || Input.GetButton("Move Right"))) {
      if (walkNoise) audio.PlayOneShot(walkNoise, walkNoiseVolume);
    }

    Invoke("WalkNoisily", walkNoiseFrequency);
  }

	void Update() {
    if (!damaged) {
      if (Input.GetButtonDown("Damage")) {
        Damaged();
      }
    }

    grounded = IsGrounded();
    anim.SetBool("grounded", grounded);
    if (grounded && body.velocity.y < 0) Undamaged();

    if (!damaged) {
      if (grounded && Input.GetButtonDown("Jump")) {
        Jump();
      }

      if (!shotRecently && Input.GetButtonDown("Shoot")) {
        Shoot();
      }

      anim.SetBool("looking_up", Input.GetButton("Look Up"));
    }
	}

  void FixedUpdate() {
    if (!damaged) {
      Walk();
    }

    LimitHorizontalSpeed();
    LimitVerticalSpeed();

    if (!damaged && !grounded && Input.GetButton("Jump") && body.velocity.y > 0) {
      JumpHigher();
    }

    FaceMovement();
  }

  void Shoot() {
    shotRecently = true;
    anim.SetBool("just_shot", true);

    if (shootNoise) audio.PlayOneShot(shootNoise, shootNoiseVolume);

    Vector3 spawnPosition = trans.position + new Vector3(0, bulletSpawnHeightOffset);

    GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

    if (Input.GetButton("Look Up")) {
      rb.velocity = Vector3.up * bulletSpeed;
    } else if (facingRight) {
      rb.velocity = Vector3.right * bulletSpeed;
    } else {
      rb.velocity = Vector3.left * bulletSpeed;
    }

    Invoke("Unshoot", shootCooldown);
  }

  void Unshoot() {
    shotRecently = false;
    anim.SetBool("just_shot", false);
  }

  public void Damaged() {
    if (damaged) return;

    if (damageNoise) audio.PlayOneShot(damageNoise, damageNoiseVolume);

    // reset momentum
    body.velocity = Vector2.zero;

    body.AddForce(new Vector2(trans.localScale.x * knockbackForce.x, knockbackForce.y));

    damaged = true;
    anim.SetBool("hurt", true);

    Invoke("Undamaged", damageDuration);
  }

  void Undamaged() {
    damaged = false;
    anim.SetBool("hurt", false);
  }

  bool IsGrounded() {
    float xSize = coll.bounds.extents.x;

    Debug.DrawRay(
        transform.position + Vector3.left * xSize + Vector3.down * distToGround,
        Vector2.down * 0.1f, Color.red);
    Debug.DrawRay(
        transform.position + Vector3.right * xSize + Vector3.down * distToGround,
        Vector2.down * 0.1f, Color.red);

    bool ret = Physics2D.Raycast(
        transform.position + Vector3.left * xSize + Vector3.down * (distToGround - 0.1f),
        Vector2.down, 0.2f)
      || Physics2D.Raycast(
          transform.position + Vector3.right * xSize + Vector3.down * (distToGround - 0.1f),
          Vector2.down, 0.2f);

    if (ret && !grounded) {
      if (landNoise) audio.PlayOneShot(landNoise, landNoiseVolume);
    }

    return ret;
  }

  void Jump() {
    if (jumpNoise) audio.PlayOneShot(jumpNoise, jumpNoiseVolume);

    body.AddForce(Vector2.up * jumpForce);
  }

  void JumpHigher() {
    body.AddForce(Vector2.up * jumpSustainForce);
  }

  void Walk() {
    if (Input.GetButton("Move Right")) {
      anim.SetBool("walking", true);
      facingRight = true;
      body.AddForce(Vector2.right * moveForce);
    }

    if (Input.GetButton("Move Left")) {
      anim.SetBool("walking", true);
      facingRight = false;
      body.AddForce(Vector2.left * moveForce);
    }

    if (!Input.GetButton("Move Right") && !Input.GetButton("Move Left")) {
      anim.SetBool("walking", false);
      body.velocity = new Vector2(0, body.velocity.y);
    }
  }

  void LimitHorizontalSpeed() {
    if (limitHorizontalSpeed && (!damaged || limitHorizontalSpeedWhenDamaged)) {
      if (body.velocity.x > maxHorizontalSpeed) {
        body.velocity = new Vector2(maxHorizontalSpeed, body.velocity.y);
      } else if (body.velocity.x < -maxHorizontalSpeed) {
        body.velocity = new Vector2(-maxHorizontalSpeed, body.velocity.y);
      }
    }
  }

  void LimitVerticalSpeed() {
    if (limitVerticalSpeed && (!damaged || limitVerticalSpeedWhenDamaged)) {
      if (limitJumpSpeed && body.velocity.y > maxVerticalSpeed) {
        body.velocity = new Vector2(body.velocity.x, maxVerticalSpeed);
      } else if (body.velocity.y < -maxVerticalSpeed) {
        body.velocity = new Vector2(body.velocity.x, -maxVerticalSpeed);
      }
    }
  }

  void FaceMovement() {
    float scale = Mathf.Abs(trans.localScale.x);

    trans.localScale = new Vector2((facingRight ? 1 : -1) * scale, scale);
  }
}
