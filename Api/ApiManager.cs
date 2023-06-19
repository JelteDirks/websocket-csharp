namespace Api
{
    public class ApiManager
    {
        public ApiManager()
        {
        }

        public void RegisterHandler()
        {
            Console.WriteLine("handler registered");
        }

        public string HandleMessage()
        {
            return "";
        }

        public bool ClientValidation()
        {
            Console.WriteLine("client is being validated by the api manager");
            return true;
        }
    }
}
