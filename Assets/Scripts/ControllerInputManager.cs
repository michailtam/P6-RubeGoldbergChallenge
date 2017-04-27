using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour {

  private SteamVR_TrackedObject trackedObject;   // The tracked objects in the scene (one HMD and two controllers)
  private SteamVR_Controller.Device device;      // The current device

  // Teleporter
  private LineRenderer laser;             // The laser pointer
  public GameObject teleportAimerObject;  // Indicator that shows were we get teleport to
  public Vector3 teleportLocation;        // Determines the 3d position the player gets teleport to
  public GameObject player;               // The player
  public LayerMask laserMask;             // This allows us to choose which layers the teleport raycast can collide with
  public float teleportRange;             // Determines the range of the teleport position
  private float yNudgeAmount = 1f;        // Determines the height nudge amount of the teleport aimer object

  // Dashing
  public float dashSpeed = 0.1f;          // Determines the speed the player is dashing
  private bool isDashing;                 // Determines if the dashing is going to be smooth
  private float lerpTime;                 // Determines the time (smooth factor)
  private Vector3 dashStartPosition;      // The start point to dash

  // Walking
  public Transform playerCam;             // The facing direction of the player
  public float moveSpeed = 4f;            // Determines the walking speed
  private Vector3 movementDirection;      // Determines the direction the player is moving

  // Use this for initialization
  void Start () 
  {
    // Gets the correct tracked object component
    trackedObject = GetComponent<SteamVR_TrackedObject>();
    laser = GetComponentInChildren<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
  {
    // Gets the intended tracked controller by the index
    device = SteamVR_Controller.Input((int)trackedObject.index);

    // Natural walking movement
    if(device.GetPress(SteamVR_Controller.ButtonMask.Grip)) 
    {
      movementDirection = playerCam.transform.forward;
      // Prevent to fly while walking
      movementDirection = new Vector3(movementDirection.x, 0, movementDirection.z);
      // Multiplies with the move speed factor and delta time of the frame
      movementDirection *= moveSpeed * Time.deltaTime;
      player.transform.position += movementDirection;
    }

    // Move the player smooth to the teleport location
    if(isDashing) 
    {
      lerpTime += Time.deltaTime * dashSpeed;
      player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, lerpTime);
      
      // Checks if the player has reached the intended location
      if (lerpTime >= 1) 
      {
        isDashing = false;
        lerpTime = 0;
      }
    }
    else 
    {
      // If the trigger of the controller gets pressed
      if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger)) {
        // Show the laser pointer and the teleport aimer object
        laser.gameObject.SetActive(true);
        teleportAimerObject.SetActive(true);

        // Sets the start point of the laser pointer
        laser.SetPosition(0, gameObject.transform.position);

        // Determin the teleport location by the range and the layer mask
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, teleportRange, laserMask)) {
          teleportLocation = hit.point;   // Records where the laser hits
          laser.SetPosition(1, teleportLocation);   // Sets the end point of the laser pointer
          teleportAimerObject.transform.position = new Vector3(
            teleportLocation.x, teleportLocation.y + yNudgeAmount, teleportLocation.z);
        }
        // If the laser pointer hits nothing
        else {
          // Moves the indicator forward the range relative to the controller  
          teleportLocation = transform.position + transform.forward * teleportRange;

          // Determine where the ground is to set the indicator onto
          RaycastHit groundRay;
          if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask)) {
            teleportLocation = new Vector3(
              transform.position.x + transform.forward.x * teleportRange,
              groundRay.point.y,
              transform.position.z + transform.forward.z * teleportRange);
          }
          laser.SetPosition(1, transform.position + transform.forward * teleportRange);

          // Sets the teleport aimer position
          teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmount, 0);
        }
      }
      // If the trigger of the controller gets released
      if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
        // Hide the laser pointer and the teleport aimer object
        laser.gameObject.SetActive(false);
        teleportAimerObject.SetActive(false);

        // Trigger the dashing
        dashStartPosition = player.transform.position;
        isDashing = true;
      }
    }   
  }
}
