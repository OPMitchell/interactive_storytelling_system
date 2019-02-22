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
				string name = hit.collider.transform.name;
				if(CheckIfPlayerWithinRange(hit.collider.transform.position, 2.75f))
				{
					if(!dialogUI.activeInHierarchy)
						SetCharacterNameUIName(hit.collider.transform.name);
					else
						characterNameUI.SetActive(false);
					if(Input.GetKeyDown(KeyCode.E))
					{
							ToggleDialogUI(name);
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

	void SetDialogUIName(string name)
	{
		GameObject.Find("Name").GetComponentInChildren<UnityEngine.UI.Text>().text = name;
	}

	void ToggleDialogUI(string name)
	{
		if(dialogUI.activeInHierarchy)
			CloseDialogUI();
		else
			ShowDialogUI(name);
	}

	void ShowDialogUI(string name)
	{
		dialogUI.SetActive (true);
		SetDialogUIName(name);
		GetComponent<FirstPersonController>().enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		MemoryManager mem = GameManager.FindGameObject(name).GetComponent<MemoryManager>();
		DialogUIManager.SetTarget(mem);
		if(mem != null)
			GameObject.Find("Content").GetComponent<TopicScrollList>().CreateTopics(mem.memoryPool);
		GameObject.Find("Dialogue").GetComponent<UnityEngine.UI.Text>().text = "";
	}

	void CloseDialogUI()
	{
		dialogUI.SetActive (false);
		GetComponent<FirstPersonController>().enabled = true;
		DialogUIManager.SetTarget(null);
	}

	bool CheckIfPlayerWithinRange(Vector3 obj, float range)
	{
		return Vector3.Distance(transform.position, obj) < range;
	}

}
