﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAConnection {
	public int id = -1;
	public int from =-1, to=-1;
	public float weight = 1.0f;
	public bool activated = true;
	public DNAConnection()
	{

	}
	public DNAConnection(int id, int from, int to)
	{
		this.id = id;
		this.from = from;
		this.to = to;
	}
	public string toString()
	{
		return id + ((activated)?"T":"F") +weight+ "(" + from + "->" + to + ")";
	}

}

public class DNANode
{
	public delegate DNANode DEL_NODE_SEARCHING_FUNCTION(int nodeid);
	public delegate void DEL_NODE_ACTIVATED(float power);
	public List<DNAConnection> connections = new List<DNAConnection>();
	public List<DEL_NODE_ACTIVATED> handleActivation = new List<DEL_NODE_ACTIVATED>();

	public virtual void activate(float signal, DEL_NODE_SEARCHING_FUNCTION function, int iterationLevel = 0)
	{
		if(iterationLevel > 10)
		{
			return;
		}
		for(int i = 0; i < handleActivation.Count; i++)
		{
			handleActivation[i](signal);
		}
		for(int i = 0; i< connections.Count; i++)
		{
			if (!connections[i].activated) continue;
			//Debug.Log("Activating " + connections[i].to);
			function(connections[i].to).activate(signal * connections[i].weight, function, iterationLevel+1);
		}
	}

	public bool isConnectedTo(int nodeNumber)
	{
		for(int i = 0; i< connections.Count; i++)
		{
			if (connections[i].id == nodeNumber) return true;
		}
		return false;
	}
}
public class DNANodeOutput:DNANode
{
	public float accumulatedSignal;
	public override void activate(float signal, DEL_NODE_SEARCHING_FUNCTION function, int iterationLevel = 0)
	{
		accumulatedSignal += signal;
		base.activate(signal, function, iterationLevel);
	}
}



public class MutationInformation {
	public enum MUTATION_TYPE { NONE, CREATE_CONNECTION, CREATE_NODE }

	public MUTATION_TYPE type = MUTATION_TYPE.NONE;
	/// <summary>
	/// ID of the mutation
	/// </summary>
	public int id = 0;
	/// <summary>
	/// in case it is create connection, a secondary id is required for the outgoing connection dna
	/// </summary>
	public int id2 = 0;

	/// <summary>
	/// if conneciton mutation is selected then the node that's being splited is recorded
	/// </summary>
	public int splittedConnection =-1;
	/// <summary>
	/// if create connection is selected then the nodes that are being connected are recorded 
	/// </summary>
	public int connectedNodeFrom = -1, connectedNodeTo =1;

	public MutationInformation()
	{

	}
	/// <summary>
	/// If it is create a node "a" is indicates splitted node's id "b" indicated second id of the mutation... yes it is confusing
	/// </summary>
	/// <param name="id"></param>
	/// <param name="type"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	public MutationInformation(int id, MUTATION_TYPE type, int a, int b = 0)
	{
		this.id = id;
		this.type = type;
		if(type == MUTATION_TYPE.CREATE_NODE)
		{
			this.splittedConnection = a;
		}else if(type == MUTATION_TYPE.CREATE_CONNECTION)
		{
			this.connectedNodeFrom = a;
			this.connectedNodeTo = b;
		}
	}

	/*
	 * 
	 * public static bool IS_SAME(MutationInformation a, MutationInformation b)
	{
		return a.type == b.type && a.connectionDNAID == b.connectionDNAID && a.nodeFrom == b.nodeFrom && a.nodeTO == b.nodeTO;
	}
	 */
}


public class Species 
{
	class MarkedDNA {
		public int id;
		public DNAConnection DNA;
		public MarkedDNA(DNAConnection dna, int id)
		{
			this.DNA = dna;
			this.id = dna.id;
		}

	}

	class DNAConnectionMatch
	{
		public DNAConnection
			A,
			B;
		public DNAConnectionMatch(DNAConnection A, DNAConnection B)
		{
			this.A = A;
			this.B = B;
		}
		static public DNAConnectionMatch ConvertMarkedDNA(MarkedDNA markedDNA)
		{
			if (markedDNA.id == 0) return new DNAConnectionMatch(markedDNA.DNA, null);
			else return new DNAConnectionMatch(null, markedDNA.DNA);
		}
	}

	public int Population { get { return offsprings.Count; } }

	List<Organism> parent = new List<Organism>();
	List<DNAConnectionMatch> parentDNAMatches = new List<DNAConnectionMatch>();

	public List<Organism> offsprings = new List<Organism>();
	List<MutationInformation> mutationHistory = new List<MutationInformation>();



