using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameTransitionPlayer : MonoBehaviour
{
    public static InGameTransitionPlayer instance;
    public Animator transition;

    public float endAnimationLength = 1f;
    public event Action animationComplete;

    private void Awake()
    {
        if (instance != null) Destroy(this.gameObject);
        else instance = this;
    }
    public void PlayEndOfLevelAnimation()
    {
        StartCoroutine(PlayAnimation());
    }

    IEnumerator PlayAnimation()
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(endAnimationLength);

        animationComplete?.Invoke();      
    }
}
