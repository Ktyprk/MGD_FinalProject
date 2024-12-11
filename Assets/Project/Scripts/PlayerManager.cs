using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    public int actorNumber;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        actorNumber = PV.Owner.ActorNumber;
    }

    void Start()
    {
        if (PV.IsMine)
        {
            Debug.Log("Actor Number: " + actorNumber);
            CreateController();
        }
    }
    

    void Update()
    {
        if (PV.Owner.IsMasterClient)
        {
            // if (OnePlayerStanding())
            // {
            //     RespawnAllPlayers();
            // }
        }
    }

    void RespawnAllPlayers()
    {
        // PlayerController[] players = FindObjectsOfType<PlayerController>();
        // foreach (PlayerController p in players)
        // {
        //     p.Respawn(p.myRespawnPoint.position);
        // }
    }

    public void CreateController()
    {
        if (!PV.IsMine)
            return;
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
        int spawnIndex = PV.Owner.ActorNumber - 1;
        Transform respawnPoint = respawnManager.respawnPoints[spawnIndex];

        
        GameObject controller = PhotonNetwork.Instantiate(
            Path.Combine("PhotonPrefabs", "Player"),
            respawnPoint.position,
            respawnPoint.rotation 
        );
        
    }
    
    





}
