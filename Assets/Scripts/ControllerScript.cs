using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComplexSound {

    public AudioSource audioSource;
    public float soundLength;
    public bool canPlaySound;

    public ComplexSound(AudioSource audioSource) {

        this.audioSource = audioSource;
        soundLength = audioSource.clip.length;
        canPlaySound = true;

    }

}

public class ControllerScript : MonoBehaviour {

    Vector3 moveDirection = Vector3.zero;

    public float moveSpeed = 10;
    public float turnSpeed = 1;

    public string[] buttons = new string[0];

    public Transform tutorialForm;

    public Transform squeakyRobot;

    public Transform robotCamera;
    public Transform robotHand;

    public Transform[] highlightObjects = new Transform[0];

    public Transform basketPosition1;

    public Transform guideArrow;

    public Transform controlBoxArea;
    public Transform basketDropArea;

    public Transform basketManager;

    public bool robotMoving = false;

    [SerializeField]
    private bool leverBeingUsed = false;

    private bool triggerHold = false;

    [SerializeField]
    private bool inControlBoxArea = false;

    [SerializeField]
    private bool inBasketDropArea = false;

    [SerializeField]
    private bool hoveringOverLever = false;

    [SerializeField]
    private bool holdingBasket = false;

    [SerializeField]
    private Transform currentSelectedLever;

    private Rigidbody robotRigidbody;
    private CharacterController robotController;

    private ComplexSound wheelTurnSFX;
    private ComplexSound servoTurnSFX;

    private SqueakyScript squeakyScript;

    BasketManagerScript basketManagerScript;

    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Control Box Trigger")) {

            inControlBoxArea = true;

