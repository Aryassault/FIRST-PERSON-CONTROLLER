using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float jumpforce = 2f;
    bool isGrounded;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        MovePlayer();
        PlayerJump();
    }

    private void MovePlayer()
    {
        float xValue = Input.GetAxis("Horizontal");
        float zValue = Input.GetAxis("Vertical");

        Vector3 Movement = new Vector3(xValue, 0, zValue);
        transform.Translate(Movement * speed * Time.deltaTime);
    }

        // to check if player is touching the ground or not
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Ground") // if player is grounded then jump.
            {
                isGrounded = true;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Ground") // if player is not grounded then dont jump.
            {
                isGrounded = false;
            }
        }

    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // checking if space button is down and isgrounded is true.
        {
            rb.AddForce(Vector3.up * jumpforce);
        }
    }
}

