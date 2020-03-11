using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTunnel : MonoBehaviour
{
	[SerializeField] private MeshRenderer tunnel_Inside;
	[SerializeField] private float rotSpeed_In, speed_In;
	private float _rotSpeed_In, _speed_In;
	private Material _inSideMat;

	[SerializeField] private MeshRenderer tunnel_Outside;
	[SerializeField] private float rotSpeed_Out, speed_Out;
	private float _rotSpeed_Out, _speed_Out;
	private Material _outSideMat;

	[SerializeField] private Transform moveObject;

	public delegate void OnWarpEndDelegate();
	public OnWarpEndDelegate OnWarpEnd;

	// Start is called before the first frame update
	void Start()
	{
		_inSideMat = tunnel_Inside.material;
		_outSideMat = tunnel_Outside.material;

		_rotSpeed_In = rotSpeed_In;
		_speed_In = speed_In;

		_rotSpeed_Out = rotSpeed_Out;
		_speed_Out = speed_Out;
	}

	// Update is called once per frame
	void Update()
	{
		_rotSpeed_Out = Mathf.Clamp(_rotSpeed_Out += Time.deltaTime * rotSpeed_Out, 1, 10);
		_inSideMat.SetFloat("_Rotation", _rotSpeed_Out);

		_speed_Out = Mathf.Clamp(_speed_Out += (Time.deltaTime * speed_Out), 1, 10);
		_inSideMat.SetFloat("_ZScale", _speed_Out);


		_rotSpeed_In = Mathf.Clamp(_rotSpeed_In += Time.deltaTime * rotSpeed_In, 1, 10);
		_outSideMat.SetFloat("_Rotation", _rotSpeed_In);

		_speed_In = Mathf.Clamp(_speed_In += Time.deltaTime * speed_In, 1, 10);
		_outSideMat.SetFloat("_ZScale", _speed_In);
	}

	public void EndWarp()
	{
		OnWarpEnd.Invoke();
	}
}
