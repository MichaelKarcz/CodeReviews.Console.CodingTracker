using Spectre.Console;

namespace CodingTracker
{
    internal static class Input
    {
        public static int MainMenu()
        {
            string menuChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("- CODING TRACKER - \nWhich [green]operation[/] would you like to perform?")
                .PageSize(7)
                .AddChoices(new[]
                {
                    "1) [green]View[/] all records",
                    "2) [green]Start[/] a new ongoing session",
                    "3) [green]Finish[/] an ongoing session",
                    "4) [green]Log[/] a new completed session",
                    "5) [yellow]Update[/] an existing record",
                    "6) [red]Delete[/] an existing record",
                    "0) [grey]Exit the application[/]"
                }));

            int menuChoiceNumber = Int32.Parse(menuChoice.Substring(0,1));

            Console.Clear();

            return menuChoiceNumber;
        }

        public static void DisplayAllRecords()
        {
            List<CodingSession> allSessions = SQLHelper.GetAllSessions();

            Console.WriteLine("\n~Coding Sessions~");
            Table table = new Table();
            table.AddColumn(new TableColumn("Id").Centered().NoWrap());
            table.AddColumn(new TableColumn("Start Time").Centered().NoWrap());
            table.AddColumn(new TableColumn("End Time").Centered().NoWrap());
            table.AddColumn(new TableColumn("Duration").Centered().NoWrap());
            foreach (CodingSession session in allSessions)
            {
                table.AddRow(session.Id.ToString(), session.StartTime, session.EndTime, session.Duration);
            }
            table.Border(TableBorder.Heavy);
            table.ShowRowSeparators();
            AnsiConsole.Write(table);

            Console.WriteLine();
        }

