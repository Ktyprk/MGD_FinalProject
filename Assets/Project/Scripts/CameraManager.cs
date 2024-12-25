using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public static CameraManager Instance;
    private Camera mainCam;
    
    private void Awake()
    {
        Instance = this;
        
        mainCam = Camera.main;
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    
    public void AssignTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    public Camera GetMainCamera()
    {
        return mainCam;
    }
}