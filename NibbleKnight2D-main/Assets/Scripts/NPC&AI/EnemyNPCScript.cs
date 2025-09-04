using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyNPCScript : MonoBehaviour
{
    [Header("Health")]
    public int damagePlayer = 10; // Set different values per enemy

    [Header("NPC Controller Script")]
    public NpcController npcControllerScript;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitSound;

    public GameObject EnemySpiderMove;
    void Start()
    {
        npcControllerScript = GetComponent<NpcController>();

        // Optional safety: auto-assign audioSource if not set
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hook Projectile"))
        {
            Debug.Log("ENEMY GOT HIT!");
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }
            Destroy(this.gameObject);
            Destroy(EnemySpiderMove);
        }
    }
}
