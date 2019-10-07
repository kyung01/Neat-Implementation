using UnityEngine;
using System.Collections;

public class GlobalIDCounter
{
	static int id =0;
	public int get { get { return id++; } }

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
