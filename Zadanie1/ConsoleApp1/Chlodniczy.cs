namespace ConsoleApp1;

public class Chlodniczy : Kontener
{
    public string TypProduktu { get; private set; }
    public double Temperatura { get; private set; }
    public double MinTemperatura { get; private set; }

    public Chlodniczy(double maxWaga, double wysokosc, double wagaKontenera, double glebokosc,
        string typProduktu, double temperatura, double minTemperatura)
        : base(maxWaga, wysokosc, wagaKontenera, glebokosc)
    {
        TypProduktu = typProduktu;
        Temperatura = temperatura;
        MinTemperatura = minTemperatura;
        // Sprawdzamy, czy temperatura kontenera nie jest niższa niż wymagana
        if (Temperatura < MinTemperatura)
        {
            throw new ArgumentException($"Temperatura kontenera ({Temperatura}°C) jest niższa niż wymagana dla produktu {typProduktu} ({MinTemperatura}°C)");
        }
    }

    protected override string typKontenera => "C";

    public override void zaladuj(double masa)
    {
        base.zaladuj(masa);
    }
    
    public override void oproznij()
    {
        masa = 0;
    }

    public override void show()
    {
        base.show();
        Console.WriteLine($"Typ: Chłodniczy, Produkt: {TypProduktu}, Temperatura: {Temperatura}C, MinTemperatura: {MinTemperatura}C");
    }
}