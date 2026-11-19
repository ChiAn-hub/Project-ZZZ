using UnityEngine;
using UnityEngine.UIElements;

public class ThirdPersonCamera : MonoBehaviour
{

    private Transform playerRoot;
    private Vector3 CameraOffset;
    private float distance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        playerRoot = GameObject.FindWithTag("Player").transform;
        CameraOffset = transform.position - playerRoot.position;
        transform.LookAt(playerRoot.position);
    }

    public void LateUpdate()
    {
        transform.position = playerRoot.position + CameraOffset;
        if (PlayerInput.CanControl)
        {
            ScrollView();
            RotateView();
        }
    }
    public void ScrollView()
    {
        distance = CameraOffset.magnitude;
        distance += Input.GetAxis("Mouse ScrollWheel") * 10.0f;
        distance = Mathf.Clamp(distance, 5, 40);
        CameraOffset = CameraOffset.normalized * distance;
    }
    public void RotateView()
    {
        Vector3 targetP = transform.position;
        Quaternion targetQ = transform.rotation;
        transform.RotateAround(playerRoot.position, Vector3.up, Input.GetAxis("Mouse X") * 2);
        transform.RotateAround(playerRoot.position, transform.right, Input.GetAxis("Mouse Y") * 1.5f);
        float currentX = transform.eulerAngles.x;

        if (currentX < 10f || currentX > 70f)
        {
            transform.rotation = targetQ;
            transform.position = targetP;
        }
        CameraOffset = transform.position - playerRoot.position;
    }
}
