using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
 
    public int maxHealth = 100;

    public AudioClip hitSFX;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            DecreaseHealth(10); 
        }
    }

    private void DecreaseHealth(int decreaseAmount)
    {
        maxHealth -= decreaseAmount;
        PlayerLook.Instance.AddShake(0.1f, 0.25f); 
        UIManager.Instance.InstantiateHitUI(); 
        AudioManager.Instance.PlaySFX(hitSFX); 
        UIManager.Instance.SetHealthValue(maxHealth); 

        if (maxHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
       Time.timeScale = 0f; 
       UIManager.Instance.EnableDeathUI(); 
       Cursor.lockState = CursorLockMode.None; 
       Cursor.visible = true; 
    }
}
