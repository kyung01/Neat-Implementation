using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Experiment : MonoBehaviour
{
	public enum EXPERIMENT_STATUS { INITIAL, BREEDING, COMPETING, EVOLVING}
	[SerializeField]
	Text textPopulationNumber;
	[SerializeField]
	OrganismRenderer PREFAB_ORGANISM_RENDERER;

	EXPERIMENT_STATUS status = EXPERIMENT_STATUS.INITIAL;
	readonly int SPECIES_OFFSPRING_COUNT = 100;

	Game game;
	OrganismRenderer playerARenderer, playerBRenderer;
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
				species[i].breed(getBaseParent());
			}
		}
	}
	// Update is called once per frame
	void Update()
	{
		if(species.Count!= 0)textPopulationNumber.text = "Population : " + species[0].Population;
		switch (status) {
			case EXPERIMENT_STATUS.INITIAL:
				updateInitial();
				status = EXPERIMENT_STATUS.BREEDING;
				break;
			case EXPERIMENT_STATUS.BREEDING:
				updateBreeding();
				status = EXPERIMENT_STATUS.COMPETING;
				break;
			case EXPERIMENT_STATUS.COMPETING:
				break;
		}

	}
}
