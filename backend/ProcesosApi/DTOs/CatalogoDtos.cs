namespace ProcesosApi.DTOs;

public class CatalogoDto
{
    public string Id { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class CatalogoCreateDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}

public class CatalogoUpdateDto
{
    public string Valor { get; set; } = string.Empty;
}
