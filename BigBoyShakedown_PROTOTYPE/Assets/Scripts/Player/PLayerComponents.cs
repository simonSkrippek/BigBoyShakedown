using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PLayerComponents : MonoBehaviour
{
    public PlayerInput playerInput;
    new public Rigidbody rigidbody;
    public SkinnedMeshRenderer modelMeshRenderer;
    public MeshRenderer quadMeshRenderer;
    public BoxCollider modelBoxCollider;
}
