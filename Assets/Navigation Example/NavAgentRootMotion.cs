using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentRootMotion : MonoBehaviour
{
    // Inspector assigned variable.
    public AIWaypointNetwork WaypointNetwork = null;
    public int CurrentIndex = 0;
    public bool HasPath = false;
    public bool PathPending = false;
    public bool PathStale = false;
    public NavMeshPathStatus PathStatus = NavMeshPathStatus.PathInvalid;
    public AnimationCurve JumpCurve = new AnimationCurve();
    public bool MixedMode = true;

    // private Members.
    NavMeshAgent _navAgent = null;
    Animator _animator = null;
    private float _smoothAngle = 0;

    void Start()
    {
        // Cache NavMeshAgent Reference.
        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();


        //navAgent.updatePosition = false;
        _navAgent.updateRotation = false;

        if (WaypointNetwork == null) return;

        SetNextDestination(false);
    }

    void SetNextDestination(bool increment)
    {
        if (!WaypointNetwork) return;

        int incStep = increment ? 1 : 0;

        int nextWaypoint = (CurrentIndex + incStep >= WaypointNetwork.WayPoints.Count) ? 0 : CurrentIndex + incStep;
        Transform nextWaypointTransform = WaypointNetwork.WayPoints[nextWaypoint];

        if (nextWaypointTransform != null)
        {
            CurrentIndex = nextWaypoint;
            _navAgent.SetDestination(nextWaypointTransform.position);
            return;
        }

        CurrentIndex++;
        
    }

    void Update()
    {

        

        HasPath = _navAgent.hasPath;
        PathPending = _navAgent.pathPending;
        PathStale = _navAgent.isPathStale;
        PathStatus = _navAgent.pathStatus;

        Vector3 localDesiredVelocity = transform.InverseTransformVector(_navAgent.desiredVelocity);
        float angle = Mathf.Atan2(localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;
        _smoothAngle = Mathf.MoveTowardsAngle(_smoothAngle, angle, 80 * Time.deltaTime);

        float speed = localDesiredVelocity.z;

        _animator.SetFloat("Angle", _smoothAngle);
        _animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);


        if(_navAgent.desiredVelocity.sqrMagnitude > Mathf.Epsilon)
        {
            if(!MixedMode || 
                (MixedMode && Mathf.Abs(angle) < 80 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion")))
            {
                Quaternion lookRotation = Quaternion.LookRotation(_navAgent.desiredVelocity, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5.0f * Time.deltaTime);
            }
            
        }


        /*if(navAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(1));
            return;
        }*/

        if((!HasPath && !PathPending) || PathStatus == NavMeshPathStatus.PathInvalid  || _navAgent.remainingDistance <= _navAgent.stoppingDistance /*|| PathStatus == NavMeshPathStatus.PathPartial*/)
        {
            SetNextDestination(true);
        }
        else
        {
            if(_navAgent.isPathStale)
            {
                SetNextDestination(false);
            }
        }
    }

    void OnAnimatorMove()
    {
        if(MixedMode && !_animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Locomotion"))
        {
            transform.rotation = _animator.rootRotation;

        }
        _navAgent.velocity = _animator.deltaPosition / Time.deltaTime;
    }

    IEnumerator Jump(float duration)
    {
        OffMeshLinkData data = _navAgent.currentOffMeshLinkData;
        Vector3 startPos = _navAgent.transform.position;
        Vector3 endPos = data.endPos + (_navAgent.baseOffset * Vector3.up);

        float time = 0;
        while (time <= duration)
        {
            float t = time / duration;
            _navAgent.transform.position = Vector3.Lerp(startPos, endPos, t) + (JumpCurve.Evaluate(t) * Vector3.up);
            time += Time.deltaTime;
            yield return null;
        }

        _navAgent.CompleteOffMeshLink();
    }
}
