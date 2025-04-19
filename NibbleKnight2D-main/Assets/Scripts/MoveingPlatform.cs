using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MoveingPlatform : MonoBehaviour
{

    public Transform[] points;
    public int StartingPoint;
    private int currentPos;

    public float speed;



    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[StartingPoint].position;
        currentPos = StartingPoint;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[currentPos].position) < 0.02f){
            currentPos++;
            if(currentPos == points.Length) {
                currentPos = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[currentPos].position, speed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D col) {
        col.transform.SetParent(transform);
    }
    
    private void OnCollisionExit2D(Collision2D col) {
        col.transform.SetParent(null);        
    }

    private void OnDrawGizmos()
    {
        int i = 1;
        int end = points.Length;
        Gizmos.DrawWireSphere(points[0].position, 0.5f);
        while(i < end) {
            Gizmos.DrawWireSphere(points[i].position, 0.5f);
            Gizmos.DrawLine(points[i].position, points[i-1].position);
            i += 1;
        }
    }

}
