using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponents : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerInput playerInput;
    new public Rigidbody rigidbody;
    public MeshRenderer modelMeshRenderer;
    public MeshFilter modelMeshFilter;
    public MeshRenderer quadMeshRenderer;
    public BoxCollider modelBoxCollider;
}
