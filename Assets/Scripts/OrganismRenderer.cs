using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismRenderer : MonoBehaviour
{
	[SerializeField]
	SpriteRenderer PREFAB_NODE;
	[SerializeField]
	NodeRenderer PREFAB_NODE_RENDERER;
	[SerializeField]
	KLineRenderer PREFAB_LINE_RENDERER;
	// Start is called before the first frame update

	List<DNANode> allNodes = new List<DNANode>();
	List<SpriteRenderer> inputNodes = new List<SpriteRenderer>();
	List<SpriteRenderer> hiddenNodes = new List<SpriteRenderer>();
	List<SpriteRenderer> outputNodes = new List<SpriteRenderer>();
	
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	SpriteRenderer hprGetNode(float x, float y, float z)
	{
		var node = Instantiate(PREFAB_NODE).GetComponent<SpriteRenderer>();
		node.transform.parent = this.transform;
		node.transform.localPosition = new Vector3(x,y,z);
		return node;
	}
	public void render(Organism organism)
	{
		allNodes.Clear();
		for (int i = 0; i < organism.HiddenNodeCount; i++)
		{
			int x = i % 10;
			int y = (int)(i /10);
			var hiddenNode = hprGetNode(4 + x, y, 0);
			hiddenNode.color = Color.green;
			hiddenNodes.Add(hiddenNode);
		}
		if(inputNodes.Count == 0)
		{
			var initialNode = hprGetNode(0, 3, 0);
			inputNodes.Add(initialNode);
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
		
	}
}
