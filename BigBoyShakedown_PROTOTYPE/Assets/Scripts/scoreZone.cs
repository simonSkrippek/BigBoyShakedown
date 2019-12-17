using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scoreZone : MonoBehaviour
{
    [SerializeField] string CHARACTER_TAG;
    [SerializeField] float scoreChange;

    public event Action<int, float> OnCharacterEnter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CHARACTER_TAG))
        {
            OnCharacterEnter?.Invoke(other.transform.root.GetComponent<PlayerInput>().playerIndex, scoreChange);
        }
    }
}
