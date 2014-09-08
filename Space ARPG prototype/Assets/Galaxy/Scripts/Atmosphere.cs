using UnityEngine;
using System.Collections;

public class Atmosphere : MonoBehaviour {

	public float atmosphereSpeed = 0.04f;
	public GameObject atmosphere;
	private Vector2 currentOffset;

	private float alphaIncrementer = 0;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (atmosphere != null)
	    {
			currentOffset = atmosphere.renderer.material.GetTextureOffset("_MainTex");
			atmosphere.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x + (atmosphereSpeed/10), currentOffset.y));

			atmosphere.renderer.material.SetFloat("_Cutoff", Mathf.Sin (alphaIncrementer));
		}

		alphaIncrementer += (Time.deltaTime/10);


	}
}

/*
using UnityEngine;
using System.Collections;

public class shipMovement : MonoBehaviour 
{
    public float controllerDeadZone = 0.2f;
    public float moveSpeed = 0.2f;

    public float xMin = -13;
    public float xMax = 13;
    public float yMin = -9.5f;
    public float yMax = 0;

    public float forgroundSpeed = 0.04f;
    public float backgroundSpeed = 0.00005f;
    public float midgroundSpeed = 0.004f;

    private GameObject background;
    private GameObject foreground;
    private GameObject midground;

    private Vector2 currentOffset;
    private Vector2 currentOffsetFg;
    private Vector2 currentOffsetFgBg;

    private GameObject mySpawnPoint;

	void Start () {
        // get gameobject that I'll be modifying later
        background = GameObject.Find("Starfield-BKG");
        foreground = GameObject.Find("Starfield-FG");
        midground = GameObject.Find("Starfield-FGBG");

        mySpawnPoint = GameObject.Find("SpawnPoint");
        transform.position = mySpawnPoint.transform.position;
        //transform.GetComponent<Animator>().StopPlayback();

    }
	
	void Update () 
    {
        //update the current texture offsets
        currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
        currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
        currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");

        // deal with input
        if (Input.GetAxis("Horizontal") > controllerDeadZone){ //RIGHT
            transform.Translate(new Vector3(moveSpeed, 0, 0));
            //transform.GetComponent<Animator>().Play("bankRight", 0);
            //animation.Blend("bankRight");

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin, xMax), transform.position.y, transform.position.z);
            background.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x - (backgroundSpeed/10), currentOffset.y));
            foreground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFg.x - (forgroundSpeed/10), currentOffsetFg.y));
            midground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFgBg.x - (midgroundSpeed/10), currentOffsetFgBg.y));

            //update the current texture offsets
            currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");
        }
        if (Input.GetAxis("Horizontal") < -controllerDeadZone){ //LEFT
            transform.Translate(new Vector3(-moveSpeed, 0, 0));
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin, xMax), transform.position.y, transform.position.z);
            background.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x + (backgroundSpeed/10), currentOffset.y));
            foreground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFg.x + (forgroundSpeed/10), currentOffsetFg.y));
            midground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFgBg.x + (midgroundSpeed/10), currentOffsetFgBg.y));

            //update the current texture offsets
            currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");
            
        }
        if (Input.GetAxis("Vertical") > controllerDeadZone){ //UP
            transform.Translate(new Vector3(0, 0, moveSpeed));
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, yMin, yMax));

            //update the current texture offsets
            currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");
        }
        if (Input.GetAxis("Vertical") < -controllerDeadZone){ //DOWN
            transform.Translate(new Vector3(0, 0, -moveSpeed));
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Clamp(transform.position.z, yMin, yMax));
            background.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x, currentOffset.y + (backgroundSpeed/2)));
            foreground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFg.x, currentOffsetFg.y + (forgroundSpeed/2)));
            midground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFgBg.x, currentOffsetFgBg.y + (midgroundSpeed/2)));

            //update the current texture offsets
            currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
            currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");
        }


        //update the current texture offsets again
        currentOffset = background.renderer.material.GetTextureOffset("_MainTex");
        currentOffsetFg = foreground.renderer.material.GetTextureOffset("_MainTex");
        currentOffsetFgBg = midground.renderer.material.GetTextureOffset("_MainTex");

        //give the illusion of constant forward motion
        background.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffset.x, currentOffset.y - backgroundSpeed));
        foreground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFg.x, currentOffsetFg.y - forgroundSpeed));
        midground.renderer.material.SetTextureOffset("_MainTex", new Vector2(currentOffsetFgBg.x, currentOffsetFgBg.y - midgroundSpeed));

	}
}

 */
