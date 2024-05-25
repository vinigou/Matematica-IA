using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIBehavior : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _root;
    private VisualElement _container;
    
    public List<VisualTreeAsset> visualTreeAssets;
    public VisualTreeAsset performanceSelectButtonTreeAsset;
    public VisualTreeAsset pdQuestionTreeAsset;
    public VisualTreeAsset pdAnswerTreeAsset;

    private List<Menu> _menus;
    public StartMenu StartMenu;
    public InsertKey InsertKey;
    public Options Options;
    public QuizSelection QuizSelection;
    public QuestionsMenu QuestionsMenu;
    public Results Results;
    public PerformanceSelect PerformanceSelect;
    public PerformanceDisplay PerformanceDisplay;
    public ExitScreen ExitScreen;

    public static UIBehavior instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this;

        _document = GetComponent<UIDocument>();
        _root = _document.rootVisualElement;
        _container = _root.Q("Container");

        _container.Clear();

        StartMenu = new StartMenu();
        InsertKey = new InsertKey();
        Options = new Options();
        QuizSelection = new QuizSelection();
        QuestionsMenu = new QuestionsMenu();
        Results = new Results();
        PerformanceSelect = new PerformanceSelect();
        PerformanceDisplay = new PerformanceDisplay();
        ExitScreen = new ExitScreen();


        _menus = new List<Menu>
        {
            StartMenu,
            InsertKey,
            Options,
            QuizSelection,
            QuestionsMenu,
            Results,
            PerformanceSelect,
            PerformanceDisplay
        };

        foreach (var menu in _menus)
        {
            menu.Initialize(_container);
            menu.Reset();
        }

        _menus.Add(ExitScreen);
        ExitScreen.Initialize(_container);
    }

    private IEnumerator Start()
    {
        WaitForSeconds waitForSeconds = new(0.5f);
        yield return waitForSeconds;
        StartMenu.Show();
        ExitScreen.Show();
        yield break;
    }
}

public enum MenuIndex
{
    StartMenu = 0,
    InsertKey = 1,
    Options = 2,
    QuizSelection = 3,
    QuestionsMenu = 4,
    Results = 5,
    PerformanceSelect = 6,
    PerformanceDisplay = 7,
    ExitScreen = 8
}

public enum QuizIndex
{
    Operacoes1, Operacoes2,
    Geometria1, Geometria2,
    Medidas1, Medidas2,
    Probabilidade1, Probabilidade2,
    Algebra
}