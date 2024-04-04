using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Test/Test float node")]
public class TestNode : Node
{
	protected TestGraph Graph => graph as TestGraph;

	// Use this for initialization
	protected override void Init() {
		base.Init();
		VerifyConnections();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}