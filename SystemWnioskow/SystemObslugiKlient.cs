namespace SystemWnioskow;


public class SystemObslugiKlient : ISystemObslugi
{
    public string Path = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().Length - 25);
    //funkcja do wysylania wnioskow
    public void Wyslij(WniosekUrlopowy wniosek)
    {
        //nadanie nazwy plikowi - wniosek_ + Id wniosku pracownika
        var fileName = $"wniosek_{wniosek.Id}z.txt";
        //zapisanie zawartosci wniosku do pliku przy uzyciu StreamWriter
        using (StreamWriter writer = new StreamWriter(Path+fileName))
        {
            writer.WriteLine(wniosek.Id);
            writer.WriteLine(wniosek.ImieINazwisko);
            writer.WriteLine(wniosek.DataZlozenia);
            writer.WriteLine(wniosek.DataRozpoczecia);
            writer.WriteLine(wniosek.DataZakonczenia);
            writer.WriteLine(wniosek.Uzasadnienie);
            writer.WriteLine(wniosek.DniUrlopowePracownika);
            writer.WriteLine(wniosek.Status);
            if (wniosek is WniosekChorobowy wniosekChorobowy)
            {
                writer.WriteLine(wniosekChorobowy.CzyPosiadaZaswiadczenie);
            }
        }
    }
    //funkcja do odbierania wnioskow
    public void Odbierz(Repozytorium<WniosekUrlopowy> repozytorium, string filePath)
    {
        //odczytanie zawartosci pliku przy uzyciu StreamReader i zapisanie go do repozytorium
        using (StreamReader reader = new StreamReader(filePath))
        {
            //odczytanie i zparsowanie wartosci
            string id = reader.ReadLine();
            string name = reader.ReadLine();
            DateTime turnedIn = DateTime.Parse(reader.ReadLine());
            DateTime begin = DateTime.Parse(reader.ReadLine());
            DateTime end = DateTime.Parse(reader.ReadLine());
            string uzasadnienie = reader.ReadLine();
            int dniUrlopowe = int.Parse(reader.ReadLine());
            WniosekUrlopowy.StatusWniosku status = (WniosekUrlopowy.StatusWniosku)Enum.Parse(typeof(WniosekUrlopowy.StatusWniosku), reader.ReadLine());
            string czyPosiadaZaswiadczenie = reader.ReadLine();
            //utworzenie obiektu klasy bazowej WniosekUrlopowy
            WniosekUrlopowy wniosek;
            //jezeli ostatnia odczytana wartosc nie jest null-em - utworzenie obiektu klasy pochodnej - WnioskuChorobowego z odczytanych wartosci
            if (czyPosiadaZaswiadczenie != null)
            {
                wniosek = new WniosekChorobowy(id, name, turnedIn, begin, end, uzasadnienie, dniUrlopowe, czyPosiadaZaswiadczenie);
            }
            //w przeciwnym przypadku, utworzenie wniosku urlopowego z odczytanych wartosci
            else
            {
                wniosek = new WniosekUrlopowy(id, name, turnedIn, begin, end, uzasadnienie, dniUrlopowe);
            }
            //zmiana statusu wniosku na odczytany z pliku
            wniosek.Status = status;
            Console.WriteLine($"\nOtrzymano rozpatrzony wniosek o id: {wniosek.Id}");
            //dodanie wniosku do repozytorium
            repozytorium.Dodaj(wniosek);
        }
    }

    //usuniecie utworzonego folderu po zakonczeniu dzialania
    public void Dispose()
    {
        try
        {
            //Directory.Delete(_path, true);
            var files = Directory.GetFiles(Path, "*.txt").ToList();
            if (files.Any())
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
        }
        catch (IOException)
        {
            Console.WriteLine($"Blad przy probie usuniecia plikow w katalogu: {Path}");
        }
    }
    
}