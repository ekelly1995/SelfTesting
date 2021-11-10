using System;
using System.Data.SqlClient;

namespace SelfTestingProgram
{
    class SelfTestMain
    {
        static void Main(string[] args)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=localhost; Initial Catalog=SelfTest; Integrated Security=True");

            // Main menu
            int selection = 0;
            string subSelection = "!!";

            Console.WriteLine("Welcome to the SelfTest Program");
            Console.WriteLine("Do you want to (1) edit/create or view existing questions or (2) take the test?");
            selection = Convert.ToInt32(Console.ReadLine());

            if (selection == 1)
            {
                // Display all available questions
                Console.WriteLine("Now displaying current questions in program:");
                Console.WriteLine("ID\tQuestion\t\t\tAnswer");
                conn.Open();
                string selectQuestionsAndAnswers = @"select QuestionID, Question, Answer from dbo.Question;";
                int selectedID;

                SqlCommand selectQuestionsAndAnswersCMD = new SqlCommand(selectQuestionsAndAnswers, conn);
                SqlDataReader selectReader = selectQuestionsAndAnswersCMD.ExecuteReader();
                while (selectReader.Read())
                {
                    Console.WriteLine(selectReader[0] + "\t" + selectReader[1] + "\t" + selectReader[2]);
                }
                selectReader.Close();

                // Provide option to add and update questions
                Console.Write("\nWould you like to insert or update questions? Enter \"I\" for Insert, \"U\" for Update, or \"X\" to exit the program.");
                subSelection = Console.ReadLine();

                if(subSelection.ToUpper() == "U")
                {
                    Console.Write("Please enter the ID number of the question you would like to update.");
                    selectedID = Convert.ToInt32(Console.ReadLine());

                    // Display record that is going to be updated
                    Console.WriteLine("Record to be updated:");

                    string updatedQuestion;
                    string updatedAnswer;
                    string selectIDCommand = "select QuestionID, Question, Answer from dbo.Question where QuestionID = " + selectedID + ";";
                    SqlCommand selectRecordToUpdateCMD = new SqlCommand(selectIDCommand, conn);
                    SqlDataReader IDReader = selectRecordToUpdateCMD.ExecuteReader();

                    while (IDReader.Read())
                    {
                        Console.WriteLine(IDReader[0] + "\t" + IDReader[1] + "\t" + IDReader[2]);
                    }
                    IDReader.Close();

                    Console.Write("Do you want to update the (Q)uestion or the (A)nswer?");
                    subSelection = "!!";
                    subSelection = Console.ReadLine();

                    if(subSelection.ToUpper() == "Q")
                    {
                        // Get updated value
                        Console.Write("Please enter the updated question text: ");
                        updatedQuestion = Console.ReadLine();

                        // Update question in database


                        // Show the updated record
                    }
                    else if (subSelection.ToUpper() == "A")
                    {
                        // Get updated value


                        // Update answer in database


                        // Show the updated record
                    }
                    else
                    {
                        Console.WriteLine("Please enter valid selection.");
                    }

                }
                else if (subSelection.ToUpper() == "I")
                {
                    // Prompt user for new question


                    // Prompt user for new answer


                    // Insert record into database


                    // Show new record that has been inserted
                }
                else if (subSelection.ToUpper() == "X")
                {
                    Console.WriteLine("Exiting program...");
                    return;
                }
                else
                {
                    Console.WriteLine("Please enter a valid selection.");
                    return;
                }

                conn.Close();

            }
            else if (selection == 2)
            {
                Console.WriteLine("Now beginning test...");
                Console.WriteLine("You will be provided questions and then you will need to type your response. After entering a response, your answer will be checked.");
                Console.WriteLine("At any time if you would like to exit the program, please type \"exit\" in the answer field, then press enter.");
            }
            else
            {
                Console.WriteLine("Please enter a valid menu option.");
                return;
            }

        }
    }
}
