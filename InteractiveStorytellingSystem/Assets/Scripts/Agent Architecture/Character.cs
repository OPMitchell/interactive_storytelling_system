using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace InteractiveStorytellingSystem
{
    public class Character : MonoBehaviour
    {
        [SerializeField] public string Name; //name of the character

        public EmotionalPersonality Personality { get; private set; } //emotional personality of the character

        private MemoryPool mp = new MemoryPool();

        void Start()
        {
            if(transform.name == "Bob")
            {
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Bob", "Killed", "Wolf"}, MemoryType.social, 1.0f));
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Wolf", "Injured", "Bob"}, MemoryType.social, 1.0f));
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Wolf", "Injured", "Rachel"}, MemoryType.social, 1.0f));
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Bob", "Ate", "Food"}, MemoryType.social, 1.0f));
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Rachel", "Sing", "Song"}, MemoryType.social, 1.0f));
                mp.AddMemoryPattern(new MemoryPattern(1, new string[]{"Rachel", "Ate", "Food"}, MemoryType.social, 1.0f));

            }
        }


    }
}
