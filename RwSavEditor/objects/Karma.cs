namespace RwSavEditor.objects;

public class Karma
{
    public const int MAX_KARMA_CAP = 10;
    
    private int currentKarma;
    private int maxKarma;
    private bool isReinforced;

    public Karma()
    {
    }

    public Karma(int currentKarma, int maxKarma, bool isReinforced)
    {
        this.currentKarma = currentKarma;
        this.maxKarma = maxKarma;
        this.isReinforced = isReinforced;
    }

    public int CurrentKarma
    {
        get => currentKarma;
        set => currentKarma = value;
    }

    public int MaxKarma
    {
        get => maxKarma;
        set => maxKarma = value;
    }

    public bool IsReinforced
    {
        get => isReinforced;
        set => isReinforced = value;
    }
}