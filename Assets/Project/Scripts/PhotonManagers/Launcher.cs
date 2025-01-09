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

    bool connectFirstTime = true;
    private bool isReady = false;
    
    public TMP_InputField NicknameInputField;
    public GameObject NicknameSetButton;

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
            startGameButton.SetActive(true); 
            readyGameButton.SetActive(false); 
        }
        else
        {
            startGameButton.SetActive(false);
            readyGameButton.SetActive(true); 
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
                if (player.IsMasterClient)
                    continue;

                if (player.CustomProperties.TryGetValue("IsReady", out object isReady))
                {
                    if (!(bool)isReady)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;

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
    
    public void SetNickname()
    {
        string nickname = NicknameInputField.text;

        if (!string.IsNullOrEmpty(nickname))
        {
            // Photon'da NickName'i ayarla
            PhotonNetwork.NickName = nickname;

            // Photon Custom Properties ile adını diğer oyuncularla senkronize et
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "NickName", nickname }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            // PlayerPrefs'te kaydet
            PlayerPrefs.SetString("Nickname", nickname);

            Debug.Log($"Nickname set to: {nickname}");
        }
        else
        {
            Debug.LogWarning("Nickname cannot be empty!");
        }
    }







}
