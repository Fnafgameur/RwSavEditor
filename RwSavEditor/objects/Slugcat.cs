namespace RwSavEditor.objects;

public class Slugcat
{
    private readonly string name;
    private int food;
    private int maxFood;
    private string currentDenPos;
    private string lastDenPos;
    private Karma karma;

    public Slugcat()
    {
        name = "";
        food = 0;
        maxFood = 0;
        currentDenPos = "";
        lastDenPos = "";
        karma = new Karma();
    }

    public Slugcat(string name, int food, int maxFood, string currentDenPos, string lastDenPos, Karma karma)
    {
        this.name = name;
        this.food = food;
        this.maxFood = maxFood;
        this.currentDenPos = currentDenPos;
        this.lastDenPos = lastDenPos;
        this.karma = karma;
    }

    public string Name => name;

    public int Food
    {
        get => food;
        set => food = value;
    }

    public int MaxFood
    {
        get => maxFood;
        set => maxFood = value;
    }

    public string CurrentDenPos
    {
        get => currentDenPos;
        set => currentDenPos = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string LastDenPos
    {
        get => lastDenPos;
        set => lastDenPos = value ?? throw new ArgumentNullException(nameof(value));
    }

    public Karma Karma
    {
        get => karma;
        set => karma = value ?? throw new ArgumentNullException(nameof(value));
    }
}