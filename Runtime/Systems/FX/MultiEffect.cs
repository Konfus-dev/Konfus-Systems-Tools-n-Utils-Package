﻿using System;
using UnityEngine;

namespace Konfus.Systems.FX
{
    [Serializable]
    public class MultiEffect : Effect
    {
        [SerializeField]
        private FxSystem fxSystem;

        public override void Play()
        {
            fxSystem.PlayEffects();
        }
    }
}