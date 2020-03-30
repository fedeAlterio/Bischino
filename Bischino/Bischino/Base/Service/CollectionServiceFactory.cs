using System.Threading.Tasks;
using Bischino.Bischino;
using Microsoft.Extensions.DependencyInjection;
using Bischino.Model;
using Bischino.Settings;

namespace Bischino.Base.Service
{
    public class CollectionServiceFactory
    {
        private readonly IDatabaseSettings _settings;

        public CollectionServiceFactory(IDatabaseSettings settings)
        {
            _settings = settings;
            CollectionService.InitializeMongoDBService(settings);
        }

        public void AddDBServices(IServiceCollection services)
        {
            IUserCollectionService<User> userService = new UserCollectionService<User>();
            services.AddSingleton(userService);

            IOwnedModelCollectionService<Room> roomService = new OwnedModelCollectionService<Room>();
            services.AddSingleton(roomService);
        }
    }
}