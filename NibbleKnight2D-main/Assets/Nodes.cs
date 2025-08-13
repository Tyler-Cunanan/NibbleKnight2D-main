using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour
{
    public List<Nodes> connections;

    // Optional: track previous node for AI path selection
    [HideInInspector]
    public Nodes previousNode;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f); // Draw node

        if (connections == null) return;

        foreach (var node in connections)
        {
            if (node != null)
                Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }
}
