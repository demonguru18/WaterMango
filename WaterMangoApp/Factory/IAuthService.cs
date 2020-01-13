using System.Threading.Tasks;
using WaterMangoApp.Model.AccountModels;
using WaterMangoApp.Model.IdentityModels;

namespace WaterMangoApp.Factory
{
    public interface IAuthService
    {
        Task<TokenResponseModel> Auth(LoginViewModel model);
    }
}