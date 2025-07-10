namespace CodingTracker
{
    public class Program()
    {
        public static void Main(string[] args)
        {
            SqlHelper.CreateDatabaseIfNotExists();

            bool runProgram = true;
            while (runProgram)
            {
                int menuChoice = Input.MainMenu();

                switch (menuChoice)
                {
                    case 0:
                        runProgram = false;
                        break;
                    case 1:
                        Input.DisplayAllRecords();
                        break;
                    case 2:
                        Input.GetNewOngoingSession();
                        break;
                    case 3:
                        Input.FinishOngoingSession();
                        break;
                    case 4:
                        Input.GetNewCompletedSession();
                        break;
                    case 5:
                        Input.UpdateSession();
                        break;
                    case 6:
                        Input.DeleteSession();
                        break;
                    default:
                        Console.WriteLine("\nError in processing selection. The app will now close.");
                        runProgram = false;
                        break;
                }
            }

            Console.WriteLine("\n\nGoodbye!");
            Console.ReadKey();
        }
    }
}