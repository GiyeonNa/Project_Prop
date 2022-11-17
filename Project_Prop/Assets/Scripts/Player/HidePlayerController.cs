using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Cinemachine;

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
			GetComponentInChildren<Camera>().gameObject.SetActive(false);
			GetComponentInChildren<Cinemachine.CinemachineFreeLook>().gameObject.SetActive(false);
			Destroy(rb);
			Destroy(ui);
		}
	}

	void Update()
	{
		if (!PV.IsMine)
			return;

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

	private void Move()
	{
		Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
		if (moveInput.sqrMagnitude > 1f)
		{
			moveInput = moveInput.normalized;
		}

		Vector3 forwardVec = new Vector3(selfCam.transform.forward.x, 0f, selfCam.transform.forward.z).normalized;
		Vector3 rightVec = new Vector3(selfCam.transform.right.x, 0f, selfCam.transform.right.z).normalized;

		Vector3 moveVec = moveInput.x * rightVec + moveInput.z * forwardVec;


		transform.position += (moveVec * walkSpeed * Time.deltaTime);
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
                //if (!hit.transform.gameObject.GetComponent<PhotonView>()) return;

                //PhotonView 말고 다른 조건도 생각 해볼것
                //if (tempHit.layer == 6)
                //{
                //    PV.RPC("RPC_PropChangeModel", RpcTarget.All, tempHit.gameObject.layer);
                //}

                GameObject tempHit = hit.collider.gameObject;
                PV.RPC("RPC_PropChangeModel", RpcTarget.All, tempHit.GetPhotonView().ViewID);
            }	
		}		
	}

	[PunRPC]
	void RPC_PropChangeModel(int targetPropID)
	{
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
