using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueakyScript : MonoBehaviour {

    public bool holdingItem = false;

    [SerializeField]
    private Transform squeakyCord;

    [SerializeField]
    private Transform squeakyRotatingBox;

    [SerializeField]
    private Transform squeakyWheel;
    
    [SerializeField]
    private Transform squeakyArm;

    void Update() {

        Ray ray = new Ray(squeakyArm.position, squeakyArm.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 2f);

        foreach (RaycastHit hit in hits) {

            if (!holdingItem) {

                if (hit.collider.CompareTag("Item")) {

                    ItemScript itemScript = hit.transform.GetComponent<ItemScript>();

                    if (itemScript.canPickUp) {

                        holdingItem = true;
                        itemScript.canPickUp = false;

                        hit.transform.SetParent(squeakyArm.Find("Slot").transform);
                        itemScript.PositionOnRobotHand();

                    }
                    
                }

            }
            else {

                if (hit.collider.CompareTag("Basket")) {

                    BasketScript basketScript = hit.transform.GetComponent<BasketScript>();

                    if (basketScript.canBeLoaded) {

                        Transform itemInHand = squeakyArm.Find("Slot").GetChild(0);

                        ItemScript itemScript = itemInHand.GetComponent<ItemScript>();

                        for (int i = 0; i < 3; i++) {

                            if (basketScript.SlotEmpty(i + 1)) {

                                basketScript.AddToSlot(i + 1, itemInHand);
                                itemScript.PositionInBasket();

                                holdingItem = false;

                                break;

                            }

                        }

                    }

                }

            }

        }

    }

    public void HandleInput(float verticalInput, float moveSpeed, float turnSpeed, Transform lever) {

        MoveLever(verticalInput, lever);

        if (lever.name == "Lever_Move") {

            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z + -verticalInput * moveSpeed * Time.deltaTime, -8.5f, 8.5f)
                );

            squeakyWheel.transform.Rotate(-verticalInput, 0, 0);

            squeakyCord.transform.localScale = new Vector3(1, 1, transform.position.z + 8.5f);

        }
        else if (lever.name == "Lever_Arm") {

            squeakyArm.transform.localPosition = new Vector3(
                squeakyArm.transform.localPosition.x,
                squeakyArm.transform.localPosition.y,
                Mathf.Clamp(squeakyArm.transform.localPosition.z + -verticalInput * 0.008f * Time.deltaTime, -0.003f, 0.005f)
                );

        }
        else if (lever.name == "Lever_Turn") {

            squeakyRotatingBox.transform.Rotate(0, 0, -verticalInput * turnSpeed);

        }

    }

    private void MoveLever(float verticalInput, Transform lever) {

        if (Mathf.Sign(-verticalInput) == 1) {

            lever.localPosition = new Vector3(
                lever.localPosition.x,
                0.0045f,
                lever.localPosition.z
            );

        }
        else {

            lever.localPosition = new Vector3(
                lever.localPosition.x,
                -0.0013f,
                lever.localPosition.z
            );

        }

    }

}
