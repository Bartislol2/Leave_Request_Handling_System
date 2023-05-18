using System.ComponentModel.DataAnnotations;

namespace SystemWnioskow;
public class WniosekUrlopowy
{
    public enum StatusWniosku
    {
        Oczekujący,
        Zatwierdzony,
        Odrzucony
    }
    [Required]
    public string Id { get; set; }
    [Required, StringLength(100)]
    public string ImieINazwisko { get; }
    [Required]
    public DateTime DataZlozenia { get; }
    [Required]
    public DateTime DataRozpoczecia { get; }
    [Required]
    public DateTime DataZakonczenia { get;}
    [Required]
    public int RazemDni { get; set; }
    [Required]
    public string Uzasadnienie { get; set; }

    [Required] public int DniUrlopowePracownika { get; set; }
    public StatusWniosku Status { get; set; }

    public WniosekUrlopowy(string id, string name, DateTime turnedIn, DateTime begin, DateTime end, string uzasadnienie, int dni)
    {
        Id = id;
        ImieINazwisko = name;
        DataZlozenia = turnedIn;
        DataRozpoczecia = begin;
        DataZakonczenia = end;
        TimeSpan roznica = DataZakonczenia - DataRozpoczecia;
        RazemDni = roznica.Days;
        Status = StatusWniosku.Oczekujący;
        Uzasadnienie = uzasadnienie;
        DniUrlopowePracownika = dni;
    }

    public virtual void Przegladaj()
    {
        Console.WriteLine($"Id: {Id}");
        Console.WriteLine("Typ wniosku: urlopowy");
        Console.WriteLine($"Imie i nazwisko: {ImieINazwisko}");
        Console.WriteLine($"Data zlozenia wniosku: {DataZlozenia}");
        Console.WriteLine($"Data rozpoczecia urlopu: {DataRozpoczecia}");
        Console.WriteLine($"Data zakonczenia urlopu: {DataZakonczenia}");
        Console.WriteLine($"Razem dni: {RazemDni}");
        Console.WriteLine($"Status wniosku: {Status}");
        Console.WriteLine($"Uzasadnienie: {Uzasadnienie}");
        Console.WriteLine($"Dostepne dni urlopowe: {DniUrlopowePracownika}");
    }
}