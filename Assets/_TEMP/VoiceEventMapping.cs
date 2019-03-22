using System;
using UnityEngine.Events;
using UnityEngine;

namespace RPG.Imported
{
    [Serializable]
    public struct VoiceEventMapping
    {
        public string name;
        public UnityEvent callback;
    }
}
