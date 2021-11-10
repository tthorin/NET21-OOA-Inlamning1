namespace OOA_Inlamning1
{
    using System;

    internal static class SqlAnswers
    {
        internal static void Start()
        {
            if (!Helpers.DBHelpers.CheckForDB()) AskToCreateDB();
        }

        private static void AskToCreateDB()
        {
            throw new NotImplementedException();
        }
    }

}
