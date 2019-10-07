using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNAConnection {
	public int id = -1;
	public int from =-1, to=-1;
	public float weight = 1.0f;
	public bool activated = true;
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

public class Organism {
	public Organism(List<DNAConnection> dnas, List<DNANode> inputNodes, List<DNANode> outputNode)
	{
		this.dnas = dnas;
		this.inputNodes = inputNodes;
		this.outputNodes = outputNode;
		updateHiddeLayers(dnas);
		for(int i = 0; i < outputNodes.Count; i++)
		{
			outputNodePowers.Add(0);
			DNANode node = outputNodes[i];
			node.handleActivation.Add(power => { recordOutput(i, power); });

		}

	}
	List<DNAConnection> dnas = new List<DNAConnection>();
	List<DNANode> inputNodes = new List<DNANode>();
	List<DNANode> hiddenNodes = new List<DNANode>();
	List<DNANode> outputNodes = new List<DNANode>();
	List<float> outputNodePowers = new List<float>();

	float individualFitness = -1;
	float evaluatedFitness = 0;
	public float IndividualFitness { get { return individualFitness; } }
	public float EvaluatedFitness { get { return evaluatedFitness; } }


	void updateHiddeLayers(List<DNAConnection> dnas)
	{
		foreach (var stripe in dnas)
		{
			var nodefrom = getNode(stripe.from);
			getNode(stripe.to); // create the end in case it does not exist
			nodefrom.connections.Add(stripe);
		}

	}

	DNANode getNode(int n)
	{
		if (n < inputNodes.Count) return inputNodes[n];
		if (n < inputNodes.Count + outputNodes.Count) return outputNodes[n - inputNodes.Count];
		n -= inputNodes.Count + outputNodes.Count;
		if(n >= hiddenNodes.Count)
		{
			for(int i = hiddenNodes.Count; i <= n; i++)
			{
				hiddenNodes.Add(new DNANode());
			}
		}
		return hiddenNodes[n];
	}

	void recordOutput(int index, float power)
	{
		outputNodePowers[index] += power;
	}

	public void activateInput(int inputNumber, float power)
	{
		inputNodes[inputNumber].activate(power, getNode);
	}
	public void addIndividualFitness(float fitness)
	{
		this.individualFitness += fitness;
	}
	public int getOutput()
	{
		int selectedOutputChannel = 0;
		float mostPower = -1;
		for (int i = 0; i < outputNodePowers.Count; i++)
		{
			if (outputNodePowers[i] > mostPower)
			{
				selectedOutputChannel = i;
				mostPower = outputNodePowers[i];
			}
		}
		for (int i = 0; i < outputNodePowers.Count; i++)
		{
			outputNodePowers[i] = 0;
		}
		return 0;
	}
}

public class MutationInformation {
	public enum MUTATION_TYPE { NONE, CREATE_CONNECTION, CREATE_NODE }

	MUTATION_TYPE type = MUTATION_TYPE.NONE;
	int mutationID;
	/// <summary>
	/// if conneciton mutation is selected then the node that's being splited is recorded
	/// </summary>
	int connectionDNAID =-1;
	/// <summary>
	/// if create connection is selected then the nodes that are being connected are recorded 
	/// </summary>
	int nodeFrom = -1, nodeTO =1;
	
	public static bool IS_SAME(MutationInformation a, MutationInformation b)
	{
		return a.type == b.type && a.connectionDNAID == b.connectionDNAID && a.nodeFrom == b.nodeFrom && a.nodeTO == b.nodeTO;
	}
}


public class Species : MonoBehaviour
{
	class DNAConnectionMatch
	{
		DNAConnection
			connectionA,
			connectionB;
	}

	List<Organism> parent = new List<Organism>();
	List<DNAConnectionMatch> parentDNAMatches = new List<DNAConnectionMatch>();

	List<Organism> offsprings = new List<Organism>();
	List<MutationInformation> mutationImformations = new List<MutationInformation>();




	public void setParent(Organism X, Organism Y)
	{
		parent.Clear();
		parent.Add(X);
		parent.Add(Y);
		parentDNAMatches.Clear();
		//update dna matches

	}

	public void breed()
	{
		//first 
		if(parent[0].EvaluatedFitness == parent[1].EvaluatedFitness)
		{
			//equal parents
		}
		else if(parent[0].EvaluatedFitness > parent[1].EvaluatedFitness)
		{
			//parent 0 dominated fitness
		}
		else
		{
			//parent 1 dominated fitness
		}
		//10% chance to create a new connection gean
		//90% chance to alter weights
		//10% chance to create a new bond
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
