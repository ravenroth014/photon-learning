using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Test/Test float node")]
public class TestNode : Node
{
    [Input] public float A;
    [Input] public float B;

    public float C;

    [Output] public float D;
    [Output] public float E;

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