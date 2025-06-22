using Inscription.Entities;

namespace Inscription.Interface
{
    public interface IInscription
    {
        Task<IList<InscriptionList>> ListInscription(string? code);

        Task<bool> DeleteInscription(string code);

        Task<bool> SaveOrUpdateInscription(Entities.Inscription inscription);

    }
}
