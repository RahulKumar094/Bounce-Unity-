using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	public ParticleSystem wallCollideParticles;
	public PrimitiveType shapeType;
	public float shapeSize = 0.2f;
	public Vector3 shapeCount = new Vector3(5,5,2);

	[Space]
	public float explosionForce = 200f;
	public float explosionRadius = 4f;
	public float explosionUpward = 0.4f;

	private float cubesPivotDistance;
	private Vector3 particlesPivot;
	private Vector3 position;

	private static ParticleManager instance;
	public static ParticleManager getInstance
	{
		get { return instance; }
	}

	void Awake()
    {
		if (instance != null)
			Destroy(this.gameObject);
		else
			instance = this;
		
		cubesPivotDistance = shapeSize * shapeCount.x / 2;
		particlesPivot = new Vector3(cubesPivotDistance, cubesPivotDistance, cubesPivotDistance);
		
		wallCollideParticles.gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
		if (wallCollideParticles.isStopped && wallCollideParticles.gameObject.activeSelf)
			wallCollideParticles.gameObject.SetActive(false);
	}

	public void createExplosion(Vector3 position, PrimitiveType particleType, Color matColor)
	{
		explode(position, particleType, matColor);
	}

	private void explode(Vector3 position, PrimitiveType shapeType, Color matColor)
	{
		this.shapeType = shapeType;
		this.position = position;

		//particle 5X5X2
		for (int x = 0; x < shapeCount.x; x++)
		{
			for (int y = 0; y < shapeCount.y; y++)
			{
				for (int z = 0; z < shapeCount.z; z++)
				{
					createPiece(x, y, z, matColor);
				}
			}
		}

		Vector3 explosionPos = position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);

		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.velocity = Vector3.zero;
				rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, explosionUpward, ForceMode.Acceleration);
			}
		}
	}

	private void createPiece(int x, int y, int z, Color color)
	{
		GameObject piece;
		piece = ParticleSpawnPool.getInstance.getParticleInstance(shapeType);
		piece.GetComponent<Renderer>().material.color = color;
		piece.tag = shapeType.ToString();

		piece.transform.position = position + new Vector3(shapeSize * x, shapeSize * y, shapeSize * z) - particlesPivot;
		piece.transform.localScale = new Vector3(shapeSize, shapeSize, shapeSize);

		AutoDestruct ad = piece.GetComponent<AutoDestruct>();
		if (ad == null)
			ad = piece.AddComponent<AutoDestruct>();

		Rigidbody rb = piece.GetComponent<Rigidbody>();
		if (rb == null)
			rb = piece.AddComponent<Rigidbody>();

		rb.mass = shapeSize;

		float f = Random.Range(1.5f, 3f);
		ad.destruct(f);
	}

	public void ParticlesOnCollision()
	{
		if (wallCollideParticles.isPlaying)
			wallCollideParticles.Stop(true);

		Vector3 pos = Bounce.Instance.getPosition;
		wallCollideParticles.transform.position =  new Vector3(pos.x, pos.y - Bounce.Instance.transform.localScale.y / 2, -2);
		wallCollideParticles.gameObject.SetActive(true);
	}

}
