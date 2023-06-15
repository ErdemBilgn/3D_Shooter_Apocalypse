using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMount : MonoBehaviour
{

    public Transform Mount = null;
    public float Speed = 5f;

    
    void Start()
    {
        
    }

    
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, Mount.position, Time.deltaTime * Speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Mount.rotation, Time.deltaTime * Speed);
    }
}
