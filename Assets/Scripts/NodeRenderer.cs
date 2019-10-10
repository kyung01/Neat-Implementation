using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRenderer : MonoBehaviour
{
	[SerializeField]
	KLineRenderer lineRenderer;
	public delegate NodeRenderer DelGetNodeRenderer(int index);

	DNANode node;
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

	}
	public void render(DNANode node, DelGetNodeRenderer search)
	{
			
	}
}
