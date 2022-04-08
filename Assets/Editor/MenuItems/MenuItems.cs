using Assets.Editor.EditorWindows;
using UnityEditor;
using UnityEngine;
namespace Assets.Editor.MenuItems
{
    public class MenuItems : MonoBehaviour
    {
        [MenuItem("Xdd/Material Automizer")]
        static void CreateMaterial()
        {
            var popUp = ScriptableObject.CreateInstance<MaterialAutomizerWindow>();

            popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

            popUp.ShowPopup();
        }

        [MenuItem("Xdd/Card Form Material")]
        static void CardFromMaterial()
        {
            var popUp = ScriptableObject.CreateInstance<CardFromMaterialCreator>();

            popUp.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

            popUp.ShowPopup();
        }
    }
}
