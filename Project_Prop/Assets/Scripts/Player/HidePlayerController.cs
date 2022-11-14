using Photon.Pun;
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
	CapsuleCollider selfCapsuleCollider;
	[SerializeField] MeshFilter changeMeshFilter;
	[SerializeField] MeshRenderer changeMeshRenderer;
	[SerializeField] MeshCollider changeMeshCollider;

	private void Awake()
    {
		base.Awake();
		selfMeshFilter = GetComponent<MeshFilter>();
		selfMeshRenderer= GetComponent<MeshRenderer>();
		selfCapsuleCollider = GetComponent<CapsuleCollider>();

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
		//관점 캠 만들기
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
                //PhotonView 말고 다른 조건도 생각 해볼것
                //if (tempHit.layer == 6)
                //{
                //    PV.RPC("RPC_PropChangeModel", RpcTarget.All, tempHit.gameObject.layer);
                //}
                PV.RPC("RPC_PropChangeModel", RpcTarget.All, tempHit.GetPhotonView().ViewID);
			}	
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

		//예외처리 수정
		//가져올것
		//MeshFilter : 겉모습
		//MeshCollider : 충돌 범위
		//MeshRenderer : 색상, 여러개면 다 가져와야함
		if (!changeMeshRenderer.enabled) changeMeshRenderer.enabled = true;
		if(!changeMeshCollider.enabled) changeMeshCollider.enabled = true;
		selfMeshRenderer.enabled = false;
		selfCapsuleCollider.enabled = false;
		changeMeshFilter.mesh = targetPV.gameObject.GetComponent<MeshFilter>()?.mesh;
		changeMeshCollider.sharedMesh = targetPV.gameObject.GetComponent<MeshCollider>()?.sharedMesh;
		changeMeshRenderer.materials = targetPV.gameObject.GetComponent<MeshRenderer>()?.materials;
	}
}
