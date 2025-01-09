using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TargetHealthController : MonoBehaviourPun
{
    public int maxHealth = 1000;
    private int currentHealth;

    public Image healthFillImage; 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    [PunRPC]
    public void TargetTakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Destroy(gameObject);
        }
        UpdateHealthBar();
        Debug.Log("Taken " + damage + " damage. Remaining Health: " + currentHealth);
    }

    private void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("TargetTakeDamage", RpcTarget.AllBuffered, damage);
        }
        else
        {
            TargetTakeDamage(damage);
        }
    }
}