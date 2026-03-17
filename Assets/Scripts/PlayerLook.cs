using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{

      public static PlayerLook Instance;


    public float mouseSensitivity = 400f;
    public Transform cam;

    private float xRotation = 0f;
    private Vector2 lookInput;

    private float shakeDuration = 0f; 
    private float shakeMagnitude = 0.1f;
    private float shakeFadeTime = 1.5f;
    private Vector3 initalCamPos;


    public void Awake()
    {
       Instance = this;
    }

    void Start()
    {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        initalCamPos = cam.localPosition;

    }


    void Update()
    {
        
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        HandMouseLook();
        HandleShake();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
        
    }
    void HandMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cam.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
        lookInput = Vector2.zero; 
    }

    void HandleShake()
    {
        if (shakeDuration > 0)
        {
            cam.localPosition = initalCamPos + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * shakeFadeTime;
        }
        else
        {
            cam.localPosition = initalCamPos;
        }
    }

    public void AddShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }
}
