using UnityEngine;
using System.Collections;


public class TicTacToeBoard {
	public enum SLOT_STATUS { EMPTY, X, O };
	public SLOT_STATUS[] slots;
	public TicTacToeBoard(){
		slots = new SLOT_STATUS[9];
	}
	public bool mark(SLOT_STATUS marker, int slotNumber)
	{
		if (marker == SLOT_STATUS.X) return markX(slotNumber);
		else if (marker == SLOT_STATUS.O) return markO(slotNumber);
		return false;
	}
	public bool markX(int slotNumber)
	{
		if (slots[slotNumber] != SLOT_STATUS.EMPTY) return false;
		slots[slotNumber] = SLOT_STATUS.X;
		return true;
	}
	public bool markO(int slotNumber)
	{
		if (slots[slotNumber] != SLOT_STATUS.EMPTY) return false;
		slots[slotNumber] = SLOT_STATUS.O;
		return true;
	}
	bool checkGameWonBy(SLOT_STATUS marker)
	{
		bool matchFound = false;
		for(int i = 0; i < 3; i++)
		{
			bool horizontalMatch = true;
			for (int j = 0;  j < 3; j++)
			{
				if (slots[i * 3 + j] != marker) { 
					horizontalMatch = false;
					break;
				}
			}
			if (horizontalMatch)
			{
				matchFound = true;
				break;
			}
		}
		if (matchFound) { Debug.Log("Horizontal"); return true; }

		for (int i = 0; i < 3; i++)
		{
			bool verticalMatch = true;
			for (int j = 0; j < 3; j++)
			{
				if (slots[j * 3 + i] != marker)
				{
					verticalMatch = false;
					break;
				}
			}
			if (verticalMatch)
			{
				matchFound = true;
				break;
			}
		}
		if (matchFound) { Debug.Log("Vertical"); return true; }

		bool digonalMatchLeft = true;
		for (int i = 0; i < 3; i++)
		{
			if (slots[i * 3 + i] != marker)
			{
				digonalMatchLeft = false;
				break;
			}
			
		}
		if (digonalMatchLeft) { Debug.Log("digonalMatchLeft"); return true; }

		bool digonalMatchRight = true;
		for (int i = 0; i < 3; i++)
		{
			if (slots[i * 3 + (2-i)] != marker)
			{
				digonalMatchRight = false;
				break;
			}
		}
		if (digonalMatchRight) { Debug.Log("Other Digonal"); return true; }
		return false;
	}
	public bool isGameWon()
	{
		if (checkGameWonBy(SLOT_STATUS.O))
		{
			Debug.Log("O Won");
		}
		else if (checkGameWonBy(SLOT_STATUS.X))
		{
			Debug.Log("X Won");
		}
		return false;
	}
	public bool isGamePlayable()
	{
		if (isGameWon()) return true;
		for (int i = 0; i < 9; i++) if (slots[i] == SLOT_STATUS.EMPTY) return true;
		return false;
	}
}

public class Game 
{
	bool debug = false;
	//game of tic tac toe 9 slots with 2 different input types and one for "ongoing" status
	int inputNumber = 9 * 2 + 1;
	int outputNumber = 9; //9 slots

	int currentPlayerIndex = 0;

	public TicTacToeBoard ticTacToe = new TicTacToeBoard();
	bool isOrganismMadeInvalidOutput = false;
	float gameFitness = 0;

	Organism playerA, playerB;
	public Game(Organism playerA, Organism playerB)
	{
		this.playerA = playerA;
		this.playerB = playerB;
	}

	public void play()
	{
		Organism currentPlayer;
		TicTacToeBoard.SLOT_STATUS marker;

		if (currentPlayerIndex++ % 2 == 0) {
			currentPlayer = playerA;
			marker = TicTacToeBoard.SLOT_STATUS.X;
		}
		else { 
			currentPlayer = playerB;
			marker = TicTacToeBoard.SLOT_STATUS.O;
		}
		currentPlayer.activateInput(0, 1);
		for (int i = 0; i < 9; i++)
		{
			if (ticTacToe.slots[i] == TicTacToeBoard.SLOT_STATUS.X)
				currentPlayer.activateInput(1 + i, 1);
			else if (ticTacToe.slots[i] == TicTacToeBoard.SLOT_STATUS.O)
				currentPlayer.activateInput(1+9 + i, 1);
		}
		int outputNumber = currentPlayer.getOutput();
		if (debug) Debug.Log("Game Output: " + outputNumber);
		if (debug && outputNumber != -1 && outputNumber != 0)
			Debug.Log("Not 0 output " + outputNumber);
		
		bool outputSuccessfullyProcessed = (outputNumber==-1)? false:ticTacToe.mark(marker, outputNumber);
		if (debug) Debug.Log("Game outputSuccessfullyProcessed: " + outputSuccessfullyProcessed);
		if (outputSuccessfullyProcessed) currentPlayer.addIndividualFitness(1);
		else {
			currentPlayer.addIndividualFitness(-0.1f);
			isOrganismMadeInvalidOutput = true;
		}
		if (ticTacToe.isGameWon()) { Debug.Log("Wow Game Won"); currentPlayer.addIndividualFitness(100); }	
	}
	public bool isGamePlayable()
	{
		return !isOrganismMadeInvalidOutput && ticTacToe.isGamePlayable();
	}
	
}
