using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float lookspeed;
    [SerializeField] Transform playercamera; // drop your camera here

    float Xrotation = 0f;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // locks cursor
        Cursor.visible = false; // hides cursor
    }
    void Update()
    {
        playerLook();
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
}
