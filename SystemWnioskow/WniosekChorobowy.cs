namespace SystemWnioskow;

public class WniosekChorobowy: WniosekUrlopowy
{
    public string CzyPosiadaZaswiadczenie;

    public WniosekChorobowy(string id, string name, DateTime turnedIn, DateTime startDate, DateTime endDate, string uzasadnienie, int dni, string zaswiadczenie) : base(id, name, turnedIn, startDate, endDate, uzasadnienie, dni)
    {
        CzyPosiadaZaswiadczenie = zaswiadczenie;
    }

    public override void Przegladaj()
    {
        Console.WriteLine($"Id: {Id}");
        Console.WriteLine("Typ wniosku: chorobowy");
        Console.WriteLine($"Imie i nazwisko: {ImieINazwisko}");
        Console.WriteLine($"Data zlozenia wniosku: {DataZlozenia}");
        Console.WriteLine($"Data rozpoczecia zwolnienia: {DataRozpoczecia}");
        Console.WriteLine($"Data zakonczenia zwolnienia: {DataZakonczenia}");
        Console.WriteLine($"Razem dni: {RazemDni}");
        Console.WriteLine($"Status wniosku: {Status}");
        Console.WriteLine($"Uzasadnienie: {Uzasadnienie}");
        Console.WriteLine($"Dostepne dni urlopowe: {DniUrlopowePracownika}");
        Console.WriteLine($"Zaswiadczenie: {CzyPosiadaZaswiadczenie}");
    }
}