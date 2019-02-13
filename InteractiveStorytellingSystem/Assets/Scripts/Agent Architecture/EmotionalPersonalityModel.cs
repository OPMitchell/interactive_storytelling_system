using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    /// <summary>
    /// Class used by each Character object to represent an emotional profile of that character.
    /// Unique emotional profiles are created by reading in each character's named .xml file.
    /// </summary>

    public class EmotionalPersonalityModel
    {
        //Each character has 22 individual emotions. Hold these in an array.
        [XmlElement("PersonalEmotion", typeof(PersonalEmotion))]
        [XmlElement("ExternalEmotion", typeof(ExternalEmotion))]
        public Emotion[] Emotions { get; set; }

        public object GetEmotionValue(EmotionRef emotionName)
        {
            return FindEmotion(emotionName).GetValue();
        }

        public int GetEmotionValue(EmotionRef emotionName, string characterName)
        {
            if ((int)emotionName >= 200)
                return ((ExternalEmotion)FindEmotion(emotionName)).GetExternalEmotionValue(characterName);
            throw new Exception("Expected name of an external emotion, not a personal emotion!");
        }

        public int GetEmotionThreshold(EmotionRef emotionName)
        {
            return FindEmotion(emotionName).Threshold;
        }

        public int GetEmotionDecay(EmotionRef emotionName)
        {
            return FindEmotion(emotionName).Decay;
        }

        private bool Exists(string name, EmotionRef emotion)
        {
            if (name == emotion.ToString())
                return true;
            return false;
        }

        private Emotion FindEmotion(EmotionRef emotion)
        {
            foreach (Emotion e in Emotions)
            {
                if (Exists(e.Name, emotion))
                    return e;
            }
            throw new KeyNotFoundException("Could not find an emotion with that name!");
        }
        
    }
}
