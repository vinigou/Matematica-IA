using OpenAI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void CoroutineStarter(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    #region OpenAIController
    public OpenAIApi api;
    public string APIKey;

    public void SetKey(string key)
    {
        api = new(key);
    }

    public async System.Threading.Tasks.Task GenerateQuiz(string subject, string difficulty, QuizIndex index, Action<bool> OnSuccess)
    {
        string systemMessageText = "Você gera questionários de matemática em português do Brasil para alunos do primeiro grau. " +
            "Um questionário consiste de uma lista de questões com 1 pergunta e 4 respostas, sendo apenas 1 correta. " +
            "Se a primeira resposta é a correta, \"correctAnswer\":1, se for a segunda, \"correctAnswer\":2, e assim por diante. " +
            "Sua resposta deve ser em formato json, seguindo a seguinte estrutura:" +
            "\r\n{\"questions\":[{\"question\":\"<Texto da Questão>\",\"answer1\":\"<Texto da Resposta 1>\",\"answer2\":\"<Texto da Resposta 2>\"," +
            "\"answer3\":\"<Texto da Resposta 3>\",\"answer4\":\"<Texto da Resposta 4>\",\"correctAnswer\":1}]}";
        string userMessageText = $"Gere um questionário {difficulty} de {subject} em json";

        List<ChatMessage> messages = new();

        ChatMessage systemMessage = new()
        {
            Role = "system",
            Content = systemMessageText
        };

        messages.Add(systemMessage);

        // Fill the user message from the input field
        ChatMessage userMessage = new()
        {
            Role = "user",
            Content = userMessageText
        };

        // Add the message to the list
        messages.Add(userMessage);

        CreateChatCompletionRequest request = new()
        {
            Model = "gpt-3.5-turbo",
            Temperature = 0.2f,
            MaxTokens = 2000,
            Messages = messages
        };

        var chatResult = await api.CreateChatCompletion(request);

        // Get the response message
        ChatMessage responseMessage = new()
        {
            Role = chatResult.Choices[0].Message.Role,
            Content = chatResult.Choices[0].Message.Content
        };

        messages.Add(responseMessage);

        _ = new
        Quiz();
        Quiz quiz = JsonUtility.FromJson<Quiz>(responseMessage.Content);

        QuizManager.SetQuiz(quiz, index);

        OnSuccess?.Invoke(true);

    }
    #endregion

    #region PlayerPerformance
    private readonly string _filePath = $"{Application.dataPath}/Data/";
    private readonly string _fileName = $"playerdata.json";

    public PlayerResults results;
    private Difficulty _adjustedDifficulty;

    private PlayerResults LoadResults()
    {
        PlayerResults results;
        string dataJson;
        

        if (!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
        }

        if (File.Exists(_filePath + _fileName))
        {
            dataJson = File.ReadAllText(_filePath + _fileName);
            results = JsonUtility.FromJson<PlayerResults>(dataJson);
        }
        else
        {
            results = new()
            {
                subjectResults = new()
            };
        }

        return results;
    }

    private void SaveResults(PlayerResults results)
    {
        string dataJson;

        if (!Directory.Exists(_filePath))
        {
            Directory.CreateDirectory(_filePath);
        }

        dataJson = JsonUtility.ToJson(results);
        File.WriteAllText(_filePath + _fileName, dataJson);

        Debug.Log($"Data saved to: {_fileName}");
    }

    public void RegisterResults()
    {
        Performance newPerformance = QuizManager.performance;
        Quiz newQuiz = QuizManager.quiz;
        QuizIndex index = QuizManager.index;

        SubjectResults subjectResults = new()
        {
            quiz = newQuiz,
            performance = newPerformance,
            index = index
        };

        results = LoadResults();
        results.subjectResults.Add(subjectResults);
        SaveResults(results);
        AdjustDifficulty();
        UIBehavior.instance.PerformanceSelect.UpdateButtons();
    }

    private void AdjustDifficulty()
    {
        if (results.subjectResults.Count >= 3)
        {
            float[] averages = new float[3];
            int count = 0;

            for (int i = results.subjectResults.Count - 3; i < results.subjectResults.Count ; i++)
            {
                int totalQuestions = results.subjectResults[i].performance.totalQuestions;
                int correctAnswers = results.subjectResults[i].performance.totalCorrect;
                averages[count] = correctAnswers / totalQuestions;
                count++;
            }

            float calculatedAverage = (averages[0] + averages[1] + averages[2]) / 3;

            if (calculatedAverage < 0.5f)
            {
                _adjustedDifficulty = Difficulty.Easy;
            }
            else if (calculatedAverage >= 0.5f &&  calculatedAverage < 0.7f)
            {
                _adjustedDifficulty = Difficulty.Medium;
            }
            else if (calculatedAverage >= 0.7f)
            {
                _adjustedDifficulty = Difficulty.Hard;
            }

            Debug.Log($"Difficulty adjusted: {_adjustedDifficulty}");
        }
    }

    public string AdjustQuizDifficulty(Difficulty difficulty)
    {
        int adjust;
        switch (_adjustedDifficulty)
        {
            case Difficulty.Easy:
                adjust = (int)difficulty - 1 ;
                difficulty = (Difficulty)adjust;
                break;
            case Difficulty.Medium:
                break;
            case Difficulty.Hard:
                adjust = (int)difficulty + 1;
                difficulty = (Difficulty)adjust;
                break;
            default:
                break;
        }

        string difficultyText = difficulty switch
        {
            Difficulty.VeryEasy => "muito fácil",
            Difficulty.Easy => "fácil",
            Difficulty.Medium => "médio",
            Difficulty.Hard => "difícil",
            Difficulty.VeryHard => "muito difícil",
            _ => "médio",
        };

        return difficultyText;
    }
    #endregion

    #region Misc
    public void QuitProgram()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
    #endregion

    #region Runtime
    private void Awake()
    {
        if (instance != null &&  instance != this)
            Destroy(gameObject);
        instance = this;

        results = LoadResults();
        AdjustDifficulty();
    }

    private void Start()
    {
        UIBehavior.instance.PerformanceSelect.UpdateButtons();

        QuizManager.QuizEnd += RegisterResults;
    }

    private void OnDisable()
    {
        QuizManager.QuizEnd -= RegisterResults;
    }
    #endregion
}

public enum Difficulty
{
    VeryEasy,
    Easy,
    Medium,
    Hard,
    VeryHard
}
