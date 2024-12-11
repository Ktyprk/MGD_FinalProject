using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    public TMP_Text roomNameText;
    public TMP_InputField roomNameInput;
    public TMP_Dropdown roomPrivacyDropdown;
    public GameObject readyGameButton;
    public GameObject startGameButton;

    public Transform roomListContent;
    public GameObject roomListItem;
    public GameObject gameIsStarted;

    bool connectFirstTime = true;
    private bool isReady = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to servers...");
        MenuManager.Instance.OpenMenu(MenuType.LoadingMenu);
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        Debug.Log("Joining lobby...");
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu(MenuType.MainMenu);
        if (connectFirstTime)
        {
            PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");

            connectFirstTime = false;
        }
        Debug.Log("Joined lobby!");
    }

    public void CreateRoom()
    {
        bool isRoomPrivate = roomPrivacyDropdown.value == 1;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = !isRoomPrivate;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 4;
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;

            Instantiate(roomListItem, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu(MenuType.RoomMenu);
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true); // Master Client için başlangıç butonu
            readyGameButton.SetActive(false); // Hazır butonu devre dışı
        }
        else
        {
            startGameButton.SetActive(false); // Diğer oyuncular için başlangıç butonu devre dışı
            readyGameButton.SetActive(true);  // Hazır butonu aktif
        }
    }

    
    public void JoinRoom(RoomInfo info)
    {
        MenuManager.Instance.OpenMenu(MenuType.LoadingMenu);
        PhotonNetwork.JoinRoom(info.Name);
    }

    public void LeaveRoom()
    {
        MenuManager.Instance.OpenMenu(MenuType.LoadingMenu);
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu(MenuType.MainMenu);
    }

    
    public void StartGame()
    {

        if (AreAllPlayersReady())
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("Starting game...");
        }
        else
        {
            Debug.Log("Tüm oyuncular hazır değil!");
        }
    }
    
    public void SetReadyStatus(bool ready)
    {
        isReady = ready;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", ready } });
    }
    
    private bool AreAllPlayersReady()
    {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                // Eğer kontrol edilen oyuncu Master Client ise atla
                if (player.IsMasterClient)
                    continue;

                if (player.CustomProperties.TryGetValue("IsReady", out object isReady))
                {
                    if (!(bool)isReady)
                        return false; // Diğer oyunculardan biri hazır değilse false döndür
                }
                else
                {
                    return false; // Eğer "IsReady" property eksikse false döndür
                }
            }
            return true; // Tüm oyuncular hazır

    }
    
    public void UpdateStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(AreAllPlayersReady());
        }
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("IsReady"))
        {
            UpdateStartButton(); 
        }
    }
    
    public void OnReadyButtonPressed()
    {
        isReady = !isReady;
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "IsReady", isReady } });
        
        readyGameButton.GetComponentInChildren<TMP_Text>().text = isReady ? "Hazır" : "Hazır Değil";
    }






}
