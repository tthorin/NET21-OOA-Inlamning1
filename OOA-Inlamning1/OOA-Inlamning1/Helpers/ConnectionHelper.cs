namespace OOA_Inlamning1
{
    using System.Configuration;

    public static class ConnectionHelper
    {
        public static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
        
}
