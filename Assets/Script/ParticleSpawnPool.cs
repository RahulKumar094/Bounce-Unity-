using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnPool : MonoBehaviour
{
	public Transform container;

	private List<GameObject> particles;
	private static ParticleSpawnPool instance;
	public static ParticleSpawnPool getInstance
	{
		get { return instance; }
	}

	void Awake()
    {
		if (instance != null)
			Destroy(this.gameObject);
		else
			instance = this;
    }

	private void Start()
	{
		particles = new List<GameObject>();
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public GameObject getParticleInstance(PrimitiveType type)
	{
		GameObject inst = particles.Find(x => !x.activeSelf && x.tag == type.ToString());
		if (inst != null)
		{
			inst.SetActive(true);
			return inst;
		}
		else
		{
			inst = GameObject.CreatePrimitive(type);
			inst.transform.parent = container;
			particles.Add(inst);
			return inst;
		}
	}

}
