using Siemens.Simatic.Simulation.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CoSimulationPlcSimAdv.Models
{
    /// <summary>
    /// Class for encapsulate the function of an PLCSIM Adv. virtual Controller and the function to read/write the PAE and PAA
    /// </summary>
    public class PLCInstance
    {

        #region Properties
        /// <summary>
        /// instance of PLCSIM Adv. virtual Controller
        /// </summary>
        public IInstance instance { get; set; }
        /// <summary>
        /// Co-Simulation
        /// </summary>
        public Cosimulation coSimulation { get; set; }
        /// <summary>
        /// indicates if Instance is configured --> tag list updatet
        /// </summary>
        private bool IsConfigured { get; set; }

        private SIPSuite4 instanceIP = new SIPSuite4("192.168.0.101", "255.255.255.0", "0.0.0.0");

        #endregion

        #region Ctor

        public PLCInstance(string instanceName)
        {
            instance = SimulationRuntimeManager.RegisterInstance(instanceName);
            instance.IsAlwaysSendOnEndOfCycleEnabled = true;
            instance.CommunicationInterface = ECommunicationInterface.Softbus;
            instance.OnConfigurationChanged += instance_OnConfigurationChanged;
            instance.OnEndOfCycle += instance_OnEndOfCycle;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event when PLC reach the End of the main Cycle, this will be called, whenever the Controller reaches the end of cycle.
        /// </summary>
        /// <param name="in_Sender">PLC which fired this event</param>
        /// <param name="in_ErrorCode">ErrorCode of Runtime of the PLC</param>
        /// <param name="in_DateTime">DateTime when the configuration changed</param>
        /// <param name="in_CycleTime_ns">current cycle time in ns of the PLC</param>
        /// <param name="in_CycleCount">current count of Cycles of the PLC</param>

        void instance_OnEndOfCycle(IInstance in_Sender, ERuntimeErrorCode in_ErrorCode, 
            DateTime in_DateTime, long in_CycleTime_ns, uint in_CycleCount)
        {
            if (IsConfigured)
            {
                try
                {
                    // Read outputs of the virtual controller and assign to variables of the Co-Simulation
                    coSimulation.setOnBeltActive = instance.ReadBool("setOnBeltActive");
                    coSimulation.moveBeltActive = instance.ReadBool("moveBeltActive");
                    coSimulation.setOffBeltActive = instance.ReadBool("setOffBeltActive");
                    coSimulation.releaseActive = instance.ReadBool("releaseActive");
                    coSimulation.acknowledgeActive = instance.ReadBool("acknowledgeActive");
                    coSimulation.restartActive = instance.ReadBool("restartActive");

                    // Call the Co-Simulation programm
                    coSimulation.CoSimProgramm();

                    // Write the Co-Simulation values to the inputs of the virtual controller  
                    instance.WriteBool("sensorStartPos", coSimulation.sensorStartPos);
                    instance.WriteBool("sensorBeltStart", coSimulation.sensorBeltStart);
                    instance.WriteBool("sensorBeltDest", coSimulation.sensorBeltDest);
                    instance.WriteBool("sensorEndPos", coSimulation.sensorEndPos);
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Event when Configuration changed of the PLC (during download)
        /// </summary>
        /// <param name="in_Sender"> PLC which fired this event</param>
        /// <param name="in_ErrorCode"> ErrorCode of Runtime of the PLC</param>
        /// <param name="in_DateTime"> DateTime when the configuration changed</param>
         void instance_OnConfigurationChanged(IInstance in_Sender, ERuntimeErrorCode in_ErrorCode, DateTime in_DateTime, 
            EInstanceConfigChanged in_InstanceConfigChanged, uint in_Param1, uint in_Param2, uint in_Param3, uint in_Param4)
        {
            
            IsConfigured = false;

            try
            {
                instance.UpdateTagList(ETagListDetails.IO);
                IsConfigured = true;
            }
            catch (Exception ex)
            {
            }
        }
        #endregion //Events

        #region Public Methods

        /// <summary>
        /// Power On PLCSIM Adv. Instanz, set IPSuite of instance
        /// </summary>
        /// <returns></returns>
        public void PowerOnPLCInstance()
        {
            instance.PowerOn(60000);
            instance.SetIPSuite(0, instanceIP, true);
        }

        /// <summary>
        /// Power Off PLCSIM Adv. Instance
        /// </summary>
        public void PowerOffPLCInstance()
        {
            instance.PowerOff(6000);
        }


        /// <summary>
        /// Run PLCSIM Adv. Instance
        /// </summary>
        public void RunPLCInstance()
        {
            instance.Run(6000);

        }

        /// <summary>
        /// Stop PLCSIM Adv. Instance
        /// </summary>
        public void StopPLCInstance()
        {
            instance.Stop(6000);
        }

        #endregion //Public Methods

    }
}
