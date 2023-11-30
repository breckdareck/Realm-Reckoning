using Samples.Cloud_Save_main.Assets._Game._Scripts.Models;
using UnityEngine;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Scriptables
{
    [CreateAssetMenu]
    public class ScriptablePersistentData : ScriptableObject
    {
        [HideInInspector] public CharacterData CharacterData;
    }
}