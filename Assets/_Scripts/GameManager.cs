using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float gameSpeed = 1f;
    private float currentTime;
    private float nextMinuteTick;
    private int years = 0;
    private int days = 0;
    private int hours = 7;
    private int minutes = 0;
    public TextMeshProUGUI clockDisplay;
    public TextMeshProUGUI calendarDisplay;
    // plastic
    public int plastic;
    public TextMeshProUGUI plasticText;
    // drift wood
    public int driftWood;
    public TextMeshProUGUI driftWoodText;
    // fish
    public int fishRaw;
    public TextMeshProUGUI fishRawText;
    // fresh water
    public int freshWater;
    public TextMeshProUGUI freshWaterText;
    // food
    public int food;
    public TextMeshProUGUI foodText;
    // food
    public int survivors;
    public TextMeshProUGUI survivorsText;

    // chance to ocean comb: (0-100k)
    public int chanceForPlastic = 200;
    public int chanceForWood = 100;
    void Start()
    {
        LoadSavedGame();
        Debug.Log("loading game...");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateResourcesDisplay();
        RunGameClock();
    }

    private void UpdateResourcesDisplay()
    {
        plasticText.text = plastic.ToString();
        driftWoodText.text = driftWood.ToString();
        fishRawText.text = fishRaw.ToString();
        freshWaterText.text = freshWater.ToString();
        foodText.text = food.ToString();
        survivorsText.text = survivors.ToString();
    }

    private void LoadSavedGame()
    {
        LoadInitialResources();
        LoadGameClock();
        UpdateCalendarDisplay();
    }

    private void LoadInitialResources()
    {
        plastic = 50;
        driftWood = 10;
        fishRaw = 10;
        freshWater = 60;
        food = 40;
        survivors = 4;
    }

    private void LoadGameClock()
    {
        currentTime = Time.realtimeSinceStartup;
        nextMinuteTick = Time.realtimeSinceStartup + 1f;
        years = 0;
        days = 0;
        hours = 7;
        minutes = 0;
    }

    private void RunGameClock()
    {
        currentTime = Time.realtimeSinceStartup;
        if (currentTime >= nextMinuteTick)
        {
            if (minutes < 59)
            {
                minutes += 1;
                nextMinuteTick = Time.realtimeSinceStartup + (1 * (1 / gameSpeed));
                RunMinuteRoutine();
            }
            else if (hours < 23)
            {
                hours += 1;
                minutes = 0;
                nextMinuteTick = Time.realtimeSinceStartup + (1 * (1 / gameSpeed));
                RunMinuteRoutine();
                RunHourlyRoutine();
            }
            else
            {
                UpdateCalendar();
                hours = 0;
                minutes = 0;
                nextMinuteTick = Time.realtimeSinceStartup + (1 * (1 / gameSpeed));
                RunMinuteRoutine();
                RunHourlyRoutine();
            }

            UpdateClockDisplay();
        }
    }

    private void UpdateCalendar()
    {
        if (days < 364)
        {
            days += 1;
        }
        else
        {
            days = 0;
            years += 1;
        }
        UpdateCalendarDisplay();
    }

    private void RunMinuteRoutine()
    {
        if (RandomChanceForResources(chanceForPlastic))
        {
            plastic += 1;
        }
        if (RandomChanceForResources(chanceForWood))
        {
            driftWood += 1;
        }
    }

    private void RunHourlyRoutine()
    {
        Debug.Log("Ding Dong");
    }

    private void UpdateClockDisplay()
    {
        string hoursText = hours < 10 ? $"0{hours}" : hours.ToString();
        string minutesText = minutes < 10 ? $"0{minutes}" : minutes.ToString();
        clockDisplay.text = $"{hoursText}:{minutesText}";
    }
    private void UpdateCalendarDisplay()
    {
        calendarDisplay.text = $"{years}y {days}d";
    }
    private bool RandomChanceForResources(int slider)
    {
        if (slider < 0) slider = 0;
        if (slider > 1000) slider = 1000;
        System.Random _random = new();
        int draw = _random.Next(0, 100000);
        return draw < slider;
    }

    public void SetGameSpeedNormal()
    {
        gameSpeed = 1;
    }
    public void SetGameSpeedFast()
    {
        gameSpeed = 10;
    }
    public void SetGameSpeedMax()
    {
        gameSpeed = 100;
    }
}
