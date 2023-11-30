using System;
using Samples.Cloud_Save_main.Assets._Game._Scripts.Scriptables;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Models
{
    [Serializable]
    public class CharacterData
    {
        public int Slot;
        public string Name;
        public int Level;
        public HeroType Type;
    }
}