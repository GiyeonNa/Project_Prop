using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{
	public static RoomManager Instance;
    private int seekPlayer;

    void Awake()
	{
		if(Instance)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		Instance = this;
	}

	public override void OnEnable()
	{
		base.OnEnable();
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		if(scene.buildIndex == 1 || scene.buildIndex == 2) // We're in the game scene
		{
            seekPlayer = Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
			if(PhotonNetwork.LocalPlayer == PhotonNetwork.PlayerList[seekPlayer])
            {
				PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SeekPlayerManager"), Vector3.zero, Quaternion.identity);
			}
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HidePlayerManager"), Vector3.zero, Quaternion.identity);
		}
	}
}