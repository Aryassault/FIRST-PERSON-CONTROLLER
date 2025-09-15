using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] float lookspeed;
    [SerializeField] Transform playercamera;

    float Xrotation = 0f;
    

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        playerLook();
    }

    private void playerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookspeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookspeed;

        // allow player to turn left/right
        transform.Rotate(Vector3.up * mouseX);

        Xrotation -= mouseY;
        Xrotation = Mathf.Clamp(Xrotation, -80f, 80f);
        playercamera.localRotation = Quaternion.Euler(Xrotation, 0, 0);
    }
}
