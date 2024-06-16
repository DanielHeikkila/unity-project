using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace CoSimulationPlcSimAdv.Models
{
    public class PropertyArgs : EventArgs
    {
        public string Message;
        public PropertyArgs(string message)
        {
            Message = message;
        }

    }

    public class Cosimulation
    {
        public event EventHandler<PropertyArgs> OnOperatingStateChanged;
        public event EventHandler<PropertyArgs> OnErrorSimulationStateChanged;

        //--// Inputs (Outputs of the virtual controller)
        public bool setOnBeltActive;
        public bool moveBeltActive;
        public bool setOffBeltActive;
        public bool releaseActive;
        public bool acknowledgeActive;
        public bool restartActive;
        //--//

        //--// Outputs (Inputs of the virtual controller)
        public bool sensorStartPos = false;
        public bool sensorBeltStart = false;
        public bool sensorBeltDest = false;
        public bool sensorEndPos = false;
        //--//

        //--// Internal variables
        private static bool run;
        private static int step = 0;
        private bool error;
        private bool packageOk;
        private static bool acknReady;
        private bool nextStep;

        private Timer movementTimer;
        private Timer sensorTimer;

        private static bool startedTimer;

        #region Ctor
        public Cosimulation(int movementTime, int sensorTime)
        {
            // New timer for simulation of the movement (roboter, belt)
            movementTimer = new Timer(movementTime);
            movementTimer.AutoReset = false;
            // New timer for simulation of an active sensor
            sensorTimer = new Timer(sensorTime);
            sensorTimer.AutoReset = false;
            // Eventhandler for elapsed timer
            movementTimer.Elapsed += movementTimer_Elapsed;
            sensorTimer.Elapsed += sensorTimer_Elapsed;  
        }
        #endregion //Ctor

        #region Events
        void sensorTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            nextStep = true; // Jump to next step when sensor simulation time elapsed
        }

        void movementTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            nextStep = true; // Jump to next step when movement simulation time elapsed
        }
        #endregion //Events
                 
        #region Cosimulation
        public void CoSimProgramm()
        {
            if (restartActive) // Restart command from the virtual controller
            {
                step = 0; // Set start step
                startedTimer = false; // Reset indicator for started timer
                nextStep = false;
                OnErrorSimulationStateChanged(this, new PropertyArgs("Black"));  // Reset "SIMULATE ERROR" button collor
            }

            if (run) // Active when "START" button pressed for Co-Simulation
            {
                OnOperatingStateChanged(this, new PropertyArgs("ACTIVE")); // Shows "ACTIVE" state of the Co-Simulation

                // Reset all sensors at every call (sensors are set in the corresponding case)
                sensorStartPos = false;
                sensorBeltStart = false;
                sensorBeltDest = false;
                sensorEndPos = false;

                switch (step)
                {
                    case 0: // Simulation of sensor: Package on start position
                        sensorStartPos = true; // Sensor active

                        if (!startedTimer) // Starts once the simulation time for the sensor
                        {
                            sensorTimer.Start(); // Start timer for sensor simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (setOnBeltActive & nextStep)// Next step after sensor simulation time elapsed and command from virtual controller
                        {
                            step = 1; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 1: // Simulation of movement: Roboter sets package on belt

                        if (!startedTimer) // Starts once the simulation time for the movement
                        {
                            movementTimer.Start(); // Start timer for movement simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (nextStep)// Next step after movement simulation time elapsed
                        {
                            step = 2; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 2: // Simulation of sensor: Package on belt start position
                        sensorBeltStart = true; // Sensor active

                        if (!startedTimer) // Starts once the simulation time for the sensor
                        {
                            sensorTimer.Start(); // Start timer for sensor simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (moveBeltActive & nextStep) // Next step after sensor simulation time elapsed and command from virtual controller
                        {
                            step = 3; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 3: // Simulation of movement: Belt moves package to destination position

                        if (!startedTimer) // Starts once the simulation time for the movement
                        {
                            movementTimer.Start(); // Start timer for movement simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (nextStep) // Next step after movement simulation time elapsed
                        {
                            step = 4; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }

                        if (error) // Active when "SIMULATE ERROR" button pressed for Co-Simulation
                        {
                            step = 99; // Jump to error simulation
                            error = false; // Reset error variable
                        }

                        break;

                    case 4: // Simulation of sensor: Package on destination position

                        sensorBeltDest = true; // Sensor active

                        if (!startedTimer) // Starts once the simulation time for the sensor
                        {
                            sensorTimer.Start(); // Start timer for sensor simulation
                            startedTimer = true; // Set indicator for started timer
                        }

                        if (setOffBeltActive & nextStep) // Next step after sensor simulation time elapsed and command from virtual controller
                        {
                            step = 5; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 5: // Simulation of movement: Roboter sets package on end position

                        if (!startedTimer) // Starts once the simulation time for the movement
                        {
                            movementTimer.Start(); // Start timer for movement simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (nextStep) // Next step after movement simulation time elapsed
                        {
                            step = 6; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 6: // Simulation of sensor: Package on end position

                        sensorEndPos = true; // Sensor active

                        if (!startedTimer) // Starts once the simulation time for the sensor
                        {
                            sensorTimer.Start(); // Start timer for sensor simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (releaseActive & nextStep) // Next step after sensor simulation time elapsed and command from virtual controller
                        {
                            step = 7; // Set number of the next step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 7: // Simulation of movement: Release Package

                        if (!startedTimer) // Starts once the simulation time for the movement
                        {
                            movementTimer.Start(); // Start timer for movement simulation
                            startedTimer = true; // Set indicator for started timer
                        }
                        if (nextStep) // Next step after movement simulation time elapsed
                        {
                            step = 0; // Return to start step
                            nextStep = false; // Reset next step variable
                            startedTimer = false; // Reset indicator for started timer
                        }
                        break;

                    case 99: // Simulation of error: Package down

                        // Troubleshooting
                        if (packageOk) // Active when "PACKAGE OK" button pressed for Co-Simulation
                        {
                            acknReady = true; // Simulation ready for acknowledge
                            packageOk = false; // Reset variable for package ok
                        }

                        if (acknReady) // Simulation of sensor: Package on destination position
                        {
                            sensorBeltDest = true; // Sensor active
                        }

                        if (acknReady & acknowledgeActive) // Active Simulation ready for acknowledge and acknowledge command from virtual controller
                        {
                            acknReady = false; // Reset variable 
                            step = 4; // Return to step 4 (regular operation)
                            OnErrorSimulationStateChanged(this, new PropertyArgs("Black")); // Reset "SIMULATE ERROR" button collor
                        }
                        break;
                }
            }
            else
            {
                OnOperatingStateChanged(this, new PropertyArgs("STOPPED"));  // Shows "STOPPED" state of the Co-Simulation
            }
        }
        #endregion //Cosimulation

        #region Methods

        public void Error() //Activate error simulation
        {
            error = true; // Set variable for error simulation
            OnErrorSimulationStateChanged(this, new PropertyArgs("Red")); // Set "SIMULATE ERRO" button collor red
        }

        public void PackageOK() //Activate simulation: Set package on belt again when package down
        {
            if (step == 99) // When package down
            packageOk = true; // Set variable for package on belt again
        }

        public void Start() // Start Co-Simulation
        {
            run = true; // Set variable to activate Co-Simulation
        }

        public void restart()
        {
            step = 0;
        }

        public void Stop() // Stop Co-Simulation
        {
            run = false; // Reset variable to deactivate Co-Simulation
        }
        #endregion //Methods
    }
}