﻿using UnityEngine;
using System.Collections;

public class Player : Singleton<Player>
{

    public GameObject gainBar;
    public GameObject triangles;
    public Color[] colors;

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
        gainBar.transform.position = new Vector3(x, y, gainBar.transform.position.z);
    }

    public void DisplayTriangle(float x, float y, float size, int selectedBand, bool isGainActive, bool isCFActive) {
        if (isCFActive) {
            triangles.transform.GetChild(selectedBand).gameObject.SetActive(isGainActive);
            triangles.transform.GetChild(selectedBand).transform.position = new Vector3(x, y, 0);
            triangles.transform.GetChild(selectedBand).transform.localScale = new Vector3(size,
                triangles.transform.GetChild(selectedBand).transform.localScale.y, 0);
            if (isGainActive) {
                gainBar.gameObject.SetActive(false);
            }
        }
        else {
            triangles.transform.GetChild(selectedBand).gameObject.SetActive(isCFActive);
        }
    }
}
