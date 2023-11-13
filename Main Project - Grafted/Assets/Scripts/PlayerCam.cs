using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField]
    protected Transform trackingTarget;
    public Vector3 offset = new Vector3(0, 1, 0);  // Example offset
    public float cameraSpeed = 0.1f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (trackingTarget == null)
        {
            return;
        }
        Vector3 targetPosition = new Vector3(trackingTarget.position.x + offset.x, trackingTarget.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
    }
}
