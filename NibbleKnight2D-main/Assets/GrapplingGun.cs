using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;
    public GrappleHook grappleHook;
    public Transform gunPivot;

    void Update()
    {
        AimGunAtMouse();

        if (Input.GetMouseButtonDown(0))
        {
            grappleHook.TryStartGrapple();
        }
    }

    void AimGunAtMouse()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;

        Vector3 direction = mouseWorldPosition - gunPivot.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0, 0, angle);
    }
}
