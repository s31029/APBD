namespace ConsoleApp1;

public abstract class Kontener
{
    private static int licznik = 1;
    
    public double masa { get; protected set; }   // masa ładunku
    public double MaxWaga { get; protected set; }   // maksymalna ładowność
    public double Wysokosc { get; protected set; }        // wysokość
    public double WagaKontenera { get; protected set; }       // waga własna kontenera
    public double Glebokosc { get; protected set; }         // głębokość
    public string NrSeryjny { get; protected set; }  // numer seryjny

    public Kontener(double maxWaga, double wysokosc, double wagaKontenera, double glebokosc)
    {
        MaxWaga = maxWaga;
        Wysokosc = wysokosc;
        WagaKontenera = wagaKontenera;
        Glebokosc = glebokosc;
        NrSeryjny = "KON-" + typKontenera + "-" + licznik++;
    }
    
    protected abstract string typKontenera { get; }

    public virtual void zaladuj(double masa)
    {
        if (masa > MaxWaga)
        {
            throw new OverfillException("Przekroczona maksymalna ładowność kontenera");
        }
        this.masa = masa;
    }

    public abstract void oproznij();

    public virtual void show()
    {
        Console.WriteLine($"Kontener {NrSeryjny}: ładunek = {masa}kg, maks. ładowność = {MaxWaga}kg");
    }
    
    
}