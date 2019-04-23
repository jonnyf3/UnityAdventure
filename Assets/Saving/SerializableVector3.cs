using System;
using UnityEngine;

namespace RPG.Saving
{
    [Serializable]
    public class SerializableVector3
    {
        private float x, y, z;

        public SerializableVector3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }
        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3 ToVector() { return new Vector3(x, y, z); }
    }
}