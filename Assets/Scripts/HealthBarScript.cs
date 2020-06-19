using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
	private RectTransform healthbar;
	private float xSize;
	
	void Start() {
		healthbar = gameObject.GetComponent<RectTransform>();
		xSize = healthbar.sizeDelta.x;
	}
	
    public void UpdateHealthBar(float percent) {
		float newX = xSize*percent;
		healthbar.sizeDelta = new Vector2(newX, healthbar.sizeDelta.y);
	}
}
