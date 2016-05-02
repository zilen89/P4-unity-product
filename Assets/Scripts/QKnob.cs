using UnityEngine;
using System.Collections;

public class QKnob : Singleton<QKnob>
{
    private float startYLocation;
    public float max_Value = Movement.Instance.qMAX;
    public float min_value = Movement.Instance.qMIN;
    public float z_degree;
    private float oldValue;
    private float currentMouseYPosition;
    private float y_pos;
    public static float Q_Value;

    // Use this for initialization
    void Start () {
        Q_Value = 3;
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
        print("Resetting");
        this.transform.localRotation = Quaternion.identity;
    }

    void OnMouseDrag() {
        currentMouseYPosition = Input.mousePosition.y - startYLocation;
        y_pos = currentMouseYPosition;
        z_degree = currentMouseYPosition - oldValue;
        if (y_pos > oldValue && Q_Value < max_Value && Player.Instance.isQActive) {
            Q_Value += 0.1f;
        }
        if (y_pos < oldValue && Q_Value > min_value && Player.Instance.isQActive) {
            Q_Value -= 0.1f;
        }
        this.transform.Rotate(0, 0, -z_degree);
        oldValue = currentMouseYPosition;
    }

    void OnMouseUp() {
        Cursor.visible = true;
    }
}


