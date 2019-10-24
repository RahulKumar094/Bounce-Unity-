using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
	private float minSwipeDist = 20f;
	private float moveDist;
	public static float horz = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		if (Input.touchCount > 0)
		{
			Touch touch = Input.touches[0];

			if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				moveDist += touch.deltaPosition.x;

				if (Mathf.Abs(moveDist) > minSwipeDist)
				{
					horz += touch.deltaTime * moveDist/Mathf.Abs(moveDist);
					horz = Mathf.Clamp(horz, -1, 1);
				}	
			}
			else if (touch.phase == TouchPhase.Began)
				moveDist = 0;
		}
		else
			horz = 0;
	}
}
