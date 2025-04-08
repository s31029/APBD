namespace ConsoleApp1;

public class NaPlyny : Kontener, IHazardNotifier
{
    public bool CzyNiebezpieczny { get; private set; }

    public NaPlyny(double maxWaga, double wysokosc, double wagaKontenera, double glebokosc, bool czyNiebezpieczny)
        : base(maxWaga, wysokosc, wagaKontenera, glebokosc)
    {
        CzyNiebezpieczny = czyNiebezpieczny;
    }

    protected override string typKontenera => "L";

    public override void zaladuj(double masa)
    {
        double maxPojemnosc = CzyNiebezpieczny ? 0.5 * MaxWaga : 0.9 * MaxWaga;
        if (masa > maxPojemnosc)
        {
            NotifyHazard(NrSeryjny, $"Próba załadowania {masa}kg przekracza dozwolony limit {maxPojemnosc}kg");
            throw new OverfillException($"Przekroczony limit dla kontenera {NrSeryjny}");
        }
        base.zaladuj(masa);
    }
    public override void oproznij()
    {
        masa = 0;
    }

    public void NotifyHazard(string nrKontenera, string message)
    {
        Console.WriteLine($"Alert! Kontener {nrKontenera}: {message}");
    }

    public override void show()
    {
        base.show();
        Console.WriteLine($"Typ: Płyn, Niebezpieczny: {CzyNiebezpieczny}");
    }
}