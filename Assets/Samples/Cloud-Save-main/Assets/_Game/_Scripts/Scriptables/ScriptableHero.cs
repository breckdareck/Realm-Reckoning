using System;
using UnityEngine;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Scriptables
{
    [CreateAssetMenu]
    public class ScriptableHero : ScriptableObject
    {
        public HeroType Type;
        public Sprite Image;
        public Color Color;
    }

    [Serializable]
    public enum HeroType
    {
        Dino = 0,
        Snorlax = 1,
        Jolteon = 2,
    }
}