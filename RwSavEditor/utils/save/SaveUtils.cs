namespace RwSavEditor.utils.save;

public class SaveUtils
{
    public static readonly string[] TAG_SV = {"svA&gt;", ";svB&gt"};
    public static readonly string[] TAG_SV_REVERSED = {"svB&gt;", ";svA&gt"};
    public static readonly string[] TAG_PROGDIV = {"progDivA&gt;", ";progDivB&gt"};
    
    public static string CreateStringPattern(string pattern, string[] tagType)
    {
        return tagType[0] + pattern + tagType[1];
    }
}