using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    public string selectedCar;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        selectedCar = RoomManager.Instance.playerSelectedCar;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Player player = PhotonNetwork.LocalPlayer;

        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint(PhotonNetwork.LocalPlayer.ActorNumber);
        GameObject playerCar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", selectedCar), spawnpoint.position, spawnpoint.rotation);

        if (playerCar != null)
        {
            player.TagObject = playerCar; // Set the player's TagObject to the car GameObject
        }
    }
}
