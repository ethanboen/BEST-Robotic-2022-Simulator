using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketManagerScript : MonoBehaviour
{

    public Transform rawImage;
    public Texture2D endingScreenImage;

    public Transform conveyorBeltStart;

    public bool loadingStationFull = false;
    public int basketsOnConveyorStart = 0;

    public void AssignBasketPosition(Transform basket) {

        basketsOnConveyorStart++;

        BasketScript basketScript = basket.GetComponent<BasketScript>();
        basketScript.conveyorBeltPosition = basketsOnConveyorStart;

    }

    public void CheckCamera() {

        int numberOfBaskets = conveyorBeltStart.childCount;

        if (numberOfBaskets >= 4) {

            RawImage screenRawImage = rawImage.GetComponent<RawImage>();
            screenRawImage.texture = endingScreenImage;

        }

    }

    public void RepositionBaskets() {

        int numberOfBaskets = conveyorBeltStart.childCount;

        for (int i = 0; i < numberOfBaskets; i++) {

            Transform child = conveyorBeltStart.GetChild(i);
            BasketScript basketScript = child.GetComponent<BasketScript>();
            
            int distanceUnitsAway = basketsOnConveyorStart - basketScript.conveyorBeltPosition;

            child.localPosition = new Vector3(0, 0, distanceUnitsAway * 1.5f);

        }

        CheckCamera();

    }

}
