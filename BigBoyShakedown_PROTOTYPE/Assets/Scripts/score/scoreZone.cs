using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class scoreZone : MonoBehaviour
{
    [SerializeField] string CHARACTER_TAG;
    [SerializeField] int scoreChange;

    public event Action<int, int> OnCharacterEnter;
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
