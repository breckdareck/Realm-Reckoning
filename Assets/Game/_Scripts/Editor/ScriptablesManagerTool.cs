using System;
using System.Linq;
using Game._Scripts.Units;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Editor
{
    public class ScriptablesManagerTool : OdinMenuEditorWindow
    {
        private static Type[] typesToDisplay =
            TypeCache.GetTypesWithAttribute<ManageableDataAttribute>().OrderBy(m => m.Name).ToArray();

        private Type selectedType;

        [MenuItem("Custom Tools/Scriptables Manager")]
        private static void OpenEditor() => GetWindow<ScriptablesManagerTool>();
    
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            foreach (var type in typesToDisplay)
            {
                if (type.Name == "UnitData")
                {
                    var factions = Resources.FindObjectsOfTypeAll<Faction>();
                    var unitData = Resources.FindObjectsOfTypeAll<UnitData>();
                    foreach (var faction in factions)
                    {
                        tree.Add($"UnitData/{faction.factionName}", null);

                        foreach (var data in unitData)
                        {
                            if (data.unitFaction.factionName == faction.factionName)
                                tree.AddAssetAtPath($"UnitData/{faction.factionName}/{data.unitName}",
                                    $"Assets/Game/Scriptables/Resources/UnitData/{data.unitName}.asset");
                        }
                    }
                }
                else
                {
                    tree.AddAllAssetsAtPath(type.Name, "Assets/Game/Scriptables/Resources", type, true, true);
                }
            }
            
            tree.SortMenuItemsByName();

            return tree;
        }
    }
}
#endif
