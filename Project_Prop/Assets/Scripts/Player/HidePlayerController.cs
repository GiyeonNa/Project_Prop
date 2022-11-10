﻿using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class HidePlayerController : PlayerController
{
    [SerializeField] Transform cam;
	[SerializeField] Camera selfCam;
	[SerializeField] LayerMask layerMask;

	MeshFilter selfMeshFilter;
	MeshRenderer selfMeshRenderer;
	[SerializeField] MeshFilter changeMeshFilter;
	[SerializeField] MeshRenderer changetMeshRenderer;


	private void Awake()
    {
		base.Awake();
		selfMeshFilter = GetComponent<MeshFilter>();
		selfMeshRenderer= GetComponent<MeshRenderer>();

	}

    void Start()
	{
        if (!PV.IsMine)
        {
			//Destroy(GetComponentInChildren<Camera>().gameObject);
			GetComponentInChildren<Camera>().gameObject.SetActive(false);
			Destroy(rb);
			Destroy(ui);
		}
	}

	void Update()
	{
		if (!PV.IsMine)
			return;

		LookAround();
		Move();
		Jump();
		Copy();
		Rotate();

		if (transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

	private void Rotate()
    {
		if (Input.GetKey(KeyCode.Q)) gameObject.transform.Rotate(Vector3.up * 2.5f);
		if (Input.GetKey(KeyCode.E)) gameObject.transform.Rotate(Vector3.up * -2.5f);
	}

	private void LookAround()
	{
		Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
		Vector3 camAngle = cam.rotation.eulerAngles;

		//추가
		float x = camAngle.x - mouseDelta.y;
		if (x < 180f)
		{
			x = Mathf.Clamp(x, -1f, 70f);
		}
		else
		{
			x = Mathf.Clamp(x, 335f, 361f);
		}

		cam.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
	}

	private void Move()
	{
		Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		bool isMove = moveInput.magnitude != 0;
		if (isMove)
		{
			Vector3 lookForward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
			Vector3 lookRight = new Vector3(cam.right.x, 0f, cam.right.z).normalized;
			Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

			transform.position += moveDir * Time.deltaTime * 5f;
		}
	}


	void Jump()
	{
		if (Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			rb.AddForce(transform.up * jumpForce);
		}
	}

	void Die()
	{
		playerManager.Die();
	}

	void Copy()
    {

		if (Input.GetKeyUp(KeyCode.F))
		{
			Debug.Log("Press F");
			Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
			Ray ray = selfCam.ScreenPointToRay(screenCenterPoint);

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
			{
				if (!hit.transform.gameObject.GetComponent<PhotonView>()) return;
				GameObject tempHit = hit.collider.gameObject;
				Debug.Log("Ray Hit " + hit.transform.gameObject.name);
				Debug.Log(tempHit.GetPhotonView());
				PV.RPC("RPC_PropChangeModel", RpcTarget.All, tempHit.GetPhotonView().ViewID);
			}	
		}		
	}

	[PunRPC]
	void RPC_Change()
	{
		Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
		Ray ray = selfCam.ScreenPointToRay(screenCenterPoint);

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
		{
			//Raycasthit은 타입으로 지원하지않음
			//Vector3 -> Transform
			Debug.Log(hit.transform.gameObject.name);
			GameObject obj = hit.collider.gameObject;
			Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
			selfMeshFilter.mesh = mesh;

		}
	}

	[PunRPC]
	void RPC_PropChangeModel(int targetPropID)
	{
		//if (!PV.IsMine)
		//	return;

		PhotonView targetPV = PhotonView.Find(targetPropID);

		if (targetPV.gameObject == null)
			return;


		//gameObject.GetComponent<MeshFilter>().mesh = targetPV.gameObject.GetComponent<MeshFilter>().mesh;
		//gameObject.GetComponent<MeshRenderer>().material = targetPV.gameObject.GetComponent<MeshRenderer>().material;

		if (!changetMeshRenderer.enabled) changetMeshRenderer.enabled = true;
		selfMeshRenderer.enabled = false;
		changeMeshFilter.mesh = targetPV.gameObject.GetComponent<MeshFilter>().mesh;
		changetMeshRenderer.material = targetPV.gameObject.GetComponent<MeshRenderer>().material;
	}
}
