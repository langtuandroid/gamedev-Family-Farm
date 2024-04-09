using UnityEngine.Serialization;

namespace _Project.Scripts_dev.Language
{
    [System.Serializable]
    public class TextLanguage 
    {
        [FormerlySerializedAs("id")] public int ID;
        [FormerlySerializedAs("eng")] public string EnglishVariant;
        [FormerlySerializedAs("vni")] public string VniVariant;
    }
}
