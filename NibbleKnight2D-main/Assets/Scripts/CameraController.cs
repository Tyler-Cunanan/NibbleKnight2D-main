using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player Follow")]
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance = 1f;
    [SerializeField] private float cameraSpeed = 5f;

    [Header("Mouse Influence")]
    [SerializeField] private float maxMouseOffset = 2f;
    [SerializeField] private float mouseFollowSpeed = 3f;

    private float lookAhead;
    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (player == null) return;

        // Detect if player is pressing WASD
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
                        Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        // Update horizontal look-ahead
        lookAhead = Mathf.Lerp(lookAhead, aheadDistance * player.localScale.x, Time.deltaTime * cameraSpeed);

        // Base target camera position: player + lookAhead on X
        Vector3 baseTargetPos = new Vector3(
            player.position.x + lookAhead,
            player.position.y,
            transform.position.z
        );

        Vector3 finalTargetPos = baseTargetPos;

        if (!isMoving)
        {
            // Only apply mouse offset when idle
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Vector3 mouseOffset = mouseWorldPos - player.position;

            if (mouseOffset.magnitude > maxMouseOffset)
                mouseOffset = mouseOffset.normalized * maxMouseOffset;

            finalTargetPos += new Vector3(mouseOffset.x, mouseOffset.y, 0);
        }

        // Smoothly move camera to final position
        transform.position = Vector3.SmoothDamp(transform.position, finalTargetPos, ref currentVelocity, 1f / mouseFollowSpeed);
    }
}
