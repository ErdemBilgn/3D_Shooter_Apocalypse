using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public float Speed = 0;
    private Animator _animator = null;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        _animator.SetFloat("Speed", Speed);
    }
}
