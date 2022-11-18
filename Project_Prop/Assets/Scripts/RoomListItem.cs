using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] TMP_Text curPlayerText;
	[SerializeField] TMP_Text maxPlayerText;

	public RoomInfo info;

	public void SetUp(RoomInfo _info)
	{
		info = _info;
		roomNameText.text = _info.Name;
		curPlayerText.text = _info.PlayerCount.ToString();
		maxPlayerText.text = _info.MaxPlayers.ToString();
	}

	public void OnClick()
	{
		Launcher.Instance.JoinRoom(info);
	}
}