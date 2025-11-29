using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore.V1;

namespace ProyectoClaseQ4.Services
{
    public class FirebaseServices
    {
        private readonly FirestoreDb _firestoreDb;

        public FirebaseServices(IConfiguration configuration)
        {
            var credentialsPath = configuration["Firebase:CredentialsPath"];
            var projectId = configuration["Firebase:ProjectId"];

            GoogleCredential credential = GoogleCredential.FromFile(credentialsPath);

            var builder = new FirestoreClientBuilder
            {
                Credential = credential
            };

            _firestoreDb = FirestoreDb.Create(projectId, builder.Build());
        }

        public FirestoreDb GetDb()
        {
            return _firestoreDb;
        }

        public CollectionReference GetCollection(string collectionName)
        {
            return _firestoreDb.Collection(collectionName);
        }
    }
}
