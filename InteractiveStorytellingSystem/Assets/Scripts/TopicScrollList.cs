using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopicScrollList : MonoBehaviour 
{
	public GameObject topicPrefab;

	public void CreateTopics(MemoryPool mp)
	{
		RemoveAll();
		List<Node> keywords = mp.GetNodes();
		foreach(Node n in keywords)
		{
			GameObject topic = Instantiate(topicPrefab);
			topic.GetComponent<TopicButton>().Setup(n.Keyword);
			topic.transform.SetParent(transform);
		}
	}

	private void RemoveAll()
	{
		foreach (Transform child in transform) 
		{
     		GameObject.Destroy(child.gameObject);
 		}
	}
}
