using Assets.Source.Scripts.BlackJack;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Editor.CustomEditors
{
    [CustomEditor(typeof(BJHandScript))]
    internal class HandScriptEditor : UnityEditor.Editor
    {
        [SerializeField, HideInInspector] private BJHandScript script;
        [SerializeField] private Texture backTexture;

        void Awake()
        {
            backTexture ??= AssetDatabase.LoadAssetAtPath<Texture2D>(@"Assets\Models\Cards\Textures\Card_00.png");
        }

        void OnEnable()
        {
            script = (BJHandScript)target;
        }

        public float cardSize = 100;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //GUILayout.BeginHorizontal();
            //if (script.cards.Count == 0)
            //{
            //    GUILayout.Label("Hand Empty");
            //    GUILayout.EndHorizontal();
            //}
            //else
            //{
            //    var elemetsDisplay = 0;
            //    var toRemove = new List<CardObject>();
            //    foreach (var card in script.cards)
            //    {
            //        if (EditorGUIUtility.currentViewWidth < (elemetsDisplay + 1) * cardSize + 50)
            //        {
            //            GUILayout.FlexibleSpace();
            //            GUILayout.EndHorizontal();
            //            GUILayout.BeginHorizontal();
            //            elemetsDisplay = 0;
            //        }

            //        GUILayout.BeginVertical();
            //        if(card == null)
            //        {
            //            GUILayout.Label(backTexture, GUILayout.Width(cardSize), GUILayout.Height(cardSize));
            //            GUILayout.Label("?");
            //            GUILayout.Label("?");
            //        }
            //        else
            //        {
            //            GUILayout.Label(card.material.mainTexture, GUILayout.Width(cardSize), GUILayout.Height(cardSize));
            //            GUILayout.Label(card.suit.ToString());
            //            GUILayout.Label(card.rank.ToString());
            //        }
            //        if (GUILayout.Button("X"))
            //        {
            //            toRemove.Add(card);
            //        }
            //        GUILayout.EndVertical();
            //        elemetsDisplay++;
            //    }

            //    GUILayout.FlexibleSpace();
            //    GUILayout.EndHorizontal();
            //    GUILayout.BeginHorizontal();
            //    GUI.enabled = false;
            //    GUILayout.Label("Score");
            //    EditorGUILayout.TextField(string.Join(", ", script.GetScore()));
            //    GUI.enabled = true;
            //    GUILayout.EndHorizontal();
            //    if (toRemove.Count > 0)
            //        NotifyEditorChange("Remove Card");

            //    foreach (var card in toRemove)
            //    {
            //        //script.Remove(card);
            //    }

            //}

            //GUILayout.BeginHorizontal();
            //GUILayout.Label("Add card");
            //var newCard = (CardObject)EditorGUILayout.ObjectField(null, typeof(CardObject), false);
            //GUILayout.EndHorizontal();

            //if (newCard != null)
            //{
            //    NotifyEditorChange("Add Card");
            //    //script.Add(newCard);
            //}
        }
        void NotifyEditorChange(string message)
        {
            if (!Application.isPlaying)
            {
                Undo.RegisterCompleteObjectUndo(script, message);
                EditorUtility.SetDirty(script);
                EditorSceneManager.MarkSceneDirty(script.gameObject.scene);
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
