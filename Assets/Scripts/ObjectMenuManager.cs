using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMenuManager : MonoBehaviour {

  public List<GameObject> objectPrefabList;  // The prefab list that contains the prefab objects
  private List<GameObject> objectList;       // The list that contains the menu objects
  private int currentMenuObjectIndex = 0;    // The current index of the menu object 

	// Use this for initialization
	void Start ()
  {
    objectList = new List<GameObject>();
    
    // Add every prefab to the list
		foreach(Transform child in transform) {
      objectList.Add(child.gameObject);
    }
	}
	
  // Shifts one menu item to the left
	public void ShiftToLeft()
  {
    objectList[currentMenuObjectIndex].SetActive(false);
    currentMenuObjectIndex--;

    if (currentMenuObjectIndex < 0)
      currentMenuObjectIndex = objectList.Count - 1;
    objectList[currentMenuObjectIndex].SetActive(true);
  }

  // Shifts one menu item to the right
  public void ShiftToRight()
  {
    objectList[currentMenuObjectIndex].SetActive(false);
    currentMenuObjectIndex++;

    if (currentMenuObjectIndex > objectList.Count-1)
      currentMenuObjectIndex = 0;
    objectList[currentMenuObjectIndex].SetActive(true);
  }

  // Spawns the selected menu item
  public void SpawnCurrentObject()
  {
    Instantiate(objectPrefabList[currentMenuObjectIndex], 
      objectList[currentMenuObjectIndex].transform.position, 
      objectList[currentMenuObjectIndex].transform.rotation);
  }
}
