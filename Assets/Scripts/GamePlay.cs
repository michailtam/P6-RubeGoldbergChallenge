using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlay : MonoBehaviour
{

  public GameObject ballPref;               // The ball prefab
  public GameObject platform;               // The platform game object
  public GameObject pedastal;               // The pedastal game object
  public float throwForce = 1.5f;           // The force to throw the ball
  private Vector3 platformSurface;          // Needed to position the ball onto the platform
  private List<int> stepsPassed = new List<int>();  // Saves the steps the user has passed
  
  // PROPERTIES
  public string step
  {
    set 
    {
      int val = 0;
      // Checks if the given value is an int
      if(Int32.TryParse(value, out val)) {
        stepsPassed.Add(val);
      }
      else {
        Debug.Log("ERROR: Unable to parse " + value.ToString());
      }
    }
  }

  private bool _groundEntered = false;
  public bool groundEntered
  {
    set { _groundEntered = value; }
    get { return _groundEntered; }
  }


  // Use this for initialization
  void Start()
  {
    CalcSpawnPoint();
  }

  void Update()
  {
    if (_groundEntered) {
      ResetGame();
      SpawnBall();
    }
  }

  // Calculates the spawn point of the ball
  private void CalcSpawnPoint() 
  {
    // Shoots a 5m ray downwards from 2m centered above the platform to determine the
    // center point of the surface of the platform
    RaycastHit hit;
    Vector3 rayStart = new Vector3(platform.transform.position.x, platform.transform.position.y + 2.0f, platform.transform.position.z);
    // To test the ray: Debug.DrawLine(rayStart, rayStart - Vector3.up * 5, Color.green);

    if (Physics.Raycast(rayStart, -Vector3.up, out hit, 5)) {
      platformSurface = hit.point;
      SpawnBall();
    }
  }

  // Resets all game properties of the current level
  private void ResetGame()
  {
    _groundEntered = false;
  }

  // Spawns a new ball at a specific location on the platform
  public void SpawnBall() 
  {
    // Wait 2 sec until a new ball gets respawn onto the pedastal
    StartCoroutine(DelayTillCreation());
  }

  IEnumerator DelayTillCreation()
  {
    yield return new WaitForSeconds(2f);

    // Create a new ball in the pedastal
    GameObject ball = Instantiate(ballPref, platformSurface, Quaternion.identity);
    ball.GetComponent<Rigidbody>().isKinematic = false;   // Allows the physics of the ball to be used by Unity
    ball.GetComponent<Rigidbody>().useGravity = true;     // Lets the ball drop onto the platform
    ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y + 0.5f, ball.transform.position.z + 0.85f);
  }

  // Grabs the ball
  public void GrabBall(Collider col, GameObject parent, SteamVR_Controller.Device device) 
  {
    col.transform.SetParent(parent.transform);
    col.GetComponent<Rigidbody>().isKinematic = true;
  }

  // Throws the ball
  public void ThrowBall(Collider col, GameObject parent, SteamVR_Controller.Device device) 
  {
    col.transform.SetParent(null);
    Rigidbody rig = col.GetComponent<Rigidbody>();
    rig.isKinematic = false;
    rig.velocity = device.velocity * throwForce;
    rig.angularVelocity = device.angularVelocity;
  }

  // Checks if the player has cheated the game steps
  public bool HasPlayerCheated()
  {
    if(stepsPassed.Count > 0) 
    {
      int stepToTest = 2; // The step number to test for (starts from step 2)
      for (int i=0; i < stepsPassed.Count; i++) 
      {
        Debug.Log("STEPS PASST: " + stepsPassed[i] + "   STEP TO TEST: " + stepToTest);

        // Checks if the first step is NOT number 1
        if (stepsPassed[0] != 1) {
          return true;
        }
        // Checks if the first step is Nr. 1
        else if (stepsPassed[i] == 1) {
          continue;
        }
        // Checks if the order of the steps is the right one
        else if (stepsPassed[i] == stepToTest) {
          Debug.Log("STEP: " + stepToTest);
          stepToTest++;
        }
        else {
          return true;
        }  
      }
      return false;
    }
    return true;
  }
}
