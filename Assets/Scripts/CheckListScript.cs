using UnityEngine;
using System.Collections;

public class CheckListScript : Singleton<CheckListScript>
{
    public bool[] isBandsActive = new bool[5];

	// Use this for initialization
    void Start() {
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < 5; i++) {
            this.transform.GetChild(i).gameObject.SetActive(isBandsActive[i]);
        }
    }
}
