using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public TMP_Text countdownText;
    public TMP_Text stopwatchText;
    public GameObject finishLine;
    public GameObject scoreboardPanel;
    public TMP_Text scoreboardText;
    public TMP_Text subScoreboardText;

    private bool raceStarted = false;
    private float raceStartTime;
    private bool raceFinished = false;
    public List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    private int playersCrossedFinishLine = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(StartRaceCountdown());
        UpdateScoreboardUI(); // Initialize scoreboard UI
    }

    private IEnumerator StartRaceCountdown()
    {
        int countdown = 3;

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";

        // Activate controls for all players
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerCar = player.TagObject as GameObject;
            if (playerCar != null)
            {
                VehicleControl vehicleControl = playerCar.GetComponent<VehicleControl>();
                if (vehicleControl != null)
                {
                    vehicleControl.activeControl = true;
                }
            }
        }

        raceStarted = true;
        raceStartTime = Time.time;

        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (raceStarted && !raceFinished)
        {
            float raceTime = Time.time - raceStartTime;
            stopwatchText.text = "Time: " + FormatTime(raceTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}:{2:D3}",
            timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
    }

    [PunRPC]
    private void HandleFinishLineCrossing(string playerName, float finishTime)
    {
        playersCrossedFinishLine++;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateScoreboardData", RpcTarget.AllBuffered, playerName, finishTime);
        }

        GameObject playerCar = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (playerCar != null && PhotonNetwork.LocalPlayer.NickName == playerName)
        {
            VehicleControl vehicleControl = playerCar.GetComponent<VehicleControl>();
            if (vehicleControl != null)
            {
                vehicleControl.activeControl = false;
                stopwatchText.gameObject.SetActive(false);
            }
        }

        if (playersCrossedFinishLine == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            raceFinished = true;
            ShowScoreboard();
        }
    }

    public void OnFinishLineCrossing(string playerName, float finishTime)
    {
        photonView.RPC("HandleFinishLineCrossing", RpcTarget.All, playerName, finishTime - raceStartTime);
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

    [Serializable]
    public class PlayerInfo
    {
        public string playerName;
        public float finishTime;

        public PlayerInfo(string name, float time)
        {
            playerName = name;
            finishTime = time;
        }
    }

    [PunRPC]
    private void UpdateScoreboardData(string playerName, float finishTime)
    {
        playerInfoList.Add(new PlayerInfo(playerName, finishTime));
        UpdateScoreboardUI();
    }

    private void UpdateScoreboardUI()
    {
        scoreboardText.text = "";
        subScoreboardText.text = "";

        foreach (PlayerInfo playerInfo in playerInfoList)
        {
            scoreboardText.text += playerInfo.playerName + " - Rank: " + (playerInfoList.IndexOf(playerInfo) + 1) + " - Time: " + FormatTime(playerInfo.finishTime) + "\n";
            subScoreboardText.text += playerInfo.playerName + " - Rank: " + (playerInfoList.IndexOf(playerInfo) + 1) + " - Time: " + FormatTime(playerInfo.finishTime) + "\n";
        }
    }

    private void ShowScoreboard()
    {
        subScoreboardText.gameObject.SetActive(false);
        scoreboardPanel.SetActive(true);
    }
}
