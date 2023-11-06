using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 0.5f;
    private Transform target;
    private Vector3 offset = new Vector3(0, 0.3f, -10);

    private Vector3 velocity = Vector3.zero;


    private void Awake()
    {
        target = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition + offset;

    }


}
