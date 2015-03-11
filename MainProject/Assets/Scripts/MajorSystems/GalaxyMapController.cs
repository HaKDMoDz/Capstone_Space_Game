using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GalaxyMapController : Singleton<GalaxyMapController>
{
    private void Start()
    {
        AudioManager.Instance.SetMainTrack(Sound.GalaxyMapTheme);
    }

}
