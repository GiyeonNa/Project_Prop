using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	public static MenuManager Instance;

	[SerializeField] Menu[] menus;
	Dictionary<string, Menu> menuDic = new Dictionary<string, Menu>();
	[SerializeField] Menu preMenu;
	void Awake()
	{
		Instance = this;
        foreach (var menu in menus)
        {
            menuDic[menu.name] = menu;
        }
    }

	public void OpenMenu(string menuName)
	{
		if (preMenu.open) CloseMenu(preMenu);
		preMenu = menuDic[menuName];
		preMenu.Open();
	}

	//Button On Click
    public void OpenMenu(Menu menu)
    {
		if (preMenu.open) CloseMenu(preMenu);
		preMenu = menuDic[menu.name];
		preMenu.Open();
    }

    public void CloseMenu(Menu menu)
	{
		menu.Close();
	}
}