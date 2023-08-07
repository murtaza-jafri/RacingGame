using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public TMP_InputField playerNameInput;
    public string playerCar;
    public string playerName;
    public TMP_InputField roomNameInputField; 

    [SerializeField]
    private List<Menu> menus;

    [SerializeField] MPLauncher mpLauncher;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in menus)
        {
            if (menu.menuName == menuName)
            {
                menu.Open();
            }
            else if (menu.isOpen == true)
            {
                menu.Close();
            }

        }
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void OnPlayerNameEntered()
    {
        playerName = playerNameInput.text;
        if(!string.IsNullOrEmpty(playerName))
        {
            mpLauncher.SetPlayerName(playerName);
            OpenMenu("carSelectionScreen");
        }
    }

    public void OnCarSelected(string selectedCar)
    {
        playerCar = selectedCar;
        RoomManager.Instance.playerSelectedCar = playerCar;
        OpenMenu("lobbyScreen");
    }

    public void OnCreateRoom()
    {
        if(!string.IsNullOrEmpty(roomNameInputField.text))
        {
            mpLauncher.CreateRoom(roomNameInputField.text);
            OpenMenu("loadingScreen");
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // If running in Unity Editor, stop play mode
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If running as a standalone build, quit the application
        Application.Quit();
#endif
    }
}
