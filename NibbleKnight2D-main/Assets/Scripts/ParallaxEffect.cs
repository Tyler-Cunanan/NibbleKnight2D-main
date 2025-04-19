using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{

    private float m_Starting_Pos;

    private float m_Length_Sprite;

    public float m_Movement_Speed;

    public Camera MainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Starting_Pos = transform.position.x;

        m_Length_Sprite = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Position = MainCamera.transform.position;

        float Temp = Position.x * (1 - m_Movement_Speed);

        float Distance = Position.x * m_Movement_Speed;

        Vector3 NewPosition = new Vector3(m_Starting_Pos + Distance, transform.position.y, transform.position.z);

        transform.position = NewPosition;
    }
}
