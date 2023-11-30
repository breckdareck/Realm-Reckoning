using System.Collections.Generic;
using System.Linq;
using Samples.Cloud_Save_main.Assets._Game._Scripts.Scriptables;
using UnityEngine;

namespace Samples.Cloud_Save_main.Assets._Game._Scripts.Services
{
    public static class ResourcesService
    {
        public static List<ScriptableHero> Heroes { get; }

        static ResourcesService()
        {
            Heroes = Resources.LoadAll<ScriptableHero>("Heroes").OrderBy(h => (int)h.Type).ToList();
        }

        public static ScriptableHero GetHeroByType(HeroType type)
        {
            return Heroes.First(h => h.Type == type);
        }
    }
}