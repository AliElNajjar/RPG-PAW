using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] private float speed = .05f;
    
    void Update()
    {
        transform.Translate(-Vector3.up * Time.deltaTime * speed);
    }
}
