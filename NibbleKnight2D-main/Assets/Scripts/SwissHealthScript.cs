using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwissHealthScript : MonoBehaviour
{

    public Slider m_HealthSlider;
    public float m_SwissCurrentHealth = 1.0F;


    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    void Update()
    {
        m_HealthSlider.value = m_SwissCurrentHealth;   
    }

    public void SwissDamaged(float damageTaken)
    {
        m_SwissCurrentHealth -= damageTaken;
        if(m_SwissCurrentHealth < 0)
        {
            m_SwissCurrentHealth = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnCollisionEnter2D(Collision2D Other)
    {
        if(Other.gameObject.CompareTag("EnemyNPC"))
        {
            Debug.Log("HIT!!");
            SwissDamaged(0.1F);
        }
        else if (Other.gameObject.CompareTag("WaterHazard"))
        {
            Debug.Log("Fell in sewer water");
            m_SwissCurrentHealth = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
