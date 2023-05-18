namespace SystemWnioskow;

public delegate void Del<in T>(T wniosek) where T : WniosekUrlopowy;
public class Repozytorium<T> where T : WniosekUrlopowy
{
    public List<T> Wnioski { get; }

    public Repozytorium()
    {
        Wnioski = new List<T>();
    }
    public void Dodaj(T wniosek)
    {
        bool containsWniosek = Wnioski.Any(w => w.Id == wniosek.Id);
        if (containsWniosek)
        {
            Wnioski.RemoveAll(w => w.Id == wniosek.Id);
        }
        Wnioski.Add(wniosek);
    }
    public void PrzegladajWnioski(Del<T> del)
    {
        foreach (T wniosek in Wnioski)
        {
            del(wniosek);
        }
    }
    public void PrzegladajWniosek(T wniosek, Del<T> del)
    {
        del(wniosek);
    }
    public void WnioskiZatwierdzone()
    {
        var wnioskiZatwierdzone = from wniosek in Wnioski
            where wniosek.Status == WniosekUrlopowy.StatusWniosku.Zatwierdzony
            select wniosek;

        if (wnioskiZatwierdzone.Any())
        {
            foreach (T wniosek in wnioskiZatwierdzone)
            {
                PrzegladajWniosek(wniosek, wniosek => wniosek.Przegladaj());
            }
        }
        else
        {
            Console.WriteLine("Nie masz zadnych zatwierdzonych wnioskow");
        }
    }
}