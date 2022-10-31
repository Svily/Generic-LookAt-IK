using System.Collections.Generic;
using System.Linq;

namespace IKAnimation
{
    public class LooKAtIKManager
    {

        private static LooKAtIKManager mInstance;
        
        public static LooKAtIKManager Instance => mInstance ?? new LooKAtIKManager();

        private LooKAtIKManager(){}
        
        private Dictionary<string, LookAtIKCtrl>    IKCtrlDict = new Dictionary<string, LookAtIKCtrl>();
        
        public Dictionary<string, LookAtIKCtrl> CtrlDic
        {
            get { return this.IKCtrlDict; }
        }

        public void AddIKCtrl(string key, LookAtIKCtrl value)
        {
            if (!this.IKCtrlDict.ContainsKey(key))
            {
                this.IKCtrlDict.Add(key, value);
            }
        }

        public LookAtIKCtrl GetIKCtrl(string key)
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