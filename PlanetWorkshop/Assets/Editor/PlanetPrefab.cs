using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlanetPrefab : EditorWindow
{
    string planetName = "Planet";
    bool ringEnabled = true;
    float ringScale = 30.0f;
    bool atmosphereEnabled = true;
    float atmosphereScale = 1.0f;
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
        Debug.Log("Awake totally happens");
        
    }

    void OnGUI()
    {
        //Debug.Log("OnGUI totally happens"); //it's true!
        planet = GameObject.Find("HighPolyPlanet");
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
            ring.SetActive(true);
        }
        else
        {
            ring.SetActive(false);
        }
        EditorGUILayout.EndToggleGroup();

        atmosphereEnabled = EditorGUILayout.BeginToggleGroup("Atmosphere", atmosphereEnabled);

        if (atmosphereEnabled)
        {
            atmosphereScale = EditorGUILayout.Slider("Atmosphere Scale", atmosphereScale, 1.0f, 1.5f);

            atmosphere.transform.localScale = new Vector3(atmosphereScale, atmosphereScale, atmosphereScale);
            atmosphere.SetActive(true);
        }
        else
        {
            atmosphere.SetActive(false);
        }

        EditorGUILayout.EndToggleGroup();

        if (GUILayout.Button("CreatePrefab"))
        {
            Debug.Log(planetName);

            Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/Planets/" + planetName + ".prefab");
            PrefabUtility.ReplacePrefab(planet, prefab, ReplacePrefabOptions.ConnectToPrefab);
        }

    }
}

