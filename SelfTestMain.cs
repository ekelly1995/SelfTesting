using System;
using System.Data.SqlClient;

namespace SelfTestingProgram
{
    class SelfTestMain
    {
        public int CheckRecordExists(int questionID)
        {
            // Create and Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Select Records with QuestionID
            string selectStatement = $"SELECT 1 FROM dbo.Questions WHERE QuestionID = {questionID};";

            SqlCommand selectCommand = new SqlCommand(selectStatement, conn);
            SqlDataReader countReader = selectCommand.ExecuteReader();

            // Check if DataReader contains rows
            // If it does, return 1
            // If it doesn't, return 0
            if (countReader.HasRows)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }


        // questionID parameter can either be a valid questionID value from the table
        // Or it can be -1 which indicates that all records should be selected from table
        // Acceptable columnFlag values:
            // 0 = All columns
            // 1 = Just the Question Column
            // 2 = Just the Answer Column
        public void SelectData(int questionID, int columnFlag)
        {
            string selectStatement = "";
            string rowToPrint = "";
            string questionIDString = "";
            string questionString = "";
            string answerString = "";

            // Set selectStatement based on columnFlag Value
            if(columnFlag == 0)     // Select All Columns
            {
                selectStatement = "SELECT QuestionID, Question, Answer FROM dbo.Question";
            }
            else if (columnFlag == 1)   // Select Question Column
            {
                selectStatement = "SELECT Question FROM dbo.Question;";
            }
            else if (columnFlag == 2)   // Select Answer Column
            {
                selectStatement = "SELECT Answer FROM dbo.Question;";
            }

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Select Appropriate Data
            if (questionID == -1)    // Get all records from table
            {
                //selectStatement = @"SELECT QuestionID, Question, Answer FROM dbo.Question;";

                SqlCommand selectQuestionsAndAnswersCMD = new SqlCommand(selectStatement, conn);
                SqlDataReader selectReader = selectQuestionsAndAnswersCMD.ExecuteReader();
                if (selectReader.HasRows)
                {
                    while (selectReader.Read())
                    {
                        questionIDString = Convert.ToString(selectReader[0]);
                        questionString = Convert.ToString(selectReader[1]);
                        answerString = Convert.ToString(selectReader[2]);

                        rowToPrint = String.Format("{0,-15}{1,-40}{2,-40}", (Convert.ToString(selectReader[0])), (Convert.ToString(selectReader[1])), (Convert.ToString(selectReader[2])));
                        Console.WriteLine(rowToPrint);
                    }
                }
                else
                {
                    Console.WriteLine("Query returned no records. Please try again with valid record values.");
                    return;
                }
                
                selectReader.Close();
            }
            else if (questionID > 0)    // Get only one record from table by QuestionID
            {
                selectStatement = selectStatement + $" WHERE QuestionID = {questionID};";

                SqlCommand selectQuestionsAndAnswersCMD = new SqlCommand(selectStatement, conn);
                SqlDataReader selectReader = selectQuestionsAndAnswersCMD.ExecuteReader();
                if(selectReader.HasRows)
                {
                    while (selectReader.Read())
                    {
                        questionIDString = Convert.ToString(selectReader[0]);
                        questionString = Convert.ToString(selectReader[1]);
                        answerString = Convert.ToString(selectReader[2]);

                        rowToPrint = String.Format("{0,-15}{1,-40}{2,-40}", (Convert.ToString(selectReader[0])), (Convert.ToString(selectReader[1])), (Convert.ToString(selectReader[2])));
                        Console.WriteLine(rowToPrint);
                    }
                }
                else
                {
                    Console.WriteLine("Query returned no records. Please try again with valid record values.");
                    return;
                }
                
                selectReader.Close();
            }

            return;
        }

        public int InsertData(string question, string answer)
        {
            string insertStatement;
            string selectStatement;
            int questionID = -1;

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Insert data into table
            insertStatement = $"INSERT INTO dbo.Question (Question, Answer) VALUES ('{question}','{answer}');";

            SqlCommand insertCommand = new SqlCommand(insertStatement, conn);
            insertCommand.ExecuteNonQuery();
            Console.WriteLine("Record has been successfully inserted.");

            // Get QuestionID value of newly created record
            selectStatement = $"SELECT QuestionID FROM dbo.Question WHERE Question = '{question}' AND Answer = '{answer}';";

            SqlCommand selectCommand = new SqlCommand(selectStatement, conn);
            SqlDataReader selectReader = selectCommand.ExecuteReader();

            if (selectReader.HasRows)
            {
                questionID = Convert.ToInt32(selectReader[0]);
            }

            conn.Close();

            return questionID;
            // If there was an issue with the insert/select, the value of questionID will still be -1

        }

