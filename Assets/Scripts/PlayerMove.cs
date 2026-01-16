using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;
    public float jumpforce = 2f;
    bool isGrounded;
    Rigidbody rb;

    // Crouch settings
    private Vector3 originalScale;
    [Tooltip("Multiply the original Y scale by this when crouching (0.5 = half height)")]
    public float crouchScaleFactor = 0.5f;
    [Tooltip("Higher = faster scale transition")]
    public float crouchLerpSpeed = 8f;
    bool isCrouching;

    // Optional collider adjustment (CapsuleCollider is common for players)
    private CapsuleCollider capsule;
    private float originalCapsuleHeight;
    private Vector3 originalCapsuleCenter;

    // Sprint settings
    [Tooltip("Multiply movement speed by this while holding sprint key")]
    public float sprintMultiplier = 1.8f;
    [Tooltip("How quickly speed changes when starting/stopping sprint")]
    public float speedLerpSpeed = 10f;
    private float currentMoveSpeed;

    // Stamina system
    [Header("Stamina")]
    [Tooltip("Maximum stamina")]
    public float maxStamina = 5f; // seconds of sprint at full drain rate
    [Tooltip("Stamina drained per second while sprinting")]
    public float staminaDrainRate = 1f;
    [Tooltip("Stamina regenerated per second when not sprinting (after regen delay)")]
    public float staminaRegenRate = 0.8f;
    [Tooltip("Seconds to wait after stopping sprint before regeneration starts")]
    public float staminaRegenDelay = 1f;
    [Tooltip("Minimum stamina required to start sprinting again after exhaustion")]
    public float minStaminaToStartSprint = 0.5f;

    [Tooltip("Optional UI Image to show stamina (fillAmount used). Leave null if not using UI.")]
    public Image staminaBar;

    private float currentStamina;
    private float regenTimer;
    private bool isSprinting;
    private bool isExhausted;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
        currentMoveSpeed = speed;

        capsule = GetComponent<CapsuleCollider>();
        if (capsule != null)
        {
            originalCapsuleHeight = capsule.height;
            originalCapsuleCenter = capsule.center;
        }

        currentStamina = maxStamina;
        regenTimer = 0f;
        isSprinting = false;
        isExhausted = false;

        UpdateStaminaUI();
    }

    void Update()
    {
        HandleMovement();
        PlayerJump();
        HandleCrouchSmooth();
    }

    private void HandleMovement()
    {
        float xValue = Input.GetAxis("Horizontal");
        float zValue = Input.GetAxis("Vertical");

        // Determine if player wants to sprint (input + conditions)
        bool wantsToSprintInput = Input.GetKey(KeyCode.LeftShift)
                                  && !isCrouching
                                  && (Mathf.Abs(xValue) > 0f || Mathf.Abs(zValue) > 0f);

        // Update stamina state (this will set isSprinting/isExhausted and update currentStamina)
        UpdateStamina(wantsToSprintInput);

        // Determine target speed based on whether we're sprinting
        float targetSpeed = isSprinting ? speed * sprintMultiplier : speed;

        // Smoothly interpolate current move speed to target speed
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, targetSpeed, Mathf.Clamp01(speedLerpSpeed * Time.deltaTime));

        Vector3 Movement = new Vector3(xValue, 0, zValue);
        transform.Translate(Movement * currentMoveSpeed * Time.deltaTime);
    }

    private void UpdateStamina(bool sprintInput)
    {
        if (sprintInput && !isExhausted && currentStamina > 0f)
        {
            // Sprinting: drain stamina
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            regenTimer = 0f;
            if (currentStamina <= 0f)
            {
                currentStamina = 0f;
                isSprinting = false;
                isExhausted = true; // require recovery before sprinting again
            }
        }
        else
        {
            // Not sprinting: start regen timer and regenerate after delay
            isSprinting = false;
            regenTimer += Time.deltaTime;
            if (regenTimer >= staminaRegenDelay)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                }

                // If exhausted, allow sprinting again only after crossing threshold
                if (isExhausted && currentStamina >= minStaminaToStartSprint)
                {
                    isExhausted = false;
                }
            }
        }

        // Safety clamps
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = Mathf.Clamp01(currentStamina / maxStamina);
        }
    }

    // to check if player is touching the ground or not
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    void PlayerJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpforce);
        }
    }

    private void HandleCrouchSmooth()
    {
        bool crouchKey = Input.GetKey(KeyCode.LeftControl);

        // desired target scale only modifies Y
        Vector3 desiredScale = crouchKey
            ? new Vector3(originalScale.x, originalScale.y * crouchScaleFactor, originalScale.z)
            : originalScale;

        // smooth transition
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Mathf.Clamp01(crouchLerpSpeed * Time.deltaTime));

        // update crouch flag
        isCrouching = transform.localScale.y < originalScale.y - 0.001f;

        // If a CapsuleCollider exists, smoothly adjust its height and center to match the visual height.
        if (capsule != null)
        {
            float targetHeight = crouchKey ? originalCapsuleHeight * crouchScaleFactor : originalCapsuleHeight;
            capsule.height = Mathf.Lerp(capsule.height, targetHeight, Mathf.Clamp01(crouchLerpSpeed * Time.deltaTime));

            // Adjust center so the feet remain roughly in the same place.
            float heightRatio = capsule.height / originalCapsuleHeight;
            Vector3 targetCenter = new Vector3(originalCapsuleCenter.x, originalCapsuleCenter.y * heightRatio, originalCapsuleCenter.z);
            capsule.center = Vector3.Lerp(capsule.center, targetCenter, Mathf.Clamp01(crouchLerpSpeed * Time.deltaTime));
        }
    }
}