using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game._Scripts
{
    [CreateAssetMenu(fileName = "New Stats", menuName = "Custom/Stats")] 
    [InlineEditor]
    [ManageableData]
    public class Stats : SerializedScriptableObject
    {
        public Dictionary<Stat, float> stats = new ();

        public float GetStatValue(Stat stat)
        {
            if (stats.TryGetValue(stat, out var value))
            {
                return value;
            }
            else
            {
                Debug.LogError($"No Stat Value found for {stat} on {this.name}");
                return 0;
            }
        }
    }
}