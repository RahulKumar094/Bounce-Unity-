using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	// Start is called before the first frame update
	public bool move;
	public bool rotate;
	public float rotateSpeed;
	public bool moveHorizontal;
	public float maxCoverDist;
	public float speed;

	private Vector3 spawnPosition;
	private float timer = 0;

    void Start()
    {
		spawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
		timer += Time.deltaTime;

		if (move)
		{
			float pos = Mathf.Sin(timer * speed) * maxCoverDist;
			if (moveHorizontal)
				transform.position = new Vector3(pos + spawnPosition.x, transform.position.y, transform.position.z);
			else
				transform.position = new Vector3(transform.position.x, pos + spawnPosition.y, transform.position.z);
		}

		if (rotate)
			transform.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);
	}

	public void kill()
	{
		transform.GetComponent<MeshCollider>().enabled = false;
		move = false;

		ParticleManager.getInstance.createExplosion(transform.position, PrimitiveType.Cube, GetComponent<Renderer>().material.color);
		Destroy(this.gameObject);
	}
}
