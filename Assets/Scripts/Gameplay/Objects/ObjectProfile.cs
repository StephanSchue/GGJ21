using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    [CreateAssetMenu(fileName = "ObjectProfile_", menuName = "Configs/ObjectProfileProfile", order = 1)]
    public class ObjectProfile : ScriptableObject
    {
        public ObjectComponent[] objects;
    }
}