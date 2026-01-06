using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public enum StageType
{
	Chunked,
	Circuit
}
public class Stage : Object
{

	public virtual List<string> ChunkList { get; set; } = new List<string>();

	public StageType StageType = StageType.Chunked;

	public virtual void OnStageStart(Scene Scene)
	{
	}

	public virtual void OnStageEnd( Scene Scene )
	{

	}

	public virtual void OnStageUpdate( Scene Scene )
	{

	}
}
