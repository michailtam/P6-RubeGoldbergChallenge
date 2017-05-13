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

  // Properties
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

  // Use this for initialization
  void Start() 
  {
    CalcSpawnPoint();
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

  // Spawns a new ball at a specific location on the platform
  public void SpawnBall() 
  {
    GameObject ball = Instantiate(ballPref, platformSurface, Quaternion.identity);

    // Set the properties of the ball to position it on specific place on the platform
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
  public bool HasPlayerChaeted()
  {
    if(stepsPassed.Count > 0) 
    {
      int previousStep = -1;
      for (int i = 0; i < stepsPassed.Count - 1; i++) 
      {
        // Checks if the first step is Nr. 1
        if (stepsPassed[0] != 1) {
          return true;
        }
        // Checks if it is the first step
        else if (stepsPassed[0] == 1) {
          previousStep = stepsPassed[i];
        }
        // Checks if the order of the steps is the right one
        else if (stepsPassed[i] == previousStep+1) {
          previousStep = stepsPassed[i];
        }
        else
          return true;
      }
      return false;
    }

    return true;
  }
}