	int hprComapreMarkedDNAs(MarkedDNA a, MarkedDNA b)
	{
		if (a.DNA.id == b.DNA.id)
		{
			return (a.id < b.id) ? -1 : 1;
		}
		else if (a.DNA.id < b.DNA.id)
		{
			return -1;
		}
		else return 1;
	}
	public void setParent(Organism X, Organism Y)
	{
		parent.Clear();
		parent.Add(X);
		parent.Add(Y);
		parentDNAMatches.Clear();
		//update dna matches
		List<MarkedDNA> markedDNAs = new List<MarkedDNA>();
		foreach (var dna in X.dnas) markedDNAs.Add(new MarkedDNA(dna, 0));
		foreach (var dna in Y.dnas) markedDNAs.Add(new MarkedDNA(dna, 1));
		markedDNAs.Sort(hprComapreMarkedDNAs);
		for(int i = 0; i < markedDNAs.Count; )
		{
			var currentDNA = markedDNAs[i];
			var nextDNA = (i + 1 < markedDNAs.Count) ? markedDNAs[i + 1] : null;
			//Debug.Log(currentDNA + " , " + nextDNA);
			//Debug.Log(i + " / " + markedDNAs.Count);
			if(nextDNA == null)
			{
				//Debug.Log("Single gene was found " + i + " , "+ markedDNAs.Count);
			}
			if(nextDNA!= null && currentDNA.id == nextDNA.id)
			{
				parentDNAMatches.Add(new DNAConnectionMatch(currentDNA.DNA, nextDNA.DNA));
				//Debug.Log("added a pair to the DNA pool");
				i += 2;
			}
			else
			{
				parentDNAMatches.Add(DNAConnectionMatch.ConvertMarkedDNA(currentDNA));
				//Debug.Log("added a single to the DNA pool");
				i += 1;
			}
		}
		for(int i = 0; i < parentDNAMatches.Count; i++)
		{
			//string a = (parentDNAMatches[i].A == null) ? "NONE" : parentDNAMatches[i].A.toString() + "(" + parentDNAMatches[i].A.id+")";
			//string b = (parentDNAMatches[i].B == null) ? "NONE" : parentDNAMatches[i].B.toString() + "(" + parentDNAMatches[i].B.id+")";
			//Debug.Log(i+" : "+a + " and " +b);
		}


	}
	public void breed(Organism unborn)
	{
		bool isEqualFitnessParent = Mathf.Sqrt(parent[0].EvaluatedFitness - parent[1].EvaluatedFitness) < 0.01f;
		bool isABetterParent = parent[0].EvaluatedFitness > parent[1].EvaluatedFitness;
		for(int i = 0; i < parentDNAMatches.Count; i++)
		{
			var dnaPair = parentDNAMatches[i];
			if(dnaPair.A != null && dnaPair.B!= null)
			{
				//both genes exist
				if (isEqualFitnessParent) {
					if (Random.RandomRange(0, 2) == 0)
					{
						unborn.addDNA(dnaPair.A);
					}
					else
						unborn.addDNA(dnaPair.B);
				}
				else
				{
					if (isABetterParent)
						unborn.addDNA(dnaPair.A);
					else
						unborn.addDNA(dnaPair.B);
				}
			}
			else if( dnaPair.A != null && dnaPair.B == null)
			{
				//only A gene exist
				if (isEqualFitnessParent)
				{
					if (Random.RandomRange(0, 2) == 0)
					{
						unborn.addDNA(dnaPair.A);
					}
					else
					{

					}
				}
				else
				{
					if (isABetterParent)
						unborn.addDNA(dnaPair.A);
					else
					{

					}
				}
			}
			else if( dnaPair.A == null && dnaPair.B != null)
			{
				//only B gene exist
				if (isEqualFitnessParent)
				{
					if (Random.RandomRange(0, 2) == 0)
					{
						unborn.dnas.Add(dnaPair.B);
					}
					else
					{

					}
				}
				else
				{
					if (isABetterParent)
						unborn.dnas.Add(dnaPair.B);
					else
					{

					}
				}
			}
			else
			{
				//No gene exist?
				Debug.Log("WHAT?");
			}
		}
		for(int i = 0; i< unborn.dnas.Count; i++)
		{
			var connectionAKADNA = unborn.dnas[i];
			unborn.getNode(unborn.dnas[i].from).connections.Add(connectionAKADNA);
		}


		addMutation(unborn);

		this.offsprings.Add(unborn);
	}

