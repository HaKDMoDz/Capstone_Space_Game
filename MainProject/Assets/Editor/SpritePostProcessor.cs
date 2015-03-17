/*
  SpritePostProcessor.cs
  Mission: Invasion
  Created by Rohun Banerji on March 14, 2015.
  Copyright (c) 2015 Rohun Banerji. All rights reserved.
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SpritePostProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (assetPath.Contains("GUI"))
        {
            Debug.LogWarning("Texture Post Processor: " + assetPath + " -- Convering to Sprite as it was placed in the GUI folder");
            TextureImporter texImporter = (TextureImporter)assetImporter;
            texImporter.textureType = TextureImporterType.Sprite;
        }
    }

}
