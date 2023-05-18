namespace Personel;

using System.Globalization;
using System.Text.RegularExpressions;
using SystemWnioskow;

public class Pracownik : Osoba
{
    public Repozytorium<WniosekUrlopowy> Repo;
    public int Id;
    private int _nrWniosku = 1;
    public string ImieINazwisko { get; }
    public int DostepneDniUrlopowe { get; set; }
    public override void PokazDane()
    {
        Console.WriteLine("Dane pracownika:");
        Console.WriteLine($"Id: {Id}");
        Console.WriteLine($"Imie i nazwisko: {ImieINazwisko}");
        Console.WriteLine($"Dostepne dni urlopowe: {DostepneDniUrlopowe}");
    }

    public Pracownik(string name, int id, int leaveBalance)
    {
        ImieINazwisko = name;
        Id = id;
        DostepneDniUrlopowe = leaveBalance;
        Repo = new Repozytorium<WniosekUrlopowy>();
    }

    public WniosekUrlopowy UtworzWniosek()
    {
        bool czyChorobowy = false;
        bool wybor = true;
        while (wybor)
        {
            Console.WriteLine("Wybierz typ wniosku: 1 - Urlopowy, 2 - Chorobowy");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    if (Repo.Wnioski.Any())
                    {
                        for (int i = 0; i < Repo.Wnioski.Count; i++)
                        {
                            if (Repo.Wnioski[i].GetType() == typeof(WniosekUrlopowy) &&
                                Repo.Wnioski[i].Status == WniosekUrlopowy.StatusWniosku.Oczekujący)
                            {
                                Console.WriteLine("Masz juz oczekujacy wniosek urlopowy");
                                return null;
                            }
                        }
                    }
                    wybor = false;
                    break;
                case "2":
                    if (Repo.Wnioski.Any())
                    {
                        for (int i = 0; i < Repo.Wnioski.Count; i++)
                        {
                            if (Repo.Wnioski[i].GetType() == typeof(WniosekChorobowy) &&
                                Repo.Wnioski[i].Status == WniosekUrlopowy.StatusWniosku.Oczekujący)
                            {
                                Console.WriteLine("Masz juz oczekujacy wniosek chorobowy");
                                return null;
                            }
                        }
                    }
                    czyChorobowy = true;
                    wybor = false;
                    break;
                default:
                    Console.WriteLine("Wybierz wlasciwa opcje!");
                    break;
            }
        }
        string pattern = @"^\d{2}.\d{2}.\d{4}$";
        Regex regex = new Regex(pattern);
        string format = "dd.MM.yyyy";
        Console.Write("Podaj date rozpoczecia urlopu (dd.MM.yyyy): ");
        var beginDate = Console.ReadLine();
        Match match = regex.Match(beginDate);
        DateTime dataPoczatkowa;
        DateTime dataKoncowa;
        while (!match.Success)
        {
            Console.WriteLine("Podaj poprawny format daty!");
            beginDate = Console.ReadLine();
            match = regex.Match(beginDate);
        }
        while (!(DateTime.TryParseExact(beginDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataPoczatkowa)&&match.Success))
        {
            Console.WriteLine("Podaj poprawny format daty!");
            beginDate = Console.ReadLine();
            match = regex.Match(beginDate);
        }
        Console.Write("Podaj date zakonczenia urlopu (dd.MM.yyyy): ");
        var endDate = Console.ReadLine();
        match = regex.Match(endDate);
        while (!match.Success)
        {
            Console.WriteLine("Podaj poprawny format daty!");
            endDate = Console.ReadLine();
            match = regex.Match(endDate);
        }
        while (!(DateTime.TryParseExact(endDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataKoncowa)&&match.Success))
        {
            Console.WriteLine("Podaj poprawny format daty!");
            endDate = Console.ReadLine();
            match = regex.Match(endDate);
        }
        Console.WriteLine("Podaj uzasadnienie wniosku: ");
        var uzasadnienie = Console.ReadLine();
        string idWniosku = Id.ToString() + "_" + _nrWniosku++.ToString();
        if (czyChorobowy)
        {
            wybor = true;
            string zaswiadczenie ="";
            while (wybor)
            {
                Console.WriteLine("Czy posiadasz zaswiadczenie od lekarza? T/N");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "T":
                        zaswiadczenie = "Tak";
                        wybor = false;
                        break;
                    case "N":
                        zaswiadczenie = "Nie";
                        wybor = false;
                        break;
                    default:
                        Console.WriteLine("Wybierz wlasciwa opcje!");
                        break;
                }
            }
            WniosekUrlopowy w = new WniosekChorobowy(idWniosku, ImieINazwisko, DateTime.Now, dataPoczatkowa, dataKoncowa, uzasadnienie, DostepneDniUrlopowe, zaswiadczenie);
            return w;
        }
        else
        {
            WniosekUrlopowy w = new WniosekUrlopowy(idWniosku, ImieINazwisko, DateTime.Now, dataPoczatkowa, dataKoncowa, uzasadnienie, DostepneDniUrlopowe);
            return w;
        }
    }
    public void AktualizujDniUrlopowe()
    {
        if (Repo.Wnioski.Any())
        {
            if (Repo.Wnioski.Last().Status == WniosekUrlopowy.StatusWniosku.Zatwierdzony)
            {
                DostepneDniUrlopowe = Repo.Wnioski.Last().DniUrlopowePracownika;
            }
        }
    }
}