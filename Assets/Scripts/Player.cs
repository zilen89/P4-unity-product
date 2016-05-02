using UnityEngine;
using System.Collections;

public class Player : Singleton<Player>
{

    public GameObject gainBar;
    public GameObject triangles;
    public Color[] colors;
    public bool isGainActive = false;
    public bool isQActive = false;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    this.GetComponent<Renderer>().material.color = colors[Movement.Instance.selectedBand];
        gainBar.GetComponent<Renderer>().material.color = colors[Movement.Instance.selectedBand];
    }

    public void DisplayGain(float x, float y, int selectedBand, bool isActive)
    {
        gainBar.gameObject.SetActive(isActive);
        gainBar.transform.position = new Vector3(x, y, 0);
        isGainActive = isActive;
    }

    public void DisplayTriangle(float x, float y, float size, int selectedBand, bool isActive)
    {
        triangles.transform.GetChild(selectedBand).gameObject.SetActive(isActive);
        triangles.transform.GetChild(selectedBand).transform.position = new Vector3(x, y, 0);
        triangles.transform.GetChild(selectedBand).transform.localScale = new Vector3(size, triangles.transform.GetChild(selectedBand).transform.localScale.y, 0);
        isQActive = isActive;
        if (isActive)
        {
            gainBar.gameObject.SetActive(false);
        }
    }
}
