using UnityEngine;
using System.Collections;

public class GlobalIDCounter
{
	static int id =0;
	static public int NewID { get { return id++; } }
	static public void UndoNewID() { id--; }

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
