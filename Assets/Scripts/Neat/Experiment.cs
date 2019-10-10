using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Experiment : MonoBehaviour
{
	public enum EXPERIMENT_STATUS { INITIAL, BREEDING, COMPETING, EVOLVING}
	[SerializeField]
	OrganismRenderer PREFAB_ORGANISM_RENDERER;
	[SerializeField]
	Text textPopulationNumber;
	[SerializeField]
	Text selectedPlayerNumber;
	[SerializeField]
	TicTacToeRenderer tictactoeRenderer;

	EXPERIMENT_STATUS status = EXPERIMENT_STATUS.INITIAL;
	readonly int SPECIES_OFFSPRING_COUNT = 10;

	Game game;
	bool hasPlayedBackward = false;
	int gamePlayerAIndex, gamePlayerBIndex;

	OrganismRenderer playerARenderer, playerBRenderer;
	List<Species> species = new List<Species>();
	// Use this for initialization
	void Start()
	{
		Application.targetFrameRate = 300;
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

	int selectedSpecies = 0;
	int selectedPlayerA = 0;
	int selectedPlayerB = 1;

	void updateCompeting()
	{
		//start coroutine
	}
	IEnumerator Competition()
	{
		for (int speciesIndex = 0; speciesIndex < species.Count; speciesIndex++)
		{
			for (int organismAIndex = 0; organismAIndex < species[speciesIndex].Population; organismAIndex++)
				for(int organismBIndex = organismAIndex+1; organismBIndex < species[speciesIndex].Population; organismBIndex++)
				{
					for(int gameNumber  = 0; gameNumber<2; gameNumber++)
					{
						selectedPlayerNumber.text = "Species " + speciesIndex + "( "+organismAIndex +" vs " + organismBIndex + " )";
						Game game;
						if (gameNumber == 0)
						{
							game = new Game(species[speciesIndex].offsprings[organismAIndex], species[speciesIndex].offsprings[organismBIndex]);
						}
						else {
							game = new Game(species[speciesIndex].offsprings[organismBIndex], species[speciesIndex].offsprings[organismAIndex]);
						}
						while (game.isGamePlayable())
						{
							game.play();
							tictactoeRenderer.render(game.ticTacToe);
							yield return null;
						}
						
						
					}
				}
			
		}
		Debug.Log("Competition FInished");
	
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
				//prepare for competition
				selectedSpecies = 0;
				selectedPlayerA = 0;
				selectedPlayerB = 1;
				game = null;
				hasPlayedBackward = false;
				StartCoroutine("Competition");
				status = EXPERIMENT_STATUS.COMPETING;
				break;
			case EXPERIMENT_STATUS.COMPETING:
				updateCompeting();
				break;
		}

	}
}
