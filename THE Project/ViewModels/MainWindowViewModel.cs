using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CoSimulationPlcSimAdv.Views;
using CoSimulationPlcSimAdv.Commands;
using CoSimulationPlcSimAdv.Models;
using Siemens.Simatic.Simulation.Runtime;

namespace CoSimulationPlcSimAdv.ViewModels
{
    /// <summary>
    /// MainViewModel of the Application
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        /// <summary>
        /// PLCSIM Adv. Instance of the virtual controller
        /// </summary>
        public PLCInstance virtualController = null;

        //CoSimulationCycle coSim = null;
        public Cosimulation transportSystem = null;

        #endregion

        #region Properties

        /// <summary>
        /// Collection for Status reporting in UI
        /// </summary>
        private ObservableCollection<String> statusListView;
        public ObservableCollection<String> StatusListView
        {
            get
            {
                return statusListView;
            }
            set
            {
                if (statusListView == value)
                {
                    return;
                }
                statusListView = value;
                base.RaisePropertyChanged("StatusListView");
            }
        }

        /// <summary>
        /// Status of PLC Instance (StartUp, Booting, Stop, Run...)
        /// </summary>
        private String statusPLCInstance;
        public String StatusPLCInstance
        {
            get { return statusPLCInstance; }
            set
            {
                if (value == statusPLCInstance)
                    return;
                statusPLCInstance = value;
                RaisePropertyChanged("StatusPLCInstance");
            }
        }

        /// <summary>
        /// Status of Cosimulation (ACTIVE,STOPPED)
        /// </summary>
        private String statusCoSimulation;
        public String StatusCoSimulation
        {
            get { return statusCoSimulation; }
            set
            {
                if (value == statusCoSimulation)
                    return;
                statusCoSimulation = value;
                RaisePropertyChanged("StatusCoSimulation");
            }
        }

        /// <summary>
        /// Collor of "SIMULATE ERROR" button
        /// </summary>
        private String coSimErrorButtonCollor = "Black";
        public String CoSimErrorButtonCollor
        {
            get { return coSimErrorButtonCollor; }
            set
            {
                if (value == coSimErrorButtonCollor)
                    return;
                coSimErrorButtonCollor = value;
                RaisePropertyChanged("CoSimErrorButtonCollor");
            }
        }


        #endregion

        #region Commands
        /// <summary>
        /// Command for power on PLCSIM Advanced instance
        /// </summary>
        private ICommand powerOnInstanceCommand;
        public ICommand PowerOnInstanceCommand
        {
            get
            {
                if (powerOnInstanceCommand == null)
                {
                    powerOnInstanceCommand = new RelayCommand(
                        param => this.PowerOnController(),
                        param => this.IsInstanceNotNull());
                }

                return powerOnInstanceCommand;
            }
        }

        /// <summary>
        /// Command for power off PLCSIM Advanced instance
        /// </summary>
        private ICommand powerOffInstanceCommand;
        public ICommand PowerOffInstanceCommand
        {
            get
            {
                if (powerOffInstanceCommand == null)
                {
                    powerOffInstanceCommand = new RelayCommand(
                        param => this.PowerOffController(),
                        param => this.IsInstanceNotNull());
                }

                return powerOffInstanceCommand;
            }
        }

        /// <summary>
        /// Command for run PLCSIM Advanced instance
        /// </summary>
        private ICommand runInstanceCommand;
        public ICommand RunInstanceCommand
        {
            get
            {
                if (runInstanceCommand == null)
                {
                    runInstanceCommand = new RelayCommand(
                        param => this.RunController(),
                        param => this.IsInstanceRunning());
                }

                return runInstanceCommand;
            }
        }

