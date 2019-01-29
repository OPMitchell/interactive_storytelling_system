using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OpenDialog : MonoBehaviour 
{
  	Ray ray;
    RaycastHit hit;
	GameObject dialogUI;
	GameObject characterNameUI;
     
	void Start()
	{
		dialogUI = GameObject.Find("DialogUI");
		dialogUI.SetActive(false);
		characterNameUI = GameObject.Find("CharacterNameUI");
		characterNameUI.SetActive(false);
	}

    void Update()
    {
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit))
		{
			if(hit.collider.tag == "Character")
			{
				if(CheckIfPlayerWithinRange(hit.collider.transform.position, 2.75f))
				{
					if(!dialogUI.activeInHierarchy)
						SetCharacterNameUIName(hit.collider.transform.name);
					else
						characterNameUI.SetActive(false);
					if(Input.GetKeyDown(KeyCode.E))
					{
							ToggleDialogUI();
					}
				}
			}
			else
			{
				characterNameUI.SetActive (false);
				if(dialogUI.activeInHierarchy)
				{
					if(Input.GetKeyDown(KeyCode.E))
						CloseDialogUI();
				}
			}
		}
    }

	void SetCharacterNameUIName(string name)
	{
		characterNameUI.SetActive(true);
		characterNameUI.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
	}

	void ToggleDialogUI()
	{
		if(dialogUI.activeInHierarchy)
			CloseDialogUI();
		else
			ShowDialogUI();
	}

	void ShowDialogUI()
	{
		dialogUI.SetActive (true);
		GetComponent<FirstPersonController>().enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	void CloseDialogUI()
	{
		dialogUI.SetActive (false);
		GetComponent<FirstPersonController>().enabled = true;
	}

	bool CheckIfPlayerWithinRange(Vector3 obj, float range)
	{
		return Vector3.Distance(transform.position, obj) < range;
	}

}