	void addMutation(Organism organism)
	{

		float selectedMutation = Random.Range(0, 1.0f);
		bool isNewMutation = true;

		if (selectedMutation < 0.51f)
		{
			//Debug.Log("Mutation : Create a new connection");
			//10% chance to create a new connection gean
			int first, second;
			int whileLoopFailSafe = 100;
			do
			{
				//input-hidden to hidden-output
				first =  Random.Range(0, organism.InputNodeCount + organism.HiddenNodeCount);
				if (first >= organism.InputNodeCount) first += organism.OutputNodeCount;
				second = Random.Range(organism.InputNodeCount, organism.InputNodeCount+ organism.OutputNodeCount+ organism.HiddenNodeCount);
				if (whileLoopFailSafe-- <= 0) { Debug.Log("BREAK CALLED" + whileLoopFailSafe); break; }
			} while (
						//these are fail conditions if one of these is true continue the loop
						organism.getNode(first).isConnectedTo(second) ||
						(first < organism.InputNodeCount && second < organism.InputNodeCount) ||
						(first == second)
						);
			Debug.Log("second " + second +   " FROM " + organism.InputNodeCount +  "to  " +(organism.OutputNodeCount + organism.HiddenNodeCount));
			int mutationID = GlobalIDCounter.NewID;
			//check if this is not new mutation
			for (int i = 0; i < mutationHistory.Count; i++)
			{
				if (mutationHistory[i].type == MutationInformation.MUTATION_TYPE.CREATE_CONNECTION &&
					mutationHistory[i].connectedNodeFrom == first && mutationHistory[i].connectedNodeTo == second)
				{
					//found the mutation existing in the history
					isNewMutation = false;
					mutationID = mutationHistory[i].id;
					GlobalIDCounter.UndoNewID();
					break;
				}
			}

			DNAConnection dna = new DNAConnection(mutationID, first, second);
			//Debug.Log("Added DNA : " + dna.from + " to " + dna.to);
			organism.dnas.Add(dna);
			organism.getNode(first).connections.Add(dna);
			if (isNewMutation)
				mutationHistory.Add(new MutationInformation(mutationID, MutationInformation.MUTATION_TYPE.CREATE_CONNECTION, first, second));


		}
		else if (selectedMutation < 0.88f)
		{
			//10% chance to create a new node
			//check condition
			if (organism.dnas.Count == 0)
			{
				Debug.Log("Cannot create a new node");
				return;
			}
			Debug.Log("create a new node");
			DNAConnection dnaConnection;
			
				int selectedDnaIndex = Random.RandomRange(0, organism.dnas.Count);
				dnaConnection = organism.dnas[selectedDnaIndex];
			
				//check if the mutation exists
				MutationInformation mutationInformationFromHistory = null;
				foreach(var history in mutationHistory)
				{
					if(history.type == MutationInformation.MUTATION_TYPE.CREATE_NODE  && history.splittedConnection == selectedDnaIndex)
					{
						//yes indeed it is a mutation that happened before
						mutationInformationFromHistory = history;
						break;
					}
				}
				int idInward, idOutgoing;
				if(mutationInformationFromHistory!= null)
				{
					idInward = mutationInformationFromHistory.id;
					idOutgoing = mutationInformationFromHistory.id2;
				}
				else
				{
					idInward = GlobalIDCounter.NewID;
					idOutgoing = GlobalIDCounter.NewID;
				}
				//disable the current dna
				organism.disable(dnaConnection);
				int newNodeIndex = organism.getNewNodeIndex();
				DNAConnection dnaInward, dnaOutward;
				dnaInward = new DNAConnection(idInward, dnaConnection.from, newNodeIndex);
				dnaOutward = new DNAConnection(idOutgoing, newNodeIndex, dnaConnection.to);
				dnaOutward.weight = dnaConnection.weight;
				organism.addDNA(dnaInward);
				organism.addDNA(dnaOutward);
				Debug.Log("dnaInward " + " FROM " + dnaInward.from + "to  " + dnaInward.to);
				Debug.Log("dnaOutward " + " FROM " + dnaOutward.from + "to  " + dnaOutward.to);
			if (mutationInformationFromHistory == null)
			{
				//update the history
				mutationInformationFromHistory = new MutationInformation(idInward, MutationInformation.MUTATION_TYPE.CREATE_NODE, selectedDnaIndex, idOutgoing);
				mutationHistory.Add(mutationInformationFromHistory);
			}
			

		}
		else
		{
			//90% chance to alter weights
			if (organism.dnas.Count == 0) return;
			var dna = organism.dnas[Random.Range(0, organism.dnas.Count)];
			dna.weight = Random.Range(dna.weight - 0.1f, dna.weight + 0.1f);
			dna.weight = Mathf.Max(0, Mathf.Min(1.0f, dna.weight));
		}

	}

	

	void init()
	{

	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
