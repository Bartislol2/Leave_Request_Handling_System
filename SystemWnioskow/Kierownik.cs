namespace SystemWnioskow;

public class Kierownik : Osoba
{
    public string ImieINazwisko { get; }
    public Repozytorium<WniosekUrlopowy> Repo;

    public Kierownik(string name)
    {
        ImieINazwisko = name;
        Repo = new Repozytorium<WniosekUrlopowy>();
    }

    public override void PokazDane()
    {
        Console.WriteLine("Dane kierownika:");
        Console.WriteLine($"Imie i nazwisko: {ImieINazwisko}");
    }
    
    public void PokazListeWnioskow()
    {
        Console.WriteLine("Lista oczekujacych wnioskow:");
        if (Repo.Wnioski.Any())
        {
            foreach (var wniosek in Repo.Wnioski)
            {
                Console.WriteLine(
                    $"Id wniosku: {wniosek.Id}, Imie i nazwisko pracownika: {wniosek.ImieINazwisko}, status: {wniosek.Status}");
            }
        }
        else
        {
            Console.WriteLine("Lista wnioskow jest pusta");
        }
    }

    public bool RozpatrzWniosek(WniosekUrlopowy wniosek)
    {
        bool czyRozpatrzony = false;
        Repo.PrzegladajWniosek(wniosek, wniosek =>wniosek.Przegladaj());
        Console.WriteLine("Co chcesz zrobic z tym wnioskiem?");
        Console.WriteLine("1 - Zatwierdz");
        Console.WriteLine("2 - Odrzuc");
        Console.WriteLine("3 - Powrot do systemu");
        bool running = true;
        while (running)
        {
            Console.Write("Wybierz opcje: ");
            var option = Console.ReadLine();
            switch (option)
            {
                case "1":
                    TimeSpan roznica = wniosek.DataZakonczenia - wniosek.DataRozpoczecia;
                    int iloscDni = roznica.Days;
                    if (wniosek.GetType() == typeof(WniosekUrlopowy))
                    {
                        if (iloscDni > wniosek.DniUrlopowePracownika)
                        {
                            Console.WriteLine("Nie mozesz zatwierdzic tego wniosku, ilosc dni na wniosku przekracza ilosc dostepnych dni urlopowych pracownika");
                        }
                        else if (iloscDni < 0)
                        {
                            Console.WriteLine("Nie mozesz zaakceptowac tego wniosku, ilosc dni jest niepoprawna");
                        }
                        else
                        {
                            wniosek.DniUrlopowePracownika -= iloscDni;
                            wniosek.Status = WniosekUrlopowy.StatusWniosku.Zatwierdzony;
                            running = false;
                            czyRozpatrzony = true;
                            Console.WriteLine($"Zatwierdzono wniosek {wniosek.Id}");
                        }
                    }
                    else if (wniosek is WniosekChorobowy)
                    {
                        if (iloscDni < 0)
                        {
                            Console.WriteLine("Nie mozesz zaakceptowac tego wniosku, ilosc dni jest niepoprawna");
                        }
                        else
                        {
                            wniosek.Status = WniosekUrlopowy.StatusWniosku.Zatwierdzony;
                            running = false;
                            czyRozpatrzony = true;
                            Console.WriteLine($"Zatwierdzono wniosek {wniosek.Id}");
                        }
                    }
                    break;
                case "2":
                    wniosek.Status = WniosekUrlopowy.StatusWniosku.Odrzucony;
                    running = false;
                    czyRozpatrzony = true;
                    Console.WriteLine("Podaj uzasadnienie: ");
                    var uzasadnienie = Console.ReadLine();
                    while (uzasadnienie.Length == 0)
                    {
                        Console.Write("Uzasadnienie nie moze byc puste, podaj uzasadnienie: ");
                        uzasadnienie = Console.ReadLine();
                    }
                    wniosek.Uzasadnienie = uzasadnienie;
                    Console.WriteLine($"Odrzucono wniosek {wniosek.Id}");
                    break;
                case "3":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Podaj wlasciwa opcje!");
                    break;
            }
        }
        return czyRozpatrzony;
    }

}