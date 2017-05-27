using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPointManager : MonoBehaviour {
  public LineRenderer line;
  public GameObject[] transformPoints;
  private Vector3 point1;
  private Vector3 point2;

	// Use this for initialization
	void Start () {
    transformPoints[0].SetActive(true);
    transformPoints[1].SetActive(false);
    point1 = transformPoints[0].transform.position;
    line = Instantiate(line, transformPoints[0].transform.position, transformPoints[0].transform.rotation);
    line.gameObject.SetActive(true);
  }
	
	// Update is called once per frame
	void Update () {
		if(transformPoints[1].activeSelf) {
      // Checks if the origin point of the transform has changed
      if(transformPoints[0].activeSelf && transformPoints[0].transform.position != point1) {
        point1 = transformPoints[0].transform.position;
        DrawTransformLine();
      }
      // Checks if the second point of the transform has changed
      if (transformPoints[1].activeSelf && transformPoints[1].transform.position != point2) {
        point2 = transformPoints[0].transform.position;
        DrawTransformLine();
      }
    }
	}

  private void DrawTransformLine()
  {

  }
}
