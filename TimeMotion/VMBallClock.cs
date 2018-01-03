/* **************************************************************************/
/* Source: This link has a good description of the problem:                 */
/* http://code.jsoftware.com/wiki/Essays/The_Ball_Clock_Problem             */
/****************************************************************************/
/* Team one: Time and Motion
Edward Barber - 7925969
Abhishek Sharma - 7719818
Mireille Tabod Epse Nubaga - 6542864
Joseph Kasumba - 8147696
Jeewan Kalia - 8032997
Shane Frost - 5600861 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TimeMotion
{
    class VMBallClock : INotifyPropertyChanged
    {
        private Queue<MDataBall> clockQueue, tempQueue;
        private List<MDataBall> minutes, fiveMinutes, hours;
        private List<int> clocksFile;
        private int nbrClocks, nbrBalls, time;
        private string fileLocation, fullName;
        private const string FOLDER_NAME = "datafile";
        private const string INPUT_FILE_NAME = "input.txt";     //source clock file
        private const string OUTPUT_FILE_NAME = "output.txt";   //Time of motions file
        private const string OUTPUT_TEXT = " balls cycle after ";
        private const string OUTPUT_DAYS = " days";
        private const string HEADER_TEXT = "Outputs: Number of balls, Time of motion (Only numbers between 27 and 127)";
        private const int ZERO = 0;
        private const int ONE = 1;
        private const int MAX_HOUR = 12;
        private const int HOURS_IN_DAY = 24;
        private const int MAX_SIZE_MIN = 4;
        private const int MAX_SIZE_FIVE_MIN = 11;
        private const int MAX_SIZE_HOURS = 11;

        #region Constructor
        public VMBallClock()
        {
            clockQueue = new Queue<MDataBall>();
            tempQueue = new Queue<MDataBall>();
            minutes = new List<MDataBall>();
            fiveMinutes = new List<MDataBall>();
            hours = new List<MDataBall>();
            clocksFile = new List<int>();
        }
        #endregion

        #region BallRange: Ball range
        private enum BallRange
        {
            MIN = 27,
            MAX = 127
        }
        #endregion

        #region ListID: ID for each list
        private enum ListID
        {
            HOUR = 1,
            FIVE_MINUTE,
            MINUTE
        }
        #endregion

        #region EvaluateTime: Time and Motion calcultation
        public void EvaluateTime()
        {
            time = 0;  //nbr of hours/days

            while (minutes.Count <= MAX_SIZE_MIN)
            {
                minutes.Add(clockQueue.Dequeue());

                //Check if a list is full and Release balls
                if (minutes.Count > MAX_SIZE_MIN)
                    ReleaseBalls((int)ListID.MINUTE);

                if (fiveMinutes.Count > MAX_SIZE_FIVE_MIN)
                    ReleaseBalls((int)ListID.FIVE_MINUTE);

                if (hours.Count > MAX_SIZE_HOURS)
                {
                    ReleaseBalls((int)ListID.HOUR);
                    time += MAX_HOUR;

                    if (time == HOURS_IN_DAY)   //24h elapsed
                    {
                        //We know that the clock has the full number of balls here, because the hour list has been released
                        //so check if the clock queue is back to the initial state
                        if (CheckInitialState(clockQueue))
                            time /= HOURS_IN_DAY;           //find number of days
                        else
                            time = ZERO;

                        break;  //Quit while loop after 24h
                    }
                }
            }

            //The relative order of balls has changed in (clock), now calculate the number of days (if clockQueue is not yet in initial state) 
            CalculateNbrDays();
        }
        #endregion

        #region CalculateNbrDays: Calculate Nbr Days to return to the initial state
        private void CalculateNbrDays()
        {
            Queue<MDataBall> savedQueue = new Queue<MDataBall>();

            while (!CheckInitialState(clockQueue))
            {
                //save clock queue with the new relative order only when time = 0
                if (time == ZERO)
                    foreach (MDataBall currBall in clockQueue)
                        savedQueue.Enqueue(currBall);

                //for each ball in clock queue (sequence), use its ballID to index a specific ball into the initial queue (tempQueue) 
                //and enqueue this ball from tempQueue into clock
                clockQueue.Clear();
                foreach (MDataBall currBall in savedQueue)
                    clockQueue.Enqueue(tempQueue.ElementAt(currBall.BallID));

                //save the n-1 state of clockqueue
                tempQueue.Clear();
                foreach (MDataBall currBall in clockQueue)
                    tempQueue.Enqueue(currBall);

                //Add one day
                time++;
            }
        }
        #endregion

        #region CheckInitialState: Check if Initial State
        private bool CheckInitialState(Queue<MDataBall> currQueue)
        {
            bool isInitialState = true;
            int i = 0;

            foreach (MDataBall currBall in currQueue)
            {
                //for each ball, if one doesn't match, the queue is not in the initial state, so leave and return false
                if (currBall.BallID != i++)
                {
                    isInitialState = false;
                    break;
                }
            }

            return isInitialState;
        }
        #endregion

        #region ReleaseBalls: Release balls from minutes to fiveMinutes, from fiveMinutes to hours, from hours to clockQueue
        public void ReleaseBalls(int listID)
        {
            MDataBall tempBall;
            switch (listID)
            {
                case (int)ListID.MINUTE:        //Release minutes balls
                    fiveMinutes.Add(minutes.Last());
                    minutes.RemoveAt(minutes.Count() - ONE);
                    minutes.Reverse();          //the first in list will be the last to leave the list

                    while (minutes.Count > 0)
                    {
                        clockQueue.Enqueue(minutes.First());
                        minutes.RemoveAt(ZERO);
                    }
                    break;
                case (int)ListID.FIVE_MINUTE:    //Release five_minute balls
                    hours.Add(fiveMinutes.Last());
                    fiveMinutes.RemoveAt(fiveMinutes.Count() - ONE);
                    fiveMinutes.Reverse();       //the first in list will be the last to leave the list

                    while (fiveMinutes.Count > 0)
                    {
                        clockQueue.Enqueue(fiveMinutes.First());
                        fiveMinutes.RemoveAt(ZERO);
                    }
                    break;
                case (int)ListID.HOUR:           //Release hour balls
                    tempBall = hours.Last();
                    hours.RemoveAt(hours.Count - ONE);
                    hours.Reverse();            //the first in list will be the last to leave the list

                    while (hours.Count > 0)
                    {
                        clockQueue.Enqueue(hours.First());
                        hours.RemoveAt(ZERO);
                    }
                    clockQueue.Enqueue(tempBall);    //Finally add the fixed ball
                    break;
            }
        }
        #endregion

        #region GetClock: get a valid number of balls from input file
        public bool GetClockBalls()
        {
            MDataBall tempBall;
            bool newInput;

            nbrClocks = clocksFile.Count;
            newInput = true;

            //Reset variables
            clockQueue.Clear();
            tempQueue.Clear();
            nbrBalls = 0;

            //Execute only if clocksFile has some data
            if (nbrClocks > ZERO)
            {
                //Don't stop the program, just find the next valid number of balls into clockFile
                while (nbrBalls < (int)BallRange.MIN || nbrBalls > (int)BallRange.MAX)
                {
                    nbrBalls = clocksFile.First();
                    clocksFile.RemoveAt(ZERO);

                    //zero: End of input
                    if (nbrBalls == ZERO)
                    {
                        newInput = false;
                        break;
                    }
                }

                //Create balls for the clock
                for (int i = 0; i < nbrBalls; i++)
                {
                    tempBall = new MDataBall(i);
                    clockQueue.Enqueue(tempBall);
                    tempQueue.Enqueue(tempBall);
                }
            }
            else
                newInput = false;

            return newInput;
        }
        #endregion

        #region OutputData: Write data (nbrBalls treated, Time/ duration of motion in days)
        public void OutputData(bool createFile = false)
        {
            StreamWriter strWrite;

            //Create file only if the file doesn't exist or the first time this function is called
            if (createFile)
            {
                fileLocation = Path.Combine(System.Environment.CurrentDirectory, FOLDER_NAME);
                fullName = Path.Combine(fileLocation, OUTPUT_FILE_NAME);

                //create or clear the file if it already exists
                File.WriteAllText(fullName, string.Concat(HEADER_TEXT, Environment.NewLine,
                                 "======================================================================", Environment.NewLine));
            }

            //Write text in file
            strWrite = File.AppendText(fullName);
            strWrite.WriteLine(Environment.NewLine + nbrBalls + OUTPUT_TEXT + time + OUTPUT_DAYS); //nbrBalls, Time in days

            strWrite.Close();
        }
        #endregion

        #region InputData: Read file and convert data to int
        public void InputData()
        {
            string clocksFileRead;
            string[] tempFileContent;

            fileLocation = Path.Combine(System.Environment.CurrentDirectory, FOLDER_NAME);
            clocksFileRead = Path.Combine(fileLocation, INPUT_FILE_NAME);
            tempFileContent = File.ReadAllLines(clocksFileRead);

            //Convert number of balls read from string to integer.
            foreach (string nbrBallsRead in tempFileContent)
                clocksFile.Add(int.Parse(nbrBallsRead));
        }
        #endregion

        #region NotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
