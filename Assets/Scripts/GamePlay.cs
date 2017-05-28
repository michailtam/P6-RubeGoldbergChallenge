using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class GamePlay : MonoBehaviour
{
  public GameObject goal;                   // The goal game object
  public GameObject star;                   // The star game object
  public Transform player;                  // The current player
  public ParticleSystem goalParticle;       // The particle system of the goal
  public GameObject ball;                   // The ball in the scene
  public Material ballMaterial;             // The right ball material
  public Material cheatBallMaterial;        // The color that indicates that the player has cheated
  public GameObject gameOverDialog;         // The dialog which indicates the successfull end of the game
  public float throwForce = 1.5f;           // The force to throw the ball
  public int levelNumber;                   // Contains the current level number 
  public AudioClip[] audioClip;             // The audio clip for the goal
  private Vector3 spawnPoint;               // The calculated spawn point of the ball
  private int previousStep = 0;             // Saves the previous step
  private GameObject[] allCollectables;     // All the collectables in the scene
  private int countCollectibles;            // The amount of collectables in the scene
  private Vector3 ballStartPosition;        // The start position of the ball (on the pedastal)

  // Use this for initialization
  void Start()
  {
    // Hide the game over dialog if the current level is the last
    if(levelNumber == 4)
      gameOverDialog.SetActive(false);

    // Gets the balls start position on the pedastal
    ballStartPosition = ball.transform.position;

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
    ResetBallPosition();    // Resets the balls position
    ResetCollectables();    // Creates again all collectables
    goal.layer = LayerMask.NameToLayer("ValidGoal");  // Resets the layer of the goal
    goal.GetComponent<Goal>().hasEnteredGoal = false;
    countCollectibles = allCollectables.Length;
  }

  // Resets the balls position (returns to the pedastal)
  private void ResetBallPosition()
  {
    // Resets ball position (returns to the pedastal) and velocities
    ball.transform.position = ballStartPosition;
    ball.GetComponent<Renderer>().material = ballMaterial;
    Rigidbody rig = ball.GetComponent<Rigidbody>();
    rig.velocity = Vector3.zero;
    rig.angularVelocity = Vector3.zero;
    rig.isKinematic = true;
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
    RaycastHit[] hits = Physics.RaycastAll(player.transform.position, -Vector3.up, 20f).OrderBy(h => h.distance).ToArray();
    foreach(RaycastHit h in hits) {
      if(string.Compare(h.transform.tag, "Platform") == 0) {
        isPlayerOnPlatform = true;
      }
    }
    // If the ball was not released from the platform the balls color will be changed to
    // red to indicate that the player has to throw the ball to the ground.
    if (!isPlayerOnPlatform) {
      ball.GetComponent<Renderer>().material = cheatBallMaterial;
      // Changes the layer of the goal to prevent going to the next level
      goal.layer = LayerMask.NameToLayer("InvalidGoal");
    }
  }

  // Grabs the moveable object
  public void GrabObject(Collider col, GameObject parent, SteamVR_Controller.Device device)
  {
    // Checks if the player has still grabbed an item
    if(col.transform.parent == null) {
      col.transform.SetParent(parent.transform);
      col.GetComponent<Rigidbody>().isKinematic = true;
    }
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
  public void HasPlayerCheated()
  {
    // Checks if the player has cheated when released the ball
    if(string.Compare(LayerMask.LayerToName(goal.layer), "InvalidGoal") == 0) {
      //Debug.Log("PLAYER HAS NOT RELEASED THE BALL FROM THE PLATFORM");
      AudioSource.PlayClipAtPoint(audioClip[1], ball.transform.position);
      ResetLevel();
    }
    // Checks if the player has collected all the stars
    else if (countCollectibles > 0) {
      //Debug.Log("PLAYER HAS NOT COLLECTED ALL THE STARS. " + countCollectibles + " STAR NOT COLLECTED.");
      AudioSource.PlayClipAtPoint(audioClip[1], ball.transform.position);
      ResetLevel();
    }
    // Player has not cheated in the game
    else {
      AudioSource.PlayClipAtPoint(audioClip[0], ball.transform.position);

      // Create a particle system for 5 sec to indicate that the ball is in the goal
      ball.SetActive(false);
      Instantiate(goalParticle, goal.transform.position, Quaternion.Euler(-90, 0, 0));

      // If the current scene is the last level
      if(levelNumber == 4) {
        gameOverDialog.SetActive(true);
        StartCoroutine(DelayBeforeNextLevelLoad());
      } else {
        StartCoroutine(DelayBeforeNextLevelLoad());
      }
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
      case 4:
        SteamVR_LoadLevel.Begin("Level1", false, 5f);
        break;
      default:
        Debug.Log("ERROR: Undefined level number");
        break;
    }
  }
}
