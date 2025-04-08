namespace ConsoleApp1;

public class NaGaz : Kontener, IHazardNotifier
{
    public double Cisnienie { get; private set; } // ciśnienie w atmosferach

    public NaGaz(double maxWaga, double wysokosc, double wagaKontenera, double glebokosc, double cisnienie)
        : base(maxWaga, wysokosc, wagaKontenera, glebokosc)
    {
        Cisnienie = cisnienie;
    }

    protected override string typKontenera => "G";

    public override void zaladuj(double masa)
    {
        base.zaladuj(masa);
    }
    
    public override void oproznij()
    {
        masa = masa * 0.05;
    }

    public void NotifyHazard(string nrKontenera, string message)
    {
        Console.WriteLine($"Alert! Kontener {nrKontenera}: {message}");
    }

    public override void show()
    {
        base.show();
        Console.WriteLine($"Typ: Gaz, Ciśnienie: {Cisnienie} atm");
    }
}