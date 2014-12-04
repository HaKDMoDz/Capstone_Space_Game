using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlanetPrefab : EditorWindow
{
    string planetName = "Planet";
    bool ringEnabled = true;
    float ringScale = 30.0f;
    bool atmosphereEnabled = true;
    float atmosphereScale = 1.12f;
    GameObject planet;
    GameObject ring;
    GameObject atmosphere;
    GameObject wholePlanet;
    public Color atmosphereColour = Color.white;
    public Texture2D ringTexture;
    public Color ringColour = Color.white;
    public Texture2D planetDayTexture;
    public Texture2D planetNightTexture;
    public Texture2D atmosphereTexture;


    [MenuItem("Window/PlanetWorkshop")]

    public static void ShowWindow()
    {
        //EditorWindow.GetWindow(typeof(PlanetPrefab)); // simple window
        EditorWindow.GetWindowWithRect(typeof(PlanetPrefab), new Rect(0.0f, 0.0f, 100.0f, 200.0f)); // window based on any Rect

    }

    void Awake()
    {
        //Debug.Log("Awake totally happens");
        
    }

    void OnGUI()
    {
        //Debug.Log("OnGUI totally happens"); //it's true!
        wholePlanet = GameObject.Find("PlanetObject");
        planet = GameObject.Find("Planet");
        ring = GameObject.Find("Ring");
        ring.SetActive(true);
        atmosphere = GameObject.Find("Atmosphere");

        atmosphere.SetActive(true);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        planetName = EditorGUILayout.TextField("Planet Name: ", planetName);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Day Texture: ", GUILayout.MaxWidth(80.0f));
        planetDayTexture = EditorGUILayout.ObjectField(planetDayTexture, typeof(Texture2D)) as Texture2D;
        planet.renderer.sharedMaterial.SetTexture("_MainTex", planetDayTexture);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Night Texture: ", GUILayout.MaxWidth(80.0f));
        planetNightTexture = EditorGUILayout.ObjectField(planetNightTexture, typeof(Texture2D)) as Texture2D;
        planet.renderer.sharedMaterial.SetTexture("_Lights", planetNightTexture);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        ringEnabled = EditorGUILayout.BeginToggleGroup("Ring", ringEnabled);

        if (ringEnabled)
        {
            ringScale = EditorGUILayout.Slider("Ring Size", ringScale, 15, 40);
            ring.transform.localScale = new Vector3(ringScale, ringScale, ringScale);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ring Texture: ", GUILayout.MaxWidth(80.0f));
            ringTexture = EditorGUILayout.ObjectField(ringTexture, typeof(Texture2D)) as Texture2D;
            EditorGUILayout.EndHorizontal();
            ringColour = EditorGUILayout.ColorField("Ring Colour: ", ringColour);

            foreach (Renderer ring_renderer in ring.GetComponentsInChildren<Renderer>())
            {
                ring_renderer.enabled = true;
                ring_renderer.sharedMaterial.SetTexture("_MainTex", ringTexture);
                ring_renderer.sharedMaterial.SetColor("_TintColor", ringColour);
            }
        }
        else
        {
            foreach (Renderer ring_renderer in ring.GetComponentsInChildren<Renderer>())
            {
                ring_renderer.enabled = false;
            }
        }
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Separator();

        atmosphereEnabled = EditorGUILayout.BeginToggleGroup("Atmosphere", atmosphereEnabled);
        if (atmosphereEnabled)
        {
            atmosphereScale = EditorGUILayout.Slider("Atmosphere Scale", atmosphereScale, 1.0f, 1.5f);
            atmosphere.transform.localScale = new Vector3(atmosphereScale, atmosphereScale, atmosphereScale);
            atmosphereColour = EditorGUILayout.ColorField("Atmosphere Colour: ", atmosphereColour);
            // set atmosphere colour
            atmosphere.renderer.sharedMaterial.SetColor("_TintColor", ringColour);

            foreach (Renderer atmo_renderer in atmosphere.GetComponentsInChildren<Renderer>())
            {
                atmo_renderer.enabled = true;
                atmo_renderer.sharedMaterial.SetColor("_TintColor", atmosphereColour);
            }
        }
        else
        {
            foreach (Renderer atmo_renderer in atmosphere.GetComponentsInChildren<Renderer>())
            {
                atmo_renderer.enabled = false;
            }
        }

        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("CreatePrefab"))
        {
            //Debug.Log(planetName);

            GameObject planetCopy = GameObject.Instantiate(wholePlanet) as GameObject;

            // ORDER IS IMPORTANT HERE. if you delete index 0 first it screws over
            // all the other deletes. there's a better way to do this but for now 
            // this'll do
            if (!ringEnabled)
            {
                DestroyImmediate(planetCopy.transform.GetChild(2).gameObject);
                //Debug.Log(planetCopy.transform.GetChild(2).gameObject.name);
            }
            else
            {
                foreach (Renderer ringRenderer in planetCopy.transform.GetChild(2).GetComponentsInChildren<Renderer>())
	            {
                    ringRenderer.sharedMaterial.SetTexture("_MainTex", ringTexture);
                    ringRenderer.sharedMaterial.SetColor("_TintColor", ringColour);
	            }
                
            }

            if (!atmosphereEnabled)
            {
                //Debug.Log(planetCopy.transform.GetChild(0).gameObject.name);
                DestroyImmediate(planetCopy.transform.GetChild(0).gameObject); // must be the LAST one deleted
            }
            else
            {
                planet.transform.GetChild(0).renderer.sharedMaterial.SetColor("_TintColor", atmosphereColour);
            }

            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Planets/" + planetName + ".prefab");
            PrefabUtility.ReplacePrefab(planetCopy, prefab, ReplacePrefabOptions.ConnectToPrefab);

            GameObject.DestroyImmediate(GameObject.Find("PlanetObject(Clone)"));
        }

    }
}

