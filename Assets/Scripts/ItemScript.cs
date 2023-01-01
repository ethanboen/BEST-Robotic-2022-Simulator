using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    
    public string rackName;
    public bool canPickUp = true;

    public Vector3 rackPosition;
    public Vector3 rackRotation;

    public Vector3 robotHandPosition;
    public Vector3 robotHandRotation;

    public Vector3 basketPosition;
    public Vector3 basketRotation;

    private void UpdatePosition(Vector3 position, Vector3 rotation) {

        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);

    }

    public void PositionOnRack() {

        UpdatePosition(rackPosition, rackRotation);
        
    }

    public void PositionOnRobotHand() {

        UpdatePosition(robotHandPosition, robotHandRotation);
        
    }

    public void PositionInBasket() {

        UpdatePosition(basketPosition, basketRotation);
        
    }

}