            if (guideArrow.gameObject.activeSelf && !holdingBasket) {

                guideArrow.gameObject.SetActive(false);

            }

        }
        else if (other.transform.CompareTag("Basket Drop Trigger")) {

            inBasketDropArea = true;

            if (holdingBasket) {

                holdingBasket = false;
                basketManagerScript.loadingStationFull = true;

                Transform basket = robotHand.GetChild(0);

                basket.SetParent(basketPosition1);
                basket.localPosition = Vector3.zero;
                basket.localRotation = Quaternion.Euler(0, 0, 0);

                guideArrow.position = controlBoxArea.position;

                BasketScript basketScript = basket.GetComponent<BasketScript>();

                basketScript.canBeLoaded = true;

            }

        }
        else if (other.transform.CompareTag("Basket")) {

            Transform basket = other.transform;
            BasketScript basketScript = basket.GetComponent<BasketScript>();

            if (basketScript.canPickUp && !holdingBasket && !basketManagerScript.loadingStationFull) {

                holdingBasket = true;
                basketScript.canPickUp = false;

                basket.SetParent(robotHand);
                basket.localPosition = Vector3.zero;
                basket.localRotation = Quaternion.Euler(0, 0, 0);

                guideArrow.position = basketDropArea.position;
                guideArrow.gameObject.SetActive(true);

            }

        }
        
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.transform.CompareTag("Control Box Trigger")) {

            inControlBoxArea = false;

        }
        else if (other.transform.CompareTag("Basket Drop Trigger")) {

            inBasketDropArea = false;

        }
        
    }

    void Start() {
        
        wheelTurnSFX = new ComplexSound(robotCamera.transform.Find("Wheel Turning Sound").GetComponent<AudioSource>());
        servoTurnSFX = new ComplexSound(robotCamera.transform.Find("Robot Servo Sound").GetComponent<AudioSource>());

        robotRigidbody = transform.GetComponent<Rigidbody>();
        robotController = transform.GetComponent<CharacterController>();

        squeakyScript = squeakyRobot.GetComponent<SqueakyScript>();

        basketManagerScript = basketManager.GetComponent<BasketManagerScript>();

        Cursor.visible = false;

    }

    void Update() {
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (Mathf.Abs(verticalInput) > 0.5f) {

            if (leverBeingUsed && (!robotMoving)) {

                squeakyScript.HandleInput(verticalInput, moveSpeed, turnSpeed, currentSelectedLever);

                if (!wheelTurnSFX.canPlaySound) {

                    wheelTurnSFX.audioSource.time = 27;

                }

                wheelTurnSFX.canPlaySound = true;

            }
            else {

                robotMoving = true;

                if (wheelTurnSFX.canPlaySound) {

                    wheelTurnSFX.audioSource.time = 0;
                    wheelTurnSFX.audioSource.Play();

                    wheelTurnSFX.canPlaySound = false;

                }
                else {

                    if (wheelTurnSFX.audioSource.time >= 10) {

                        wheelTurnSFX.audioSource.time = 7;

                    }

                }

                robotController.Move(transform.forward * -verticalInput * moveSpeed * Time.deltaTime);

            }

        }
        else {

            robotMoving = false;

            if (hoveringOverLever && triggerHold) {

                currentSelectedLever.transform.localPosition = new Vector3(
                    currentSelectedLever.transform.localPosition.x,
                    0.0015f,
                    currentSelectedLever.transform.localPosition.z
                );

            }

            if (!wheelTurnSFX.canPlaySound) {

                wheelTurnSFX.audioSource.time = 27;

            }

            wheelTurnSFX.canPlaySound = true;

        }

        if (Mathf.Abs(horizontalInput) > 0.5f) {

            if (!(leverBeingUsed)) {

                if (servoTurnSFX.canPlaySound) {

                    servoTurnSFX.audioSource.time = 3;
                    servoTurnSFX.audioSource.Play();

                    servoTurnSFX.canPlaySound = false;

                }
                else {

                    if (servoTurnSFX.audioSource.time >= 10) {

                        servoTurnSFX.audioSource.time = 3;

                    }

                }

                transform.Rotate(0, horizontalInput * turnSpeed, 0);

                if (currentSelectedLever) {

                    currentSelectedLever.transform.localPosition = new Vector3(
                        currentSelectedLever.transform.localPosition.x,
                        0.0015f,
                        currentSelectedLever.transform.localPosition.z
                    );

                    triggerHold = false;

                }

            }

        }
        else {

            if (!servoTurnSFX.canPlaySound) {

                servoTurnSFX.audioSource.time = 3;
                servoTurnSFX.audioSource.Stop();

            }

            servoTurnSFX.canPlaySound = true;

        }

        HandleLeverControls();
        HandleLeverClick();
        HandleGameReset();

    }

    private void HandleLeverHover() {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        bool foundButton = false;

        foreach (RaycastHit hit in hits) {

            if (hit.collider.CompareTag("Button")) {

                if (inControlBoxArea) {

                    Transform highlightObject = hit.transform.Find("Highlight");

                    if (!highlightObject.gameObject.activeSelf) {

                        highlightObject.gameObject.SetActive(true);

                    }

                    foundButton = true;
                    break;

                }

            }

        }

        if (!foundButton) {

            hoveringOverLever = false;

            foreach (Transform highlightObject in highlightObjects) {

                highlightObject.gameObject.SetActive(false);

            }

        }
        else {

            bool foundHighlighted = false;

            foreach (Transform highlightObject in highlightObjects) {

                if (!foundHighlighted) {

                    if (highlightObject.gameObject.activeSelf) {

                        foundHighlighted = true;

                        Transform parentObject = highlightObject.transform.parent.transform;

                        if (parentObject.CompareTag("Button")) {

                            if (inControlBoxArea) {

                                hoveringOverLever = true;
                                currentSelectedLever = highlightObject.transform.parent.transform;
                            
                            }

                        }

                    }

                }
                else {

                    highlightObject.gameObject.SetActive(false);

                }

            }
            
        }

    }

    private void HandleLeverClick() {

        Transform highlight = currentSelectedLever.transform.Find("Highlight");

        if (leverBeingUsed) {

            if (!highlight.gameObject.activeSelf) {

                highlight.gameObject.SetActive(true);

            }

        }
        else {

            HandleLeverHover();

        }

    }

    private void HandleLeverControls() {

        if (Input.GetKeyDown("joystick button 1")) {

            if (tutorialForm.gameObject.activeSelf) {

                tutorialForm.gameObject.SetActive(false);

            }
            else {

                tutorialForm.gameObject.SetActive(true);

            }

        }

        if (Input.GetKeyDown("joystick button 0")) {

            triggerHold = true;

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (RaycastHit hit in hits) {

                if (hit.collider.CompareTag("Button")) {

                    if (inControlBoxArea) {

                        Transform highlightObject = hit.transform.Find("Highlight");

                        currentSelectedLever = highlightObject.transform.parent.transform;

                        leverBeingUsed = true;

                        break;

                    }

                }

            }

        }
        else if (Input.GetKeyUp("joystick button 0")) {

            currentSelectedLever.transform.localPosition = new Vector3(
                    currentSelectedLever.transform.localPosition.x,
                    0.0015f,
                    currentSelectedLever.transform.localPosition.z
                );

            triggerHold = false;

            leverBeingUsed = false;

        }

    }

    private void HandleGameReset() {

        for (int i = 3; i <= 6; i++) {

            if (Input.GetKeyUp(buttons[i - 1])) {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }

        }

    }

}
