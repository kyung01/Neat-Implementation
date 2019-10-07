using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Experiment : MonoBehaviour
{
	public enum EXPERIMENT_STATUS { INITIAL, BREEDING, COMPETING, EVOLVING}
	EXPERIMENT_STATUS status = EXPERIMENT_STATUS.INITIAL;
	readonly int SPECIES_OFFSPRING_COUNT = 10;

	Game game;
	List<Species> species = new List<Species>();
	// Use this for initialization
	void Start()
	{

	}
	Organism getBaseParent()
	{
		List<DNANode>
			nodeInput00 = new List<DNANode>(),
			nodeOutput00 = new List<DNANode>();
		nodeInput00.Add(new DNANode());
		for (int i = 0; i < 9; i++)
		{
			nodeInput00.Add(new DNANode());
			nodeInput00.Add(new DNANode());
			nodeOutput00.Add(new DNANode());

		}
		return new Organism(new List<DNAConnection>(), nodeInput00, nodeOutput00);
	}
	void updateInitial()
	{
		species.Add(new Species());
		species[0].setParent(getBaseParent(), getBaseParent());
	}
	void updateBreeding()
	{
		for(int i = 0; i < species.Count; i++)
		{
			for(int j = 0; j < SPECIES_OFFSPRING_COUNT; j++)
			{
				species[0].breed();
			}
		}
	}
	// Update is called once per frame
	void Update()
	{
		switch (status) {
			case EXPERIMENT_STATUS.INITIAL:
				updateInitial();
				status = EXPERIMENT_STATUS.BREEDING;
				break;
			case EXPERIMENT_STATUS.BREEDING:
				updateBreeding();
				break;
		}

	}
}
