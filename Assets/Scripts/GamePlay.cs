using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlay : MonoBehaviour
{

  public GameObject ballPref;               // The ball prefab
  public GameObject platform;               // The platform game object
  public GameObject pedastal;               // The pedastal game object
  public GameObject goal;                   // The goal game object
  public GameObject star;                   // The star game object
  public ParticleSystem goalParticle;       // The particle system of the goal
  public float throwForce = 1.5f;           // The force to throw the ball
  public int levelNumber;                   // Contains the current level number
  private Vector3 spawnPoint;               // The calculated spawn point of the ball
  private List<int> stepsPassed = new List<int>();  // Saves the steps the user has passed
  private GameObject[] allCollectables;     // All the collectables in the scene
  private int countCollectibles;            // The amount of collectables in the scene
  private GameObject ballInstance;          // The one ball instance in the scene


  // PROPERTIES
  public string step  // Contains the number of the current step
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
    // Gets the amount of collectibles;
    allCollectables = GameObject.FindGameObjectsWithTag("Collectable");
    countCollectibles = allCollectables.Length;
    foreach(GameObject go in allCollectables) {
      go.SetActive(true);
    }

    // Calculates the spawn point of the ball
    CalcSpawnPoint();
  }

  void Update()
  {
    if (_groundEntered) {
      ResetGame();
    }
  }

  // Counts the amount of the collectables in the scene
  public void decreaseCollectibles(GameObject go)
  {
    go.SetActive(false);
    countCollectibles--;
  }

  // Saves all collectables (Stars) in a list
  private void ResetCollectables()
  {
    foreach(GameObject go in allCollectables) {
      go.SetActive(true);
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
      // Calculates the spawn point of the ball
      spawnPoint = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z + 0.85f);
      ballInstance = Instantiate(ballPref, spawnPoint, Quaternion.identity);
      // Sets its physics properties
      Rigidbody rig = ballInstance.GetComponent<Rigidbody>();
      rig.isKinematic = false;  // Allows the physics of the ball to be used by Unity
      rig.useGravity = true;    // Lets the ball drop onto the platform
    }
  }

  // Resets all game properties of the current level
  private void ResetGame()
  {
    _groundEntered = false;
    stepsPassed.Clear();    // Deletes all saved steps in the list
    ResetCollectables();    // Creates again all collectables
    ResetBallPosition();
  }

  // Spawns a new ball at a specific location on the platform
  public void ResetBallPosition() 
  {
    ballInstance.transform.position = spawnPoint; 
    
    Rigidbody rig = ballInstance.GetComponent<Rigidbody>();
    rig.isKinematic = false;  // Allows the physics of the ball to be used by Unity
    rig.velocity = Vector3.zero;
    rig.angularVelocity = Vector3.zero;
    rig.useGravity = true;    // Lets the ball drop onto the platform
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

  // Chechs if the player has cheated and if he has collected all collectables
  public void CheckPlayer()
  {
    // Checks if the player has cheated the steps
    if (HasPlayerCheated() || countCollectibles > 0) {
      Debug.Log("PLAYER HAS CHEATED THE STEPS");
      ResetGame();
    }
    else {
      // Create a particle system for 5 sec to indicate that the ball is in the goal
      GameObject ball = GameObject.FindGameObjectWithTag("Throwable");
      Destroy(ball);    // Destroy the ball after entering the goal
      Instantiate(goalParticle, goal.transform.position, Quaternion.Euler(-90, 0, 0));
      StartCoroutine(DelayBeforeNextLevelLoad());
    }
  }

  IEnumerator DelayBeforeNextLevelLoad()
  {
    yield return new WaitForSeconds(3.0f);
    LoadNextLevel();
  }

  // Loads the level related to the level number
  public void LoadNextLevel()
  {
    switch (levelNumber) 
    {
      case 1:
        SteamVR_LoadLevel.Begin("Level2", false, 2f);
        break;
      case 2:
        SteamVR_LoadLevel.Begin("Level3");
        break;
      case 3:
        SteamVR_LoadLevel.Begin("Level4");
        break;
      case 4:
        SteamVR_LoadLevel.Begin("Level5");
        break;
      default:
        Debug.Log("ERROR: Undefined level number");
        break;
    }
  }
}
