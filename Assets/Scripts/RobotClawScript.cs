using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotClawScript : MonoBehaviour {

    public Transform basketManager;

    public Transform slot;
    public Transform conveyorBeltStart;

    Animator anim;
    BasketManagerScript basketManagerScript;

    private float FrameToSeconds(int frame) {

        return (float) (((double) (1.0 / 60.0)) * ((double) frame));

    }

    void Start() {

        anim = transform.GetComponent<Animator>();
        basketManagerScript = basketManager.GetComponent<BasketManagerScript>();

    }

    public void PickUpBasket(Transform basket) {

        if (basket.CompareTag("Basket")) {

            basketManagerScript.loadingStationFull = false;

            anim.Play("Robot_Claw_Grab_Animation");

            StartCoroutine(WaitForArmToGrab(basket));

        }

    }

    IEnumerator WaitForArmToGrab(Transform basket) {

        yield return new WaitForSeconds(FrameToSeconds(68));

        basket.SetParent(slot);
        basket.localPosition = Vector3.zero;

        yield return new WaitForSeconds(FrameToSeconds(117 - 68));

        basket.SetParent(conveyorBeltStart);
        basket.localRotation = Quaternion.Euler(0, 0, 0);
        
        basketManagerScript.AssignBasketPosition(basket);
        basketManagerScript.RepositionBaskets();

        anim.Play("Robot_Idle_Animation");

    }

}
