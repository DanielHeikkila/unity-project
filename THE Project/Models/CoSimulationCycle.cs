using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Siemens.Simatic.Simulation.Runtime;



namespace CrossLinkPlcSimAdv.Models
{
    public class PropertyArgsTest : EventArgs
    {
        public string MessageTest;
        public PropertyArgsTest(string message)
        {
            MessageTest = message;
        }

    }
    public class CoSimulationCycle
    {
        public event EventHandler<PropertyArgsTest> OnOperatingStateChanged;
        // Inputs of the PLC
        public bool sensorStartPos ;//{ get; set; }
        public bool sensorBeltStart ;//{ get; set; }
        public bool sensorBeltDest ;//{ get; set; }
        public bool sensorEndPos ;//{ get; set; }

        // Outputs of the PLC
        public bool setOnBeltActive;//{ get; set; }
        public bool moveBeltActive;//{ get; set; }
        public bool setOffBeltActive;//{ get; set; }
        public bool releaseActive;//{ get; set; }

        public bool acknowledgeActive;//{ get; set; }
        public bool restartActive;//{ get; set; }

        // Internal variables
        private bool run;
        private static int step = 0;
        public bool error ;
        public static bool packageOk;
        private static bool acknReady;
        public bool nextStep { get; set; }

        public static int counter = 0;
        const int cycleCountMove = 2000;
        const int cyclicCountSensor = 500;

        #region Ctor

        public CoSimulationCycle()
        {
        }
#endregion //Ctor

        #region Cosimulation
        public void CoSimProgramm()
        {
            if (run)
            {
                if (restartActive)
                {
                    step = 0;
                    counter = 0;
                }
                switch (step)
                {
                    case 0:
                        sensorStartPos = true;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cyclicCountSensor)
                            {
                                nextStep = true;
                                counter = 0;
                            }

                        }


                        if (setOnBeltActive & nextStep)
                        {
                            step = 1;
                            nextStep = false;
                        }
                        break;

                    case 1:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cycleCountMove)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }

                        if (nextStep)
                        {
                            step = 2;
                            nextStep = false;
                        }
                        break;

                    case 2:
                        sensorStartPos = false;
                        sensorBeltStart = true;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cyclicCountSensor)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }

                        if (moveBeltActive & nextStep)
                        {
                            step = 3;
                            nextStep = false;
                        }
                        break;

                    case 3:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cycleCountMove)
                            {
                                nextStep = true;
                                counter = 0;
                            }

                        }
                        if (nextStep)
                        {
                            step = 4;
                            nextStep = false;
                        }

                        if (error)
                        {
                            step = 99;
                            error = false;

                        }

                        break;

                    case 99:
                        if (packageOk)
                        {
                            acknReady = true;
                            packageOk = false;
                        }

                        if (acknReady)
                        {
                            sensorBeltDest = true;
                        }

                        if (acknReady & acknowledgeActive)
                        {
                            acknReady = false;
                            step = 4;
                        }
                        break;

                    case 4:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = true;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cyclicCountSensor)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }

                        if (setOffBeltActive & nextStep)
                        {
                            step = 5;
                            nextStep = false;
                        }
                        break;

                    case 5:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cycleCountMove)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }
                        if (nextStep)
                        {
                            step = 6;
                            nextStep = false;
                        }
                        break;

                    case 6:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = true;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cyclicCountSensor)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }

                        if (releaseActive & nextStep)
                        {
                            step = 7;
                            nextStep = false;
                        }
                        break;

                    case 7:
                        sensorStartPos = false;
                        sensorBeltStart = false;
                        sensorBeltDest = false;
                        sensorEndPos = false;

                        if (!nextStep)
                        {
                            counter += 1;

                            if (counter == cycleCountMove)
                            {
                                nextStep = true;
                                counter = 0;
                            }
                        }
                        if (nextStep)
                        {
                            step = 0;
                            nextStep = false;
                        }
                        break;
                }
            }
        }

#endregion //Cosimulation

        #region Methods
        public void Error()
        {
            error = true;
        }

        public void ResetError()
        {
            error = false;
        }

        public void PackageOk()
        {
            packageOk= true;
        }

        public void Start()
        {
            run = true;
            OnOperatingStateChanged(this, new PropertyArgsTest("START"));
        }

        public void Stop()
        {
            run = false;
            OnOperatingStateChanged(this, new PropertyArgsTest("STOP"));
        }
        #endregion //Methods
    }
    }


