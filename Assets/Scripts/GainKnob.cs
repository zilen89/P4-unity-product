using UnityEngine;
using System.Collections;

public class GainKnob : Singleton<GainKnob>
{
    private float startYLocation;
    public float max_Value = Movement.Instance.gainMAX;
    public float min_value = Movement.Instance.gainMIN;
    public float z_degree;
    private float oldValue;
    private float currentMouseYPosition;
    private float y_pos;
    public static float gain_value;

    // Use this for initialization
    void Start () {
        gain_value = Movement.Instance.gainMAX;
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
        //this.transform.Rotation = Quaternion.Euler(Vector3(0, 0, 0);
    }

    void OnMouseDrag() {
        currentMouseYPosition = Input.mousePosition.y - startYLocation;
        y_pos = currentMouseYPosition;
        z_degree = currentMouseYPosition - oldValue;
        if (y_pos > oldValue && gain_value < max_Value && Movement.Instance.currentCFHit) {
            gain_value += 10;
        }
        if (y_pos < oldValue && gain_value > min_value && Movement.Instance.currentCFHit) {
            gain_value -= 10;
        }
        this.transform.Rotate(0, 0, -z_degree);
        oldValue = currentMouseYPosition;
    }

    void OnMouseUp() {
        Cursor.visible = true;
    }
}


