using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipScript : MonoBehaviour
{
	private static TooltipScript instance;
	private Text tooltipText;
	private RectTransform backgroundRectTransform;
	
    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		backgroundRectTransform = transform.Find("Background").GetComponent<RectTransform>();
		tooltipText = transform.Find("Text").GetComponent<Text>();
		
		HideTooltip_Static();
    }
	
	void Update() {
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
		transform.localPosition = localPoint;
	}

    // Update is called once per frame
    void ShowTooltip(string tooltipString) 
    {
        gameObject.SetActive(true);
		
		tooltipText.text = tooltipString;
		float paddingSize = 4f;
		Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + paddingSize * 2f, tooltipText.preferredHeight + paddingSize * 2f);
		backgroundRectTransform.sizeDelta = backgroundSize;
    }
	
	void HideTooltip() {
		gameObject.SetActive(false);
	}
	
	public static void ShowTooltip_Static(string tooltipString) {
		instance.ShowTooltip(tooltipString);
	}
	
	public static void HideTooltip_Static() {
		instance.HideTooltip();
	}
}
