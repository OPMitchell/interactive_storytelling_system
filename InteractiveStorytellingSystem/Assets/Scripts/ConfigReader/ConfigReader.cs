using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace InteractiveStorytellingSystem.ConfigReader
{
    static class ConfigReader
    {
        public static EmotionalPersonality ReadEmotionData (string personalXMLPath)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(EmotionalPersonality), new XmlRootAttribute { ElementName = "Emotions" });
            if (File.Exists(personalXMLPath))
            {
                using (FileStream myFileStream = new FileStream(personalXMLPath, FileMode.Open))
                {
                    EmotionalPersonality e;
                    e = (EmotionalPersonality)mySerializer.Deserialize(myFileStream);
                    return e;
                }
            }
            else
                throw new FileNotFoundException("Could not find personality file: " + personalXMLPath + "!");
        }

        public static List<Character> ReadCharacterData(string characterListXMLPath)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Character>), new XmlRootAttribute { ElementName = "CharacterList" });
            if (File.Exists(characterListXMLPath))
            {
                using (FileStream myFileStream = new FileStream(characterListXMLPath, FileMode.Open))
                {
                    List<Character> cl;
                    cl = (List<Character>)mySerializer.Deserialize(myFileStream);
                    return cl;
                }
            }
            else
                throw new FileNotFoundException("Could not find character list file: " + characterListXMLPath + "!");
        }

        public static List<Action> ReadActionList(string actionListXMLPath)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Action>), new XmlRootAttribute { ElementName = "Actions" });
            if (File.Exists(actionListXMLPath))
            {
                using (FileStream myFileStream = new FileStream(actionListXMLPath, FileMode.Open))
                {
                    List<Action> al;
                    al = (List<Action>)mySerializer.Deserialize(myFileStream);
                    return al;
                }
            }
            else
                throw new FileNotFoundException("Could not find action list file: " + actionListXMLPath + "!");
        }

        public static List<Dialog> ReadDialogList(string dialogListXMLPath)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Dialog>), new XmlRootAttribute { ElementName = "Dialogs" });
            if (File.Exists(dialogListXMLPath))
            {
                using (FileStream myFileStream = new FileStream(dialogListXMLPath, FileMode.Open))
                {
                    List<Dialog> dl;
                    dl = (List<Dialog>)mySerializer.Deserialize(myFileStream);
                    return dl;
                }
            }
            else
                throw new FileNotFoundException("Could not find dialog list file: " + dialogListXMLPath + "!");
        }

    }
}
