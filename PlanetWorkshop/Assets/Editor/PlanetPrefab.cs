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
    Texture2D ringTexture;


    [MenuItem("PlanetWorkshop/Create Planet Prefab...")]

    public static void ShowWindow()
    {
        //EditorWindow.GetWindow(typeof(createPlanetPrefab)); // simple window
        EditorWindow.GetWindowWithRect(typeof(PlanetPrefab), new Rect(0.0f, 0.0f, 100.0f, 200.0f)); // window based on any Rect

    }

    void Awake()
    {
        //Debug.Log("Awake totally happens");
        
    }

    void OnGUI()
    {
        //Debug.Log("OnGUI totally happens"); //it's true!
        planet = GameObject.Find("Planet");
        ring = GameObject.Find("Ring");
        ring.SetActive(true);
        atmosphere = GameObject.Find("Atmosphere");
        atmosphere.SetActive(true);
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        planetName = EditorGUILayout.TextField("Planet Name: ", planetName);


        ringEnabled = EditorGUILayout.BeginToggleGroup("Ring", ringEnabled);

        if (ringEnabled)
        {
            ringScale = EditorGUILayout.Slider("Ring Size", ringScale, 15, 40);
            ring.transform.localScale = new Vector3(ringScale, ringScale, ringScale);

            foreach (Renderer ring_renderer in ring.GetComponentsInChildren<Renderer>())
            {
                ring_renderer.enabled = true;
            }

            EditorGUILayout.ObjectField(ringTexture, typeof(Texture2D));
        }
        else
        {
            foreach (Renderer ring_renderer in ring.GetComponentsInChildren<Renderer>())
            {
                ring_renderer.enabled = false;
            }
        }
        EditorGUILayout.EndToggleGroup();

        atmosphereEnabled = EditorGUILayout.BeginToggleGroup("Atmosphere", atmosphereEnabled);

        if (atmosphereEnabled)
        {
            atmosphereScale = EditorGUILayout.Slider("Atmosphere Scale", atmosphereScale, 1.0f, 1.5f);

            atmosphere.transform.localScale = new Vector3(atmosphereScale, atmosphereScale, atmosphereScale);
            
            foreach (Renderer atmo_renderer in atmosphere.GetComponentsInChildren<Renderer>())
            {
                atmo_renderer.enabled = true;
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
            Debug.Log(planetName);

            GameObject planetCopy = GameObject.Instantiate(planet) as GameObject;

            // ORDER IS IMPORTANT HERE. if you delete index 0 first it screws over
            // all the other deletes. there's a better way to do this but for now 
            // this'll do
            if (!ringEnabled)
            {
                DestroyImmediate(planetCopy.transform.GetChild(2).gameObject);
                //Debug.Log(planetCopy.transform.GetChild(2).gameObject.name);
            }

            if (!atmosphereEnabled)
            {
                //Debug.Log(planetCopy.transform.GetChild(0).gameObject.name);
                DestroyImmediate(planetCopy.transform.GetChild(0).gameObject);
            }

            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Planets/" + planetName + ".prefab");
            PrefabUtility.ReplacePrefab(planetCopy, prefab, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(GameObject.Find("Planet(Clone)"));
        }

    }
}

