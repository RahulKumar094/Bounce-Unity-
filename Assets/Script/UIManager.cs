using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Slider focusSlider;
	public Slider focusSliderParent;
	private static UIManager instance;
	public static UIManager getInstance
	{
		get { return instance; }
	}

	void Awake()
    {
		if (instance != null)
			Destroy(this.gameObject);
		else
			instance = this;

		UpdateMaxValue();
	}

    // Update is called once per frame
    void LateUpdate()
    {
		float focusTime = focusSliderParent.maxValue - Bounce.Instance.getFocusTime;
		focusSlider.value = focusTime;
		focusSliderParent.value = Mathf.FloorToInt(focusTime);
	}

	public void UpdateMaxValue()
	{
		focusSlider.maxValue = Bounce.Instance.maxFocusTime;
		focusSliderParent.maxValue = Bounce.Instance.maxFocusTime;
	}
}
