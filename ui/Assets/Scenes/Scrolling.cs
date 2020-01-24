using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    public Material material;
    public Vector2 scrollSpeed;
   

    // Update is called once per frame
    void Update()
    {
        material.mainTextureOffset += scrollSpeed * Time.deltaTime;
    }
}
