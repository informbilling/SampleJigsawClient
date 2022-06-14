using InformBilling_JigsawClient_Example.APIClient;
using InformBilling_JigsawClient_Example.Models;

namespace InformBilling_JigsawClient_Example.Services
{
    public class AccountService : IAccountService
    {
        private readonly IInformHttpClientBase _jigsawClient;

        public AccountService(IInformHttpClientBase jigsawClient)
        {
            _jigsawClient = jigsawClient;
        }

        public void GetAccountById()
        {
            try
            {
                var result = Task.Run(() => GetCustomerByIdAsync(1)).Result;
                var account = result.ReturnModel;

                Console.WriteLine("Results:");

                Console.WriteLine($"Id: {account.Id}");
                Console.WriteLine($"AccountNumber: {account.AccountNumber}");
                Console.WriteLine($"Name: {account.Name}");

                Console.ReadLine();
            }
            catch (Exception)
            {
                Console.WriteLine("There was a problem calling the jigsaw API");
            }
        }

        public async Task<SuccessReturnModelDto<AccountDto>> GetCustomerByIdAsync(int accountId)
        {
            var result = await _jigsawClient.GetAsyncObject<SuccessReturnModelDto<AccountDto>>($"v1/Account/{accountId}");
            return result as SuccessReturnModelDto<AccountDto>;
        }


    }
}
