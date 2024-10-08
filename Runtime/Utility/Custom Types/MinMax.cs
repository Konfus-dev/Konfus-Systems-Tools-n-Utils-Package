﻿using System;
using UnityEngine;

namespace Konfus.Utility.Custom_Types
{
    [Serializable]
    public struct MinMaxFloat
    {
        public MinMaxFloat(float minimum, float maximum)
        {
            min = minimum;
            max = maximum;
        }

        [SerializeField]
        private float min;
        [SerializeField]
        private float max;

        public float Min => min;
        public float Max => max;
    }
}