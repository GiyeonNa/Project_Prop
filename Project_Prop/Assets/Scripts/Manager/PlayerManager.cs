using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
	protected PhotonView PV;
	protected GameObject controller;

	protected int kills;
	protected int deaths;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if(PV.IsMine)
		{
			CreateController();
		}
	}

    public virtual void CreateController()
	{
		Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		//controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HidePlayerController"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });
	}

	public void CreateObserver()
    {
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "ObserverPlayerController"), 
											   Vector3.zero, Quaternion.identity, 0, new object[] { PV.ViewID });
	}

	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		//재생성이 아닌 관전자로 생성해야함
		CreateObserver();
		//다시 재생성
		//CreateController();

		//deaths++;
		//Hashtable hash = new Hashtable();
		//hash.Add("deaths", deaths);
		//PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
	}

    //public void GetKill()
    //{
    //    PV.RPC(nameof(RPC_GetKill), PV.Owner);
    //}

    //[PunRPC]
    //void RPC_GetKill()
    //{
    //    kills++;

    //    Hashtable hash = new Hashtable();
    //    hash.Add("kills", kills);
    //    PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    //}

    public static PlayerManager Find(Player player)
	{
		return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => (bool)(x.PV.Owner == player));
	}
}