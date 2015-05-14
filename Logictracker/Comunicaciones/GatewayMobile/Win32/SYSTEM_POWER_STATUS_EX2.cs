using System;

using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Win32
{
    // http://msdn.microsoft.com/en-us/library/ms941842.aspx
    [StructLayout(LayoutKind.Sequential)]
    public class  SYSTEM_POWER_STATUS_EX2
    {
        //AC power status. 
        public ACLineStatus ACLineStatus;
        //Battery charge status
        public BatteryFlag BatteryFlag;
        // Percentage of full battery charge remaining. Must be in 
        // the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN if 
        // percentage of battery life remaining is unknown
        public byte BatteryLifePercent;
        byte Reserved1;
        //Percentage of full battery charge remaining. Must be 
        // in the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN 
        // if percentage of battery life remaining is unknown. 
        public int BatteryLifeTime;
        // Number of seconds of battery life when at full charge, 
        // or BATTERY_LIFE_UNKNOWN if full lifetime of battery is unknown
        public int BatteryFullLifeTime;
         byte Reserved2;
         // Backup battery charge status.
        public BatteryFlag BackupBatteryFlag;
        // Percentage of full backup battery charge remaining. Must be in 
        // the range 0 to 100, or BATTERY_PERCENTAGE_UNKNOWN if percentage 
        // of backup battery life remaining is unknown. 

        public byte BackupBatteryLifePercent;
        byte Reserved3;
        // Number of seconds of backup battery life when at full charge, or 
        // BATTERY_LIFE_UNKNOWN if number of seconds of backup battery life 
        // remaining is unknown. 
        public int BackupBatteryLifeTime;
        // Number of seconds of backup battery life when at full charge, or 
        // BATTERY_LIFE_UNKNOWN if full lifetime of backup battery is unknown
        public int BackupBatteryFullLifeTime;
        // Number of millivolts (mV) of battery voltage. It can range from 0 
        // to 65535
        public int BatteryVoltage;
        // Number of milliamps (mA) of instantaneous current drain. It can 
        // range from 0 to 32767 for charge and 0 to –32768 for discharge. 
        public int BatteryCurrent;
        //Number of milliseconds (mS) that is the time constant interval 
        // used in reporting BatteryAverageCurrent. 
        public int BatteryAverageCurrent;
        // Number of milliseconds (mS) that is the time constant interval 
        // used in reporting BatteryAverageCurrent. 

        public int BatteryAverageInterval;
        // Average number of milliamp hours (mAh) of long-term cumulative 
        // average discharge. It can range from 0 to –32768. This value is 
        // reset when the batteries are charged or changed. 

        public int BatterymAHourConsumed;
        // Battery temperature reported in 0.1 degree Celsius increments. It 
        // can range from –3276.8 to 3276.7. 
        public int BatteryTemperature;
        // Number of millivolts (mV) of backup battery voltage. It can range 
        // from 0 to 65535.
        public int BackupBatteryVoltage;
        // Type of battery.
        public BatteryChemistry BatteryChemistry;
        //  Add any extra information after the BatteryChemistry member.
    }
}
