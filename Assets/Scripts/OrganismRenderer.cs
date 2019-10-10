using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganismRenderer : MonoBehaviour
{
	[SerializeField]
	SpriteRenderer PREFAB_NODE;
	[SerializeField]
	KLineRenderer PREFAB_LINE_RENDERER;
	// Start is called before the first frame update

	List<SpriteRenderer> inputNodes = new List<SpriteRenderer>();
	List<SpriteRenderer> outputNodes = new List<SpriteRenderer>();

	GameObject outputNodeGroups = null;

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void render(Organism organism)
	{
		if(inputNodes.Count == 0)
		{
			var initialNode = Instantiate(PREFAB_NODE).GetComponent<SpriteRenderer>();
			initialNode.transform.parent = this.transform;
			initialNode.transform.localPosition = new Vector3(0, 3, 0);
			inputNodes.Add(initialNode);
			for (int i = 0; i < 2; i++)
			{
				for (int y = 0; y < 3; y++)
					for (int x = 0; x < 3; x++)
					{
						var node = Instantiate(PREFAB_NODE).GetComponent<SpriteRenderer>();
						node.transform.parent = this.transform;
						node.transform.localPosition = new Vector3(3 * i + x, y, 0);
						inputNodes.Add(node);
					}
			}

		}
		if(outputNodeGroups == null)
		{
			outputNodeGroups = new GameObject();
			outputNodeGroups.transform.parent = this.transform;
			outputNodeGroups.transform.localPosition = new Vector3(15, 0, 0);
			for (int y = 0; y < 3; y++)
			{
				for (int x = 0; x < 3; x++)
				{
					var node = Instantiate(PREFAB_NODE).GetComponent<SpriteRenderer>();
					node.transform.parent = outputNodeGroups.transform;
					node.transform.localPosition = new Vector3(x, y, 0);
					outputNodes.Add(node);
				}
			}
				
		}
	}
}
