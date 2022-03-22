using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;
using UnityEditor;

namespace Assets.Editor.CustomEditors
{
    [CustomEditor(typeof(CardMono))]
    public class CardMonoEditor : UnityEditor.Editor
    {
        private CardMono script;

        void OnEnable()
        {
            script = (CardMono)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (script.card?.material != null)
            {
                GUILayout.Label(script.card.material.mainTexture, GUILayout.Width(200), GUILayout.Height(200));
            }
        }
    }
}
