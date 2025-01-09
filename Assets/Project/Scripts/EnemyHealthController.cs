using UnityEngine;
using Photon.Pun;

public class EnemyHealthController : MonoBehaviour
{
    public PhotonView photonView;
    public int Health = 10;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    public void EnemyTakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);
        
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            DestroyEnemy();
        }
        Debug.Log("Taken " + damage + " damage. Remaining Health: " + Health);
    }

    void DestroyEnemy()
    {
        if (photonView.IsMine) 
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}