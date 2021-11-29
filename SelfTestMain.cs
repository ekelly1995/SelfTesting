using System;
using System.Data.SqlClient;

namespace SelfTestingProgram
{
    class SelfTestMain
    {
        //public int CheckRecordExists(int questionID)
        //{
        //    // Create and Open Database Connection
        //    SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
        //    conn.Open();

        //    // Select Records with QuestionID
        //    string selectStatement = $"SELECT 1 FROM dbo.Questions WHERE QuestionID = {questionID};";

        //    SqlCommand selectCommand = new SqlCommand(selectStatement, conn);
        //    SqlDataReader countReader = selectCommand.ExecuteReader();

        //    // Check if DataReader contains rows
        //    // If it does, return 1
        //    // If it doesn't, return 0
        //    if (countReader.HasRows)
        //    {
        //        return 1;
        //    }
        //    else
        //    {
        //        return 0;
        //    }

        //}
        
        
        // questionID parameter can either be a valid questionID value from the table
        // Or it can be -1 which indicates that all records should be selected from table
        public void SelectData(int questionID)
        {
            string selectStatement;

            // Create & Open Database Connection
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");
            conn.Open();

            // Select Appropriate Data
            if (questionID == -1)    // Get all records from table
            {
                selectStatement = @"SELECT QuestionID, Question, Answer FROM dbo.Question;";

                SqlCommand selectQuestionsAndAnswersCMD = new SqlCommand(selectStatement, conn);
                SqlDataReader selectReader = selectQuestionsAndAnswersCMD.ExecuteReader();
                if (selectReader.HasRows)
                {
                    while (selectReader.Read())
                    {
                        Console.WriteLine(selectReader[0] + "\t" + selectReader[1] + "\t" + selectReader[2]);
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
                selectStatement = $"SELECT QuestionID, Question, Answer FROM dbo.Question WHERE QuestionID = {questionID};";

                SqlCommand selectQuestionsAndAnswersCMD = new SqlCommand(selectStatement, conn);
                SqlDataReader selectReader = selectQuestionsAndAnswersCMD.ExecuteReader();
                if(selectReader.HasRows)
                {
                    while (selectReader.Read())
                    {
                        Console.WriteLine(selectReader[0] + "\t" + selectReader[1] + "\t" + selectReader[2]);
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
                Console.WriteLine("You are about to begin the self-testing program. To exit at any time, type \"exit\" in the answer field.");

                /*
                 * 
                 * ADD STUFF HERE
                 * 
                 */
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
                        functionCall.SelectData(newQuestionID);
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
                        functionCall.SelectData(updateQuestionID);

                        // Update the record
                        functionCall.UpdateData(updateQuestionID, newQuestion, newAnswer);

                        Console.WriteLine("Record has been updated to the follow values: ");
                        // Display Values After Updating
                        functionCall.SelectData(updateQuestionID);
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
                Console.WriteLine("Now displaying all existing questions and answers for you to review:");

                int questionIDSelectAll = -1;
                SelfTestMain functionCall = new SelfTestMain();
                functionCall.SelectData(questionIDSelectAll);
            }
            else
            {
                Console.WriteLine("Please enter a valid value to use the program");
                return;
            }
            

        }
    }
}
