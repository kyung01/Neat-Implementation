using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Organism
{
	public static int CompareByEvaluatedFitness(Organism a, Organism b)
	{
		float difference = a.evaluatedFitness - b.evaluatedFitness;
		if (difference * difference < 0.01f) return 0;
		if (a.EvaluatedFitness < b.EvaluatedFitness) return -1;
		return 1;
	}
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
			int n = i;
			node.handleActivation.Add(power => { recordOutput(n, power); });

		}

	}
	public List<DNAConnection> dnas = new List<DNAConnection>();
	List<DNANode> inputNodes = new List<DNANode>();
	List<DNANode> hiddenNodes = new List<DNANode>();
	List<DNANode> outputNodes = new List<DNANode>();
	List<float> outputNodePowers = new List<float>();

	bool isAlive = true;
	float individualFitness = -1;
	float evaluatedFitness = 0;
	public float IndividualFitness { get { return individualFitness; } }
	public float EvaluatedFitness { get { return evaluatedFitness; } set { evaluatedFitness = value; } }


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
		if(index< 0 || index >= outputNodePowers.Count) { Debug.Log(index + " and " + outputNodePowers.Count); }
		outputNodePowers[index] += power;
	}

	public void activateInput(int inputNumber, float power)
	{
		if (!isAlive) return;
		inputNodes[inputNumber].activate(power, getNode);
	}
	public void addIndividualFitness(float fitness)
	{
		this.individualFitness += fitness;
	}
	public int getOutput()
	{
		int selectedOutputChannel = -1;
		float mostPower = 0;
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
		return selectedOutputChannel;
	}
	public void addDNA(DNAConnection dna)
	{
		dnas.Add(dna);
		getNode(dna.from).connections.Add(dna);
	}
	public void disable(DNAConnection dna)
	{
		dna.activated = false;
		getNode(dna.from).connections.Remove(dna);
	}
	public int getNewNodeIndex()
	{
		hiddenNodes.Add(new DNANode());
		return InputNodeCount + OutputNodeCount+  HiddenNodeCount - 1;
	}
	public void kill()
	{
		isAlive = false;
	}
	public string printDNA()
	{
		string dnaString = "";
		foreach (var piece in dnas) dnaString += piece.toString() + " ";
		return dnaString;
	}
}
