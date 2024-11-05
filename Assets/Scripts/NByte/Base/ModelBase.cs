using UnityEngine;

namespace NByte
{
    public abstract class ModelBase : ScriptableObject
    {
        public string Id;
        public string Name;

        protected virtual void OnValidate()
        {
            string[] values = name.Split(' ');
            Id = values[0];
            if (values.Length > 1)
            {
                Name = values[1];
            }
        }
    }
}