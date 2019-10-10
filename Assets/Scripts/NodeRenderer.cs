using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : MonoBehaviour
{
	[SerializeField]
	KLineRenderer PREFAB_LINE_RENDERER;
	public delegate NodeRenderer DelGetNodeRenderer(int index);


	DNANode node;
	List<KLineRenderer> connections = new List<KLineRenderer>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	/// <summary>
	/// resets all the connections
	/// </summary>
	public void reset()
	{
		for(int i = 0; i < connections.Count; i++)
		{
			GameObject.Destroy(connections[i].gameObject);
		}
		connections.Clear();
	}
	void addNewConnection(NodeRenderer endNode)
	{
		var connection = Instantiate(PREFAB_LINE_RENDERER).GetComponent<KLineRenderer>();
		connection.transform.parent = this.transform;
		connection.transform.localPosition = new Vector3(0, 0, 1);
		connection.set(this.transform, endNode.transform);
		this.connections.Add(connection);
	}
	public void render(DNANode node, DelGetNodeRenderer search)
	{
		this.node = node;
		foreach(var connection in node.connections)
		{
			var end = search(connection.to);
			addNewConnection(end);

		}
			
	}
}
