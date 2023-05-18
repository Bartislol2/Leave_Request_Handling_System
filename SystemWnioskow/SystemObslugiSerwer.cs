namespace SystemWnioskow;


class SystemObslugiSerwer : ISystemObslugi
{
    //deklaracja sciezek folderu wlasnego oraz klienta
    public string Path = Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().Length - 31);
    
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
            //dodanie wniosku do repozytorium
            Console.WriteLine($"\nOtrzymano nowy wniosek o id: {wniosek.Id}");
            repozytorium.Dodaj(wniosek);
        }
    }
    //funkcja do wysylania wnioskow
    public void Wyslij(WniosekUrlopowy wniosekUrlopowy)
    {
        //nadanie nazwy plikowi - wniosek_ + Id wniosku pracownika
        var fileName = $"wniosek_{wniosekUrlopowy.Id}r.txt";
        using (StreamWriter writer = new StreamWriter(Path+fileName))
        {
            //zapisanie zawartosci wniosku do pliku przy uzyciu StreamWriter
            writer.WriteLine(wniosekUrlopowy.Id);
            writer.WriteLine(wniosekUrlopowy.ImieINazwisko);
            writer.WriteLine(wniosekUrlopowy.DataZlozenia);
            writer.WriteLine(wniosekUrlopowy.DataRozpoczecia);
            writer.WriteLine(wniosekUrlopowy.DataZakonczenia);
            writer.WriteLine(wniosekUrlopowy.Uzasadnienie);
            writer.WriteLine(wniosekUrlopowy.DniUrlopowePracownika);
            writer.WriteLine(wniosekUrlopowy.Status);
            if (wniosekUrlopowy is WniosekChorobowy wniosekChorobowy)
            {
                writer.WriteLine(wniosekChorobowy.CzyPosiadaZaswiadczenie);
            }
        }
    }
    //usuniecie utworzonego folderu po zakonczeniu dzialania
    public void Dispose()
    {
        try
        {
            var files = Directory.GetFiles(Path, "*r.txt").ToList();
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
            Console.WriteLine($"Blad przy probie usuniecia plikow z katalogu: {Path}");
        }
    }
}