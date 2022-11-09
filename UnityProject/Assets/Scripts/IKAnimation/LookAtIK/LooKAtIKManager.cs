using System.Collections.Generic;
using System.Linq;
using GYGame.World;
using RootMotion;

namespace IKAnimation
{
    public class LooKAtIKManager
    {
        
        private LooKAtIKManager(){}
        
        private static LooKAtIKManager _instance;
        
        public static LooKAtIKManager instance
        {
            get { return _instance ??= new LooKAtIKManager(); }
        }
        
        private Dictionary<string, LookAtIKCtrl>    IKCtrlDict = new Dictionary<string, LookAtIKCtrl>();
        
        public Dictionary<string, LookAtIKCtrl> CtrlDic => this.IKCtrlDict;

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