        /// <summary>
        /// Command for run PLCSIM Advanced instance
        /// </summary>
        private ICommand stopInstanceCommand;
        public ICommand StopInstanceCommand
        {
            get
            {
                if (stopInstanceCommand == null)
                {
                    stopInstanceCommand = new RelayCommand(
                        param => this.StopController(),
                        param => this.IsInstanceRunning());
                }

                return stopInstanceCommand;
            }
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        private ICommand testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (testCommand == null)
                {
                    testCommand = new RelayCommand(
                        param => this.StartBeltSensor(),
                        param => this.IsInstanceRunning());
                }

                return testCommand;
            }
        }

        private ICommand checkStateCommand;
        public ICommand CheckStateCommand
        {
            get
            {
                if (checkStateCommand == null)
                {
                    checkStateCommand = new RelayCommand(
                        param => this.CheckState(),
                        param => this.IsInstanceRunning());
                }

                return checkStateCommand;
            }
        }

        /// <summary>
        /// Exit the Application
        /// </summary>
        private ICommand exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new RelayCommand(
                        param => this.ExitApplication(),
                        param => this.IsInstanceRunning());
                }

                return exitCommand;
            }
        }

        /// <summary>
        /// Command for start Cosimulation
        /// </summary>
        private ICommand cosimulationStartCommand;
        public ICommand CosimulationStartCommand
        {
            get
            {
                if (cosimulationStartCommand == null)
                {
                    cosimulationStartCommand = new RelayCommand(
                        param => this.StartCosimulation());
                }

                return cosimulationStartCommand;
            }
        }

        /// <summary>
        /// Command for stop Cosimulation
        /// </summary>
        private ICommand cosimulationStopCommand;
        public ICommand CosimulationStopCommand
        {
            get
            {
                if (cosimulationStopCommand == null)
                {
                    cosimulationStopCommand = new RelayCommand(
                        param => this.StopCosimulation());
                }

                return cosimulationStopCommand;
            }
        }

        /// <summary>
        /// Command for simulate an error in Cosimulation
        /// </summary>
        private ICommand cosimulationErrorCommand;
        public ICommand CosimulationErrorCommand
        {
            get
            {
                if (cosimulationErrorCommand == null)
                {
                    cosimulationErrorCommand = new RelayCommand(
                        param => this.SimulateError());
                }

                return cosimulationErrorCommand;
            }
        }

        /// <summary>
        /// Command for set package again on belt in the Cosimulation
        /// </summary>
        private ICommand cosimulationPackageOKCommand;
        public ICommand CosimulationPackageOKCommand
        {
            get
            {
                if (cosimulationPackageOKCommand == null)
                {
                    cosimulationPackageOKCommand = new RelayCommand(
                        param => this.PackageOKCommand());
                }

                return cosimulationPackageOKCommand;
            }
        }

        #endregion

        #region C´tor

        public MainWindowViewModel()
        {
            StatusListView = new ObservableCollection<String>();

            try
            {
                // New PLC instance Name:"TransportControl"
                virtualController = new PLCInstance("testing testing 123456");
                WriteStatusEntry(String.Format("Instance registered: {0}", virtualController.instance.Name));

                // Eventhandler, when operating state of instance changed
                virtualController.instance.OnOperatingStateChanged += plcInstance_OnOperatingStateChanged;
                
                // New Cosimulation "transportSystem
                // Simulation time movement 2000ms 
                // Simulation time sensor 1000ms
                transportSystem = new Cosimulation(2000, 1000);
                // Assign the Cosimulation (transportSystem) to the instance of virtual controller
                virtualController.coSimulation = transportSystem;

                // Eventhandler, when operating state of Cosimulation changed
                transportSystem.OnOperatingStateChanged += transportSystem_OnOperatingStateChanged;
                // Eventhandler, when state of error simulation in Cosimulation changed
                transportSystem.OnErrorSimulationStateChanged += transportSystem_OnErrorSimulationStateChanged;

                

            }
            catch (SimulationRuntimeException simRuntimeEx)
            {
                WriteStatusEntry("Error during Register of Instance: " + simRuntimeEx.Message);
            }

        }

       
        #endregion

        #region Events

        void transportSystem_OnErrorSimulationStateChanged(object sender, PropertyArgs e)
        {
            // Change collor of "SIMULATE ERROR" button
            CoSimErrorButtonCollor = e.Message;
        }

        void plcInstance_OnOperatingStateChanged(IInstance in_Sender, ERuntimeErrorCode in_ErrorCode, DateTime in_DateTime, EOperatingState in_PrevState, EOperatingState in_OperatingState)
        {
            StatusPLCInstance = in_OperatingState.ToString();
        }

        void transportSystem_OnOperatingStateChanged(object sender, PropertyArgs e)
        {
            StatusCoSimulation = e.Message;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Write status Message to Listbox in MainView
        /// </summary>
        /// <param name="statusText"></param>
        public void WriteStatusEntry(String statusText)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
            {
                StatusListView.Insert(0, DateTime.Now + ": " + statusText);

            }));

        }

        /// <summary>
        /// Power On registred Instance of virtual controller
        /// </summary>
        public void PowerOnController()
        {
            try
            {
                WriteStatusEntry(String.Format("Power On Instance: {0}", virtualController.instance.Name));
                virtualController.PowerOnPLCInstance();
            }
            catch (SimulationRuntimeException simRtEx)
            {
                WriteStatusEntry(String.Format("PowerOn Instance failed: {0}", simRtEx.Message));
            }
        }

        /// <summary>
        /// Power Off registred Instance of virtual controller
        /// </summary>
        public void PowerOffController()
        {
            try
            {
                WriteStatusEntry(String.Format("Power Off Instance: {0}", virtualController.instance.Name));
                virtualController.PowerOffPLCInstance();
            }
            catch (SimulationRuntimeException simRtEx)
            {
                WriteStatusEntry(String.Format("PowerOff Instance failed: {0}", simRtEx.Message));
            }
        }

        /// <summary>
        /// Run registred Instance of virtual controller
        /// </summary>
        public void RunController()
        {
            try
            {
                WriteStatusEntry(String.Format("Run Instance: {0}", virtualController.instance.Name));
                virtualController.RunPLCInstance();
            }
            catch (SimulationRuntimeException simRtEx)
            {
                WriteStatusEntry(String.Format("Run Instance failed: {0}! Please load plc program before execute RUN.", simRtEx.Message));

            }
        }

        /// <summary>
        /// Stop registred Instance of virtual controller
        /// </summary>
        public void StopController()
        {
            try
            {
                WriteStatusEntry(String.Format("Stop Instance: {0}", virtualController.instance.Name));
                virtualController.StopPLCInstance();

            }
            catch (SimulationRuntimeException simRtEx)
            {
                WriteStatusEntry(String.Format("Stop Instance failed: {0}", simRtEx.Message));
            }
        }

        /// <summary>
        /// Unregister Instance of virtual controller by closing Application
        /// </summary>
        public void ExitApplication()
        {

            //Exit application
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Start Cosimulation
        /// </summary>
        public void StartCosimulation()
        {
            if (virtualController.instance.OperatingState != EOperatingState.Run) 
            {
                WriteStatusEntry("Set PLC Instance in RUN");
            }
                else
            {
                transportSystem.Start();
                WriteStatusEntry("Co-Simulation started");
            }
        }

        /// <summary>
        /// Stop Cosimulation
        /// </summary>
        public void StopCosimulation()
        {
            if (virtualController.instance.OperatingState != EOperatingState.Run)
            {
                WriteStatusEntry("Set PLC Instance in RUN");
            }
            else
            {
                transportSystem.Stop();
                WriteStatusEntry("Co-Simulation stopped");
            }
        }
        
        /// <summary>
        /// Simulate an error in Cosimulation
        /// </summary>
        public void SimulateError()
        {
            if (virtualController.instance.OperatingState != EOperatingState.Run)
            {
                WriteStatusEntry("Set PLC Instance in RUN");
            }
            else
            {
                transportSystem.Error();
            }
        }

        /// <summary>
        /// Set package on belt again, Cosimulation
        /// </summary>
        public void PackageOKCommand()
        {
            if (virtualController.instance.OperatingState != EOperatingState.Run)
            {
                WriteStatusEntry("Set PLC Instance in RUN");
            }
            else
            {
                transportSystem.PackageOK();
            }
        }
        
        public void CheckState()
        {
            try
            {
                WriteStatusEntry("Setting package on belt " + transportSystem.setOnBeltActive.ToString());
                WriteStatusEntry("Belt moving " + transportSystem.moveBeltActive.ToString());
                WriteStatusEntry("Setting package off belt " + transportSystem.setOffBeltActive.ToString());
                WriteStatusEntry("Releasing package " + transportSystem.releaseActive.ToString());
                //WriteStatusEntry("Setting package back on belt " + transportSystem.acknowledgeActive.ToString());
                //WriteStatusEntry("Restart " + transportSystem.restartActive.ToString());
            }
            catch
            {
                WriteStatusEntry("...");
            }
        }

        public void StartBeltSensor()
        {
            try
            {
                WriteStatusEntry("Sensor Triggered: Start");
                transportSystem.restart();
            }
            catch (SimulationRuntimeException simRtEx)
            {
                WriteStatusEntry("...");

            }
        }


        #endregion // Public Methods

        #region Private Methods

        private bool IsInstanceNotNull()
        {
            return !(virtualController == null );

        }

        private bool IsInstanceRunning()
        {
            return true;

        }
        #endregion
    }
}
