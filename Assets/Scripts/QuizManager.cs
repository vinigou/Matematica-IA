using System;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager
{
    public static Quiz quiz = new();
    public static QuizIndex index;
    public static Performance performance;
    public static int currentQuestion;

    public static event Action QuestionAnswered;
    public static event Action QuizEnd;

    public static void StartQuiz()
    {
        performance = new()
        {
            totalQuestions = quiz.questions.Count,
            correctAnswers = new bool[quiz.questions.Count],
            totalCorrect = 0,
            selectedAnswers = new int[quiz.questions.Count]
        };


    }

    public static void SetQuiz(Quiz newQuiz, QuizIndex quizIndex)
    {
        quiz = newQuiz;
        index = quizIndex;
        currentQuestion = 0;
    }

    public static bool HandleAnswer(int answer)
    {
        Debug.Log(answer);
        performance.selectedAnswers[currentQuestion] = answer;

        if (quiz.questions[currentQuestion].correctAnswer == answer)
        {
            performance.correctAnswers[currentQuestion] = true;
            performance.totalCorrect++;
        }
        else
        {
            performance.correctAnswers[currentQuestion] = false;
        }

        currentQuestion++;

        if (currentQuestion < quiz.questions.Count)
        {
            return false; // Show next question
        }
        else
        {
            EndQuiz();
            return true;
        }
    }

    private static void EndQuiz()
    {
        QuizEnd?.Invoke();
    }

    public static List<Question> GenerateTestQuiz()
    {
        string json = "{\"questions\":[\r\n{\"question\":\"Qual é a figura que tem quatro lados iguais?\",\r\n\"answer1\":\"Triângulo\",\r\n\"answer2\":\"Quadrado\",\r\n\"answer3\":\"Círculo\",\r\n\"answer4\":\"Losango\",\r\n\"correctAnswer\":2},\r\n{\"question\":\"Qual é a medida de um metro?\",\r\n\"answer1\":\"76,2 centímetros\",\r\n\"answer2\":\"100 centímetros\",\r\n\"answer3\":\"25,4 centímetros\",\r\n\"answer4\":\"15,2 centímetros\",\r\n\"correctAnswer\":2},\r\n{\"question\":\"Qual é a medida do diâmetro de um círculo com área 50,26 cm²?\",\r\n\"answer1\":\"7,5 cm\",\r\n\"answer2\":\"10 cm\",\r\n\"answer3\":\"15 cm\",\r\n\"answer4\":\"20 cm\",\r\n\"correctAnswer\":3},\r\n{\"question\":\"Qual é a medida do lado de um quadrado com área 9 dm²?\",\r\n\"answer1\":\"3 dm\",\r\n\"answer2\":\"6 dm\",\r\n\"answer3\":\"9 dm\",\r\n\"answer4\":\"12 dm\",\r\n\"correctAnswer\":1}\r\n]\r\n}";
        json.Replace("\r", "");
        json.Replace("\n", "");

        _ = new Quiz();
        Quiz quiz = JsonUtility.FromJson<Quiz>(json);

        _ = new
        List<Question>();
        List<Question> questions = quiz.questions;

        return questions;
    }

}

[Serializable]
public struct Quiz
{
    public List<Question> questions;
}

[Serializable]
public struct Question
{
    public string question, answer1, answer2, answer3, answer4;
    public int correctAnswer;
}

[Serializable]
public struct Performance
{
    public bool[] correctAnswers;
    public int totalCorrect, totalQuestions;
    public int[] selectedAnswers;
}

[Serializable]
public struct SubjectResults
{
    public QuizIndex index;

    public Quiz quiz;
    public Performance performance;
}

[Serializable]
public struct PlayerResults
{
    public List<SubjectResults> subjectResults;
}
