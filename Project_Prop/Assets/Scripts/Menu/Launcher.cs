using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    #region 변수
    public static Launcher Instance;
	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;
    #endregion
    void Awake()
	{
		Instance = this;
	}

	void Start()
	{
#if UNITY_EDITOR
		Debug.Log("Connecting to Master");
#endif
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
#if UNITY_EDITOR
		Debug.Log("Connected to Master");
#endif
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}
	public override void OnJoinedLobby()
	{
		MenuManager.Instance.OpenMenu("TitleMenu");
#if UNITY_EDITOR
		Debug.Log("Joined Lobby");
#endif
	}
	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("RoomMenu");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;
		Player[] players = PhotonNetwork.PlayerList;

		foreach (Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
#if UNITY_EDITOR
		Debug.LogError("Room Creation Failed: " + message);
#endif
		MenuManager.Instance.OpenMenu("ErrorMenu");
	}
	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("TitleMenu");
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
			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}

	public void StartGame()
	{
		//1 TestMap 2 GameMap
		//PhotonNetwork.LoadLevel(1);
		PhotonNetwork.LoadLevel(2);
	}
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("LoadingMenu");
	}
	public void JoinRoom(RoomInfo info)
	{
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("LoadingMenu");
	}
	public void CreateRoom()
	{
		if (string.IsNullOrEmpty(roomNameInputField.text)) return;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 8; // 인원 지정.
		PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
		MenuManager.Instance.OpenMenu("LoadingMenu");
	}
}

