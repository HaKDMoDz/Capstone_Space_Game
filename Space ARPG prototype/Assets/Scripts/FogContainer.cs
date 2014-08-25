using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

[XmlRoot("MonsterCollection")]
public class FogContainer
{
    [XmlArray("ActiveFogSquares"), XmlArrayItem("FogSquare")]
    public FogSquare[] FogSquares;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(FogContainer));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static FogContainer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(FogContainer));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as FogContainer;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static FogContainer LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(FogContainer));
        return serializer.Deserialize(new StringReader(text)) as FogContainer;
    }
}