        public static void GetNewOngoingSession()
        {
            CodingSession session = new CodingSession();

            bool sessionStartedNow = AnsiConsole.Prompt(
                new TextPrompt<bool>("Did you just start this coding session?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "Yes" : "No"));

            if (!sessionStartedNow)
            {
                session.StartTime = GetStartTime();   
            }
            else
            {
                Console.WriteLine("\nA new coding session has been added!\n");
            }

                SQLHelper.InsertSingleSession(session); // the constructor instantiates the object with DateTime.Now for the StartTime field
        }

        public static void FinishOngoingSession()
        {
            List<CodingSession> unfinishedSessions = SQLHelper.GetUnfinishedSessions();
            if (unfinishedSessions.Count > 0)
            {
                CodingSession unfinishedSession = SelectASessionById(unfinishedSessions);

                bool sessionEndedNow = AnsiConsole.Prompt(
                    new TextPrompt<bool>("Did you just finish this coding session?")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(true)
                    .WithConverter(choice => choice ? "Yes" : "No"));

                if (!sessionEndedNow)
                {
                    unfinishedSession.EndTime = GetEndTime(unfinishedSession.StartTime);
                }
                else
                {
                    unfinishedSession.EndTime = DateTime.Now.ToString("MM-dd-yyyy hh:mm tt");
                }

                unfinishedSession.CalculateDuration();

                if (SQLHelper.UpdateSession(unfinishedSession.Id, unfinishedSession))
                {
                    Console.WriteLine("\nThe coding session has been completed!\n");
                }
            }
            else Console.WriteLine("\nThere are no sessions to update.\n");
        }

        public static void GetNewCompletedSession()
        {
            CodingSession session = new CodingSession();

            session.StartTime = GetStartTime();
            session.EndTime = GetEndTime(session.StartTime);
            session.CalculateDuration();

            SQLHelper.InsertSingleSession(session);
        }

        public static void UpdateSession()
        {
            List<CodingSession> allSessions = SQLHelper.GetAllSessions();
            if (allSessions.Count > 0)
            {
                CodingSession sessionToUpdate = SelectASessionById(allSessions);
                List<string> updatesToMake = new List<string>();

                updatesToMake = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                    .Title("What would you like to [green]update[/] about the session?")
                    .NotRequired()
                    .PageSize(3)
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a selection, " +
                        "and press [green]<enter>[/] to accept)[/]")
                    .AddChoices(new[]
                    {
                        "Start Time", "End Time"
                    }));

                if (updatesToMake.Count > 0)
                {
                    if (updatesToMake.Contains("Start Time"))
                    {
                        sessionToUpdate.StartTime = GetStartTime();
                    }
                    if (updatesToMake.Contains("End Time"))
                    {
                        sessionToUpdate.EndTime = GetEndTime(sessionToUpdate.StartTime);
                    }

                    sessionToUpdate.CalculateDuration();

                    if (SQLHelper.UpdateSession(sessionToUpdate.Id, sessionToUpdate))
                    {
                        Console.WriteLine("\nThe session was updated successfully!\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nNo changes were made.\n");
                }
            }
            else Console.WriteLine("\nThere are no sessions to update.\n");

        }

        public static void DeleteSession()
        {
            List<CodingSession> allSessions = SQLHelper.GetAllSessions();
            if (allSessions.Count > 0)
            {
                CodingSession sessionToDelete = SelectASessionById(allSessions);

                bool confirmDelete = AnsiConsole.Prompt(
                    new TextPrompt<bool>($"Are you sure you'd like to delete this record?"
                                         + $"\n{sessionToDelete.Id}\t|\t{sessionToDelete.StartTime}\t|\t{sessionToDelete.EndTime}\t|\t{sessionToDelete.Duration}")
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(false)
                    .WithConverter(choice => choice ? "Yes" : "No"));

                if (!confirmDelete)
                {
                    Console.WriteLine("\nThe session was not deleted.\n");
                }
                else
                {
                    bool sessionDeleted = SQLHelper.DeleteSession(sessionToDelete.Id);
                    if (sessionDeleted)
                    {
                        Console.WriteLine("The session has been deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("There was an error deleting the record.");
                    }
                }
            }
            else Console.WriteLine("\nThere are no sessions to delete.\n");

        }

        public static string GetStartTime()
        {
            string startTime = AnsiConsole.Prompt(
                new TextPrompt<string>("Please enter the starting time in the format MM-dd-yyyy hh:mm tt where tt is AM or PM ('03-14-2025 03:22 PM' for example): ")
                .Validate<string>(n => Validation.ValidateGenericTime(n)));

            return startTime;
        }

        public static string GetEndTime(string startTime)
        {
            string endTime = AnsiConsole.Prompt(
                new TextPrompt<string>("Please enter the ending time in the format MM-dd-yyyy hh:mm tt where tt is AM or PM ('03-14-2025 03:22 PM' for example): ")
                .Validate<string>(n => Validation.ValidateEndTime(n, startTime)));

            return endTime;
        }

        private static CodingSession SelectASessionById(List<CodingSession> sessions)
        {
            string[] tableRows = new string[sessions.Count];

            int index = 0;

            foreach (CodingSession session in sessions)
            {
                tableRows[index] = $"{session.Id}\t|\t{session.StartTime}\t|\t";
                tableRows[index] += string.IsNullOrEmpty(session.EndTime) ? "\t\t\t|\t\t" : $"{session.EndTime}\t|\t";
                tableRows[index] += session.Duration;
                index++;
            }

            string recordChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Which [green]session[/] would you like to modify?"
                        + "\nId\t|\t\tStart Time\t|\t\tEnd Time\t|\tDuration")
                .PageSize(sessions.Count > 3 ? sessions.Count : 3)
                .MoreChoicesText("[grey](Move up and down to reveal more entries)[/]")
                .AddChoices(tableRows));

            int recordChoiceId = Int32.Parse(recordChoice.Split('|').First().Trim());


            return sessions.Find(e => e.Id == recordChoiceId);
        }
    }
}
