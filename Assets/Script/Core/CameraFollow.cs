using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    private Vector3 cameraOffset;

    [Range(0.01f, 1.0f)]
    public float smoothness = 0.5f;

    public float zoomSpeed = 2.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 20.0f;

    private float currentZoom = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        cameraOffset = transform.position - player.transform.position;
        currentZoom = cameraOffset.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        // 處理滑鼠滾輪縮放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Vector3 newPos = player.position + cameraOffset.normalized * currentZoom;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothness);
    }
}

