using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    public Gun gun;
    private bool isHoldingShoot = false;

    void OnShoot(InputValue value)
    {
        float val = value.Get<float>();
        isHoldingShoot = val > 0.5f; 
    }

    void OnReload(InputValue value)
    {
        if (gun != null)
            gun.TryReload();
    }

    void Update()
    {
        if (isHoldingShoot && gun != null)
            gun.Shoot();
    }
}