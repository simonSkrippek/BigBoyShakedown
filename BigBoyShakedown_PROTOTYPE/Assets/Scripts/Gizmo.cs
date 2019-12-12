using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    [Range(.1f,10f)]
    [SerializeField] private float size;
    [ColorUsage(true)]
    [SerializeField] private Color color = Color.yellow;

    void OnDrawGizmos()
    {
        Gizmos.color = this.color;
        Gizmos.DrawWireSphere(this.transform.position, this.size);
    }
}
