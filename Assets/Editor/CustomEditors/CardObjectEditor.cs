using UnityEditor;
using UnityEngine;

namespace Assets.Editor.CustomEditors
{
    [CustomEditor(typeof(CardObject))]
    internal class CardObjectEditor : UnityEditor.Editor
    {

        private CardObject script;

        void OnEnable()
        {
            script = (CardObject)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (script.material != null)
            {
                GUILayout.Label(script.material.mainTexture, GUILayout.Width(300), GUILayout.Height(300));
            }
        }
    }
}
