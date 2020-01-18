using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapshot : MonoBehaviour
{
    private Transform followTransform;
    private Transform lookAtTransform;

    void Update()
    {
        if (followTransform != null && lookAtTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, followTransform.transform.position, Time.deltaTime * 3F);

            transform.LookAt(lookAtTransform);
        }
    }

    public void SetFollowTransform(Transform _transformToFollow)
    {
        followTransform = _transformToFollow;
    }

    public void SetLookAtTransform(Transform _transformToLookAt)
    {
        lookAtTransform = _transformToLookAt;
    }
}
