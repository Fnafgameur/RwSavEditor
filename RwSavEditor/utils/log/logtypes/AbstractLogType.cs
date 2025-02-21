namespace RwSavEditor.utils.logs.logtypes;

public abstract class AbstractLogType
{
    private readonly string prefix;
    private readonly ConsoleColor textColor;
    
    public AbstractLogType(string prefix, ConsoleColor textColor)
    {
        this.prefix = prefix;
        this.textColor = textColor;
    }

    public string Prefix => prefix;

    public ConsoleColor TextColor => textColor;
}