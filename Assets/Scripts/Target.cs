using UnityEngine;
using System.Collections;

public class Target : Singleton<Target> {

    public GameObject gainBar;
    public GameObject triangles;
    public Color[] colors;

    // Use this for initialization
    void Start() {
        colors[0] = Color.blue;
        colors[1] = Color.green;
        colors[2] = Color.red;
        colors[3] = Color.yellow;
        colors[4] = Color.white;
    }

    // Update is called once per frame
    void Update() {
        this.GetComponent<Renderer>().material.color = colors[Movement.Instance.selectedBand];
        gainBar.GetComponent<Renderer>().material.color = colors[Movement.Instance.selectedBand];
    }

    public void UpdatePosition(float x) {
        this.transform.position = new Vector3(x, this.transform.position.y, 0);
    }

    public void DisplayGain(float x, float y, int selectedBand, bool isActive) {
        gainBar.gameObject.SetActive(isActive);
        gainBar.transform.position = new Vector3(x, y, 0);
    }

    public void DisplayTriangle(float x, float y, float size, int selectedBand, bool isActive) {
        triangles.transform.GetChild(selectedBand).gameObject.SetActive(isActive);
        triangles.transform.GetChild(selectedBand).transform.position = new Vector3(x, y, 0);
        triangles.transform.GetChild(selectedBand).transform.localScale = new Vector3(size, triangles.transform.GetChild(selectedBand).transform.localScale.y, 0);
        if (isActive) {
            gainBar.gameObject.SetActive(false);
        }
    }
}
