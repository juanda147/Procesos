namespace ProcesosApi.DTOs;

public class CampoGlobalDto
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class CampoGlobalCreateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = "texto";
}

public class CampoGlobalUpdateDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
}

public class CampoPropioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
}
