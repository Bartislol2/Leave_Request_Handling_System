namespace SystemWnioskow;

public interface ISystemObslugi : IDisposable
{
    public void Wyslij(WniosekUrlopowy wniosek);
    public void Odbierz(Repozytorium<WniosekUrlopowy> repozytorium, string filePath);
    
}