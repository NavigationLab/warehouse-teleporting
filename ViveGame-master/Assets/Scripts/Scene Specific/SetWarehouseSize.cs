using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWarehouseSize : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("Warehouse_Big").SetActive(GameSettings.warehouse_big);
        GameObject.Find("Warehouse_Small").SetActive(!GameSettings.warehouse_big);
    }
}
