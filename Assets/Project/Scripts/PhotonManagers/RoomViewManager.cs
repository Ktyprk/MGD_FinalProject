using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RoomViewManager : MonoBehaviourPunCallbacks
{
    [Header("Player Settings")]
    public GameObject[] playerPrefabs;

    [Header("UI Settings")]
    public TMP_Text[] playerNamesTexts;
    public TMP_Text[] readyTexts;

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int spawnIndex = player.ActorNumber - 1;

            // Oyuncu prefab'ını aktif etme
            if (spawnIndex >= 0 && spawnIndex < playerPrefabs.Length)
            {
                if (!playerPrefabs[spawnIndex].activeSelf)
                {
                    playerPrefabs[spawnIndex].SetActive(true);
                }
            }

            // Oyuncu ismini ve hazır durumunu güncelle
            string nickname = (string)player.CustomProperties["NickName"];
            bool isReady = player.CustomProperties.ContainsKey("IsReady") ? (bool)player.CustomProperties["IsReady"] : false;

            UpdatePlayerName(spawnIndex, nickname);
            UpdateReadyStatus(spawnIndex, isReady);
        }
    }

    private void UpdatePlayerName(int playerIndex, string nickname)
    {
        if (playerIndex >= 0 && playerIndex < playerNamesTexts.Length)
        {
            playerNamesTexts[playerIndex].text = nickname;
        }
    }

    private void UpdateReadyStatus(int playerIndex, bool isReady)
    {
        if (playerIndex >= 0 && playerIndex < readyTexts.Length)
        {
            readyTexts[playerIndex].text = isReady ? "Hazır" : "Hazır Değil";
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        int playerIndex = targetPlayer.ActorNumber - 1;

        // Oyuncunun adı güncellendi mi?
        if (changedProps.ContainsKey("NickName"))
        {
            string updatedNickname = (string)targetPlayer.CustomProperties["NickName"];
            UpdatePlayerName(playerIndex, updatedNickname);
        }

        // Hazır durumu güncellendi mi?
        if (changedProps.ContainsKey("IsReady"))
        {
            bool isReady = (bool)changedProps["IsReady"];
            UpdateReadyStatus(playerIndex, isReady);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} odaya katıldı.");
        SpawnPlayer();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} odadan ayrıldı.");
        int playerIndex = otherPlayer.ActorNumber - 1;

        // Oyuncu odadan ayrıldığında UI'ı temizle
        if (playerIndex >= 0 && playerIndex < playerNamesTexts.Length)
        {
            playerNamesTexts[playerIndex].text = "";
        }
        if (playerIndex >= 0 && playerIndex < readyTexts.Length)
        {
            readyTexts[playerIndex].text = "";
        }
    }
}
