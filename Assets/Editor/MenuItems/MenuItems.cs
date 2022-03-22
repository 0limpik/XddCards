using Assets.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
    [MenuItem("0limpik/Material Automizer")]
    static void CreateMaterial()
    {
        var popUp = ScriptableObject.CreateInstance<MaterialAutomizerWindow>();

        popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

        popUp.ShowPopup();
    }

    [MenuItem("0limpik/Card Form Material")]
    static void CardFromMaterial()
    {
        var popUp = ScriptableObject.CreateInstance<CardFromMaterialCreator>();

        popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

        popUp.ShowPopup();
    }
}
