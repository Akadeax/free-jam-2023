using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class PixelCreateAnimationMenu
{
    [MenuItem("Pixel/Animation Wizard")]
    public static void AnimationWizard()
    {
        PixelCreateAnimationEditor.LaunchEditor();
    }
}
