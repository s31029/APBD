namespace ConsoleApp1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Symulacja działania systemu zarządzania kontenerami.");

            // Utworzenie kontenerowca
            Statek statek = new Statek(maxLiczbaKontenerów: 5, maxWaga: 40, maxPredkosc: 20);

            try
            {
                // Utworzenie kontenera na płyny (niebezpieczny)
                NaPlyny naPlyny = new NaPlyny(
                    maxWaga: 10000,  // w kg
                    wysokosc: 300,         // w cm
                    wagaKontenera: 2000,     // w kg
                    glebokosc: 250,          // w cm
                    czyNiebezpieczny: true
                );
                // Ładujemy 4000 kg (dozwolone, bo 50% z 10000 = 5000)
                naPlyny.zaladuj(4000);
                statek.zaladujKontener(naPlyny);

                // Utworzenie kontenera na gaz
                NaGaz naGaz = new NaGaz(
                    maxWaga: 8000,
                    wysokosc: 290,
                    wagaKontenera: 1800,
                    glebokosc: 240,
                    cisnienie: 2.5       // atm
                );
                naGaz.zaladuj(7500); // ładunek w granicach maxCapacity
                statek.zaladujKontener(naGaz);

                // Utworzenie kontenera chłodniczego
                Chlodniczy chlodniczy = new Chlodniczy(
                    maxWaga: 12000,
                    wysokosc: 310,
                    wagaKontenera: 2200,
                    glebokosc: 260,
                    typProduktu: "Banany",
                    temperatura: 10,           // aktualna temperatura
                    minTemperatura: 8  // minimalna wymagana temperatura
                );
                chlodniczy.zaladuj(10000);
                statek.zaladujKontener(chlodniczy);
            }
            catch (OverfillException ex)
            {
                Console.WriteLine("Błąd załadowania kontenera: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Inny błąd: " + ex.Message);
            }

            // Wyświetlenie informacji o statku i jego ładunku
            statek.showStatek();

            // Przykładowa operacja – opróżnianie kontenera na gaz
            Console.WriteLine("\nSymulacja operacji na kontenerze na gaz:");
            foreach (var k in statek.kontenery)
            {
                if (k is NaGaz)
                {
                    Console.WriteLine("Przed opróżnieniem:");
                    k.show();
                    k.oproznij();
                    Console.WriteLine("Po opróżnieniu (pozostaje 5% ładunku):");
                    k.show();
                }
            }

            // Możliwość rozszerzenia o interfejs konsolowy z menu (pętla while)
            Console.WriteLine("\nSymulacja zakończona.");
            Console.ReadLine();
    }
}