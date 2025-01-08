using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Bullet : MonoBehaviour
{
    PhotonView PV;
    Rigidbody rb;
    public float timer = 1f;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        
    }
    public float bulletSpeed = 10f;
    public int bulletDamage = 1;
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    public void SetVelocity(Vector3 dir)
    {
        PV.RPC("RPC_SetVelocity", RpcTarget.All,dir);
    }

    [PunRPC]
    void RPC_SetVelocity(Vector3 dir)
    {
        rb.linearVelocity = dir * bulletSpeed;
    }

    void OnCollisionEnter(Collision col)
    {
        if (PV.IsMine)
        {
            var enemyHealthController = col.gameObject.GetComponent<EnemyHealthController>();

            if (enemyHealthController != null)
            {
                if (PV.Owner == enemyHealthController.photonView.Owner)
                {
                    enemyHealthController.EnemyTakeDamage(bulletDamage);
                    PhotonNetwork.Destroy(gameObject);
                }
                else
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                // EnemyHealthController yoksa sadece mermiyi yok et
                PhotonNetwork.Destroy(gameObject);
            }
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
