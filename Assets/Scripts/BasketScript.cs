using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketScript : MonoBehaviour {
    
    public Transform robotClaw;

    public int conveyorBeltPosition = -1;

    public bool basketFull = false;
    public bool canPickUp = true;
    public bool canBeLoaded = false;
    
    public void AddToSlot(int slotId, Transform item) {

        if (item.CompareTag("Item") && slotId > 0 && slotId <= 3) {

            Transform slot = transform.Find("Slot " + slotId.ToString());

            item.SetParent(slot);
            item.localPosition = Vector3.zero;

            UpdateBasketState();

            if (basketFull) {

                canBeLoaded = false;

                RobotClawScript robotClawScript = robotClaw.GetComponent<RobotClawScript>();

                robotClawScript.PickUpBasket(transform);

            }

        }

    }

    public bool SlotEmpty(int slotId) {

        if (slotId > 0 && slotId <= 3) {

            Transform slot = transform.Find("Slot " + slotId.ToString());

            if (slot.childCount > 0) {

                return false;

            }

            return true;

        }
        
        return false;

    }

    private void UpdateBasketState() {

        bool full = true;

        for (int i = 0; i < 3; i++) {

            if (this.SlotEmpty(i + 1)) {

                full = false;

            }

        }

        basketFull = full;

    }

}
