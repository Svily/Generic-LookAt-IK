using System.Collections.Generic;
using System.Linq;

namespace IKAnimation
{
    public class LooKAtIKManager 
    {
        private static LooKAtIKManager              mInstance;

        public static LooKAtIKManager               Instance => mInstance ??= new LooKAtIKManager();

        private Dictionary<string, LookAtIKModelCtrl>    IKCtrlDict = new Dictionary<string, LookAtIKModelCtrl>();


        public Dictionary<string, LookAtIKModelCtrl> CtrlDic
        {
            get { return this.IKCtrlDict; }
        }

        public void AddIKCtrl(string key, LookAtIKModelCtrl value)
        {
            if (!this.IKCtrlDict.ContainsKey(key))
            {
                this.IKCtrlDict.Add(key, value);
            }
        }

        public LookAtIKModelCtrl GetIKCtrl(string key)
        {
            if (this.IKCtrlDict.ContainsKey(key))
            {
                return this.IKCtrlDict[key];
            }
            return null;
        }

        public void RemoveIKCtrl(string key)
        {
            if (this.IKCtrlDict.ContainsKey(key))
            {
                this.IKCtrlDict.Remove(key);
            }
            
            
        }
    }
}