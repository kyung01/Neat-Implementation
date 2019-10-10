using System.Collections;
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

}

public class DNANode
{
	public delegate DNANode DEL_NODE_SEARCHING_FUNCTION(int nodeid);
	public delegate void DEL_NODE_ACTIVATED(float power);
	public List<DNAConnection> connections = new List<DNAConnection>();
	public List<DEL_NODE_ACTIVATED> handleActivation = new List<DEL_NODE_ACTIVATED>();

	public virtual void activate(float signal, DEL_NODE_SEARCHING_FUNCTION function)
	{
		for(int i = 0; i < handleActivation.Count; i++)
		{
			handleActivation[i](signal);
		}
		for(int i = 0; i< connections.Count; i++)
		{
			if (!connections[i].activated) continue;
			function(connections[i].to).activate(signal * connections[i].weight, function);
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
	public override void activate(float signal, DEL_NODE_SEARCHING_FUNCTION function)
	{
		accumulatedSignal += signal;
		base.activate(signal, function);
	}
}



public class MutationInformation {
	public enum MUTATION_TYPE { NONE, CREATE_CONNECTION, CREATE_NODE }

	public MUTATION_TYPE type = MUTATION_TYPE.NONE;
	public int id = 0;
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
			this.id = id;
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
			if(nextDNA!= null && currentDNA.id == nextDNA.id)
			{
				parentDNAMatches.Add(new DNAConnectionMatch(currentDNA.DNA, nextDNA.DNA));
				i += 2;
			}
			else
			{
				parentDNAMatches.Add(DNAConnectionMatch.ConvertMarkedDNA(currentDNA));
				i += 1;
			}
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
						unborn.dnas.Add(dnaPair.A);
					}
					else
						unborn.dnas.Add(dnaPair.B);
				}
				else
				{
					if (isABetterParent)
						unborn.dnas.Add(dnaPair.A);
					else
						unborn.dnas.Add(dnaPair.B);
				}
			}
			else if( dnaPair.A != null && dnaPair.B == null)
			{
				//only A gene exist
				if (isEqualFitnessParent)
				{
					if (Random.RandomRange(0, 2) == 0)
					{
						unborn.dnas.Add(dnaPair.A);
					}
					else
					{

					}
				}
				else
				{
					if (isABetterParent)
						unborn.dnas.Add(dnaPair.A);
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



		float selectedMutation = Random.Range(0, 1.0f);

		if (selectedMutation < 0.2f)
		{
			//Debug.Log("Mutation : Create a new connection");
			//10% chance to create a new connection gean
			int first, second;
			int whileLoopFailSafe = 100;
			do
			{
				first = Random.Range(0, unborn.InputNodeCount + unborn.HiddenNodeCount);
				second = Random.Range(unborn.InputNodeCount + unborn.HiddenNodeCount, unborn.NodeCount);
			} while (whileLoopFailSafe-- > 0 && !unborn.getNode(first).isConnectedTo(second));
			if (whileLoopFailSafe == 0)
			{
				Debug.Log("Fail safe is triggered");
				return;
			}
			int mutationID = GlobalIDCounter.NewID;
			bool isNewMutation = true;
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
			unborn.dnas.Add(dna);
			unborn.getNode(first).connections.Add(dna);
			if (isNewMutation)
				mutationHistory.Add(new MutationInformation(mutationID, MutationInformation.MUTATION_TYPE.CREATE_CONNECTION, first, second));


		}
		else if (selectedMutation < 0.18f)
		{
			//10% chance to create a new node

		}
		else
		{
			//90% chance to alter weights

		}

		this.offsprings.Add(unborn);
	}

	public void kBreed(Organism unborn)
	{
		//first 
		bool isUnequalParents = false;
		bool isADominating = false;
		List<DNAConnection> offspringDNA = new List<DNAConnection>();

		if (parent[0].EvaluatedFitness == parent[1].EvaluatedFitness)
		{
			//equal parents
			for(int i = 0; i< parentDNAMatches.Count; i++)
			{
				if (Random.RandomRange(0, 2) == 0)
				{
					//first 
					offspringDNA.Add(parentDNAMatches[i].A);
				}
				else
				{
					//second
					offspringDNA.Add(parentDNAMatches[i].B);
				}
			}
		}
		else if(parent[0].EvaluatedFitness > parent[1].EvaluatedFitness)
		{
			//parent 0 dominated fitness
			isUnequalParents = true;
			isADominating = true;
		}
		else
		{
			isUnequalParents = true;
			//parent 1 dominated fitness
		}
		if (isUnequalParents)
		{
			for(int i = 0; i < parentDNAMatches.Count; i++)
			{
				bool disjointed = false;
				if (parentDNAMatches[i].A == null || parentDNAMatches[i].B == null)
					disjointed = true;
				if (!disjointed)
				{
					offspringDNA.Add(parentDNAMatches[i].A);
				}
				else
				{
					if (isADominating)
					{
						if(parentDNAMatches[i].A!= null) offspringDNA.Add(parentDNAMatches[i].A);
					}
					else
					{
						if (parentDNAMatches[i].B != null) offspringDNA.Add(parentDNAMatches[i].B);
					}
				}
			}
		}
		unborn.dnas = offspringDNA;
		for(int i = 0; i < offspringDNA.Count; i++)
		{
			var dna = offspringDNA[i];
			var kNode = unborn.getNode(dna.from);
			kNode.connections.Add(dna);
			unborn.getNode(dna.to);
		}


		float selectedMutation = Random.Range(0, 1.0f);

		if (selectedMutation < 0.2f)
		{
			Debug.Log("Mutation : Create a new connection");
			//10% chance to create a new connection gean
			int first, second;
			int whileLoopFailSafe = 100;
			do
			{
				first = Random.Range(0, unborn.InputNodeCount + unborn.HiddenNodeCount);
				second = Random.Range(unborn.InputNodeCount + unborn.HiddenNodeCount, unborn.NodeCount);
			} while (whileLoopFailSafe-- > 0 && !unborn.getNode(first).isConnectedTo(second));
			if (whileLoopFailSafe == 0)
			{
				Debug.Log("Fail safe is triggered");
				return;
			}
			int mutationID = GlobalIDCounter.NewID;
			bool isNewMutation = true;
			//check if this is not new mutation
			for(int i = 0; i < mutationHistory.Count; i++)
			{
				if(mutationHistory[i].type == MutationInformation.MUTATION_TYPE.CREATE_CONNECTION &&
					mutationHistory[i].connectedNodeFrom == first && mutationHistory[i].connectedNodeTo == second)
				{
					//found the mutation existing in the history
					isNewMutation = false;
					mutationID = mutationHistory[i].id;
					GlobalIDCounter.UndoNewID();
					break;
				}
			}

			DNAConnection dna = new DNAConnection(mutationID,first,second);
			Debug.Log("Added DNA : " + dna.from + " to "+ dna.to);
			unborn.dnas.Add(dna);
			unborn.getNode(first).connections.Add(dna);
			if(isNewMutation)
				mutationHistory.Add(new MutationInformation(mutationID, MutationInformation.MUTATION_TYPE.CREATE_CONNECTION, first, second));


		}
		else if(selectedMutation < 0.18f)
		{
			//10% chance to create a new node

		}
		else
		{
			//90% chance to alter weights

		}

		this.offsprings.Add(unborn);
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
