using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Menu[] menus;
    public static MenuManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(MenuType menuToOpen)
    {
        foreach (var menu in menus)
        {
            if (menu.menuType == menuToOpen)
            {
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        }
    }

    public void OpenMenu(Menu menuToOpen)
    {
        foreach (var menu in menus)
        {
            menu.Close();
        }
        menuToOpen.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
