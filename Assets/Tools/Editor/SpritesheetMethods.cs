using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SpritesheetMethods
{
    static Object[] GetSelectedTextures()
    {
        return Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
    }


    public static void SplitSprites(int sliceWidth, int sliceHeight, int pixelsPerUnit, Vector2 spritePivot)
    {
        Object[] textures = GetSelectedTextures();

        foreach (Texture2D texture in textures)
        {
            //The relative path from 'Assets/'
            string path = AssetDatabase.GetAssetPath(texture);
            //Importing the asset
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;

            if (ti.spriteImportMode == SpriteImportMode.Multiple)
            {
                // Bug? Need to convert to single then back to multiple in order to make changes when it's already sliced
                ti.spriteImportMode = SpriteImportMode.Single;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = pixelsPerUnit;

            //Set new data (contains the single sliced sprites)
            List<SpriteMetaData> newData = new();

            //Go through each top-left corner and set the size to (sliceWidth, sliceHeight)
            //for (int i = 0; i < texture.width; i += sliceWidth)
            //{
            //    for (int j = texture.height; j > 0; j -= sliceHeight)
            //    {
            //        SpriteMetaData smd = new()
            //        {
            //            alignment = 9,
            //            name = $"{Path.GetFileName(path.Split('.').First())}_{((i / sliceWidth) * (texture.width / sliceWidth)) + (j / sliceHeight)}",
            //            pivot = spritePivot,
            //            rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight)
            //        };
            //        newData.Add(smd);
            //    }
            //}
            for (int j = texture.height; j > 0; j -= sliceHeight)
            {
                int heightIndex = (texture.height - j) / sliceHeight;

                for (int i = 0; i < texture.width; i += sliceWidth)
                {
                    int widthIndex = i / sliceWidth;

                    SpriteMetaData smd = new()
                    {
                        alignment = 9,
                        name = $"{Path.GetFileName(path.Split('.').First())}_{heightIndex * (texture.width / sliceWidth) + widthIndex}",
                        pivot = spritePivot,
                        rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight)
                    };
                    newData.Add(smd);
                }
            }

            //Apply the new spritesheet and force the editor to update
            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }
}