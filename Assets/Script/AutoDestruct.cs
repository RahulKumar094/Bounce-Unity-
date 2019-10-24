using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
	private float timeToDestroy;
	private float timer;
	private bool destroy = false;
	private bool disableOnly = false;
	
    void Update()
    {
		if (destroy)
		{
			timer += Time.deltaTime;
			if (timer >= timeToDestroy)
			{
				if (disableOnly)
					this.gameObject.SetActive(false);
				else
					Destroy(this.gameObject);

				Reset();
			}
		}
    }

	public void destruct(float timeToDestroy, bool disableOnly = true)
	{
		this.timeToDestroy = timeToDestroy;
		this.disableOnly = disableOnly;

		destroy = true;
	}

	private void Reset()
	{
		timer = 0;
		destroy = false;
		transform.rotation = Quaternion.Euler(Vector3.zero);
	}

}
