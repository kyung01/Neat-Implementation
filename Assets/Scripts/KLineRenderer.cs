using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KLineRenderer : MonoBehaviour 
{
	public Transform objectLineFrom,objectLineTo;
	LineRenderer lineRenderer;
	// Start is called before the first frame update

	private void Awake()
	{
		this.lineRenderer = GetComponent<LineRenderer>();
		if (objectLineFrom == null) objectLineFrom = this.transform;
	}
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (objectLineTo.gameObject == null || objectLineTo.gameObject.active == false) return;
		lineRenderer.SetPosition(0, objectLineFrom.position);
		lineRenderer.SetPosition(1, objectLineTo.position);
	}
}
