using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicButton : MonoBehaviour 
{
	private Button button;
	private Text topicName;

	public void Setup(string name)
	{
		button = GetComponent<Button>();
		topicName = GetComponentInChildren<UnityEngine.UI.Text>();
		button.onClick.AddListener(() => ButtonClicked());
		topicName.text = name;
	}

	void ButtonClicked()
    {
		if(DialogUIManager.GetTargetMemoryManager() != null)
		{
			MemoryManager mem = DialogUIManager.GetTargetMemoryManager();
			MemoryPattern memory = mem.RetrieveMemoryPattern(topicName.text);
			GameObject.Find("Dialogue").GetComponentInChildren<UnityEngine.UI.Text>().text = memory.Description;
		}
    }
}
