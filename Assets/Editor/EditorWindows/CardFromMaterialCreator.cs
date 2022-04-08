using System.IO;
using System.Linq;
using Assets.Source.Model.Enums;
using Assets.Source.Model.Games;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor.EditorWindows
{
    public class CardFromMaterialCreator : EditorWindow
    {
        private DefaultAsset importFolder;
        private DefaultAsset exportFolder;

        void OnEnable()
        {
            GetWindow<CardFromMaterialCreator>();
        }

        void OnGUI()
        {
            importFolder = (DefaultAsset)EditorGUILayout.ObjectField("Import Folder",
                    importFolder, typeof(DefaultAsset), false);

            exportFolder = (DefaultAsset)EditorGUILayout.ObjectField("Export Folder",
                    exportFolder, typeof(DefaultAsset), false);


            if (GUILayout.Button("Create"))
            {
                var path = Application.dataPath;
                path = path.Substring(0, path.Length - 6);
                path += AssetDatabase.GetAssetPath(importFolder);
                var files = Directory.GetFiles(path)
                    .Where(x => Path.GetExtension(x) != ".meta")
                    .Select(x => x.Remove(0, x.LastIndexOf("Assets")))
                    .ToList();

                var ranks = new Ranks[] { Ranks.Ace, Ranks.Two, Ranks.Three, Ranks.Four, Ranks.Five,
                    Ranks.Six, Ranks.Seven, Ranks.Eight, Ranks.Nine, Ranks.Ten, Ranks.Jack, Ranks.Queen, Ranks.King};
                var suits = new Suits[] { Suits.Clubs, Suits.Hearts, Suits.Spades, Suits.Diamonds };

                var cards = new Card[ranks.Count() * suits.Count()];

                var num = 0;

                foreach (var suit in suits)
                {
                    foreach (var rank in ranks)
                    {
                        cards[num++] = new Card { rank = rank, suit = suit };
                    }
                }

                foreach (var file in files)
                {
                    var material = AssetDatabase.LoadAssetAtPath<Material>(file);

                    Debug.Log(material.name);

                    if (!material.name.Contains("_"))
                        continue;

                    var cutName = material.name.Remove(0, material.name.LastIndexOf("_") + 1);

                    if (int.TryParse(cutName, out int cardNum))
                    {
                        cardNum--;
                        var card = CreateInstance<CardObject>();

                        card.material = material;
                        card._suit = cards[cardNum].suit;
                        card._rank = cards[cardNum].rank;

                        AssetDatabase.CreateAsset(card, @$"{AssetDatabase.GetAssetPath(exportFolder)}/{card.suit} {card.rank}.asset");
                    }
                }
            }
        }
    }
}
