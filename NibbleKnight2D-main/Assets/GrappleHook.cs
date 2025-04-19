using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{

    public GameObject m_Options;
    public GameObject m_PauseMenuUI;
    public GameObject player;
    public GameObject Hook;
    private GameObject _Hook, currentOb, newOb;
    public Camera mainCam;
    public LineRenderer _lineRender;
    public DistanceJoint2D joint;
    private DistanceJoint2D moveingJoint;
    public float RopeMaxLength = 10f;
    public float ropeSpeed = 5f;
    public float hookSpeed;

    public LayerMask _grappableEnviorment;
    private Vector2 mousePos, currentAnchor;
    private string obTag;
    private GameObject GrabbedObject;

    [SerializeField] private Transform[] hooks;
    private int hookCount = 0;
    
    private bool hooked = false;


    // Start is called before the first frame update
    void Start()
    {
        joint.enabled = false;
        _lineRender.enabled = false;    
    }

    // Update is called once per frame
    void Update()
    {
        //RaycastHit hit;
        //Ray directHit = Camera.main.ScreenPointToRay(Input.mousePosition);
        if((!m_Options.activeInHierarchy && !m_PauseMenuUI.activeInHierarchy ))
        {

            //if (Input.GetKeyDown(KeyCode.shift)) {
                // moved through the list
            //}

            if (Input.GetKeyDown(KeyCode.Mouse0)) {
            mousePos = (Vector2)mainCam.ScreenToWorldPoint(Input.mousePosition);
            Collider2D col = Physics2D.OverlapCircle(mousePos, 0.3f, _grappableEnviorment);
            newOb = col.gameObject;
            if(newOb.layer == _grappableEnviorment && newOb != currentOb)
                if(_Hook) {
                    stopGrapple();
                }
                obTag = newOb.tag;
                currentOb = newOb;
                currentAnchor = newOb.transform.position;
            //_Hook = Instantiate(Hook, playerPos.position, playerPos.rotation);
                startGrapple();
            }
            if ((Input.GetKeyDown(KeyCode.Mouse1) || (Input.GetButtonDown("Jump") && (obTag != "Object"))) && hooked) {
                // releace grapple if player right clicks. or jumps on a non grapple object
                stopGrapple();
            }

            if(obTag == "Object") {
                moveingJoint.connectedAnchor = (Vector2)transform.position;
                _Hook.transform.position = currentOb.transform.position;
            }

            if (hooked) {
                DrawRope();
                if(Input.GetKey(KeyCode.W)) {
                    if(joint.distance > 1f)
                        if(obTag == "Object")
                            moveingJoint.distance = moveingJoint.distance - (Time.deltaTime * ropeSpeed);
                        else
                            joint.distance = joint.distance - (Time.deltaTime * ropeSpeed);
                }
                if(Input.GetKey(KeyCode.S)) {                
                    if(joint.distance < RopeMaxLength)
                        if(obTag == "Object")
                            moveingJoint.distance = moveingJoint.distance + (Time.deltaTime * ropeSpeed);
                        else
                            joint.distance = joint.distance + (Time.deltaTime * ropeSpeed);
                }
            }
            else {
                Vector2 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                //RotateGun(mousePos, true);
            }

            if(joint.enabled) {
                _lineRender.SetPosition(1, transform.position);
            }
        }
    }

    /**/
    private void startGrapple() {
        //joint = playerPos.gameObject.AddComponent<DistanceJoint2D>();

        if (obTag == "Object") {
            moveingJoint = currentOb.AddComponent<DistanceJoint2D>();
            moveingJoint.autoConfigureConnectedAnchor = false;
            moveingJoint.anchor = transform.position;
            moveingJoint.connectedAnchor = (Vector2)transform.position;
            moveingJoint.connectedBody = player.GetComponent<Rigidbody2D>();
            moveingJoint.distance = Vector2.Distance(currentAnchor, transform.position);

            moveingJoint.enabled = true;
        } else {
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = (Vector2)currentOb.transform.position;
            joint.distance = Vector2.Distance(currentAnchor, transform.position);
            joint.enabled = true;
        }
        _Hook = Instantiate(Hook, (Vector2)currentAnchor, Quaternion.identity);
        _lineRender.enabled = true;
        hooked = true;
    }

    private void stopGrapple() {
        Destroy(_Hook);
        if(obTag == "Object") {
            moveingJoint.enabled = false;
            Destroy(moveingJoint);
        }
        else
            joint.enabled = false;
        obTag = "";
        _lineRender.enabled = false;
        hooked = false;
    }

    void DrawRope() {
        _lineRender.SetPosition(0, currentOb.transform.position);
        _lineRender.SetPosition(1, transform.position);
    }

    /**

    void OnTriggerEnter2D(Collider2D col) {
        if(hookCount < hook.Length) {
            hooks[hookCount] = col.transform.position;
            hookCount++;
        } else {
            for(int i = 0; i < hooks.Length; i++) {
            
            }
            hooks[hookCount - 1] = col.transform.position
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        int shift the arrat
        for (int i = 0; i < hookCount; i++) {
            if(hook[i] == col.transform.postiion) {
            
            }
        }
        hooks[hookCount] = col.transform.position;
        hookCount--;
    }
    /**/

    /**
    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void grappleUp() {
        
    }

    private void grappleDown() {

    }
    /**/
}
