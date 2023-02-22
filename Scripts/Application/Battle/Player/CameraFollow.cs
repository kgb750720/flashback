using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Camera otherCamera;
    private Camera playerCamera;
    private Transform followTarget;
    private bool isInit = false;


    public float smooth = 0.1f;
    
    public void SetCameraAndTarget(Camera other, Camera player, Transform target)
    {
        otherCamera = other;
        playerCamera = player;
        followTarget = target;
        isInit = true;
    }

    private void Update()
    {
        if (isInit)
        {
            Vector2 cameraFollow = Vector2.Lerp(playerCamera.transform.position, followTarget.transform.position, smooth);
            otherCamera.transform.position = cameraFollow;
            playerCamera.transform.position = cameraFollow;

        }
    }
}
