using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [Header("References")]
    public GameObject m_Options;
    public GameObject m_PauseMenuUI;
    public GameObject player;
    public GameObject hookProjectilePrefab;
    public Camera mainCam;
    public LineRenderer _lineRender;

    [Header("Grappling Settings")]
    public float RopeMaxLength = 10f;
    public float ropeSpeed = 5f;
    public float hookProjectileSpeed = 15f;

    [Header("Cooldown")]
    public float grappleCooldown = 1.5f;
    private bool canGrapple = true;

    [Header("Layer Masks")]
    public LayerMask hookableLayer;

    [Header("Joint References")]
    public DistanceJoint2D joint;

    private GameObject currentOb;
    private Vector2 currentAnchor;
    private bool hooked = false;
    private bool hookActive = false;

    public GrapplingGun grapplingGunScript;

    void Start()
    {
        joint.enabled = false;
        _lineRender.enabled = false;
    }

    void Update()
    {
        if (m_Options.activeInHierarchy || m_PauseMenuUI.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryStartGrapple();
        }

        if ((Input.GetKeyDown(KeyCode.Mouse1) || Input.GetButtonDown("Jump")) && hooked)
        {
            StopGrapple();
        }

        if (hooked)
        {
            DrawRope();

            float input = Input.GetKey(KeyCode.W) ? -1 : Input.GetKey(KeyCode.S) ? 1 : 0;
            joint.distance = Mathf.Clamp(joint.distance + input * ropeSpeed * Time.deltaTime, 1f, RopeMaxLength);
        }
    }

    public void TryStartGrapple()
    {
        if (!canGrapple || hookActive) return;

        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDir = (mouseWorldPos - (Vector2)transform.position).normalized;

        //Grab the Gun Pivot from GrapplingGun script position.
        GameObject projectile = Instantiate(hookProjectilePrefab, grapplingGunScript.gunPivot.transform.position, Quaternion.identity);
        
        HookProjectile hookScript = projectile.GetComponent<HookProjectile>();
        hookScript.Initialize(shootDir, hookProjectileSpeed, this, hookableLayer);

        hookActive = true;
        canGrapple = false;
    }

    public void StartGrappleFromProjectile(Vector2 hitPos, GameObject hitObject)
    {
        hookActive = false;
        currentAnchor = hitPos;
        currentOb = hitObject;
        StartCoroutine(AnimateRope(hitPos));
    }

    public void FailGrapple()
    {
        hookActive = false;
        canGrapple = true;
    }

    IEnumerator AnimateRope(Vector2 target)
    {
        hooked = true;
        _lineRender.enabled = true;
        _lineRender.SetPosition(0, transform.position);
        _lineRender.SetPosition(1, transform.position);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * ropeSpeed;
            Vector2 currentPoint = Vector2.Lerp(transform.position, target, t);
            _lineRender.SetPosition(1, currentPoint);
            yield return null;
        }

        StartGrapple();
    }

    void StartGrapple()
    {
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = currentAnchor;
        joint.distance = Vector2.Distance(currentAnchor, transform.position);
        joint.enabled = true;

        _lineRender.enabled = true;
        hooked = true;
    }

    void StopGrapple()
    {
        joint.enabled = false;
        _lineRender.enabled = false;
        hooked = false;
        StartCoroutine(GrappleCooldownTimer());
    }

    void DrawRope()
    {
        _lineRender.SetPosition(0, currentAnchor);
        _lineRender.SetPosition(1, transform.position);
    }

    IEnumerator GrappleCooldownTimer()
    {
        yield return new WaitForSeconds(grappleCooldown);
        canGrapple = true;
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
