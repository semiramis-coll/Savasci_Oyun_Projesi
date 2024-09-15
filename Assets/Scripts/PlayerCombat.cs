using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public int maxHealth = 100;
    int currentHealth;


    private void Start()
    {
        currentHealth = maxHealth ;
    }

    public void DamageEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            //Debug.Log("Zarar" + enemy.name);

            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
            FindObjectOfType<AudioManager>().Play("swing");
            FindObjectOfType<AudioManager>().Play("hurt");

        }

    }

    public void OnDrawGizmosSelected()
    {
        if (attackPoint==null)
            return;


        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        

        if (currentHealth <= 0)
        {
            Die();
        }
        void Die()
        {

           

            this.enabled = false;
            GetComponent<Collider2D>().enabled = false;
           
            Destroy(gameObject, 1.6f);


        }

    }





}
