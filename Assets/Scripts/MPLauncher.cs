using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class MPLauncher : MonoBehaviourPunCallbacks
{
    public static MPLauncher Instance;

    public TextMeshProUGUI connectionStatus;
    public string roomName;

    [SerializeField] TextMeshProUGUI errorMessage;
    [SerializeField] TextMeshProUGUI roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    private bool isGameStarted = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        connectionStatus.SetText("Connecting to Multiplayer");
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        connectionStatus.SetText("Multiplayer connected");
        Debug.Log("Joined Lobby");
    }

    public void SetPlayerName(string playerName)
    {
        PhotonNetwork.NickName = playerName;
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("joinedRoomScreen");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name + " - Player's List";

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            // Room is full, hide it from the room list
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorMessage.text = "Room Creation Failed:" + message;
        MenuManager.Instance.OpenMenu("errorScreen");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("loadingScreen");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loadingScreen");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("lobbyScreen");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        foreach(RoomInfo room in roomList)
        {
            if(!room.RemovedFromList)
            {
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            // Room is full, hide it from the room list
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(1);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // If the game hasn't started yet, make the room visible and open again when a player leaves.
        if (!isGameStarted)
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }
}
