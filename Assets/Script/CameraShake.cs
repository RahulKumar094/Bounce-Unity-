using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public float duration;
	public float amplitude;

	private float elapsedTime;
	private bool startShake;
	private CinemachineBrain brain;
	private Vector3 pos;
	private Vector3 lastPos;

	private static CameraShake instance;
	public static CameraShake getInstance
	{
		get { return instance; }
	}

	private void Awake()
	{
		if (instance != null)
			Destroy(this.gameObject);
		else
			instance = this;
	}

	private void Start()
	{
		brain = GetComponent<CinemachineBrain>();
	}

	private void Update()
	{
		if (startShake)
		{
			elapsedTime += Time.deltaTime;

			if (elapsedTime < duration)
			{
				pos = Vector3.zero;
				pos.x = Random.Range(-1, 1) * amplitude;
				pos.y = Random.Range(-1, 1) * amplitude;
				pos.z = 0;

				pos = lastPos + pos;
				transform.position = Vector3.Lerp(transform.position, pos, 2f);
			}
			else
				resetValues();
		}
	}

	public void shake()
	{
		startShake = true;
		lastPos = transform.position;
		brain.enabled = false;
	}

	public void setValues(float shakeDuration, float shakeAmplitude)
	{
		duration = shakeDuration;
		amplitude = shakeAmplitude;
	}

	private void resetValues()
	{
		elapsedTime = 0;
		startShake = false;
		transform.position = lastPos;
		brain.enabled = true;
	}
}
