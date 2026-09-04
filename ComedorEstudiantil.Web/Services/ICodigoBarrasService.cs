namespace ComedorEstudiantil.Web.Services
{
    public interface ICodigoBarrasService
    {
        byte[] GenerarPng(string codigo);
    }
}