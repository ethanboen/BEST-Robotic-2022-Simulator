using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerScript : MonoBehaviour {

    public GameObject[] items = new GameObject[0];

    void Start() {
        
        foreach (GameObject item in items) {

            ItemScript itemScript = item.transform.GetComponent<ItemScript>();

            Transform rack = GameObject.Find(itemScript.rackName).transform;

            Transform slot1 = rack.Find("Slot 1");
            Transform slot2 = rack.Find("Slot 2");

            Transform item1 = Instantiate(item, Vector3.zero, Quaternion.identity).transform;
            Transform item2 = Instantiate(item, Vector3.zero, Quaternion.identity).transform;

            item1.transform.SetParent(slot1);
            item2.transform.SetParent(slot2);

            ItemScript itemScript1 = item1.GetComponent<ItemScript>();
            ItemScript itemScript2 = item2.GetComponent<ItemScript>();

            itemScript1.PositionOnRack();
            itemScript2.PositionOnRack();

        }

    }

}
