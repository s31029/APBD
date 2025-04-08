namespace ConsoleApp1;

public class Statek
{
    public List<Kontener> kontenery { get; private set;} = new List<Kontener>();
    public int MaxLiczbaKontenerów { get; private set; } 
    public double MaxWaga {get; private set;}
    public double MaxPredkosc {get; private set;}

    public Statek(int maxLiczbaKontenerów, double maxWaga, double maxPredkosc)
    {
        MaxLiczbaKontenerów = maxLiczbaKontenerów;
        MaxWaga = maxWaga;
        MaxPredkosc = maxPredkosc;
    }

    public void zaladujKontener(Kontener kontener)
    {
        if (kontenery.Count >= MaxLiczbaKontenerów)
        {
            Console.WriteLine("Nie można dodać kontenera, przekroczono maksymalną liczbę kontenerów");
        }

        double maxDozwolonaWaga = 0;

        foreach (var k in kontenery)
        {
            maxDozwolonaWaga += k.masa + k.WagaKontenera;
        }

        if (maxDozwolonaWaga + kontener.masa + kontener.WagaKontenera > maxDozwolonaWaga * 1000)
        {
            Console.WriteLine("Nie mozna dodać kontenera: przekroczona maksymalna waga ładunku");
        }
        
        kontenery.Add(kontener);
        Console.WriteLine($"Dodano kontener {kontener.NrSeryjny} do statku.");
    }

    public void usunKontener(string nrSeryjny)
    {
        var kontener = kontenery.Find(k => k.NrSeryjny == nrSeryjny);
        if (kontener != null)
        {
            kontenery.Remove(kontener);
            Console.WriteLine($"Usunięto kontener {nrSeryjny} ze statku");
        }
        else
        {
            Console.WriteLine($"Kontener {nrSeryjny} nie został znaleziony");
        }
    }

    public void showStatek()
    {
        Console.WriteLine("=== Informacje o statku ===");
        Console.WriteLine($"Maksymalna prędkość: {MaxPredkosc} węzłów, Maksymalna liczba kontenerów: {MaxLiczbaKontenerów}, Maksymalna waga: {MaxWaga} ton");
        Console.WriteLine("Kontenery na statku:");
        if (kontenery.Count == 0)
        {
            Console.WriteLine("[Brak]");
        }
        else
        {
            foreach (var k in kontenery)
            {
                k.show();
                Console.WriteLine("------------------------");
            }
        }
    }
}