
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ControllerInputManager : MonoBehaviour
{

  // HTC Vive Controllers 
  private SteamVR_TrackedObject trackedObject;    // The tracked objects in the scene (one HMD and two controllers)
  private SteamVR_Controller.Device device;       // The controller device
  private int indexLeft;                          // Index of the left controller
  private int indexRight;                         // Index of the right controller

  // Game play
  public GamePlay gamePlay;               // Game play script of the GamePlay game object

  // Teleporter
  private LineRenderer laser;             // The laser pointer
  public GameObject teleportTarget;       // Indicator that shows were we get teleport to
  public Vector3 teleportLocation;        // Determines the 3d position the player gets teleport to
  public GameObject player;               // The player
  public LayerMask laserMask;             // This allows us to choose which layers the teleport raycast can collide with
  public float teleportRange;             // Determines the range of the teleport position
  public Material laserPointerColor;      // The color of the laser pointer
  public Material restrictedColor;        // The color of the laser pointer pointing to a restriced position
  private float maxHeightNav = 5f;        // The maximum height (4m) the player is able to rise 

  // Dashing
  public float dashSpeed = 0.1f;          // Determines the speed the player is dashing
  private bool isDashing;                 // Determines if the dashing is going to be smooth
  private float lerpTime;                 // Determines the time (smooth factor)
  private Vector3 dashStartPosition;      // The start point to dash

  // Walking
  public Transform playerCam;             // The facing direction of the player
  public float moveSpeed = 4f;            // Determines the walking speed
  private Vector3 movementDirection;      // Determines the direction the player is moving

  // Object menu
  public ObjectMenuManager objectMenuManager; // The menu manager object
  private float swipeSum;                 // The calculated swipe sum
  private float touchLast;                // The last touch position onto the axis of the controller
  private float touchCurrent;             // The new swipe position onto the axis of the controller  
  private float distance;                 // The calculated distance of the touch position
  private bool hasSwipedLeft;             // Prevents from unwanted swiping to the left 
  private bool hasSwipedRight;            // Prevents from unwanted swiping to the right

  // Tutorial UI
  public GameObject tutorialUI;           // The tutorial UI which explains the gameplay


  // Use this for initialization
  void Start() {
    // Distinguish between controllers
    trackedObject = GetComponent<SteamVR_TrackedObject>();
    indexLeft = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
    indexRight = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

    // Gets the correct tracked object component
    trackedObject = GetComponent<SteamVR_TrackedObject>();
    laser = GetComponentInChildren<LineRenderer>();
  }

  // Update is called once per frame
  void Update()
  {
    // Gets the current device index
    device = SteamVR_Controller.Input((int)trackedObject.index);

    if(device.index == indexLeft)
      Movement();         // Manages the movement with the left controller
    if(device.index == indexRight)
      ManageObjectMenu(); // Manages the menu objects with the right controller
    DisplayTutorialUI();  // Displays or hides the tutorial UI
  }

  // Displays or hides the tutorial UI
  private void DisplayTutorialUI()
  {
    if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
      if (tutorialUI.activeInHierarchy) {
        tutorialUI.SetActive(false);
      }
      else {
        tutorialUI.SetActive(true);
      }
    }
  }

  // Manages the object menu
  private void ManageObjectMenu()
  {
    // Set the object menu visible as long as the user is touching the touchpad
    if(device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)) 
    {
      objectMenuManager.gameObject.SetActive(true);

      // Sets the first touch position onto the x-axis of the controller
      touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
      
      // Calculates and executes the scroll direction (which is the next menu object) of the object menu
      // Get the current touch position on the x-axis of the controller
      touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
      distance = touchCurrent - touchLast;  // Calculates the new touch distance
      touchLast = touchCurrent;             // Saves the new touch position
      swipeSum += distance;                 // Adds the new distance to the previous

      // Swipe in the approriate direction in relation to the swipe sum
      if (!hasSwipedRight) {
        if (swipeSum > 0.5f) {
          swipeSum = 0;
          objectMenuManager.ShiftToRight();
          hasSwipedRight = true;
          hasSwipedLeft = false;
        }
      }

      if (!hasSwipedLeft) {
        if (swipeSum < -0.5f) {
          swipeSum = 0;
          objectMenuManager.ShiftToLeft();
          hasSwipedRight = false;
          hasSwipedLeft = true;
        }
      }
    }
    
    // Resets all the values and hides the object menu
    if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)) 
    { 
      swipeSum = 0;
      touchCurrent = 0;
      touchLast = 0;
      hasSwipedLeft = false;
      hasSwipedRight = false;
      objectMenuManager.gameObject.SetActive(false);
    }

    // Spawns the current selected menu object
    if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
      objectMenuManager.SpawnCurrentObject();
    }
  }

  // Movement management
  private void Movement()
  {
    // Rises player one meter up or down
    if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad)) 
    {
      float up = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y;
      float playerNavigationHeight = playerCam.transform.position.y;

      // Rises the player up
      if (up >= 0.5) {
        if(player.transform.position.y <= maxHeightNav) {
          player.transform.position = new Vector3(
          player.transform.position.x,
          player.transform.position.y + 1.0f,
          player.transform.position.z);
        }
      }
      // Falls one meter down 
      else if (up <= -0.5 && playerNavigationHeight >= playerCam.transform.position.y) {
        if(player.transform.position.y >= 1) {
          player.transform.position = new Vector3(
          player.transform.position.x,
          player.transform.position.y - 1.0f,
          player.transform.position.z);
        }
      }
    }

    // Move the player smooth to the teleport location
    if (isDashing) {
      lerpTime += Time.deltaTime * dashSpeed;
      player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, lerpTime);

      // Checks if the player has reached the intended location
      if (lerpTime >= 1) {
        isDashing = false;
        lerpTime = 0;
      }
    }
    else 
    {
      // If the trigger of the controller gets pressed
      if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) 
      {
        if (laser == null) return;

        // Show the laser pointer and the teleport aimer object
        laser.gameObject.SetActive(true);
        teleportTarget.SetActive(true);

        // Sets the start point of the laser pointer
        laser.SetPosition(0, gameObject.transform.position);

        // VERY IMPORTANT: The Unity doc https://docs.unity3d.com/ScriptReference/Physics.RaycastAll.html notice that the returned
        // order of the array objects is not guaranteed. So we order them ourself by the distance
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, teleportRange).OrderBy(h=>h.distance).ToArray();

        // Check if the pointer points direct onto the restricted point
        for (int i = 0; i < hits.Length; i++) 
        {   
          if (hits[0].transform.CompareTag("Restricted") || hits[0].transform.CompareTag("Structure")) 
          {
            laser.SetPosition(1, transform.position + transform.forward * (hits[0].distance));
            laser.material = restrictedColor;

            RaycastHit hitGround;
            if (Physics.Raycast(transform.position, -Vector3.up, out hitGround, 10, laserMask)) {
              teleportTarget.transform.position = player.transform.position;
              teleportLocation = player.transform.position;
              dashStartPosition = player.transform.position;
              device.TriggerHapticPulse();
            }
            return;
          }
        }

        // Determines the teleport location by the range and the layer mask (laser hits the ground)
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, teleportRange, laserMask)) 
        {
          teleportLocation = hit.point;   // Records where the laser hits
          laser.SetPosition(1, teleportLocation);   // Sets the end point of the laser pointer
          laser.material = laserPointerColor;
          teleportTarget.transform.position = teleportLocation;
        }
        // If the laser pointer hits nothing
        else 
        {
          // Moves the indicator forward the range relative to the controller  
          teleportLocation = transform.position + transform.forward * teleportRange;

          // Determines where the ground is to set the indicator onto
          RaycastHit groundRay;
          if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask)) 
          {
            teleportLocation = new Vector3(
              transform.position.x + transform.forward.x * teleportRange,
              groundRay.point.y,
              transform.position.z + transform.forward.z * teleportRange);
          }
          laser.SetPosition(1, transform.position + transform.forward * teleportRange);
          laser.material = laserPointerColor;

          // Sets the teleport aimer position
          teleportTarget.transform.position = teleportLocation;
        }
      }

      // If the trigger of the controller gets released
      if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) 
      {
        if (laser == null) return;

        // Hide the laser pointer and the teleport aimer object
        laser.gameObject.SetActive(false);
        teleportTarget.SetActive(false);

        // Trigger the dashing
        dashStartPosition = player.transform.position;
        isDashing = true;
      }
    }
  }

  // Gets invoked till the foreign object exits the collider
  private void OnTriggerStay(Collider col)
  {
    // If the collided object is a throwable (ball)
    if (col.gameObject.CompareTag("Throwable")) {
      // Grabs the throwable object
      if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
        gamePlay.GrabBall(col, gameObject, device);
      }
      // Throws the throwable object
      else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
        gamePlay.ThrowBall(col, gameObject, device);
      }
    }

    // If the collided object is a throwable (ball)
    if(col.gameObject.CompareTag("Structure")) {
      // Grabs the moveable object
      if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
        gamePlay.GrabObject(col, gameObject, device);
      }
      // Throws the throwable object
      else if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
        gamePlay.ReleaseObject(col, gameObject, device);
      }
    }
  }
}
