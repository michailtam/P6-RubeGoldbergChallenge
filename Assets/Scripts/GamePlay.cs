using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class GamePlay : MonoBehaviour
{
  public GameObject goal;                   // The goal game object
  public GameObject star;                   // The star game object
  public Transform player;                 // The current player
  public ParticleSystem goalParticle;       // The particle system of the goal
  public GameObject ball;                   // The ball in the scene
  public Material ballMaterial;             // The right ball material
  public Material cheatBallMaterial;        // The color that indicates that the player has cheated
  public float throwForce = 1.5f;           // The force to throw the ball
  public int levelNumber;                   // Contains the current level number 
  private Vector3 spawnPoint;               // The calculated spawn point of the ball
  private int previousStep = 0;             // Saves the previous step
  private GameObject[] allCollectables;     // All the collectables in the scene
  private int countCollectibles;            // The amount of collectables in the scene
  private bool cheatStatus = false;         // Indicates if the player has cheated

  // Use this for initialization
  void Start()
  {
    // Gets the amount of collectibles;
    allCollectables = GameObject.FindGameObjectsWithTag("Collectable");
    countCollectibles = allCollectables.Length;
    foreach(GameObject go in allCollectables) {
      go.SetActive(true);
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

  // Resets all game properties of the current level
  public void ResetLevel()
  {
    previousStep = 0;
    ResetCollectables();    // Creates again all collectables
    ball.GetComponent<Renderer>().material = ballMaterial;
  }

  // Grabs the throwable object (ball)
  public void GrabBall(Collider col, GameObject parent, SteamVR_Controller.Device device) 
  {
    col.transform.SetParent(parent.transform);
    col.GetComponent<Rigidbody>().isKinematic = true;
  }

  // Throws the throwable object (ball)
  public void ThrowBall(Collider col, GameObject parent, SteamVR_Controller.Device device) 
  {
    col.transform.SetParent(null);
    Rigidbody rig = col.GetComponent<Rigidbody>();
    rig.isKinematic = false;
    rig.velocity = device.velocity * throwForce;
    rig.angularVelocity = device.angularVelocity;

    // Checks if the ball was NOT released from the platform
    bool isPlayerOnPlatform = false;
    RaycastHit[] hits = Physics.RaycastAll(player.transform.position, -Vector3.up, 10f).OrderBy(h => h.distance).ToArray();
    foreach(RaycastHit h in hits) {
      if(string.Compare(h.transform.tag, "Platform") == 0) {
        isPlayerOnPlatform = true;
      }
    }
    if(!isPlayerOnPlatform) {
      ball.GetComponent<Renderer>().material = cheatBallMaterial;
      cheatStatus = true;
    }
  }

  // Grabs the moveable object
  public void GrabObject(Collider col, GameObject parent, SteamVR_Controller.Device device)
  {
    col.transform.SetParent(parent.transform);
    col.GetComponent<Rigidbody>().isKinematic = true;
  }

  // Releases the moveable object at the current position
  public void ReleaseObject(Collider col, GameObject parent, SteamVR_Controller.Device device)
  {
    col.transform.SetParent(null);
    col.GetComponent<Rigidbody>().isKinematic = true;

    // Checks if the gameobject of the collider is the trampoline
    string s = col.gameObject.name;
    if (s.Contains("Trampoline")) {
      Rigidbody rig = col.GetComponent<Rigidbody>();
      rig.useGravity = true;
      rig.isKinematic = false;
      col.isTrigger = false;
    }
  }

  // Chechs if the player has cheated and if he has collected all collectables
  public void CheckPlayer()
  {
    // Checks if the player has cheated the steps
    if (countCollectibles > 0) {
      Debug.Log("PLAYER HAS CHEATED THE STEPS");
      ResetLevel();
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
        SteamVR_LoadLevel.Begin("Level3", false, 2f);
        break;
      case 3:
        SteamVR_LoadLevel.Begin("Level4", false, 2f);
        break;
      default:
        Debug.Log("ERROR: Undefined level number");
        break;
    }
  }
}
