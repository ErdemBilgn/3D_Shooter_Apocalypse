using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorState { Open, Animating, Closed};

public class SlidingDoorDemo : MonoBehaviour
{
    public float SlidingDistance = 4.5f;
    public float Duration = 1.5f;
    public AnimationCurve JumpCurve = new AnimationCurve();

    Vector3 _openPos = Vector3.zero;
    Vector3 _closedPos = Vector3.zero;
    DoorState _doorState = DoorState.Closed;

    void Start()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + (transform.right * SlidingDistance);
    }


    void Update()
    {
        if(_doorState != DoorState.Animating && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AnimateDoor((_doorState == DoorState.Open) ? DoorState.Closed : DoorState.Open));
        }
    }

    IEnumerator AnimateDoor(DoorState newState)
    {
        _doorState = DoorState.Animating;
        float time = 0;
        Vector3 startPos = (newState == DoorState.Open) ? _closedPos : _openPos;
        Vector3 endPos = (newState == DoorState.Open) ? _openPos : _closedPos;

        while (time <= Duration)
        {
            float t = time / Duration;
            transform.position = Vector3.Lerp(startPos, endPos, JumpCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        _doorState = newState;
    }
}
