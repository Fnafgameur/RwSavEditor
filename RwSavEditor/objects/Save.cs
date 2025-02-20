namespace RwSavEditor.objects;

public class Save
{
    private int seed;
    private Slugcat slugcat;
    private int cyclesPassed;
    private int cyclesSurvived;
    private int cyclesDied;
    private int totalTime;

    public Save()
    {
        seed = 0;
        slugcat = new Slugcat();
        cyclesPassed = 0;
        cyclesSurvived = 0;
        cyclesDied = 0;
        totalTime = 0;
    }

    public Save(int seed, Slugcat slugcat, int cyclesPassed, int cyclesSurvived, int cyclesDied, int totalTime)
    {
        this.seed = seed;
        this.slugcat = slugcat;
        this.cyclesPassed = cyclesPassed;
        this.cyclesSurvived = cyclesSurvived;
        this.cyclesDied = cyclesDied;
        this.totalTime = totalTime;
    }

    public int Seed
    {
        get => seed;
        set => seed = value;
    }

    public Slugcat Slugcat
    {
        get => slugcat;
        set => slugcat = value ?? throw new ArgumentNullException(nameof(value));
    }

    public int CyclesPassed
    {
        get => cyclesPassed;
        set => cyclesPassed = value;
    }

    public int CyclesSurvived
    {
        get => cyclesSurvived;
        set => cyclesSurvived = value;
    }

    public int CyclesDied
    {
        get => cyclesDied;
        set => cyclesDied = value;
    }

    public int TotalTime
    {
        get => totalTime;
        set => totalTime = value;
    }
}