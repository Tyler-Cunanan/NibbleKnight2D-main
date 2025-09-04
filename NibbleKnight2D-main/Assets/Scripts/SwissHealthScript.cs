using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwissHealthScript : MonoBehaviour
{

    public Slider m_HealthSlider;
    public float m_SwissCurrentHealth = 100.0F;

    public bool invulnerable;
    [Header("Knockback")]
    public Rigidbody2D playerRb;
    public float knockbackForceX = 1000f; //Note: Due to the exist player chara physic/gravity that was modified by previous person, I just apply as many X force as possible
    public float knockbackForceY = 15f;

    void Update()
    {
        m_HealthSlider.value = m_SwissCurrentHealth;   
    }

    public void SwissDamaged(float damageTaken, Transform enemyTransform = null)
    {
        m_SwissCurrentHealth -= damageTaken;
        if(m_SwissCurrentHealth < 0)
        {
            m_SwissCurrentHealth = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (!invulnerable)
        {
            ApplyKnockback(enemyTransform);
        }
    }

    //void OnCollisionEnter2D(Collision2D Other)
    //{
    //    if (Other.gameObject.CompareTag("EnemyNPC") && !invulnerable)
    //    {
    //        Debug.Log("HIT!!");

    //        //Get the damage from the specific enemy's script
    //        EnemyNPCScript enemyDamageScript = Other.gameObject.GetComponent<EnemyNPCScript>();
    //        if (enemyDamageScript != null)
    //        {
    //            float damage = enemyDamageScript.damagePlayer;
    //            SwissDamaged(damage, enemyDamageScript.transform);
    //        }

    //        invulnerable = true;
    //        StartCoroutine(ResetGetHit());
    //    }
    //    else if (Other.gameObject.CompareTag("WaterHazard"))
    //    {
    //        Debug.Log("Fell in sewer water");
    //        m_SwissCurrentHealth = 0;
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //    }
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyNPC") && !invulnerable)
        {
            Debug.Log("WHY STYA!!");

            //Get the damage from the specific enemy's script
            EnemyNPCScript enemyDamageScript = collision.gameObject.GetComponent<EnemyNPCScript>();
            if (enemyDamageScript != null)
            {
                float damage = enemyDamageScript.damagePlayer;
                SwissDamaged(damage, enemyDamageScript.transform);
            }

            invulnerable = true;
            StartCoroutine(ResetGetHit());
        }
    }

    private void ApplyKnockback(Transform enemyTransform)
    {
        Vector2 knockDir = (transform.position - enemyTransform.position).normalized;
        Vector2 knockback = new Vector2(knockDir.x * knockbackForceX, knockbackForceY);
        playerRb.velocity = Vector2.zero; // Optional: cancel current motion
        playerRb.AddForce(knockback, ForceMode2D.Impulse);
    }

    private IEnumerator ResetGetHit()
    {
        yield return new WaitForSeconds(3f);
        invulnerable = false;
    }
}
