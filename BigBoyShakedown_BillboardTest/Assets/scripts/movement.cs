using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    Vector3 currentMovement;
    [SerializeField] float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        currentMovement = new Vector3(Input.GetAxisRaw("Vertical"), 0f, -Input.GetAxisRaw("Horizontal"));
    }
    private void FixedUpdate()
    {
        transform.position += currentMovement * moveSpeed;
    }
}
