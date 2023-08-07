using UnityEngine;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerCar") && PhotonNetwork.IsConnected)
        {
            GameManager.Instance.OnFinishLineCrossing(PhotonNetwork.LocalPlayer.NickName, Time.time);
        }
    }
}
