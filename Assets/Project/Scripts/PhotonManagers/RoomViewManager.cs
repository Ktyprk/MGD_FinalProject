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
    public GameObject playerNameListUI; 
    public TMPro.TextMeshProUGUI playerNameTextPrefab; 
    public TMP_Text[] playerNamesTexts;

    private List<string> playerNames = new List<string>(); 

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
            if (spawnIndex >= 0 && spawnIndex < playerPrefabs.Length)
            {
                if (!playerPrefabs[spawnIndex].activeSelf)
                {
                    playerPrefabs[spawnIndex].SetActive(true);
                }
            }
            string nickname = (string)player.CustomProperties["NickName"];
            int playerIndex = player.ActorNumber - 1;
            AddPlayerName(playerIndex, nickname);
        }
    }

    private void AddPlayerName(int spawnIndex, string nickname)
    {
        if (spawnIndex < playerNamesTexts.Length)
        {
            playerNamesTexts[spawnIndex].text = nickname; 
        }
        
        if (!playerNames.Contains(nickname))
        {
            playerNames.Add(nickname);
        }

        UpdatePlayerNameUI();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("NickName"))
        {
            string updatedNickname = (string)changedProps["NickName"];
            int playerIndex = targetPlayer.ActorNumber - 1; 
            AddPlayerName(playerIndex, updatedNickname); 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} odaya katıldı.");
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            int spawnIndex = player.ActorNumber - 1;
            if (spawnIndex >= 0 && spawnIndex < playerPrefabs.Length)
            {
                if (!playerPrefabs[spawnIndex].activeSelf)
                {
                    playerPrefabs[spawnIndex].SetActive(true);
                }
            }

            string nickname = (string)player.CustomProperties["NickName"];
            int playerIndex = player.ActorNumber - 1;
            AddPlayerName(playerIndex, nickname);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} odadan ayrıldı.");
        playerNames.Remove(otherPlayer.NickName);
        UpdatePlayerNameUI();
    }

    private void UpdatePlayerNameUI()
    {
        if (playerNameListUI != null)
        {
            foreach (Transform child in playerNameListUI.transform)
            {
                Destroy(child.gameObject);
            }
            
            foreach (string name in playerNames)
            {
                TMPro.TextMeshProUGUI newText = Instantiate(playerNameTextPrefab, playerNameListUI.transform);
                newText.text = name;
            }
        }
    }
}
