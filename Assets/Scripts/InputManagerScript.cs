using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public Vector3 GetMovementInput() {
		float xMovement = Input.GetAxisRaw("Horizontal");
		float zMovement = Input.GetAxisRaw("Vertical");
		//dont let player move in both axes
		if (xMovement != 0 && zMovement != 0) {
			return Vector3.zero;
		}
		return new Vector3(xMovement, 0, zMovement);
	}
}
