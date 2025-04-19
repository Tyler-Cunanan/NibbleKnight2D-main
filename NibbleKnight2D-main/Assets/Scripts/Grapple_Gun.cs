using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple_Gun : MonoBehaviour
{
    private LineRenderer _lineRender;
    public LayerMask _grappableEnviorment;
    public LayerMask _grappableObject;
    private SpringJoint joint;
    
    [Header("Objects")]
    public Transform FirePoint;
    public Transform Cam;
    public Transform player;
    public Rigidbody grappleSource;
    public float minRope = 1f;
    public float maxRope = 30f;
    public float maxDistance = 50f;
    private Vector3 _grabPoint;

    void Awake() {
        _lineRender = GetComponent<LineRenderer>();
    }

    void Update() {
        //Debug.Log("Hello World");
        //add in range for grapple
        //indicator/assist
        //adjust mass
        if(Input.GetMouseButtonDown(0)) {
            /**/
            Debug.Log("click");
            /**/
            startGrapple();
        }
        else if(Input.GetMouseButtonUp(0)) {
            Debug.Log("unclick");
            stopGrapple();
        }

        if(Input.GetKey("w") && joint) {
            if(joint.maxDistance < maxRope)
               joint.maxDistance -= Time.deltaTime;
        }
        if(Input.GetKey("s") && joint) {
            if(joint.maxDistance < minRope)
                joint.maxDistance += Time.deltaTime;
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    void DrawRope() {
        if(!joint) return;

        Debug.Log("Line");

        _lineRender.SetPosition(0, FirePoint.position);
        _lineRender.SetPosition(1, _grabPoint);
    }

    void startGrapple() {
        RaycastHit hit;
        Ray directHit = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log("pew");
        //UnityEngine.Physics.Raycast(directHit, out hit, maxDistance, _grappable);
        //Debug.Log(hit.point);

        //add in additional logic if the tag is either layer
        //envior will link player to ob. the other will link the ob to player
        if (UnityEngine.Physics.Raycast(directHit, out hit, maxDistance, _grappableEnviorment)) {
            Debug.Log("catch");
            _grabPoint = hit.point;
            _grabPoint.z = 0f;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = _grabPoint;
            joint.connectedBody = grappleSource;

            float toGrabPoint = Vector3.Distance(player.position, _grabPoint);
            joint.maxDistance = toGrabPoint * 0.4f;
            joint.minDistance = toGrabPoint * 0.3f;

            /**/
            joint.spring = 10f;
            joint.damper = 3f;
            joint.massScale = 4f;
            /**/

            _lineRender.positionCount = 2;
        }

        //function to stop the player streatching the rope to far
    }
    /**/
    void stopGrapple() {
        _lineRender.positionCount = 0;
        Destroy(joint);
    }
    /**/
}
