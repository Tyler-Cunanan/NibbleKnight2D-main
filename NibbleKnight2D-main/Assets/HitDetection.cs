using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private Rigidbody2D _R2D;
    // Start is called before the first frame update
    void Start()
    {
        _R2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D col) {
        if(col.gameObject.layer == 7) {
            switch(col.gameObject.tag) {
                case "Anchor":
                    _R2D.constraints = RigidbodyConstraints2D.FreezeAll;
                break;
                case "object":
                    col.transform.parent = transform;
                break;
                default:
                    //do nothing
                break;
            }
        }
    }
}
