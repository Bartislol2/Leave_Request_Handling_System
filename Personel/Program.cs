using System.ComponentModel.DataAnnotations;
using Personel;
using SystemWnioskow;
public class Program
{
    public static void Main(string[] args)
    {
        //utworzenie listy pracownikow
        List<Pracownik> pracownicy = new List<Pracownik>()
        {
            new Pracownik("Jan Kowalski", 1, 21),
            new Pracownik("Anna Nowak", 2, 15),
            new Pracownik("Zygmunt Zgrzyt", 3, 18),
            new Pracownik("Joanna Jakastam", 4, 13)
        };
        //wypisanie imion i nazwisk pracownikow
        Console.WriteLine("Lista pracownikow: ");
        foreach (var pracownik in pracownicy)
        {
            Console.WriteLine(pracownik.ImieINazwisko);
        }
        //utworzenie obiektu SystemObslugiKlient
        SystemObslugiKlient klient = new SystemObslugiKlient();
        Console.WriteLine("Witaj w systemie obslugi wnioskow urlopowych - proces pracownika");
        //ustawienie aktualnego pracownika na pierwszego z listy
        Pracownik p = pracownicy[0];
        bool running = true;
        //zmienna odpowiedzialna za przechowanie ostatniego czasu sprawdzania katalogu
        DateTime lastCheck = DateTime.MinValue;
        //utworzenie tokenu sluzacego do cancellowania tasku - uzycie CancellationToken
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;
        //Komunikacja automatyczna, programowanie asynchroniczne - Task API, delegaty - metoda anonimowa
        Task.Run(async () =>
        {
            while (running)
            {
                //sprawdzenie czy task zostal scancellowany
                if (token.IsCancellationRequested)
                {
                    return;
                }
                //kod do sprawdzenia pojawienia się nowych plików w katalogu - LINQ i wyrazenie lambda
                var newFiles = Directory.GetFiles(klient.Path, "*r.txt")
                    .Where(file => File.GetLastWriteTime(file) > lastCheck)
                    .ToList();
                if (newFiles.Any())
                {
                    //kod do obsługi nowych plików
                    foreach (var file in newFiles)
                    {
                        using (StreamReader reader = new StreamReader(file))
                        {
                            //odczytanie id pracownika z nowego pliku
                            string idWniosku = reader.ReadLine();
                            int idPracownika = int.Parse(idWniosku.Substring(0, 1));
                            //odebranie rozpatrzonego wniosku przez pracownika, ktory go zlozyl - Delegaty - wyrazenia lambda
                            klient.Odbierz(pracownicy.Find(pracownik=>pracownik.Id == idPracownika).Repo, file);
                            //aktualizacja dostepnych dni urlopowych
                            p.AktualizujDniUrlopowe();
                        }
                    }
                }
                //aktualizacja ostatniego czasu sprawdzenia
                lastCheck = DateTime.Now;
                
                try
                {
                    //zatrzymanie taska na sekunde tak, aby mozliwe bylo jego scancellowanie w trakcie
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }, token);
        while (running)
        {
            p.PokazDane();
            Console.WriteLine("Dostepne opcje: ");
            Console.WriteLine("1 - Utworz wniosek");
            Console.WriteLine("2 - Przegladaj moje wnioski");
            Console.WriteLine("3 - Zmien pracownika");
            Console.WriteLine("0 - Wyjdz");
            Console.Write("Wybierz opcje: "); ;
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    var w = p.UtworzWniosek();
                    if(w != null)
                        DodajWniosek(p, klient, w);
                    break;
                case "2":
                    if (p.Repo.Wnioski.Any())
                    {
                        p.Repo.PrzegladajWnioski(wniosek => wniosek.Przegladaj());
                        bool wybor = true;
                        while (wybor)
                        {
                            Console.WriteLine("Czy chcesz zobaczyc tylko zatwierdzone? T/N");
                            var option1 = Console.ReadLine();
                            switch (option1)
                            {
                                case "T":
                                    p.Repo.WnioskiZatwierdzone();
                                    wybor = false;
                                    break;
                                case "N":
                                    wybor = false;
                                    break;
                                default:
                                    Console.WriteLine("Wybierz wlasciwa opcje!");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nie masz na razie zadnych wnioskow");
                    }
                    break;
                case "3":
                    Console.WriteLine("Podaj swoje imie i nazwisko");
                    var name = Console.ReadLine();
                    var pracownik = pracownicy.Where(pr => pr.ImieINazwisko == name).FirstOrDefault();
                    if (pracownik != null)
                    {
                        p = pracownik;
                    }
                    else
                    {
                        Console.WriteLine("Nie ma takiego pracownika");
                    }
                    break;
                case "0":
                    Console.WriteLine("Zamykam proces...");
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    klient.Dispose();
                    running = false;
                    Task.WaitAll();
                    break;
                default:
                    Console.WriteLine("Wybierz wlasciwa opcje!");
                    break;
            }
            Console.WriteLine();
        }
    }
    private static void DodajWniosek(Pracownik p, SystemObslugiKlient k, WniosekUrlopowy wniosek)
    {
        var context = new ValidationContext(wniosek, null, null);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(wniosek, context, results, true))
        {
            foreach (var result in results)
            {
                Console.WriteLine(result.ErrorMessage);
            }
        }
        else
        {
            p.Repo.Dodaj(wniosek);
            k.Wyslij(wniosek);
        }
    }

}