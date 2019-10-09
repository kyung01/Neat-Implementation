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
	TicTacToeRenderer tictactoeRenderer;

	EXPERIMENT_STATUS status = EXPERIMENT_STATUS.INITIAL;
	readonly int SPECIES_OFFSPRING_COUNT = 100;

	Game game;
	int gamePlayerAIndex, gamePlayerBIndex;
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

	int selectedSpecies = 0;
	int selectedPlayerA = 0;
	int selectedPlayerB = 1;
	void updateCompeting()
	{
		if(game == null)
		{
			//createa a new game
			game = new Game(species[selectedSpecies].offsprings[selectedPlayerA], species[selectedSpecies].offsprings[selectedPlayerB]);
			return;
		}
		//play the game
		game.play();
		tictactoeRenderer.render(game.ticTacToe);
		if (!game.isGamePlayable())
		{
			//game is no longer playable
			game = null;
			//get reay the next players 
			if (selectedPlayerB >= species[selectedSpecies].offsprings.Count)
			{
				//we ran out of playerB therefore now increment playerA
				//check if playerAindex as well reached the maximum
				if(selectedPlayerA+1 >= species[selectedSpecies].offsprings.Count)
				{
					//yes it did indeed, then step onto the next species but check if we reached max species we have
					if(selectedSpecies +1 >= species.Count)
					{
						//yes we did indeed... we need to stop competing and start evolving
						status = EXPERIMENT_STATUS.EVOLVING;
					}
					else
					{
						selectedSpecies++;
						selectedPlayerA = 0;
						selectedPlayerB = 1;
					}
				}
				else
				{
					//no player A index is not reached to the end, so we can continue competition
					selectedPlayerA++;
					selectedPlayerB = selectedPlayerA + 1;
				}
			}
			else
			{
				selectedPlayerB++;

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
				//prepare for competition
				selectedSpecies = 0;
				selectedPlayerA = 0;
				selectedPlayerB = 1;
				game = null;
				status = EXPERIMENT_STATUS.COMPETING;
				break;
			case EXPERIMENT_STATUS.COMPETING:
				updateCompeting();
				break;
		}

	}
}
