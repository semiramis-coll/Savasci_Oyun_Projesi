using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    // Karakterin canı
    public int health = 100;

    // Hasar alma süresi kontrolü için
    public float damageCooldown = 1.0f;  // 1 saniyede bir hasar alır
    private float nextDamageTime = 0f;

    // Hareket için gerekli değişkenler
    public float movespeed;
    private Animator anim;
    private Rigidbody2D rb2d;
    float movehorizontal;
    public bool facingright;
    public float jumpforce;
    public bool isgrounded;
    public bool candoublejump;

    // Karakterin saldırı komutu
    PlayerCombat playercombat;

    void Start()
    {
        movespeed = 5;
        movehorizontal = Input.GetAxis("Horizontal");
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        playercombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        CharacterMovement();
        CharacterAnimation();
        CharacterAttack();
        CharacterRunAttack();
        CharacterJump();
    }

    void CharacterMovement()
    {
        movehorizontal = Input.GetAxis("Horizontal");
        rb2d.linearVelocity = new Vector2(movehorizontal * movespeed, rb2d.linearVelocity.y);
    }

    void CharacterAnimation()
    {
        if (movehorizontal > 0)
        {
            anim.SetBool("isRunning", true);
        }
        if (movehorizontal < 0)
        {
            anim.SetBool("isRunning", true);
        }
        if (movehorizontal == 0)
        {
            anim.SetBool("isRunning", false);
        }

        if (facingright == false && movehorizontal > 0)
        {
            CharacterFlip();
        }
        if (facingright == true && movehorizontal < 0)
        {
            CharacterFlip();
        }
    }

    void CharacterFlip()
    {
        facingright = !facingright;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    public void CharacterAttack()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("isAttack");
            playercombat.DamageEnemy();
            FindObjectOfType<AudioManager>().Play("sword");
        }
    }

    public void CharacterRunAttack()
    {
        if (Input.GetKeyDown(KeyCode.E) && movehorizontal != 0)
        {
            anim.SetTrigger("isRunAttack");
            playercombat.DamageEnemy();
            FindObjectOfType<AudioManager>().Play("swing");
        }
    }

    void CharacterJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("isJumping", true);
            if (isgrounded)
            {
                rb2d.linearVelocity = Vector2.up * jumpforce;
                candoublejump = true;
            }
            else if (candoublejump)
            {
                jumpforce = jumpforce / 1.5f;
                rb2d.linearVelocity = Vector2.up * jumpforce;
                candoublejump = false;
                jumpforce = jumpforce * 1.5f;
            }
        }
    }

    // Zıplama kontrolleri
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Grounded")
        {
            isgrounded = true;
            anim.SetBool("isJumping", false);
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "Grounded")
        {
            isgrounded = true;
        }

        // Düşmanla çarpışma anında hasar almak
        if (col.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (Time.time >= nextDamageTime) // Hasar alma süresi dolduysa
            {
                TakeDamage(20);  // Düşmandan alınan hasar miktarı
                nextDamageTime = Time.time + damageCooldown;  // Hasar alma aralığını ayarla
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Grounded")
        {
            isgrounded = false;
        }
    }

    // Hasar alma fonksiyonu
    void TakeDamage(int damage)
    {
        health -= damage;

        // Sağlık sıfıra düşerse ölüm animasyonu oynat
        if (health <= 0)
        {
            Die();
        }
    }

    // Ölüm fonksiyonu
    void Die()
    {
        Destroy(gameObject);  // Karakteri yok et
    }
}
