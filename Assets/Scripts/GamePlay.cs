using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour {

  public GameObject ballPref;         // The ball prefab
  public GameObject platform;         // The platform where the player stands
  private Vector3 platformSurface;    // Needed to position the ball onto the platform

  // Use this for initialization
  void Start() 
  {
    CalcPlatformSurface();
  }

  // Calculates the platforms surface center
  private void CalcPlatformSurface() 
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
    float radius = ball.GetComponent<SphereCollider>().radius;
    ball.GetComponent<Rigidbody>().isKinematic = false;
    ball.GetComponent<Rigidbody>().useGravity = true;
    ball.transform.position = new Vector3(ball.transform.position.x, ball.transform.position.y + radius, ball.transform.position.z + 0.5f);
  }
}
