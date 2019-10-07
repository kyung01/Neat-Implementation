using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organism
{
	public Organism(List<DNAConnection> dnas, List<DNANode> inputNodes, List<DNANode> outputNode)
	{
		this.dnas = dnas;
		this.inputNodes = inputNodes;
		this.outputNodes = outputNode;
		updateHiddeLayers(dnas);
		for (int i = 0; i < outputNodes.Count; i++)
		{
			outputNodePowers.Add(0);
			DNANode node = outputNodes[i];
			node.handleActivation.Add(power => { recordOutput(i, power); });

		}

	}
	public List<DNAConnection> dnas = new List<DNAConnection>();
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

	public DNANode getNode(int n)
	{
		if (n < inputNodes.Count) return inputNodes[n];
		if (n < inputNodes.Count + outputNodes.Count) return outputNodes[n - inputNodes.Count];
		n -= inputNodes.Count + outputNodes.Count;
		if (n >= hiddenNodes.Count)
		{
			for (int i = hiddenNodes.Count; i <= n; i++)
			{
				hiddenNodes.Add(new DNANode());
			}
		}
		return hiddenNodes[n];
	}
	public int NodeCount { get { return inputNodes.Count + hiddenNodes.Count + outputNodes.Count; } }
	public int InputNodeCount { get { return inputNodes.Count; } }
	public int HiddenNodeCount { get { return hiddenNodes.Count; } }
	public int OutputNodeCount { get { return outputNodes.Count; } }

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
