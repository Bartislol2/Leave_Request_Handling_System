using SystemWnioskow;

public class Program
{
    public static void Main(string[] args)
    {
        //utworzenie obiektu klasy SystemObslugiSerwer
        SystemObslugiSerwer serwer = new SystemObslugiSerwer();
        //utworzenie obiektu kierownika
        Kierownik k = new Kierownik("Adrian Nowak");
        k.PokazDane();
        Console.WriteLine("Witaj w systemie obslugi wnioskow urlopowych - proces kierownika");
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
                var newFiles = Directory.GetFiles(serwer.Path, "*z.txt")
                    .Where(file => File.GetLastWriteTime(file) > lastCheck)
                    .ToList();

                if (newFiles.Any())
                {
                    //kod do obsługi nowych plików
                    foreach (var file in newFiles)
                    {
                        //odebranie zlozonego przez pracownika wniosku
                        serwer.Odbierz(k.Repo, file);
                    }
                }
                //aktualizacja czasu ostatniego sprawdzenia
                lastCheck = DateTime.Now;
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                }
            }
        }, token);
        Console.WriteLine("Dostepne opcje: ");
        while (running)
        {
            Console.WriteLine("1 - Przegladaj wnioski");
            Console.WriteLine("2 - Rozpatrz wniosek");
            Console.WriteLine("0 - Wyjdz");
            Console.Write("Wybierz opcje: ");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    k.PokazListeWnioskow();
                    break;
                case "2":
                    Console.WriteLine("Podaj id wniosku: ");
                    var id = Console.ReadLine();
                    bool containsWniosek = k.Repo.Wnioski.Any(w => w.Id == id);
                    if (containsWniosek)
                    {
                        var w = k.Repo.Wnioski.FirstOrDefault(w => w.Id == id);
                        
                        if (w.Status != WniosekUrlopowy.StatusWniosku.Oczekujący)
                        {
                            Console.WriteLine("Wniosek zostal juz rozpatrzony");
                        }
                        else
                        {
                            if (k.RozpatrzWniosek(w))
                            {
                                serwer.Wyslij(w);
                                k.Repo.Wnioski.Remove(w);
                            } 
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nie ma takiego wniosku");
                    }
                    break;
                case "3":
                    tokenSource.Cancel();
                    break;
                case "0":
                    Console.WriteLine("Zamykam proces...");
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    serwer.Dispose();
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
}