using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float lookspeed;
    [SerializeField] Transform playercamera; // drop your camera here

    RaycastHit hit;

    float Xrotation = 0f;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // locks cursor
        Cursor.visible = false; // hides cursor
    }

    void Update()
    {
        playerLook();
        itemIntraction();
    }

    private void playerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookspeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookspeed;

        transform.Rotate(Vector3.up * mouseX); // allow player to turn left/right

        Xrotation -= mouseY;
        Xrotation = Mathf.Clamp(Xrotation, -80f, 80f); // stops camera from clamping
        playercamera.localRotation = Quaternion.Euler(Xrotation, 0, 0); // allow camera to turn up/down
    }

    private void itemIntraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(transform.position, transform.forward, out hit, 10f);
            Debug.DrawRay(transform.position, transform.forward, color: Color.red);
            Debug.Log($"you collided with {hit.collider.gameObject.name}");
        }
    }
}
