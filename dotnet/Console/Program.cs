using Dotnet.UploadFile;

namespace Implementations.Dotnet.Client {
    public static class Client {
        public static async Task Main(string[] args) {
            Console.WriteLine("HelloWorld");
            if(args.Length <= 0){
                throw new InvalidOperationException("Please provide args");
            }
            var impToTest = args[0];
            switch(impToTest.ToLower()) {
                case "uploadfile":
                    await testUpload(args);
                    break;
                case "uploadsync":
                    testSyncUpload(args);
                    break;
                default:
                    throw new InvalidOperationException("No implementation to test matching provided argument");
            }
        }

        private static async Task testUpload(string[] args) {
            if (args.Length != 8) {
                throw new InvalidOperationException("not enough args provided");
            }
            Console.WriteLine("Testing UploadFile Operation");
            var success = await UploadFile.ExecuteWorkflow.Execute(args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
            Console.WriteLine($"SSIS operation completed with a result of success = {success}");
        }

        private static void testSyncUpload(string[] args) {
            if (args.Length != 8) {
                throw new InvalidOperationException("not enough args provided");
            }
            Console.WriteLine("Testing Sync UploadFile Operation");
            var success = ExecuteSync.Execute(args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
            Console.WriteLine($"upload Sync operation completed with a result of success = {success}");
        }
    }
}