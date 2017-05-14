using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1StepsIndicator : MonoBehaviour {

  public LineRenderer line;
  public Transform[] gameSteps;
  
  void Start()
  {
    line = Instantiate(line, gameSteps[0].position, gameSteps[0].rotation);
    DrawSteps();  // Draws all the lines from step to step
  }

  // The line indicators from step to step the player has forward to
  private void DrawSteps()
  {
    line.gameObject.SetActive(true);
    line.numPositions = gameSteps.Length;

    var distance = Vector3.Distance(gameSteps[0].position, gameSteps[4].position);
    line.materials[0].mainTextureScale = new Vector3(distance, 1, 1);

    var steps = new Vector3[gameSteps.Length];
    int index = 0;
    foreach (Transform trans in gameSteps) {
      steps[index] = new Vector3(trans.position.x, trans.position.y, trans.position.z);
      index++;
    }
    line.SetPositions(steps);
  }

  // Hides the indicator
  public void HideStepsIndicator()
  {
    line.gameObject.SetActive(false);
  }
}
