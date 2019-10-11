using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismRenderer : MonoBehaviour
{
	[SerializeField]
	NodeRenderer PREFAB_NODE_RENDERER;
	[SerializeField]
	KLineRenderer PREFAB_LINE_RENDERER;
	// Start is called before the first frame update

	List<NodeRenderer> allNodes = new List<NodeRenderer>();
	List<NodeRenderer> inputNodes = new List<NodeRenderer>();
	List<NodeRenderer> hiddenNodes = new List<NodeRenderer>();
	List<NodeRenderer> outputNodes = new List<NodeRenderer>();
	
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	NodeRenderer hprGetNode(float x, float y, float z)
	{
		var node = Instantiate(PREFAB_NODE_RENDERER).GetComponent<NodeRenderer>();
		node.transform.parent = this.transform;
		node.transform.localPosition = new Vector3(x,y,z);
		return node;
	}
	NodeRenderer searchNodeRenderer(int index)
	{
		Debug.Log("index" + index + " , " + allNodes.Count);
		return allNodes[index];
	}
	public void render(Organism organism)
	{
		Debug.Log("ORganism with nodes " + organism.NodeCount);
		allNodes.Clear();
		foreach(var node in hiddenNodes)
		{
			GameObject.Destroy(node.gameObject);
		}
		hiddenNodes.Clear();


		for (int i = 0; i < organism.HiddenNodeCount; i++)
		{
			hiddenNodes.Add(hprGetNode(7 + i % 10, (int)(i / 10), 0));
		}
		if(inputNodes.Count == 0)
		{
			inputNodes.Add(hprGetNode(0, 3, 0));
			for (int i = 0; i < 2; i++)
			{
				for (int y = 0; y < 3; y++)
					for (int x = 0; x < 3; x++)
					{
						inputNodes.Add(hprGetNode(3 * i + x, y, 0));
					}
			}

		}
		if(outputNodes.Count== 0)
		{
			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					outputNodes.Add(hprGetNode(15 + x, y, 0));
				}
			}

		}
		foreach (var node in inputNodes)	allNodes.Add(node);
		foreach (var node in outputNodes)	allNodes.Add(node);
		foreach (var node in hiddenNodes)	allNodes.Add(node);
		for (int i = 0; i < allNodes.Count; i++)
			allNodes[i].reset();
		for(int i = 0; i < organism.NodeCount; i++)
		{
			allNodes[i].render(organism.getNode(i), searchNodeRenderer);
		}

	}
}
