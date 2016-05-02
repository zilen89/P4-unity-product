using UnityEngine;
using System.Collections;

public class CFKnob : Singleton<CFKnob>
{
    private float startYLocation;
    public float max_Value = Movement.FreqMAX;
    public float min_value = Movement.FreqMIN;
    public float z_degree;
    private float oldValue;
    private float currentMouseYPosition;
    private float y_pos;
    public static float CF_value;

    // Use this for initialization
    void Start () {
        CF_value = 90;
    }
	
	// Update is called once per frame
    void Update()
    {

	}

    void OnMouseDown() {
        //screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position); // I removed this line to prevent centring 
        startYLocation = Input.mousePosition.y - oldValue;
        Cursor.visible = false;
    }

    public void ResetWheel()
    {
        z_degree = 0;
        this.transform.localRotation = Quaternion.identity;
    }

    void OnMouseDrag() {
        currentMouseYPosition = Input.mousePosition.y - startYLocation;
        y_pos = currentMouseYPosition;
        z_degree = currentMouseYPosition - oldValue;
        if (y_pos > oldValue && CF_value < max_Value) {
            CF_value += 10;
        }
        if (y_pos < oldValue && CF_value > min_value) {
            CF_value -= 10;
        }
        this.transform.Rotate(0, 0, -z_degree);
        oldValue = currentMouseYPosition;
    }

    void OnMouseUp() {
        Cursor.visible = true;
    }
}


