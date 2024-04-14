using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using XNode;

[CreateNodeMenu("Test/Test float node")]
public class TestNode : Node
{
	public Ball _prefabBall;

	public TestNodeObject GetNodeObject(NetworkRunner runner)
	{
		TestNodeObject newObject = new TestNodeObject(_prefabBall, runner);
		return newObject;
	}
	
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

public class TestNodeObject
{
	public Ball PrefabBall { get; }

	private readonly NetworkRunner _runner;

	public TestNodeObject(Ball prefab, NetworkRunner runner)
	{
		PrefabBall = prefab;
		_runner = runner;
	}

	public void SpawnBall(Vector3 direction, PlayerRef playerRef, Player owner)
	{
		_runner.Spawn(PrefabBall, owner.gameObject.transform.position + direction, Quaternion.LookRotation(direction), playerRef,
			(runner, networkObj) =>
			{
				networkObj.GetComponent<Ball>().Init(owner);
			});
	} 
}

