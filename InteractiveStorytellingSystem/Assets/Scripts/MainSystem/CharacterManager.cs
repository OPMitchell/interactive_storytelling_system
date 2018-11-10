using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace InteractiveStorytellingSystem
{
    /// <summary>
    /// Static class that holds data about all characters in the game.
    /// Characters are defined in CharacterList.xml and the total number
    /// of characters is the number of <character/> entries in CharacterList.xml
    /// </summary>
    static class CharacterManager
    {
        //constant path to CharacterList.xml file 
        private const string characterListXMLPath = "CharacterList.xml";

        //A list of Character objects representing each character in the game
        private static List<Character> characters = new List<Character>();

        /// <summary>
        /// Reads in CharacterList.xml and creates a list of Character objects.
        /// </summary>
        public static void CreateCharacters()
        {
            //Use ConfigReader helper class to read in character data from CharacterList.xml
            characters = ConfigReader.ConfigReader.ReadCharacterData(characterListXMLPath);
            foreach(Character c in characters)
            {
                c.CreatePersonality();
                c.CreateActionList();
            }
        }

        public static Character GetCharacterByName(string name)
        {
            foreach(Character c in characters)
            {
                if (c.Name == name)
                    return c;
            }
            return null;
        }

        public static Character GetCharacterByIndex(int index)
        {
            return characters[index];
        }
    }
}
