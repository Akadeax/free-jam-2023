using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritesheetMakerWindow : EditorWindow
{
    [MenuItem("Window/Custom/Spritesheet maker")]
    public static void ShowWindow()
    {
        GetWindow<SpritesheetMakerWindow>("Spritesheet maker");
    }

    Vector2 pivotPoint = new Vector2();

    int sliceWidth = 16;
    int sliceHeight = 16;
    int slicePixelsPerUnit = 16;

    private void OnGUI()
    {
        pivotPoint = EditorGUILayout.Vector2Field("Pivot", pivotPoint);
        sliceWidth = EditorGUILayout.IntField("Slice width", sliceWidth);
        sliceHeight = EditorGUILayout.IntField("Slice height", sliceHeight);
        slicePixelsPerUnit = EditorGUILayout.IntField("Sprite Pixels per unit", slicePixelsPerUnit);

        if(GUILayout.Button("Apply settings to selected textures"))
        {
            SpritesheetMethods.SplitSprites(sliceWidth, sliceHeight, slicePixelsPerUnit, pivotPoint);
        }
    }
}