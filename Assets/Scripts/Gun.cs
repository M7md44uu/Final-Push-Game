using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magazineSize = 30;
    public GameObject bullet;
    public Transform bulletSpawnPoint;

    public AudioClip shootSound;

    public GameObject weaponFlash;

    private int currentAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Quaternion initalRotation;       
    private Vector3 initalPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);

    void Start()
    {
        currentAmmo = magazineSize;
        initalRotation = transform.localRotation;
        initalPosition = transform.localPosition;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextFireTime) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return; 
        }

        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        UIManager.Instance.ammoText.text = currentAmmo.ToString();

        AudioManager.Instance.PlaySFX(shootSound, 0.25f);

        Quaternion ajustRotation = bulletSpawnPoint.rotation * Quaternion.Euler(-2f, -3f, 0f); 

        Instantiate(bullet, bulletSpawnPoint.position, ajustRotation);
        Instantiate(weaponFlash, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    IEnumerator Reload()                      
    {
        isReloading = true;
        Quaternion targetRotation = Quaternion.Euler(initalRotation.eulerAngles + reloadRotationOffset);
        float halfReloadTime = reloadTime / 2f;
        float t = 0f;

        while (t < halfReloadTime)           
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(initalRotation, targetRotation, t / halfReloadTime);
            yield return null;
        }

        t = 0f;
        while (t < halfReloadTime)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(targetRotation, initalRotation, t / halfReloadTime);
            yield return null;
        }

        currentAmmo = magazineSize;
        UIManager.Instance.ammoText.text = currentAmmo.ToString();
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magazineSize) return;
        StartCoroutine(Reload());
    }
}