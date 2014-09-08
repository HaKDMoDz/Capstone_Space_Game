using UnityEngine;
using System.Collections;

public class Starfire : MonoBehaviour {

	public float GlobalSpeed = 0.04f;
	public Material[] Layers;
	private float layerXspeed = 1.5f, layerYspeed = 1.0f;
	private Vector2 currentOffset;

	// Use this for initialization
	void Start () 
	{
        currentOffset = new Vector2(0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		for (int i = 0; i < Layers.Length; i++)
		{
			currentOffset = Layers[i].GetTextureOffset("_MainTex");
			Layers[i].SetTextureOffset("_MainTex", new Vector2(currentOffset.x + (GlobalSpeed * layerXspeed * i * i), currentOffset.y + (GlobalSpeed * layerYspeed * i * i)));
		}

	}
}