        public void UpdateData(int questionID, string question, string answer)
        {
            string updateStatement;

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Check that record exists with provided questionID before running update query
            string checkExists = $"SELECT questionID FROM dbo.Question WHERE QuestionID = {questionID};";
            SqlCommand checkExistsCmd = new SqlCommand(checkExists, conn);
            SqlDataReader existsReader = checkExistsCmd.ExecuteReader();

            if(existsReader.HasRows)    // If the record exists in table, update
            {
                SqlCommand updateCommand = new SqlCommand();

                if (question == "" && answer != "")  // Updating just the answer
                {
                    updateStatement = $"UPDATE dbo.Question SET Answer = '{answer}' WHERE QuestionID = {questionID};";
                    updateCommand.Connection = conn;
                    updateCommand.CommandText = updateStatement;
                    updateCommand.ExecuteNonQuery();
                }
                else if (question != "" && answer == "") // Updating just the question
                {
                    updateStatement = $"UPDATE dbo.Question SET Question = '{question}' WHERE QuestionID = {questionID};";
                    updateCommand.Connection = conn;
                    updateCommand.CommandText = updateStatement;
                    updateCommand.ExecuteNonQuery();
                }
                else if (question != "" && answer != "") // Updating question and answer
                {
                    updateStatement = $"UPDATE dbo.Question SET Question = '{question}', Answer = '{answer}' WHERE QuestionID = {questionID};";
                    updateCommand.Connection = conn;
                    updateCommand.CommandText = updateStatement;
                    updateCommand.ExecuteNonQuery();
                }
                else if (question == "" && answer == "") // No update information provided
                {
                    Console.WriteLine("No useful update information provided. No actions performed.");
                    return;
                }

                conn.Close();
                return;
            }
            else    // If record does not exist, exit method
            {
                Console.WriteLine("Specified record to update does not exist. Please try again with valid record values.");
                return;
            }

            
        }

        public void DeleteData(int questionID)
        {
            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            string deleteStatement = $"DELETE FROM dbo.Question WHERE QuestionID = {questionID};";

            SqlCommand deleteCommand = new SqlCommand(deleteStatement, conn);
            deleteCommand.ExecuteNonQuery();

            conn.Close();
            return;
        }

        public int checkAnswer(int questionID, string userAnswer)
        {
            string storedAnswer = "";
            string getAnswerStatement = $"SELECT Answer FROM dbo.Question WHERE QuestionID = {questionID};";

            // Create & open database connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Get answer from DB for QuestionID
            SqlCommand getDataCommand = new SqlCommand(getAnswerStatement, conn);
            SqlDataReader selectReader = getDataCommand.ExecuteReader();

            if(selectReader.HasRows)
            {
                storedAnswer = Convert.ToString(selectReader[0]);
            }
            else
            {
                Console.WriteLine("There was an issue with reading from the database.");
            }

            // Compare stored answer to user's answer
                // Incorrect = 0
                // Correct = 1
            if(storedAnswer == userAnswer)  // User was correct
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }

        public int GetMaxQuestionID()
        {
            int maxQuestionID = 0;
            string selectMaxQuestionIDQuery = "SELECT MAX(QuestionID) as MaxQuestionID FROM dbo.Question;";

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Execute query on database
            SqlCommand selectStatement = new SqlCommand(selectMaxQuestionIDQuery, conn);
            SqlDataReader selectReader = selectStatement.ExecuteReader();

            if (selectReader.HasRows)
            {
                maxQuestionID = Convert.ToInt32(selectReader[0]);
            }
            else
            {
                Console.WriteLine("Something went wrong when trying to get the max QuestionID value.");
            }

            // Close Database Connection
            conn.Close();

            return maxQuestionID;
        }

        public int GetMinQuestionID()
        {
            int minQuestionID = 0;
            string selectMinQuestionIDQuery = "SELECT MIN(QuestionID) as MinQuestionID FROM dbo.Question;";

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Execute query on database
            SqlCommand selectStatement = new SqlCommand(selectMinQuestionIDQuery, conn);
            SqlDataReader selectReader = selectStatement.ExecuteReader();

            if(selectReader.HasRows)
            {
                minQuestionID = Convert.ToInt32(selectReader[0]);
            }
            else
            {
                Console.WriteLine("Something went wrong when trying to get the min QuestionID value.");
            }

            // Close Database Connection
            conn.Close();

            return minQuestionID;
        }


