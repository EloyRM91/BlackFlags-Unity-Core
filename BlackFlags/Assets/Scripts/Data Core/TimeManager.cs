//Core
using System.Collections;
using UnityEngine;
//Date Time
using System;
//Mechanics
using GameMechanics.Sound;

namespace GameMechanics.Data
{
    /// <summary>
    /// A calendar manager class
    /// Parameters: World Date (DateTime), Current Speed (float)
    ///  ** Constructors: None (Monobehaviour)
    /// Dispatched Events: NewDay
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        //Singleton
        public static TimeManager instance;

        //Calendar time
        public static DateTime WorldDate;

        //Refs
        [SerializeField] private GameObject pausePanel;

        //Events
        public delegate void OnDateChange(DateTime date);
        public delegate void OnPeriodEvent();

        /// <summary>
        /// Event launched when a new calenday day is reached (24 in-game hours);
        /// </summary>
        public static OnDateChange NewDay;

        /// <summary>
        /// Event is launched every 20 days to Update market and characters offers.
        /// </summary>
        public static OnPeriodEvent UpdateMarketDay;
        public static OnPeriodEvent UnsubscribeMarketEvent;
        public static OnPeriodEvent DailyConsumptionEvent;

        //public delegate void OnTimeScaleChange();
        /// <summary>
        /// Event launched when the timescale value is set to a new value;
        /// </summary>
        //public static OnTimeScaleChange onTimeScaleChange;


        private float[] _speed = new float[] {0, 0.5f, 1, 2, 5, 20};  //Speed modes: (x0 = pause)
        private byte currentIndex = 2;
        private static float _currentspeed = 1;
        private float timer = 0;
        public float Timer { get { return timer;}}
        private float playedTime = 0;
        public uint PlayedTime { get { return (uint)playedTime; } }
        private int counter20;
        public int Counter20 { get { return counter20; } }

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            if(UnsubscribeMarketEvent != null)
                UnsubscribeMarketEvent();
        }
        private void Start()
        {
            WorldDate = new DateTime(1719, 10, 20);
            NewDay(WorldDate);
        }
        private void Update()
        {
            playedTime += Time.deltaTime;
        }
        private void LateUpdate()
        {
            if(timer < 144) //10 mis/s at timespeed = 1 -- (1 day at 144 sec)
            {
                timer += Time.deltaTime;
            }
            else
            {
                //Transcurre un día
                timer = 0;
                WorldDate = WorldDate.AddDays(1);

                //Lanza eventos
                NewDay(WorldDate);
                DailyConsumptionEvent();

                //Actualiza mercados cada veinte días
                counter20++;
                if (counter20 == 20)
                {
                    counter20 = 0;
                    UpdateMarketDay(); //Lanza evento
                }
            }
        }
        public static DateTime GetDate()
        {
            return WorldDate;
        }
        public static DateTime GetFutureDate(int days)
        {
            DateTime date = WorldDate;
            return date.AddDays(days);
        }

        public static string GetDateString()
        {
            return WorldDate.ToString("d MMMM yyyy");
        }

        public static string GetDateString(DateTime date)
        {
            return date.ToString("d MMMM yyyy");
        }

        public static string GetFutureDateString(int days)
        {
            DateTime date = WorldDate;
            date.AddDays(days);
            return date.ToString("d MMMM yyyy");
        }

        public void SetTimeSpeed(float val) 
        { 
            _currentspeed = val;
            Time.timeScale = val;
        }
        public void SpeedUp()
        {
            if (!GameManager.forcedPause)
            {
                if(Time.timeScale == 0)
                {
                    currentIndex = 1;
                    TimeManager.PauseST();
                }
                else if (currentIndex < _speed.Length - 1)
                {
                    if (Time.timeScale == 0)
                    {
                        currentIndex = 1;
                        pausePanel.SetActive(false);
                        Ambience.Pause_ST(1);
                    }
                    else
                    {
                        currentIndex++;
                    }
                    var newSpeed = _speed[currentIndex];
                    SetTimeSpeed(newSpeed);
                    UIMap.ui.UpdateSpeed(newSpeed);
                }
            }
        }
        public void SpeedDown()
        {
            if (!GameManager.forcedPause)
            {
                if (currentIndex > 0 && Time.timeScale != 0)
                {
                    currentIndex--;
                    var newSpeed = _speed[currentIndex];
                    SetTimeSpeed(newSpeed);
                    UIMap.ui.UpdateSpeed(newSpeed);

                    if (currentIndex == 0) Pause();
                }
            }
        }
        /// <summary>
        /// Set time speed to zero
        /// </summary>
        public void Pause()
        {
            if (!GameManager.forcedPause)
            {
                SetTimeSpeed(0);
                UIMap.ui.UpdateSpeed(0);
                pausePanel.SetActive(true);
                Ambience.Pause_ST(0);
            }
        }
        /// <summary>
        /// a pause singleton call
        /// </summary>
        public static void PauseST()
        {
            if (!GameManager.forcedPause)
            {
                if (_currentspeed == 0)
                {
                    instance.RestartTime();
                    Ambience.Pause_ST(1);
                }

                else
                    instance.Pause();
            }
        }

        /// <summary>
        /// Set time speed to one
        /// </summary>
        public void Normalize()
        {
            currentIndex = 2; //Modificación:
            SetTimeSpeed(1);
            UIMap.ui.UpdateSpeed(1);
            if (pausePanel.activeSelf) pausePanel.SetActive(false);
        }
        /// <summary>
        /// a Normalize speed singleton call.
        /// </summary>
        public static void NormalizeST()
        {
            instance.Normalize();
        }
        /// <summary>
        /// Set previous time speed before pause or normalize
        /// </summary>
        public void RestartTime()
        {
            pausePanel.SetActive(false);
            currentIndex = currentIndex == 0 ? (byte)1 : currentIndex;
            SetTimeSpeed(_speed[currentIndex]);
            UIMap.ui.UpdateSpeed(_speed[currentIndex]);
        }
        /// <summary>
        /// a previous speed time setter singleton call
        /// </summary>
        public static void RestartTimeST() {instance.RestartTime(); }
        IEnumerator TimeCount()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                WorldDate = WorldDate.AddHours(_currentspeed * 0.0834f);
                NewDay(WorldDate);
            }
        }
    }
}

