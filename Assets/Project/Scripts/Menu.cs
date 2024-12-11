using UnityEngine;

public class Menu : MonoBehaviour
{
    public MenuType menuType;
    public bool open = false;

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }
}

public enum MenuType
{
    MainMenu,
    CreateGameMenu,
    RoomMenu,
    LoadingMenu,
    LobbiesMenu,
}
