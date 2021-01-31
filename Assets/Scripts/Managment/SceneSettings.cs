using GGJ21.Gameplay.Objects;
using GGJ21.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.General
{
    public class SceneSettings : MonoBehaviour
    {
        [Header("References")]
        public Transform boardOrigin;
        public PathMovement character;
        public TreasureCheast treasureCheast;

        [Header("Profiles")]
        public MatchConditionsProfile matchConditions;
        public ObjectProfile objectProfile;
    }
}