        static void Main(string[] args)
        {

            // Declare Variables
            int menuSelection = 0;
            string subMenuSelection = "-1";
            string newQuestion;
            string newAnswer;

            // Start Main Menu
            Console.WriteLine("Welcome to the SelfTest Program, where you can write and take your own custom tests.");
            Console.WriteLine("Press (1) to take the test with existing questions, (2) to edit the test, or (3) to review existing test questions and answers:");
            menuSelection = Convert.ToInt32(Console.ReadLine());

            // Three selection paths
            if(menuSelection == 1)
            {
                Console.WriteLine("You are about to begin the self test. To exit at any time, type \"exit\" in the answer field.");

                // Get the min and max QuestionID values
                SelfTestMain functionCall = new SelfTestMain();

                int minQuestionID = functionCall.GetMinQuestionID();
                int maxQuestionID = functionCall.GetMaxQuestionID();
                int idExists = 0;
                int wasUserCorrect = 0;
                string userAnswer = "";

                // Loop through all questions by QuestionID value
                for (int currentQuestionID = minQuestionID; currentQuestionID <= maxQuestionID; currentQuestionID++)
                {
                    // Verify that record exists with provided QuestionID value
                    // If it does not, continue loop at next value
                    idExists = functionCall.CheckRecordExists(currentQuestionID);
                    if(idExists == 0)   // Record doesn't exist
                    {
                        continue;   // Go to next QuestionID value with loop
                    }
                    else    // Record exists, display question
                    {
                        // Display the question
                        functionCall.SelectData(currentQuestionID, 1);

                        // Prompt user to enter an answer
                        Console.Write("Answer: ");
                        userAnswer = Console.ReadLine();

                        // Check if user has entered "exit" to stop test
                        if (userAnswer.ToUpper() != "EXIT")
                        {
                            // Check if user's answer was correct
                            wasUserCorrect = functionCall.checkAnswer(currentQuestionID, userAnswer);

                            // Display correct answer and user's answer and inform them if they were correct
                            Console.Write("Correct Answer: ");
                            functionCall.SelectData(currentQuestionID, 2);
                            
                            Console.WriteLine($"Your answer: {userAnswer}");
                            if(wasUserCorrect == 0)
                            {
                                Console.WriteLine("You were incorrect.");
                            }
                            else
                            {
                                Console.WriteLine("You were correct!");
                            }

                        }
                        else    //User wants to exit
                        {
                            return;
                        }
                    }
                }


            }
            else if (menuSelection == 2)
            {
                Console.WriteLine("All current questions and their corresponding answers will be displayed here shortly.");
                Console.WriteLine("Please enter the ID value of the question you would like to edit, or type \"NEW\" to create a new question:");
                subMenuSelection = Console.ReadLine();

                // Check if user wants to update existing question or add a new question
                if (subMenuSelection.ToUpper() == "NEW")
                {
                    int newQuestionID = 0;
                    int checkRecordExists = -1;

                    Console.WriteLine("You have selected to add a new question to the test.");
                    Console.Write("Question: ");
                    newQuestion = Console.ReadLine();
                    Console.Write("Answer: ");
                    newAnswer = Console.ReadLine();

                    // Call method to insert question into database
                    // Insert method returns the ID value of the question that is added
                    SelfTestMain functionCall = new SelfTestMain();
                    newQuestionID = functionCall.InsertData(newQuestion, newAnswer);

                    // Validate that question was created successfully by selecting record from database and showing it to user
                    if(newQuestionID != -1) // If the QuestionID exists, display the record
                    {
                        Console.WriteLine("New question was successfully entered: ");
                        functionCall.SelectData(newQuestionID,0);
                    }
                    else
                    {
                        Console.WriteLine("There was an issue with inserting the record into the database. Please try again.");
                    }

                }
                else if (subMenuSelection == "-1")   // No selection entered
                {
                    Console.WriteLine("Please enter a valid value to use the program. Value values are \"NEW\" or a number greater than 0.");
                }
                else    // User has entered the ID of a specific question to update
                {
                    Console.Write($"Please enter the new Question value for ID {subMenuSelection}, or hit Enter to keep Question value as-is: ");
                    newQuestion = Console.ReadLine();

                    Console.Write($"Please enter the new Answer value for ID {subMenuSelection}, or hit Enter to keep Answer value as-is: ");
                    newAnswer = Console.ReadLine();

                    // Check if user entered Question and/or Answer values or just hit Enter
                    if(newQuestion != "" || newAnswer != "")    // If either a new question or answer value was specified, run the UpdateData function
                    {
                        int updateQuestionID = Convert.ToInt32(subMenuSelection);

                        // Display Values Before Updating
                        Console.WriteLine($"Current values of specified record: ");
                        SelfTestMain functionCall = new SelfTestMain();
                        functionCall.SelectData(updateQuestionID,0);

                        // Update the record
                        functionCall.UpdateData(updateQuestionID, newQuestion, newAnswer);

                        Console.WriteLine("Record has been updated to the follow values: ");
                        // Display Values After Updating
                        functionCall.SelectData(updateQuestionID,0);
                    }
                    else if(newQuestion == "" && newAnswer == "")   // If no new values were supplied to use to update ther records, let user know and end program
                    {
                        Console.WriteLine("You did not enter any values to be updated. Please enter values to update for the specified ID.");
                        return;
                    }
                }
            }
            else if (menuSelection == 3)    // Show all existing questions and answers in the database
            {
                string headerRow = "";
                Console.WriteLine("Now displaying all existing questions and answers for you to review:");
                headerRow = String.Format("{0,-15}{1,-40}{2,-40}", "QuestionID", "Question", "Answer");
                Console.WriteLine(headerRow);

                int questionIDSelectAll = -1;
                SelfTestMain functionCall = new SelfTestMain();
                functionCall.SelectData(questionIDSelectAll,0);
            }
            else
            {
                Console.WriteLine("Please enter a valid value to use the program");
                return;
            }
            

        }
    }
}
