using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Vector2 pos1;
    public Vector2 pos2;
    public float leftrightspeed;
    public float distance;
    public float followspeed;

    private float oldPosition;
    private Transform target;
    private Animator anim;

    void Start()
    {
        Physics2D.queriesStartInColliders = false;
        FindPlayer();  // Player'ı başlangıçta bul
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (target != null)  // Eğer player sahnede varsa takip etmeye devam et
        {
            EnemyAi();
        }
        else
        {
            // Eğer player yok edilirse, sadece devriye hareketi yap
            EnemyMove();
        }
    }

    // Player objesini bulmaya çalış
    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.GetComponent<Transform>();
        }
    }

    void EnemyMove()
    {
        // İki nokta arasında hareket etme
        transform.position = Vector3.Lerp(pos1, pos2, Mathf.PingPong(Time.time * leftrightspeed, 1.0f));

        if (transform.position.x > oldPosition)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (transform.position.x < oldPosition)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        oldPosition = transform.position.x;
    }

    void EnemyAi()
    {
        RaycastHit2D hitEnemy = Physics2D.Raycast(transform.position, -transform.right, distance);

        if (hitEnemy.collider != null)
        {
            // Eğer düşman bir şeyi algılarsa saldır
            Debug.DrawLine(transform.position, hitEnemy.point, Color.red);
            anim.SetBool("Attack", true);
            EnemyFollow();
        }
        else
        {
            // Algılanan bir şey yoksa devriye hareketine geri dön
            Debug.DrawLine(transform.position, transform.position - transform.right * distance, Color.green);
            anim.SetBool("Attack", false);
            EnemyMove();
        }
    }

    void EnemyFollow()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, gameObject.transform.position.y, target.position.x);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, followspeed * Time.deltaTime);
        }
    }
}
