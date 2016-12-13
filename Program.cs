using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuzzFeed
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Data Source=10.1.10.148;Initial Catalog=Buzzfeed02; User ID=academy_admin;Password=12345");
            connection.Open();

            Console.WriteLine("Welcome to TRASHY CONSOLE BUZZFEED!");

            //Offer option to log in/get username variable (access users, select relevant user; access sessions, add new session with datetime)
            Console.WriteLine("Would you like to log in? Select [yes] or [no].");
            string userAnswer = Console.ReadLine();

            if (userAnswer == "yes")
            {
                SqlCommand getUsers = new SqlCommand("SELECT * From [Users]", connection);
                SqlDataReader userReader = getUsers.ExecuteReader();
                if (userReader.HasRows)
                {
                    while (userReader.Read())
                    {
                        Console.WriteLine(userReader["UserId"] + ":" + userReader["Username"]);
                    }
                }
                userReader.Close();

                Console.WriteLine("Please enter your username.");
                string userName = Console.ReadLine();

                SqlCommand logIn = new SqlCommand("SELECT * FROM [Users] WHERE ID= " + userName, connection);
            }

            if (userAnswer == "no")
            {
                Console.WriteLine("Welcome, guest!");
            }

            //List tests (show ID & title) -- random test option later (access tests)
            SqlCommand listTests = new SqlCommand("SELECT * from Tests", connection);
            SqlDataReader testReader = listTests.ExecuteReader();

            while (testReader.Read())
            {
                Console.WriteLine($"{testReader["TestId"]} {testReader["Title"]}");
            }

            testReader.Close();

            //User selects their choice 
            Console.WriteLine("Which test would you like? Enter the test ID:");
            string testChoice = Console.ReadLine();

            //Print test title (acesss tests)
            SqlCommand printTest = new SqlCommand($"Select * from Tests WHERE TestId={testChoice}", connection);
            SqlDataReader titleReader = printTest.ExecuteReader();

            if (titleReader.HasRows)
            {
                titleReader.Read();
                Console.WriteLine("You have selected: " + titleReader["Title"]);
            }

            titleReader.Close();

            SqlCommand questionJoin = new SqlCommand($"SELECT * From answers JOIN questions ON Questions.QuestionId = Answers.QuestionId WHERE TestId = {testChoice}", connection);
            SqlDataReader questionReader = questionJoin.ExecuteReader();

            string newQuestion = "";
            int x = 0;
            int finalPoints = 0;
            int[] ansArray = new int[25];

            if (questionReader.HasRows)
            {
                while (questionReader.Read())
                {                    
                        string currentQuestion = questionReader["Question"].ToString();
                        if (currentQuestion != newQuestion)
                        {
                        if (x > 0)
                            {
                                //User selects an answer (added to responses with relevant sessionID)
                                Console.WriteLine("Choose the number associated with your answer:");
                                string answer = Console.ReadLine();
                                ansArray[x] = Convert.ToInt32(answer);
                            }
                        Console.WriteLine(questionReader["Question"]);
                        newQuestion = currentQuestion;
                        x++;
                    }
                    Console.Write($"{questionReader["AnswerID"]} ");
                    Console.WriteLine(questionReader["Answer"]);
                }
            }
            x += 1;
            Console.WriteLine("Choose the number associated with your answer:");
            string answer2 = Console.ReadLine();
            ansArray[x] = Convert.ToInt32(answer2);
            questionReader.Close();

            //Points from answers are added to the total
            for (int i = 0; i < x; i++)
            {
                SqlCommand valueFinder = new SqlCommand($"SELECT Value from Answers WHERE AnswerId={ansArray[x]}", connection);
                SqlDataReader valueReader = valueFinder.ExecuteReader();

                valueReader.Read();
                int addValue = Convert.ToInt32(valueReader["Value"]);

                finalPoints += addValue;

                valueReader.Close();
            }
            //Send total value & return result (access relevant result based on final value)

            SqlCommand valuesReturn = new SqlCommand("SELECT Value from Results", connection);
            SqlDataReader valuesReader = valuesReturn.ExecuteReader();

            int y = 0;

            if (valuesReader.HasRows)
            {
                while (valuesReader.Read())
                {
                    y += 1;
                }
            }

            int[] @valueArray = new int[y];

            for (int i = 0; i < y; i++)
            {
                while (valuesReader.Read())
                {
                    Console.WriteLine(i);
                    valueArray[i] = Convert.ToInt32(valuesReader["Value"]);
                }
            }
            
            int finalValue = 0;
            for (int i = 0; i < y; i++)
            {
                if (finalPoints < valueArray[0])
                {
                    finalValue = valueArray[0];
                }

                else if (finalPoints > valueArray[i] && finalPoints < valueArray[i + 1])
                {
                    if ((finalPoints - valueArray[i]) >= 5)
                    {
                        finalValue = valueArray[i + 1];
                        i = y;
                    }
                    else if ((finalPoints - valueArray[i]) < 5)
                    {
                        finalValue = valueArray[i];
                        i = y;
                    }
                }
                else if (finalPoints > valueArray[y - 1])
                {
                    finalValue = valueArray[y - 1];
                    i = y;
                }
            }

            //Save result to sessions (access sessions)

            connection.Close();
            Console.ReadLine();
        }
    }
}
