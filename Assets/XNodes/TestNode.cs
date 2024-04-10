using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using XNode;

[CreateNodeMenu("Test/Test float node")]
public class TestNode : Node
{
	// Use this for initialization
	protected override void Init() {
		base.Init();
		VerifyConnections();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}

	public void Execute(Player newPlayer)
	{
		ExecuteTask(newPlayer);
		Debug.Log($"{newPlayer}'s Task is called.");
	}

	private async Task ExecuteTask(Player newPlayer)
	{
		await Task.Delay(3000);
		newPlayer.FinishExecuteTask();
	}
}

