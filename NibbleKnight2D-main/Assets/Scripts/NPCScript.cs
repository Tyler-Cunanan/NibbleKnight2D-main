using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCScript : MonoBehaviour
{

    public bool m_hasObjective = false;
    public int m_numOfPointsNeeded = 0;

    public bool m_isFinished = false;

    public GameObject m_TextBubble;

    public GameObject m_DestructableObjective;

    public TMP_Text m_Text;

    public string m_QuestDialog;

    public string m_NotEnoughDialog;

    public string m_FinishedDialog;

    public string m_TalkingDialog;

    public NpcController aiMovementScript;

    // Start is called before the first frame update
    void Start()
    {
        aiMovementScript = GetComponent<NpcController>();
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // if(m_TextBubble.activeInHierarchy && Input.GetKeyDown(KeyCode.Return))
    //     // {
    //     //     m_TextBubble.SetActive(false);
    //     // }
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Player collides with NPC
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ran into NPC.");
            aiMovementScript.enabled = false;

            //Is the quest already completed.
            if (m_isFinished)
            {
                m_TextBubble.SetActive(true);
                m_Text.text = m_FinishedDialog;
                return;
            }
            //If NPC has an quest to give to the player.
            else if (m_hasObjective && !m_TextBubble.activeInHierarchy)
            {
                SwissInventoryScript m_inv = collision.gameObject.GetComponent<SwissInventoryScript>();
                m_TextBubble.SetActive(true);
                //Not enough coins collected.
                if (m_inv.GetCoinsCollected() < m_numOfPointsNeeded)
                {
                    // Debug.Log(m_NotEnoughDialog);
                    m_Text.text = m_NotEnoughDialog;
                    // Debug.Log("Current coin count: " + m_inv.GetCoinsCollected() + " Needed coin count: " + m_numOfPointsNeeded);
                }
                //Complete the quest.
                else
                {
                    m_inv.SubtractCoins(m_numOfPointsNeeded);
                    m_Text.text = m_QuestDialog;
                    // Debug.Log("New coin count: " + m_inv.GetCoinsCollected());
                    if (m_DestructableObjective != null)
                    {
                        m_DestructableObjective.SetActive(false);
                    }
                    m_isFinished = true;
                    m_hasObjective = false;
                }
            }
            //NPC has only a talking bubble.
            else if (!m_hasObjective)
            {
                m_TextBubble.SetActive(true);
                m_Text.text = m_TalkingDialog;
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (m_TextBubble != null && m_TextBubble.activeInHierarchy)
        {
            m_TextBubble.SetActive(false);
            aiMovementScript.enabled = true;
        }
    }
}
