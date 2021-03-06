﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset;

    private float cameraFollowSpeed = 10f;

    void Update()
    {
        Vector3 newPosition = player.position + offset;
        SetupCameraPosition(newPosition);
    }

    private void SetupCameraPosition(Vector3 newPosition)
    {
        transform.position = Vector3.Lerp(transform.position, newPosition, cameraFollowSpeed * Time.deltaTime);
    }
}
